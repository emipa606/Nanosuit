using System;
using HarmonyLib;
using Verse;

namespace Nanosuit;

[HarmonyPatch(typeof(Pawn_CarryTracker))]
[HarmonyPatch("TryDropCarriedThing")]
[HarmonyPatch(new[]
{
    typeof(IntVec3),
    typeof(ThingPlaceMode),
    typeof(Thing),
    typeof(Action<Thing, int>)
}, new ArgumentType[]
{
    0,
    0,
    ArgumentType.Out,
    0
})]
public static class TryDropCarriedThingPatch
{
    public static Pawn pawn;

    public static bool Prefix(Pawn_CarryTracker __instance, IntVec3 dropLoc, ThingPlaceMode mode, Thing resultingThing,
        Action<Thing, int> placedAction = null)
    {
        if (__instance.pawn != pawn)
        {
            return true;
        }

        pawn = null;
        return false;
    }
}