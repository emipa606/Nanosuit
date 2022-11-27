using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Nanosuit;

[HarmonyPatch(typeof(FireUtility), "CanEverAttachFire")]
public class CanEverAttachFire_Patch
{
    public static void Postfix(Thing t, ref bool __result)
    {
        if (t is Pawn pawn && pawn.GetNanosuits().Any())
        {
            __result = false;
        }
    }
}