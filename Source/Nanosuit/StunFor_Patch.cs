using HarmonyLib;
using RimWorld;

namespace Nanosuit;

[HarmonyPatch(typeof(StunHandler), "StunFor")]
public class StunFor_Patch
{
    private static bool Prefix()
    {
        return !Notify_Teleported.preventEndingJob;
    }
}