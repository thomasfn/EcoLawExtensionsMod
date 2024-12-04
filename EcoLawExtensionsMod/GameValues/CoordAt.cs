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

    using Gameplay.Civics.GameValues;

    [Eco, LocCategory("World"), LocDescription("The X coord of a location.")]
    public class XCoordAt : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The position to read the X coord of.")] public GameValue<Vector3i> Location { get; set; }

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var location = this.Location?.Value(action); if (location?.Val == null) return this.FailNullSafeFloat(location, nameof(this.Location));
            
            var value = location.Val.X;
            return Eval.Make($"{Text.StyledNum(value)} (X coord)", (float)value);
        }
        public override LocString Description() => Localizer.Do($"X coord");
    }

    [Eco, LocCategory("World"), LocDescription("The Z coord of a location.")]
    public class ZCoordAt : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The position to read the Z coord of.")] public GameValue<Vector3i> Location { get; set; }

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var location = this.Location?.Value(action); if (location?.Val == null) return this.FailNullSafeFloat(location, nameof(this.Location));
            
            var value = location.Val.Z;
            return Eval.Make($"{Text.StyledNum(value)} (Z coord)", (float)value);
        }
        public override LocString Description() => Localizer.Do($"Z coord");
    }
}
