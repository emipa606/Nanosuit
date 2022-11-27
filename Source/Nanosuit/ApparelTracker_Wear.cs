using HarmonyLib;
using RimWorld;

namespace Nanosuit;

[HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
public static class ApparelTracker_Wear
{
    public static void Postfix(Apparel newApparel, Pawn_ApparelTracker __instance)
    {
        var pawn = __instance?.pawn;
        if (pawn != null && newApparel is Apparel_Nanosuit nanosuit && nanosuit.def.hardRemoval != null)
        {
            __instance.Lock(newApparel);
        }
    }
}