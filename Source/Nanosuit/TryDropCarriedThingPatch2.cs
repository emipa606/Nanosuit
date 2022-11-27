using System;
using HarmonyLib;
using Verse;

namespace Nanosuit;

[HarmonyPatch(typeof(Pawn_CarryTracker))]
[HarmonyPatch("TryDropCarriedThing")]
[HarmonyPatch(new[]
{
    typeof(IntVec3),
    typeof(int),
    typeof(ThingPlaceMode),
    typeof(Thing),
    typeof(Action<Thing, int>)
}, new ArgumentType[]
{
    0,
    0,
    0,
    ArgumentType.Out,
    0
})]
public static class TryDropCarriedThingPatch2
{
    public static bool Prefix(Pawn_CarryTracker __instance, int count, IntVec3 dropLoc, ThingPlaceMode mode,
        Thing resultingThing, Action<Thing, int> placedAction = null)
    {
        if (__instance.pawn != TryDropCarriedThingPatch.pawn)
        {
            return true;
        }

        TryDropCarriedThingPatch.pawn = null;
        return false;
    }
}