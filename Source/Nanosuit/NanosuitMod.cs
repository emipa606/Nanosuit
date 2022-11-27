using Mlie;
using UnityEngine;
using Verse;

namespace Nanosuit;

internal class NanosuitMod : Mod
{
    public static NanosuitSettings settings;
    public static string currentVersion;

    public NanosuitMod(ModContentPack pack) : base(pack)
    {
        settings = GetSettings<NanosuitSettings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(ModLister.GetActiveModWithIdentifier("Mlie.Nanosuit"));
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Nanosuit";
    }
}