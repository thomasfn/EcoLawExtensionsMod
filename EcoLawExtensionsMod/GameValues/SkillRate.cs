using System;

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

    [Eco, LocCategory("Citizens"), LocDescription("The citizen's current skill rate from food and/or housing.")]
    public class SkillRate : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The citizen whose skill rate is being calculated.")] public GameValue<User> Citizen { get; set; }

        [Eco, Advanced, LocDescription("Whether to consider skill rate from food bonuses.")] public GameValue<bool> ConsiderFood { get; set; } = new Yes();

        [Eco, Advanced, LocDescription("Whether to consider skill rate from housing.")] public GameValue<bool> ConsiderHousing { get; set; } = new Yes();

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var user = this.Citizen?.Value(action); if (user?.Val == null) return this.FailNullSafeFloat(user, nameof(this.Citizen));
            var considerFood = this.ConsiderFood?.Value(action); if (considerFood?.Val == null) return this.FailNullSafeFloat(user, nameof(this.ConsiderFood));
            var considerHousing = this.ConsiderHousing?.Value(action); if (considerHousing?.Val == null) return this.FailNullSafeFloat(user, nameof(this.ConsiderHousing));

            float skillRate = 0.0f;
            if (considerFood.Val) { skillRate += user.Val?.Stomach.NutrientSkillRate() ?? 0.0f; }
            if (considerHousing.Val) { skillRate += user.Val?.ResidencyPropertyValue?.Value ?? 0.0f; }
            return Eval.Make($"{Text.StyledNum(skillRate)} ({user?.Val.UILink()}'s current skill rate {DescribeConsiderFoodHousing(considerFood.Val, considerHousing.Val)})", skillRate);
        }

        public override LocString Description()
            => Localizer.Do($"current skill rate of {this.Citizen.DescribeNullSafe()} {DescribeConsiderFoodHousing()}");

        private LocString DescribeConsiderFoodHousing()
        {
            if ((ConsiderFood is Yes || ConsiderFood is No) && (ConsiderHousing is Yes || ConsiderHousing is No))
            {
                return DescribeConsiderFoodHousing(ConsiderFood is Yes, ConsiderHousing is Yes);
            }
            else
            {
                return Localizer.Do($"from food when {ConsiderFood.DescribeNullSafe()} and housing when {ConsiderHousing.DescribeNullSafe()}");
            }
        }

        private static LocString DescribeConsiderFoodHousing(bool considerFood, bool considerHousing)
        {
            if (considerFood && !considerHousing)
            {
                return Localizer.DoStr("from food only");
            }
            else if (!considerFood && considerHousing)
            {
                return Localizer.DoStr("from housing only");
            }
            else if (considerFood && considerHousing)
            {
                return Localizer.DoStr("from food and housing");
            }
            else
            {
                return Localizer.DoStr("from nothing");
            }
        }
    }
}
