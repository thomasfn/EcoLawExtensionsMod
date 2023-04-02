using System;
using System.Collections.Generic;

namespace Eco.Mods.LawExtensions
{
    using Eco.Gameplay.Aliases;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay.PowerGrids;
    
    using Shared.Utils;
    using System.Diagnostics;

    public class PowerGridComponentLawManager
    {
        public PowerGridComponent PowerGridComponent { get; private set; }

        private float accumPowerAvailable = 0.0f;
        private float accumPowerProduced = 0.0f;
        private float accumPowerConsumed = 0.0f;

        public PowerGridComponentLawManager(PowerGridComponent powerGridComponent)
        {
            PowerGridComponent = powerGridComponent;
        }

        public void Tick(float delta)
        {
            if (!PowerGridComponent.IncludedInGrid || PowerGridComponent.IsDestroyed || PowerGridComponent.Parent == null || PowerGridComponent.Parent.IsDestroyed) { return; }
            accumPowerAvailable += PowerGridComponent.EnergySupply * delta;
            if (PowerGridComponent.Enabled)
            {
                accumPowerProduced += PowerGridComponent.EnergySupply * delta;
                accumPowerConsumed += PowerGridComponent.EnergyDemand * delta;
            }
        }

        public void TriggerAction(GameActionPack gameActionPack, float deltaTime)
        {
            if (PowerGridComponent.IsDestroyed || PowerGridComponent.Parent == null || PowerGridComponent.Parent.IsDestroyed) { return; }
            if (accumPowerAvailable > 0.0f || accumPowerProduced > 0.0f)
            {
                var gameAction = new GameActions.PowerGenerated
                {
                    ActionLocation = PowerGridComponent.Parent.Position3i,
                    PowerGenerator = PowerGridComponent.Parent.CreatingItem,
                    IsElectrical = PowerGridComponent.EnergyType is ElectricPower,
                    IsMechanical = PowerGridComponent.EnergyType is MechanicalPower,
                    PowerProduced = accumPowerProduced,
                    PowerAvailable = accumPowerAvailable,
                    TimeGenerating = deltaTime,
                    Citizen = PowerGridComponent.Parent.Owners.FirstUser(),
                };
                gameActionPack.AddGameAction(gameAction);
            }
            if (accumPowerConsumed > 0.0f)
            {
                var gameAction = new GameActions.PowerConsumed
                {
                    ActionLocation = PowerGridComponent.Parent.Position3i,
                    PowerConsumer = PowerGridComponent.Parent.CreatingItem,
                    IsElectrical = PowerGridComponent.EnergyType is ElectricPower,
                    IsMechanical = PowerGridComponent.EnergyType is MechanicalPower,
                    PowerUsed = accumPowerConsumed,
                    TimeConsuming = deltaTime,
                    Citizen = PowerGridComponent.Parent.Owners.FirstUser(),
                };
                gameActionPack.AddGameAction(gameAction);
            }
            accumPowerAvailable = accumPowerProduced = accumPowerConsumed = 0.0f;
        }

    }

    public class PowerGridLawManager : Singleton<PowerGridLawManager>
    {
        private readonly object syncRoot = new object();

        private readonly Stopwatch tickTimer = new Stopwatch();
        private readonly ISet<PowerGridComponent> powerGridComponentsSeenThisTick = new HashSet<PowerGridComponent>();
        private readonly IDictionary<PowerGridComponent, PowerGridComponentLawManager> powerGridComponentManagers = new Dictionary<PowerGridComponent, PowerGridComponentLawManager>();

        public void PowerGridManagerPreTick(PowerGridManager powerGridManager)
        {
            if (LawExtensionsPlugin.Obj.Config.TickInterval <= 0) { return; }
            lock (syncRoot)
            {
                var elapsed = (float)tickTimer.Elapsed.TotalSeconds;
                tickTimer.Restart();
                powerGridComponentsSeenThisTick.Clear();
                foreach (var powerGrid in powerGridManager.PowerGrids)
                {
                    PowerGridPreTick(powerGrid, elapsed);
                }
            }
            
        }

        public void PowerGridManagerPostTick(PowerGridManager powerGridManager)
        {
            if (LawExtensionsPlugin.Obj.Config.TickInterval <= 0) { return; }
            lock (syncRoot)
            {
                foreach (var powerGrid in powerGridManager.PowerGrids)
                {
                    PowerGridPostTick(powerGrid);
                }
                var toRemove = new List<PowerGridComponent>();
                foreach (var pair in powerGridComponentManagers)
                {
                    if (!powerGridComponentsSeenThisTick.Contains(pair.Key))
                    {
                        toRemove.Add(pair.Key);
                    }
                }
                foreach (var component in toRemove)
                {
                    powerGridComponentManagers.Remove(component);
                }
            }
        }

        private void PowerGridPreTick(PowerGrid powerGrid, float timeSinceLastTick)
        {
            foreach (var powerGridComponent in powerGrid.Components)
            {
                powerGridComponentsSeenThisTick.Add(powerGridComponent);
                if (!powerGridComponentManagers.TryGetValue(powerGridComponent, out var manager))
                {
                    manager = new PowerGridComponentLawManager(powerGridComponent);
                    powerGridComponentManagers.Add(powerGridComponent, manager);
                }
                if (timeSinceLastTick > 0.0f)
                {
                    manager.Tick(timeSinceLastTick);
                }
            }
        }

        private void PowerGridPostTick(PowerGrid powerGrid)
        {
            foreach (var powerGridComponent in powerGrid.Components)
            {
                powerGridComponentsSeenThisTick.Add(powerGridComponent);
            }
        }

        public void DoLawTick(float deltaTime)
        {
            lock (syncRoot)
            {
                //Logger.Debug($"Doing power law tick on {powerGridComponentManagers.Count} tracked power grid components...");
                var gameActionPack = new GameActionPack();
                foreach (var pair in powerGridComponentManagers)
                {
                    pair.Value.TriggerAction(gameActionPack, deltaTime);
                }
                var result = gameActionPack.TryPerform(null);
                //Logger.Debug($"Result = {result}");
            }
        }
    }
}
