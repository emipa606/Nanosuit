using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(ThoughtWorker_PsychicDrone), "CurrentStateInternal")]
public static class ThoughtWorker_PsychicDrone_CurrentStateInternal
{
    public static void Postfix(Pawn p, ref ThoughtState __result)
    {
        if (__result.StageIndex < 0)
        {
            return;
        }

        foreach (var apparel in p.GetNanosuits())
        {
            if (apparel.def.psychicWaveControl != null)
            {
                __result = false;
            }
        }
    }
}