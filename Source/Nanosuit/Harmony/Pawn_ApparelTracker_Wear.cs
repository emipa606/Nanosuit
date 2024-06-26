﻿using HarmonyLib;
using RimWorld;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Wear))]
public static class Pawn_ApparelTracker_Wear
{
    public static void Postfix(Apparel newApparel, Pawn_ApparelTracker __instance)
    {
        var pawn = __instance?.pawn;
        if (pawn != null && newApparel is Apparel_Nanosuit nanosuit && nanosuit.def.hardRemoval != null)
        {
            __instance.Lock(newApparel);
        }
    }
}