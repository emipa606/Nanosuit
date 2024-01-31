using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Verb_MeleeAttackDamage), "ApplyMeleeDamageToTarget")]
public static class Patch_ApplyMeleeDamageToTarget
{
    public static void Prefix(Verb_MeleeAttackDamage __instance, LocalTargetInfo target)
    {
        if (__instance.CasterPawn == null || target.Thing is not Pawn victim)
        {
            return;
        }

        foreach (var apparel in __instance.CasterPawn.GetNanosuits())
        {
            if (!apparel.IsActive(ApparelMode.StrengthMode) ||
                !Rand.Chance(apparel.def.strengthMode.meleeCriticalChance))
            {
                continue;
            }

            if (apparel.def.strengthMode.meleeCriticalEnergyConsumption > 0 &&
                apparel.Energy < apparel.def.strengthMode.meleeCriticalEnergyConsumption)
            {
                continue;
            }

            var list = (from x in victim.RaceProps.body.AllParts
                where !victim.health.hediffSet.PartIsMissing(x)
                      && x.depth == BodyPartDepth.Outside && x.coverage > 0.1f
                select x).ToList();
            if (list.Count == 0)
            {
                return;
            }

            if (list.TryRandomElement(out var bodyPartRecord))
            {
                var missingBodyPart =
                    HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, victim, bodyPartRecord);
                victim.health.AddHediff(missingBodyPart);
            }

            if (apparel.def.strengthMode.meleeCriticalEnergyConsumption > 0)
            {
                apparel.Energy -= apparel.def.strengthMode.meleeCriticalEnergyConsumption;
            }
        }
    }
}