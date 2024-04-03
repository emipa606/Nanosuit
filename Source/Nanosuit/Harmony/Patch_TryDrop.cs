using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.TryDrop),
    [typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool)],
    [ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal])]
public static class Patch_TryDrop
{
    private static void Postfix(Pawn_ApparelTracker __instance, Apparel ap)
    {
        if (ap?.def != NS_DefOf.NS_Apparel_Nanosuit)
        {
            return;
        }

        var apparel =
            __instance.pawn.apparel?.WornApparel.FirstOrDefault(x => x.def == NS_DefOf.NS_Apparel_NanosuitHelmet);
        if (apparel == null)
        {
            return;
        }

        __instance.pawn.apparel.TryDrop(apparel, out var resultingAp);
        resultingAp.Destroy();
    }
}