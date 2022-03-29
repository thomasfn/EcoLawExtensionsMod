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

    using Simulation.WorldLayers;

    [Eco, LocCategory("World"), LocDescription("The value of a world layer at a location.")]
    public class LayerValueAt : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The position to test.")] public GameValue<Vector3i> Location { get; set; }

        [Eco, AllowNullInView, AllowEmpty, LocDescription("The world layer to read."), ] public string WorldLayer { get; set; }

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var location = this.Location?.Value(action); if (location?.Val == null) return this.FailNullSafeFloat(location, nameof(this.Location));
            if (string.IsNullOrEmpty(WorldLayer)) return Eval.Make($"Invalid WorldLayer specified on {GetType().GetLocDisplayName()}.", float.MinValue);

            var layer = WorldLayerManager.Obj.GetLayer(WorldLayer);
            if (layer == null) return Eval.Make($"Invalid WorldLayer specified on {GetType().GetLocDisplayName()}.", float.MinValue);

            var value = layer[layer.WorldPosToLayerPos(new Vector2i(location.Val.X, location.Val.Z))];
            return Eval.Make($"{Text.StyledNum(value)} (value of world layer {WorldLayer})", value);
        }
        public override LocString Description() => Localizer.Do($"value of world layer {WorldLayer}");
    }
}
