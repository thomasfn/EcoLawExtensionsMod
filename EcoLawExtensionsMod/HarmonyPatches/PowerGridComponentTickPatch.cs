using HarmonyLib;

namespace Eco.Mods.LawExtensions.HarmonyPatches
{
    using Gameplay.PowerGrids;

    [HarmonyPatch(typeof(PowerGridManager), "Tick")]
    internal class PowerGridManagerTickPatch
    {
        internal static void Prefix(PowerGridManager __instance)
        {
            PowerGridLawManager.Obj.PowerGridManagerPreTick(__instance);
        }

        internal static void Postfix(PowerGridManager __instance)
        {
            PowerGridLawManager.Obj.PowerGridManagerPostTick(__instance);
        }
    }
}
