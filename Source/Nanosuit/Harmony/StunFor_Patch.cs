using HarmonyLib;
using RimWorld;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(StunHandler), nameof(StunHandler.StunFor))]
public class StunFor_Patch
{
    private static bool Prefix()
    {
        return !Notify_Teleported.preventEndingJob;
    }
}