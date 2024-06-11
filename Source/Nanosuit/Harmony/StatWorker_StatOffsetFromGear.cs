using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.StatOffsetFromGear))]
public class StatWorker_StatOffsetFromGear
{
    public static void Postfix(ref float __result, Thing gear, StatDef stat)
    {
        if (stat != StatDefOf.ShootingAccuracyPawn || gear is not Apparel_Nanosuit nanosuit)
        {
            return;
        }

        if (nanosuit.NightVisionWorks())
        {
            __result += nanosuit.def.nightVisor.accuracyBonus;
        }
    }
}