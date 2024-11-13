﻿using System;
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
    using Shared.IoC;
    using Shared.Voxel;

    using Gameplay.Civics.GameValues;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Objects;
    

    [Eco, LocCategory("World"), LocDescription("How close the nearest world object of a particular type is.")]
    public class DistanceToClosestWorldObject : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The position to test.")] public GameValue<Vector3i> Location { get; set; }

        [Eco, AllowNullInView, AllowEmpty, LocDescription("The object to search for.")] public GamePickerList<WorldObject> ObjectType { get; set; } = new ();

        [Eco, Advanced, LocDescription("Whether to ignore world objects at the target location.")] public GameValue<bool> IgnoreAtLocation { get; set; } = new No();

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var location = this.Location?.Value(action); if (location?.Val == null) return this.FailNullSafeFloat(location, nameof(this.Location));
            var ignoreAtLocation = this.IgnoreAtLocation?.Value(action); if (ignoreAtLocation?.Val == null) return this.FailNullSafeFloat(ignoreAtLocation, nameof(this.IgnoreAtLocation));

            var allRelevantObjects = ServiceHolder<IWorldObjectManager>.Obj.All
                .Where(x => ObjectType.ContainsType(x.GetType()));
            if (!allRelevantObjects.Any())
            {
                return Eval.Make($"{Text.Style(Text.Styles.Currency, "infinite")} (distance to nearest {ObjectType.DescribeEntries(Localizer.DoStr(","))})", float.MaxValue);
            }

            var objectsWithDistances = allRelevantObjects
                            .Select(x => (x, World.WrappedDistance(x.Position, location.Val)));

            if (ignoreAtLocation.Val)
            {
                objectsWithDistances = objectsWithDistances
                    .Where(x => x.Item2 > 0.0f);
            }

            var nearest = objectsWithDistances
                .MinBy(x => x.Item2);

            return Eval.Make($"{Text.StyledNum(nearest.Item2)} (distance to {nearest.x.UILink()})", nearest.Item2);
        }
        public override LocString Description() => Localizer.Do($"distance to nearest {ObjectType.DescribeEntries(Localizer.DoStr(","))}");
    }
}
