using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(PawnFlyer), "RespawnPawn")]
public class PawnFlyer_RespawnPawn
{
    public static void Prefix(PawnFlyer __instance, out Pawn __state)
    {
        Pawn_CarryTracker_TryDropCarriedThing.pawn = null;
        __state = __instance.FlyingPawn;
    }

    public static void Postfix(Pawn __state)
    {
        if (__state == null || !__state.GetNanosuits().Any())
        {
            return;
        }

        if (__state.carryTracker.CarriedThing != null)
        {
            __state.carryTracker.TryDropCarriedThing(__state.Position, ThingPlaceMode.Direct,
                out _);
        }
    }
}