using HarmonyLib;
using RimWorld;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(StunHandler), nameof(StunHandler.StunFor))]
public static class StunHandler_StunFor
{
    public static bool Prefix()
    {
        return !Pawn_Notify_Teleported.preventEndingJob;
    }
}