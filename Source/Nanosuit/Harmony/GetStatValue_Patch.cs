using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
public static class GetStatValue_Patch
{
    private static void Postfix(Thing thing, StatDef stat, ref float __result)
    {
        if (thing is not Pawn pawn)
        {
            return;
        }

        if (stat != StatDefOf.MoveSpeed)
        {
            return;
        }

        foreach (var apparel in pawn.GetNanosuits())
        {
            if (apparel.IsActive(ApparelMode.SpeedMode) && apparel.inSpeedState)
            {
                __result *= apparel.def.speedMode.movementSpeedFactor;
            }
        }
    }
}