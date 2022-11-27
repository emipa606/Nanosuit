using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Nanosuit;

public class Recipe_RemoveNanosuit : Recipe_Surgery
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (thing is not Pawn pawn)
        {
            return false;
        }

        foreach (var apparel in pawn.GetNanosuits())
        {
            if (apparel.def.hardRemoval != null && apparel.def.hardRemoval.surgeryList.Contains(recipe))
            {
                return true;
            }
        }

        return false;
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        MedicalRecipesUtility.IsClean(pawn, part);
        if (billDoer != null)
        {
            if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
            {
                return;
            }

            TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);

            var apparel = pawn
                .GetNanosuits()
                .FirstOrDefault(x => x.def.hardRemoval != null && x.def.hardRemoval.surgeryList.Contains(recipe));
            pawn.apparel.TryDrop(apparel);
        }

        if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
        {
            ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
        }
    }
}