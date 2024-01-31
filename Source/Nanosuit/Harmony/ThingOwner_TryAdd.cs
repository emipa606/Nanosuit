using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(ThingOwner<Thing>))]
[HarmonyPatch("TryAdd")]
[HarmonyPatch([
    typeof(Thing),
    typeof(bool)
])]
internal static class ThingOwner_TryAdd
{
    private static void Postfix(ThingOwner<Thing> __instance, bool __result, Thing item)
    {
        if (!__result || __instance.Owner is not Pawn_ApparelTracker apparelTracker ||
            item?.def != NS_DefOf.NS_Apparel_Nanosuit)
        {
            return;
        }

        var apparel = ThingMaker.MakeThing(NS_DefOf.NS_Apparel_NanosuitHelmet) as Apparel;
        apparelTracker.pawn.apparel.Wear(apparel);
    }
}