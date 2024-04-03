using System;
using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[StaticConstructorOnStartup]
public static class ModCompatibility
{
    public static readonly Type ShieldMechBubbleType;
    public static readonly Type ArchotechShieldBelt;
    public static readonly Type RangedShieldBelt;

    static ModCompatibility()
    {
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Save Our Ship 2"))
        {
            try
            {
                var type = AccessTools.TypeByName("ShipInteriorMod2");
                var harmony = new HarmonyLib.Harmony("Remo.Nanosuit");
                harmony.Patch(AccessTools.Method(type, "HasSpaceSuitSlow"), null,
                    new HarmonyMethod(typeof(ModCompatibility), "HasSpaceSuitSlow_Postfix"));
                harmony.Patch(AccessTools.Method(type, "hasSpaceSuit"), null,
                    new HarmonyMethod(typeof(ModCompatibility), "hasSpaceSuit_Postfix"));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        ShieldMechBubbleType = AccessTools.TypeByName("ShieldMechBubble");
        ArchotechShieldBelt = AccessTools.TypeByName("VFEI.ArchotechShieldBelt");
        RangedShieldBelt = AccessTools.TypeByName("RangedShieldBelt");
    }

    public static void hasSpaceSuit_Postfix(Pawn pawn, ref bool __result)
    {
        if (!__result)
        {
            __result = HasSpaceProtection(pawn);
        }
    }

    public static void HasSpaceSuitSlow_Postfix(Pawn pawn, ref bool __result)
    {
        if (!__result)
        {
            __result = HasSpaceProtection(pawn);
        }
    }

    private static bool HasSpaceProtection(Pawn pawn)
    {
        foreach (var apparel in pawn.GetNanosuits())
        {
            if (apparel.def.rebreather)
            {
                return true;
            }
        }

        return false;
    }
}