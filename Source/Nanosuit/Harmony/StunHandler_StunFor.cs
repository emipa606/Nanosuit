using HarmonyLib;
using RimWorld;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(StunHandler), nameof(StunHandler.StunFor))]
public class StunHandler_StunFor
{
    private static bool Prefix()
    {
        return !Pawn_Notify_Teleported.preventEndingJob;
    }
}