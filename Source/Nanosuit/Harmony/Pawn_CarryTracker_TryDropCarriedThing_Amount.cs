using System;
using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Pawn_CarryTracker), nameof(Pawn_CarryTracker.TryDropCarriedThing),
    [typeof(IntVec3), typeof(int), typeof(ThingPlaceMode), typeof(Thing), typeof(Action<Thing, int>)],
    [0, 0, 0, ArgumentType.Out, 0])]
public static class Pawn_CarryTracker_TryDropCarriedThing_Amount
{
    public static bool Prefix(Pawn_CarryTracker __instance)
    {
        if (__instance.pawn != Pawn_CarryTracker_TryDropCarriedThing.pawn)
        {
            return true;
        }

        Pawn_CarryTracker_TryDropCarriedThing.pawn = null;
        return false;
    }
}