using System;
using System.Collections.Generic;
using System.Linq;

namespace Eco.Mods.LawExtensions
{
    using Core.Controller;
    using Core.Utils;
    using Core.Utils.PropertyScanning;

    using Shared.Localization;
    using Shared.Networking;
    using Shared.Utils;

    using Gameplay.Civics.GameValues;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Players;

    [Eco, LocCategory("Citizens"), LocDescription("The citien's current nutrition values.")]
    public class Nutrition : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The citizen whose nutrition is being calculated.")] public GameValue<User> Citizen { get; set; }

        [Eco, Advanced, LocDescription("Whether to consider carbohydrates (red slice).")] public GameValue<bool> ConsiderCarbs { get; set; } = new Yes();

        [Eco, Advanced, LocDescription("Whether to consider vitamins (green slice).")] public GameValue<bool> ConsiderVits { get; set; } = new Yes();

        [Eco, Advanced, LocDescription("Whether to consider protein (orange slice).")] public GameValue<bool> ConsiderProtein { get; set; } = new Yes();

        [Eco, Advanced, LocDescription("Whether to consider fat (yellow slice).")] public GameValue<bool> ConsiderFat { get; set; } = new Yes();

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var user = this.Citizen?.Value(action); if (user?.Val == null) return this.FailNullSafeFloat(user, nameof(this.Citizen));
            var considerCarbs = this.ConsiderCarbs?.Value(action); if (considerCarbs?.Val == null) return this.FailNullSafeFloat(user, nameof(this.ConsiderCarbs));
            var considerVits = this.ConsiderVits?.Value(action); if (considerVits?.Val == null) return this.FailNullSafeFloat(user, nameof(this.ConsiderVits));
            var considerProtein = this.ConsiderProtein?.Value(action); if (considerProtein?.Val == null) return this.FailNullSafeFloat(user, nameof(this.ConsiderProtein));
            var considerFat = this.ConsiderFat?.Value(action); if (considerFat?.Val == null) return this.FailNullSafeFloat(user, nameof(this.ConsiderFat));

            float nutrition = 0.0f;
            if (considerCarbs.Val) { nutrition += user.Val?.Stomach.Nutrients.Carbs ?? 0.0f; }
            if (considerVits.Val) { nutrition += user.Val?.Stomach.Nutrients.Vitamins ?? 0.0f; }
            if (considerProtein.Val) { nutrition += user.Val?.Stomach.Nutrients.Protein ?? 0.0f; }
            if (considerFat.Val) { nutrition += user.Val?.Stomach.Nutrients.Fat ?? 0.0f; }

            return Eval.Make($"{Text.StyledNum(nutrition)} ({user?.Val.UILink()}'s current {DescribeConsiderNutrition(considerCarbs.Val, considerVits.Val, considerProtein.Val, considerFat.Val)})", nutrition);
        }

        public override LocString Description()
            => Localizer.Do($"current {DescribeConsiderNutrition()} of {this.Citizen.DescribeNullSafe()}");

        private LocString DescribeConsiderNutrition()
        {
            if ((ConsiderCarbs is Yes || ConsiderCarbs is No) && (ConsiderVits is Yes || ConsiderVits is No) && (ConsiderProtein is Yes || ConsiderProtein is No) && (ConsiderFat is Yes || ConsiderFat is No))
            {
                return DescribeConsiderNutrition(ConsiderCarbs is Yes, ConsiderVits is Yes, ConsiderProtein is Yes, ConsiderFat is Yes);
            }
            else
            {
                return Localizer.Do($"nutrition from carbs when {ConsiderCarbs.DescribeNullSafe()}, vitamins when {ConsiderVits.DescribeNullSafe()}, protein when {ConsiderProtein.DescribeNullSafe()} and fat when {ConsiderFat.DescribeNullSafe()}");
            }
        }

        private static LocString DescribeConsiderNutrition(bool considerCarbs, bool considerVits, bool considerProtein, bool considerFat)
        {
            if (considerCarbs && considerVits && considerProtein && considerFat)
            {
                return Localizer.DoStr("overall nutrition");
            }
            else
            {
                var list = new List<string>();
                if (considerCarbs) { list.Add("carbs"); }
                if (considerVits) { list.Add("vitamins"); }
                if (considerProtein) { list.Add("protein"); }
                if (considerFat) { list.Add("fat"); }
                if (list.Count == 0) { return Localizer.DoStr("nutrition from nothing"); }
                if (list.Count == 1) { return Localizer.Do($"nutrition from {list[0]} only"); }
                return Localizer.Do($"nutrition from {string.Join(", ", list.Take(list.Count - 1))} and {list[^1]} only");
            }
        }
    }
}
