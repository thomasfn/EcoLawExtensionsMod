namespace Eco.Mods.LawExtensions
{
    using Core.Plugins.Interfaces;

    public class LawExtensionsMod : IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "LawExtensions",
            ModDescription = "Extends the law system with a number of helpful utility game values and legal actions.",
            ModDisplayName = "Law Extensions",
        };
    }
}
