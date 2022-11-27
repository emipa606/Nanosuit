using HarmonyLib;
using Verse;
using Verse.AI;

namespace Nanosuit;

[HarmonyPatch(typeof(Pawn_PathFollower), "StartPath")]
public class StartPathPatch
{
    private static void Postfix(Pawn_PathFollower __instance, Pawn ___pawn)
    {
        foreach (var apparel in ___pawn.GetNanosuits())
        {
            if (!apparel.IsActive(ApparelMode.SpeedMode))
            {
                continue;
            }

            var inCombat = ___pawn.InCombat();
            if ((!inCombat || !apparel.jumpModeInCombat) && (inCombat || !apparel.jumpModeOutsideCombat))
            {
                continue;
            }

            var distance = __instance.Destination.Cell.DistanceTo(___pawn.Position);
            if (apparel.Energy >= apparel.def.speedMode.jumpEnergyConsumption
                && distance <= apparel.def.speedMode.jumpMaxDistance && distance > 3
                && Rand.Chance(apparel.def.speedMode.jumpChance))
            {
                apparel.Jump(__instance.Destination.Cell, apparel.def.speedMode.jumpEnergyConsumption);
            }
        }
    }
}