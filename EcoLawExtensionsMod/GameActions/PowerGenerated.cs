using System;
using System.Collections.Generic;

namespace Eco.Mods.LawExtensions.GameActions
{
    using Gameplay.Players;
    using Gameplay.GameActions;
    using Gameplay.Items;
    using Gameplay.Settlements;

    using Shared.Localization;
    using Shared.Networking;
    using Shared.Math;
    
    using Core.Controller;

    using Stats;

    [NoStats, Eco, LocCategory("Power"), LocDescription("Triggered when a world object made power available to a power grid.")]
    public class PowerGenerated : GameAction, IUserGameAction, IPositionGameAction
    {
        [Eco, LocDescription("The citizen who is responsible for generating the power."), CanAutoAssign] public User Citizen { get; set; }

        [Eco, LocDescription("The location of the object generating the power.")] public Vector3i ActionLocation { get; set; }

        [Eco, LocDescription("The object generating the power."), RequiredTag("World Object")] public IPlaceableItem PowerGenerator { get; set; }

        [Eco, LocDescription("What interval of time was spent generating?")] public float TimeGenerating { get; set; }

        [Eco, LocDescription("Is the power generated electrical in nature?")] public bool IsElectrical { get; set; }

        [Eco, LocDescription("Is the power generated mechanical in nature?")] public bool IsMechanical { get; set; }

        [Eco, LocDescription("How much power, in Joules, was made available to the power grid by this generator.")] public float PowerAvailable { get; set; }

        [Eco, LocDescription("How much power, in Joules, was actually produced by this generator. This could be '0' if the generator is disabled by the power grid to save resources.")] public float PowerProduced { get; set; }

        public override IEnumerable<Settlement> SettlementScopes => SettlementUtils.GetSettlementsAtPos(this.ActionLocation); //Scope based on position
    }
}
