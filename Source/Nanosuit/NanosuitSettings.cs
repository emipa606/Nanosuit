using UnityEngine;
using Verse;

namespace Nanosuit;

internal class NanosuitSettings : ModSettings
{
    private static Vector2 scrollPosition = Vector2.zero;
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
        if (NanosuitMod.currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("NS.CurrentModVersion".Translate(NanosuitMod.currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
        Write();
    }
}