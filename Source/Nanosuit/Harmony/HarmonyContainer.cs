using Verse;

namespace Nanosuit.Harmony;

[StaticConstructorOnStartup]
internal static class HarmonyContainer
{
    static HarmonyContainer()
    {
        new HarmonyLib.Harmony("Remo.Nanosuit").PatchAll();
    }
}