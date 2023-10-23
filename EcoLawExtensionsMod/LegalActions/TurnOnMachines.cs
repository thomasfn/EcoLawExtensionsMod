using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.Mods.LawExtensions
{
    using Core.Controller;
    using Core.Utils.PropertyScanning;

    using Shared.Localization;
    using Shared.Networking;
    using Shared.Utils;

    using Gameplay.Civics;
    using Gameplay.Civics.GameValues;
    using Gameplay.Civics.Laws;
    using Gameplay.GameActions;
    using Gameplay.Aliases;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Objects;
    using Gameplay.Components;
    using Eco.Gameplay.Settlements;

    [Eco, LocCategory("Misc"), CreateComponentTabLoc("Eco Law Extensions", IconName = "Law"), LocDisplayName("Turn On Machines"), LocDescription("Tries to turn on all inactive machines that match a set of conditions.")]
    public class TurnOnMachines_LegalAction : LegalAction
    {
        [Eco, Advanced, LocDescription("The player or group to include on machines for.")]
        public GameValue<IAlias> Target { get; set; } // TODO: Default to "Everyone"

        [Eco, Advanced, LocDescription("Whether to include machines that were turned off by the player.")] public GameValue<bool> ByPlayer { get; set; } = new Yes();

        [Eco, Advanced, LocDescription("Whether to include machines that were turned off by a law.")] public GameValue<bool> ByLaw { get; set; } = new Yes();

        public override LocString Description()
            => Localizer.Do($"Attempt to turn on {DescribeFilter()} belonging to {this.Target.DescribeNullSafe()}");

        private static LocString DescribeFilter(bool byPlayer, bool byLaw)
        {
            if (byPlayer && byLaw) { return Localizer.DoStr("all machines"); }
            if (!byPlayer && !byLaw) { return Localizer.DoStr("nothing"); }
            return byPlayer
                ? Localizer.DoStr("only machines turned off by the player")
                : Localizer.DoStr("only machines turned off legally");
        }

        private LocString DescribeFilter()
        {
            if ((ByPlayer is Yes || ByPlayer is No) && (ByLaw is Yes || ByLaw is No)) { return DescribeFilter(ByPlayer is Yes, ByLaw is Yes); }
            return Localizer.Do($"machines turned off by the player when {ByPlayer.DescribeNullSafe()} and machines turned off legally when {ByPlayer.DescribeNullSafe()}");
        }

        protected override PostResult Perform(Law law, GameAction action) => this.Do(law.UILinkNullSafe(), action, law?.Settlement);
        //PostResult IExecutiveAction.PerformExecutiveAction(User user, IContextObject context) => this.Do(Localizer.Do($"Executive Action by {(user is null ? Localizer.DoStr("the Executive Office") : user.UILink())}"), context, null);

        private PostResult Do(LocString description, IContextObject context, Settlement jurisdictionSettlement)
        {
            var targetAlias = this.Target?.Value(context).Val;
            var byPlayer = this.ByPlayer?.Value(context).Val ?? false;
            var byLaw = this.ByLaw?.Value(context).Val ?? false;

            var users = targetAlias?.UserSet.ToArray();
            if (users == null || users.Length == 0) { return new PostResult($"Reactivate dormant machines without target citizen(s) skipped.", true); }

            return new PostResult(() =>
            {
                int cnt = 0;
                var allRelevantObjects = WorldObjectUtil.AllObjsWithComponent<OnOffComponent>()
                    .Where(x => x != null && !x.On && (jurisdictionSettlement?.Influences(x.Parent.WorldPosXZi()) ?? true));
                foreach (var onOffComponent in allRelevantObjects)
                {
                    var worldObject = onOffComponent.Parent;
                    if (worldObject == null || worldObject.Owners == null) { continue; }
                    if (targetAlias != null && !worldObject.Owners.UserSet.Any(x => x != null && targetAlias.ContainsUser(x))) { continue; }
                    var statusComponent = worldObject.GetComponent<StatusComponent>();
                    if (statusComponent == null) { continue; }
                    var wasTurnedOffByLaw = statusComponent.Statuses.Any(x => x != null && DoesStatusSayIllegal(x.Message).GetValueOrDefault());
                    if ((wasTurnedOffByLaw && byLaw) || (!wasTurnedOffByLaw && byPlayer))
                    {
                        onOffComponent.SetOnOff(null, true);
                        ++cnt;
                    }
                }
                return Localizer.Do($"Turning on {cnt} machines belonging to {targetAlias.UILinkGeneric()}");
            });
        }

        private bool? DoesStatusSayIllegal(LocString locString)
        {
            // One of "Legally allowed to pollute." or "Legally prevented from polluting. Turn on to retry."
            // But what about translations, if the server is in a different language? Let's not worry about that for now...
            var rawMsg = locString.ToString();
            if (rawMsg.Contains("Legally allowed")) { return false; }
            if (rawMsg.Contains("Legally prevented")) { return true; }
            return null;
        }
    }
}
