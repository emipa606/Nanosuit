using Verse;

namespace Nanosuit.Harmony;

[StaticConstructorOnStartup]
internal static class HarmonyContainer
{
    public static HarmonyLib.Harmony harmony;

    static HarmonyContainer()
    {
        harmony = new HarmonyLib.Harmony("Remo.Nanosuit");
        harmony.PatchAll();
    }
}