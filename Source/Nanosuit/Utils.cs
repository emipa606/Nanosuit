using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nanosuit;

public static class Utils
{
    private static readonly HashSet<JobDef> combatJobs =
    [
        JobDefOf.AttackMelee,
        JobDefOf.AttackStatic,
        JobDefOf.FleeAndCower,
        JobDefOf.ManTurret,
        JobDefOf.Wait_Combat,
        JobDefOf.Flee
    ];

    public static IEnumerable<Apparel_Nanosuit> GetNanosuits(this Pawn pawn)
    {
        var apparels = pawn.apparel?.WornApparel?.OfType<Apparel_Nanosuit>();
        if (apparels == null)
        {
            yield break;
        }

        foreach (var apparel in apparels)
        {
            yield return apparel;
        }
    }

    public static bool InCombat(this Pawn pawn)
    {
        if (combatJobs.Contains(pawn.CurJobDef))
        {
            return true;
        }

        if (pawn.mindState?.duty?.def.alwaysShowWeapon ?? false)
        {
            return true;
        }

        return pawn.CurJobDef?.alwaysShowWeapon ?? false;
    }

    public static bool ShouldBeDowned(this Pawn pawn)
    {
        if (!pawn.health.InPainShock && pawn.health.capacities.CanBeAwake)
        {
            return !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Moving);
        }

        return true;
    }
}