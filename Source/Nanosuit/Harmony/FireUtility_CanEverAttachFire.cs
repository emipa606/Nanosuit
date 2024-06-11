using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(FireUtility), nameof(FireUtility.CanEverAttachFire))]
public class FireUtility_CanEverAttachFire
{
    public static void Postfix(Thing t, ref bool __result)
    {
        if (t is Pawn pawn && pawn.GetNanosuits().Any())
        {
            __result = false;
        }
    }
}