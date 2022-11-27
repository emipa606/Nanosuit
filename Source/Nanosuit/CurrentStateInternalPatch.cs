﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit;

[HarmonyPatch(typeof(ThoughtWorker_PsychicDrone), "CurrentStateInternal")]
public static class CurrentStateInternalPatch
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