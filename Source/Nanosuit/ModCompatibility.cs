using System;
using HarmonyLib;
using Verse;

namespace Nanosuit;

[StaticConstructorOnStartup]
public static class ModCompatibility
{
    public static Type ShieldMechBubbleType;
    public static Type ArchotechShieldBelt;
    public static Type RangedShieldBelt;

    static ModCompatibility()
    {
        if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Save Our Ship 2"))
        {
            try
            {
                var type = AccessTools.TypeByName("ShipInteriorMod2");
                HarmonyContainer.harmony.Patch(AccessTools.Method(type, "HasSpaceSuitSlow"), null,
                    new HarmonyMethod(typeof(ModCompatibility), "HasSpaceSuitSlow_Postfix"));
                HarmonyContainer.harmony.Patch(AccessTools.Method(type, "hasSpaceSuit"), null,
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