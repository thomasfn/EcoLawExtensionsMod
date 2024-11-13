using System;
using System.Linq;

namespace Eco.Mods.LawExtensions
{
    using Core.Controller;
    using Core.Utils;
    using Core.Utils.PropertyScanning;

    using Shared.Localization;
    using Shared.Networking;
    using Shared.Utils;
    using Shared.Math;
    using Shared.Voxel;

    using Gameplay.Civics.GameValues;

    using Simulation;
    using Simulation.Types;

    [Eco, LocCategory("World"), LocDescription("How close the nearest plant of a particular type is.")]
    public class DistanceToClosestPlant : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The position to test.")] public GameValue<Vector3i> Location { get; set; }

        [Eco, AllowNullInView, AllowEmpty, LocDescription("The plant to search for.")] public GamePickerList<PlantSpecies> PlantType { get; set; } = new ();

        [Eco, Advanced, LocDescription("Whether to ignore plants at the target location.")] public GameValue<bool> IgnoreAtLocation { get; set; } = new No();

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var location = this.Location?.Value(action); if (location?.Val == null) return this.FailNullSafeFloat(location, nameof(this.Location));
            var ignoreAtLocation = this.IgnoreAtLocation?.Value(action); if (ignoreAtLocation?.Val == null) return this.FailNullSafeFloat(ignoreAtLocation, nameof(this.IgnoreAtLocation));

            var allRelevantPlants = EcoSim.PlantSim.All
                .Where(x => PlantType.ContainsType(x.Species.GetType()));
            if (!allRelevantPlants.Any())
            {
                return Eval.Make($"{Text.Style(Text.Styles.Currency, "infinite")} (distance to nearest {PlantType.DescribeEntries(Localizer.DoStr(","))})", float.MaxValue);
            }

            var plantsWithDistances = allRelevantPlants
                .Select(x => (x, World.WrappedDistance(x.Position, location.Val)));

            if (ignoreAtLocation.Val)
            {
                plantsWithDistances = plantsWithDistances
                    .Where(x => x.Item2 > 0.0f);
            }

            var nearest = plantsWithDistances
                .MinBy(x => x.Item2);

            return Eval.Make($"{Text.StyledNum(nearest.Item2)} (distance to {nearest.x.Species.DisplayName})", nearest.Item2);
        }
        public override LocString Description() => Localizer.Do($"distance to nearest {PlantType.DescribeEntries(Localizer.DoStr(","))}");
    }
}
