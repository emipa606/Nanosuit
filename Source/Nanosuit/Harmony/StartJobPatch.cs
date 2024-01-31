using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
public class StartJobPatch
{
    private static bool Prefix(Pawn ___pawn, Job newJob)
    {
        if (newJob.def != JobDefOf.Goto)
        {
            return true;
        }

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

            var distance = newJob.targetA.Cell.DistanceTo(___pawn.Position);
            if (!(apparel.Energy >= apparel.def.speedMode.jumpEnergyConsumption)
                || !(distance <= apparel.def.speedMode.jumpMaxDistance) || !(distance > 3)
                || !Rand.Chance(apparel.def.speedMode.jumpChance))
            {
                continue;
            }

            apparel.Jump(newJob.targetA.Cell, apparel.def.speedMode.jumpEnergyConsumption);
            return false;
        }

        return true;
    }
}