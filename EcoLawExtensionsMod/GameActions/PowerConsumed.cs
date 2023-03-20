using System;

namespace Eco.Mods.LawExtensions.GameActions
{
    using Gameplay.Players;
    using Gameplay.GameActions;
    using Gameplay.Items;

    using Shared.Localization;
    using Shared.Networking;
    using Shared.Math;
    
    using Core.Controller;

    [Eco, LocCategory("Power"), LocDescription("Triggered when a world object drew power from a power grid.")]
    public class PowerConsumed : GameAction, IUserGameAction, IPositionGameAction
    {
        [Eco, LocDescription("The citizen who is responsible for consuming the power."), CanAutoAssign] public User Citizen { get; set; }

        [Eco, LocDescription("The location of the object consuming the power.")] public Vector3i ActionLocation { get; set; }

        [Eco, LocDescription("The object consuming the power."), RequiredTag("World Object")] public Item PowerConsumer { get; set; }

        [Eco, LocDescription("What interval of time was spent consuming?")] public float TimeConsuming { get; set; }

        [Eco, LocDescription("Is the power generated electrical in nature?")] public bool IsElectrical { get; set; }

        [Eco, LocDescription("Is the power generated mechanical in nature?")] public bool IsMechanical { get; set; }

        [Eco, LocDescription("How much power, in Joules, was actually consumed by this generator.")] public float PowerUsed { get; set; }
    }
}
