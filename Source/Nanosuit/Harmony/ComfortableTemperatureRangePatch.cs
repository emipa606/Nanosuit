using HarmonyLib;
using Verse;

namespace Nanosuit.Harmony;

[HarmonyPatch(typeof(GenTemperature), nameof(GenTemperature.ComfortableTemperatureRange), typeof(Pawn))]
public static class ComfortableTemperatureRangePatch
{
    public static bool dontCheckThis;

    public static void Postfix(Pawn p, ref FloatRange __result)
    {
        if (dontCheckThis)
        {
            return;
        }

        var ambientTemperature = p.AmbientTemperature;
        if (__result.Includes(ambientTemperature))
        {
            return;
        }

        foreach (var apparel in p.GetNanosuits())
        {
            if (apparel.def.environmentalControl != null &&
                apparel.Energy >= apparel.def.environmentalControl.energyConsumptionWhenActive)
            {
                __result = apparel.def.environmentalControl.temperatureProtectionRange;
            }
        }
    }
}