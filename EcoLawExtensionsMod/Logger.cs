namespace Eco.Mods.LawExtensions
{
    using Shared.Localization;
    using Shared.Logging;

    public static class Logger
    {
        public static void Debug(string message)
        {
            Log.Write(Localizer.Do($"[LawExtensions] DEBUG: {message}\n"));
        }

        public static void Info(string message)
        {
            Log.Write(Localizer.Do($"[LawExtensions] {message}\n"));
        }

        public static void Error(string message)
        {
            Log.Write(Localizer.Do($"[LawExtensions] ERROR: {message}\n"));
        }
    }
}