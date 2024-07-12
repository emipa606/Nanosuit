using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
public static class VerbProperties_AdjustedAccuracy
{
    public static void Postfix(ref float __result, Thing equipment)
    {
        if (equipment is not { ParentHolder: Pawn_EquipmentTracker { pawn: not null } pawn_EquipmentTracker })
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