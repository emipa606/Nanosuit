﻿using System;
using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Pawn_CarryTracker), nameof(Pawn_CarryTracker.TryDropCarriedThing),
    [typeof(IntVec3), typeof(ThingPlaceMode), typeof(Thing), typeof(Action<Thing, int>)], [0, 0, ArgumentType.Out, 0])]
public static class Pawn_CarryTracker_TryDropCarriedThing
{
    public static Pawn pawn;

    public static bool Prefix(Pawn_CarryTracker __instance)
    {
        if (__instance.pawn != pawn)
        {
            return true;
        }

        pawn = null;
        return false;
    }
}