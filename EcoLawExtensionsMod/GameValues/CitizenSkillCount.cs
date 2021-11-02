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

    using Gameplay.Civics.GameValues;
    using Gameplay.Systems.TextLinks;
    using Gameplay.Players;

    [Eco, LocCategory("Citizens"), LocDescription("The number of skills held by a citizen, including Self Improvement.")]
    public class CitizenSkillCount : GameValue<float>
    {
        [Eco, Advanced, LocDescription("The legal person whose company's skill count is being evaluated.")] public GameValue<User> Citizen { get; set; }

        [Eco, Advanced, LocDescription("Whether to also consider skills that are learnt but not specialised.")] public GameValue<bool> IncludeUnspecialised { get; set; } = new No();

        private Eval<float> FailNullSafeFloat<T>(Eval<T> eval, string paramName) =>
            eval != null ? Eval.Make($"Invalid {Localizer.DoStr(paramName)} specified on {GetType().GetLocDisplayName()}: {eval.Message}", float.MinValue)
                         : Eval.Make($"{Localizer.DoStr(paramName)} not set on {GetType().GetLocDisplayName()}.", float.MinValue);

        public override Eval<float> Value(IContextObject action)
        {
            var citizen = this.Citizen?.Value(action); if (citizen?.Val == null) return this.FailNullSafeFloat(citizen, nameof(this.Citizen));
            var includeUnspecialised = this.IncludeUnspecialised?.Value(action); if (includeUnspecialised?.Val == null) return this.FailNullSafeFloat(includeUnspecialised, nameof(this.IncludeUnspecialised));

            var skills = citizen.Val.Skillset.Skills
                .Where(skill => skill.Level >= (includeUnspecialised.Val ? 0 : 1) && skill.RootSkillTree != skill.SkillTree)
                .Select(skill => skill.GetType());
            float skillCount = skills.Count();

            return Eval.Make($"{Text.StyledNum(skillCount)} ({(includeUnspecialised.Val ? "learnt " : "specialised ")}skill count of {citizen.Val.UILink()})", skillCount);
        }

        public override LocString Description() => Localizer.Do($"({(IncludeUnspecialised is Yes ? "learnt " : IncludeUnspecialised is No ? "specialised " : $"learnt (when {IncludeUnspecialised.DescribeNullSafe()}, specialised otherwise) ")}skill count of {Citizen.DescribeNullSafe()}");
    }
}
