using UnityEngine;
using Verse;

namespace Nanosuit;

internal class NanosuitSettings : ModSettings
{
    public bool nanosuitModesAtTheSameTime;
    public bool useSkipForJumping;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref nanosuitModesAtTheSameTime, "nanosuitModesAtTheSameTime");
        Scribe_Values.Look(ref useSkipForJumping, "useSkipForJumping");
    }

    public void DoSettingsWindowContents(Rect inRect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);
        listingStandard.CheckboxLabeled("NS.MultipleModesAtTheSameTime".Translate(), ref nanosuitModesAtTheSameTime);
        listingStandard.CheckboxLabeled("NS.UseSkipForJumping".Translate(), ref useSkipForJumping);
        if (NanosuitMod.CurrentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("NS.CurrentModVersion".Translate(NanosuitMod.CurrentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
        Write();
    }
}