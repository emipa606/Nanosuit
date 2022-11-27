using HarmonyLib;
using Verse;

namespace Nanosuit;

[HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
public static class Patch_AdjustedAccuracy
{
    public static void Postfix(ref float __result, Thing equipment)
    {
        if (equipment.ParentHolder is not Pawn_EquipmentTracker { pawn: { } } pawn_EquipmentTracker)
        {
            return;
        }

        foreach (var apparel in pawn_EquipmentTracker.pawn.GetNanosuits())
        {
            if (!apparel.IsActive(ApparelMode.StrengthMode))
            {
                continue;
            }

            if (__result < apparel.def.strengthMode.minAccuracyValue)
            {
                __result = apparel.def.strengthMode.minAccuracyValue;
            }
        }
    }
}