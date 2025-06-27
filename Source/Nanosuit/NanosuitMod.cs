using Mlie;
using UnityEngine;
using Verse;

namespace Nanosuit;

internal class NanosuitMod : Mod
{
    public static NanosuitSettings Settings;
    public static string CurrentVersion;

    public NanosuitMod(ModContentPack pack) : base(pack)
    {
        Settings = GetSettings<NanosuitSettings>();
        CurrentVersion = VersionFromManifest.GetVersionFromModMetaData(pack.ModMetaData);
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        Settings.DoSettingsWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "Nanosuit";
    }
}