using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.Notify_Teleported))]
public static class Pawn_Notify_Teleported
{
    public static bool preventEndingJob;

    public static void Prefix(ref bool endCurrentJob)
    {
        if (preventEndingJob)
        {
            endCurrentJob = false;
        }
    }
}