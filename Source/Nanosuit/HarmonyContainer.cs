using HarmonyLib;
using Verse;

namespace Nanosuit;

[StaticConstructorOnStartup]
internal static class HarmonyContainer
{
    public static Harmony harmony;

    static HarmonyContainer()
    {
        harmony = new Harmony("Remo.Nanosuit");
        harmony.PatchAll();
    }
}