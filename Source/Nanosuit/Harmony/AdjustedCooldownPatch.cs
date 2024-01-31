using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Tool), "AdjustedCooldown", typeof(Thing))]
internal class AdjustedCooldownPatch
{
    private static void Postfix(ref float __result, Thing ownerEquipment)
    {
        if (ownerEquipment == null || !(__result > 0) || ownerEquipment.ParentHolder?.ParentHolder == null)
        {
            return;
        }

        if (ownerEquipment.ParentHolder.ParentHolder is not Pawn pawn ||
            ownerEquipment.def is not { IsMeleeWeapon: true })
        {
            return;
        }

        foreach (var apparel in pawn.GetNanosuits())
        {
            if (!apparel.IsActive(ApparelMode.SpeedMode))
            {
                continue;
            }

            if (!apparel.def.speedMode.meleeCooldownFactor.HasValue ||
                !(apparel.Energy >= apparel.def.speedMode.meleeCooldownFactorEnergyConsumption))
            {
                continue;
            }

            __result *= apparel.def.speedMode.meleeCooldownFactor.Value;
            apparel.Energy -= apparel.def.speedMode.meleeCooldownFactorEnergyConsumption;
        }
    }
}