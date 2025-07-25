﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nanosuit.Harmony;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Nanosuit;

public class Apparel_Nanosuit : Apparel
{
    private readonly MethodInfo breakMethod = AccessTools.Method(typeof(CompShield), "Break");
    private Ability ability;
    private ApparelMode activeMode;
    private Dictionary<ApparelMode, bool> activeModes;
    private List<bool> boolValues;

    private CompAbilityEffect_Teleport customTeleportComp;

    private float energy;
    private float energyBonus;

    public bool inSpeedState;
    public bool jumpModeInCombat;
    public bool jumpModeOutsideCombat;

    private List<ApparelMode> modeKeys;
    private bool nightVisorActive;

    private int skipTicks;
    private bool symbiosisActive;

    public float Energy
    {
        get
        {
            if (energyBonus < 0)
            {
                energyBonus = 0;
            }

            if (energy > def.maxEnergyAmount)
            {
                energy = def.maxEnergyAmount;
            }

            return Math.Min(energy + energyBonus, def.maxEnergyAmount);
        }
        set
        {
            energy = value;
            if (energy >= 0)
            {
                return;
            }

            if (energyBonus <= 0)
            {
                return;
            }

            energyBonus -= Mathf.Abs(energy);
            if (energyBonus < 0)
            {
                energyBonus = 0;
            }
        }
    }

    public new NanosuitDef def => base.def as NanosuitDef;

    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (!IsActive(ApparelMode.ArmorMode))
        {
            return false;
        }

        float energyToConsume;
        if (dinfo.Def.isRanged && def.armorMode.rangeDamageAbsorbingRate.HasValue)
        {
            energyToConsume = dinfo.Amount * def.armorMode.rangeDamageAbsorbingRate.Value;
            if (!(Energy >= energyToConsume))
            {
                return false;
            }

            Energy -= energyToConsume;
            return true;
        }

        if (dinfo.Def.isExplosive && def.armorMode.explosionDamageAbsorbingRate.HasValue)
        {
            energyToConsume = dinfo.Amount * def.armorMode.explosionDamageAbsorbingRate.Value;
            if (!(Energy >= energyToConsume))
            {
                return false;
            }

            Energy -= energyToConsume;
            return true;
        }

        if ((dinfo.Weapon != null || dinfo.Instigator is not Pawn) && dinfo.Weapon?.IsMeleeWeapon != true &&
            dinfo.Weapon != ThingDefOf.Human && dinfo.Def.isRanged ||
            !def.armorMode.meleeDamageAbsorbingRate.HasValue)
        {
            return false;
        }

        energyToConsume = dinfo.Amount * def.armorMode.meleeDamageAbsorbingRate.Value;
        if (!(Energy >= energyToConsume))
        {
            return false;
        }

        Energy -= energyToConsume;
        return true;
    }

    protected override void Tick()
    {
        base.Tick();
        Energy += def.energyRegenerationPerTick;
        if (Energy > def.maxEnergyAmount)
        {
            Energy = def.maxEnergyAmount;
        }

        if (Wearer == null)
        {
            return;
        }

        if (IsActive(ApparelMode.SpeedMode) && (Wearer.pather?.MovingNow ?? false))
        {
            if (Energy >= def.speedMode.energyConsumptionPerTickWhenActive)
            {
                Energy -= def.speedMode.energyConsumptionPerTickWhenActive;
                inSpeedState = true;
            }
            else
            {
                inSpeedState = false;
            }
        }

        if (def.symbiosis != null && symbiosisActive && Wearer.IsHashIntervalTick(def.symbiosis.healsPerTick) &&
            energy >= def.symbiosis.energyConsumption)
        {
            var missingParts = (from x in Wearer.RaceProps.body.AllParts
                where Wearer.health.hediffSet.PartIsMissing(x)
                select x).ToList();
            if (missingParts.Any())
            {
                var partToRestore = missingParts.RandomElement();
                Wearer.health.RestorePart(partToRestore);
                Energy -= def.symbiosis.energyConsumption;
            }
            else
            {
                var badHediffs = Wearer.health.hediffSet.hediffs.Where(x =>
                    (x.TryGetComp<HediffComp_GetsPermanent>()?.IsPermanent ?? false) || x.def.isBad).ToArray();
                if (badHediffs.Any())
                {
                    var hediff = badHediffs.RandomElement();

                    var comp = hediff.TryGetComp<HediffComp_GetsPermanent>();
                    if (comp is { IsPermanent: true } || hediff.def.isBad)
                    {
                        Wearer.health.hediffSet.hediffs.Remove(hediff);
                        Energy -= def.symbiosis.energyConsumption;
                    }
                }
            }

            if (Wearer.Downed && !Wearer.ShouldBeDowned())
            {
                Traverse.Create(Wearer.health).Method("MakeUndowned").GetValue();
            }

            Wearer.health.hediffSet.DirtyCache();
        }

        if (def.psychicWaveControl != null)
        {
            var psychicDroneLevel = getPsychicDroneLevel();
            if (psychicDroneLevel > PsychicDroneLevel.None)
            {
                energyBonus += def.psychicWaveControl.energyGainPerTickWhenActive;
            }
        }

        if (def.environmentalControl != null && energy > 0)
        {
            GenTemperature_ComfortableTemperatureRange.dontCheckThis = true;
            var ambientTemperature = Wearer.AmbientTemperature;
            var conformableTemperature = Wearer.ComfortableTemperatureRange();
            if (!conformableTemperature.Includes(ambientTemperature))
            {
                Energy -= def.environmentalControl.energyConsumptionWhenActive;
            }

            GenTemperature_ComfortableTemperatureRange.dontCheckThis = false;
        }

        var cloakHediff = Wearer.health.hediffSet.GetFirstHediffOfDef(NS_DefOf.NS_CloakMode);
        if (def.cloakMode != null && cloakHediff != null)
        {
            if (Wearer.pather?.Moving ?? false)
            {
                if (Energy >= def.cloakMode.energyConsumptionPerTickMoving)
                {
                    Energy -= def.cloakMode.energyConsumptionPerTickMoving;
                }
                else
                {
                    Wearer.health.RemoveHediff(cloakHediff);
                }
            }
            else
            {
                if (Energy >= def.cloakMode.energyConsumptionPerTick)
                {
                    Energy -= def.cloakMode.energyConsumptionPerTick;
                }
                else
                {
                    Wearer.health.RemoveHediff(cloakHediff);
                }
            }
        }

        if (def.nightVisor != null && NightVisionWorks())
        {
            Energy -= def.nightVisor.energyConsumptionPerTickWhenActive;
            Traverse.Create(Wearer.needs.mood.recentMemory).Field("lastLightTick")
                .SetValue(Find.TickManager.TicksGame);
        }

        if (ability == null)
        {
            return;
        }

        ability.AbilityTick();
        if (skipTicks == 0)
        {
            Wearer.jobs?.curDriver?.Notify_PatherArrived();
        }

        if (skipTicks > -2)
        {
            skipTicks--;
        }
    }

    public bool NightVisionWorks()
    {
        var pawn = Wearer;
        return pawn is { Map: not null } && pawn.Map.glowGrid.PsychGlowAt(pawn.Position) == PsychGlow.Dark &&
               nightVisorActive && energy >= def.nightVisor.energyConsumptionPerTickWhenActive;
    }

    private PsychicDroneLevel getPsychicDroneLevel()
    {
        var p = Wearer;
        var psychicDroneLevel = PsychicDroneLevel.None;
        if (p.Map != null)
        {
            var highestPsychicDroneLevelFor =
                p.Map.gameConditionManager.GetHighestPsychicDroneLevelFor(p.gender, p.Map);
            if ((int)highestPsychicDroneLevelFor > (int)psychicDroneLevel)
            {
                psychicDroneLevel = highestPsychicDroneLevelFor;
            }

            return psychicDroneLevel;
        }

        if (!p.IsCaravanMember())
        {
            return psychicDroneLevel;
        }

        foreach (var site in Find.World.worldObjects.Sites)
        {
            foreach (var part in site.parts)
            {
                if (part.conditionCauser.DestroyedOrNull() ||
                    part.def.Worker is not SitePartWorker_ConditionCauser_PsychicDroner)
                {
                    continue;
                }

                var compCauseGameConditionPsychicEmanation =
                    part.conditionCauser.TryGetComp<CompCauseGameCondition_PsychicEmanation>();
                if (compCauseGameConditionPsychicEmanation.ConditionDef.conditionClass ==
                    typeof(GameCondition_PsychicEmanation) &&
                    compCauseGameConditionPsychicEmanation.InAoE(p.GetCaravan().Tile) &&
                    compCauseGameConditionPsychicEmanation.gender == p.gender &&
                    (int)compCauseGameConditionPsychicEmanation.Level > (int)psychicDroneLevel)
                {
                    psychicDroneLevel = compCauseGameConditionPsychicEmanation.Level;
                }
            }
        }

        foreach (var map in Find.Maps)
        {
            foreach (var activeCondition in map.gameConditionManager.ActiveConditions)
            {
                var compCauseGameConditionPsychicEmanation2 = activeCondition.conditionCauser
                    .TryGetComp<CompCauseGameCondition_PsychicEmanation>();
                if (compCauseGameConditionPsychicEmanation2 != null &&
                    compCauseGameConditionPsychicEmanation2.InAoE(p.GetCaravan().Tile) &&
                    compCauseGameConditionPsychicEmanation2.gender == p.gender &&
                    (int)compCauseGameConditionPsychicEmanation2.Level > (int)psychicDroneLevel)
                {
                    psychicDroneLevel = compCauseGameConditionPsychicEmanation2.Level;
                }
            }
        }

        return psychicDroneLevel;
    }

    private TargetingParameters forLoc(Pawn user)
    {
        var targetingParameters = new TargetingParameters
        {
            canTargetLocations = true
        };
        if (def.strengthMode.jumpMaxDistance > 0)
        {
            targetingParameters.validator = x => user.Position.DistanceTo(x.Cell) <= def.strengthMode.jumpMaxDistance;
        }

        return targetingParameters;
    }

    private TargetingParameters forHackableShields(Pawn user)
    {
        var targetingParameters = new TargetingParameters
        {
            canTargetBuildings = true,
            canTargetPawns = true,
            validator = x =>
                isHackableShield(x.Thing) && user.Position.DistanceTo(x.Cell) <= def.transceiverDevice.maxRangeEffect
        };
        return targetingParameters;
    }

    private bool isHackableShield(Thing target)
    {
        if (target is Pawn pawn)
        {
            if (pawn.apparel?.WornApparel?.Any(apparel => apparel.def.HasComp(typeof(CompShield))) == true)
            {
                return true;
            }

            foreach (var comp in pawn.AllComps)
            {
                if (isCustomShieldBeltComp(comp))
                {
                    return true;
                }
            }

            foreach (var apparel in pawn.apparel?.WornApparel ?? [])
            {
                if (isCustomShieldBelt(apparel))
                {
                    return true;
                }

                foreach (var comp in apparel.AllComps)
                {
                    if (isCustomShieldBeltComp(comp))
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            var comp = target.TryGetComp<CompProjectileInterceptor>();
            if (comp != null)
            {
                return true;
            }
        }

        return false;
    }

    private static bool isCustomShieldBelt(Thing thing)
    {
        var type = thing.GetType();
        return type == ModCompatibility.ShieldMechBubbleType || type == ModCompatibility.ArchotechShieldBelt ||
               type == ModCompatibility.RangedShieldBelt;
    }

    private static bool isCustomShieldBeltComp(ThingComp thingComp)
    {
        var type = thingComp.GetType();
        return type == ModCompatibility.ShieldMechBubbleType || type == ModCompatibility.ArchotechShieldBelt ||
               type == ModCompatibility.RangedShieldBelt;
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        foreach (var wornGizmo in base.GetWornGizmos())
        {
            yield return wornGizmo;
        }

        if (Find.Selector.SingleSelectedThing == Wearer && Wearer.IsColonistPlayerControlled)
        {
            var gizmoNanosuitEnergyStatus = new Gizmo_NanosuitEnergyStatus
            {
                nanosuit = this
            };
            yield return gizmoNanosuitEnergyStatus;
        }

        if (def.armorMode != null)
        {
            yield return new Command_Toggle
            {
                defaultLabel = "NS.ArmorMode".Translate(),
                defaultDesc = "NS.ArmorModeDesc".Translate(),
                hotKey = NS_DefOf.NS_ArmorMode,
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/armor"),
                isActive = () => IsActive(ApparelMode.ArmorMode),
                toggleAction = delegate { switchApparelMode(ApparelMode.ArmorMode); }
            };
        }

        if (def.strengthMode != null)
        {
            yield return new Command_Toggle
            {
                defaultLabel = "NS.StrengthMode".Translate(),
                defaultDesc = "NS.StrengthModeDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/strength"),
                isActive = () => IsActive(ApparelMode.StrengthMode),
                hotKey = NS_DefOf.NS_StrengthMode,
                toggleAction = delegate { switchApparelMode(ApparelMode.StrengthMode); }
            };
            if (def.strengthMode.jumpAbility)
            {
                var command = new Command_Action
                {
                    defaultLabel = "NS.Jump".Translate(),
                    defaultDesc = "NS.JumpDesc",
                    hotKey = NS_DefOf.NS_JumpMode,
                    icon = ContentFinder<Texture2D>.Get("UI/Buttons/jump"),
                    action = delegate
                    {
                        Find.Targeter.BeginTargeting(forLoc(Wearer),
                            delegate(LocalTargetInfo x) { Jump(x.Cell, def.strengthMode.jumpEnergyConsumption); },
                            null, null);
                    }
                };
                if (def.strengthMode.jumpEnergyConsumption > 0 && Energy < def.strengthMode.jumpEnergyConsumption)
                {
                    command.Disable("NS.LowEnergy".Translate());
                }

                yield return command;
            }
        }

        if (def.speedMode != null)
        {
            yield return new Command_Toggle
            {
                defaultLabel = "NS.SpeedMode".Translate(),
                defaultDesc = "NS.SpeedModeModeDesc".Translate(),
                hotKey = NS_DefOf.NS_SpeedMode,
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/speed"),
                isActive = () => IsActive(ApparelMode.SpeedMode),
                toggleAction = delegate { switchApparelMode(ApparelMode.SpeedMode); }
            };
            if (IsActive(ApparelMode.SpeedMode))
            {
                if (def.speedMode.canJumpOutsideCombat)
                {
                    yield return new Command_Toggle
                    {
                        defaultLabel = "NS.SpeedModeJumpModeOutsideCombat".Translate(),
                        defaultDesc = "NS.SpeedModeJumpModeOutsideCombatDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Buttons/speed_non_combat"),
                        isActive = () => jumpModeOutsideCombat,
                        toggleAction = delegate { jumpModeOutsideCombat = !jumpModeOutsideCombat; }
                    };
                }

                if (def.speedMode.canJumpInCombat)
                {
                    yield return new Command_Toggle
                    {
                        defaultLabel = "NS.SpeedModeJumpModeInCombat".Translate(),
                        defaultDesc = "NS.SpeedModeJumpModeInCombatDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Buttons/speed_in_combat_2"),
                        isActive = () => jumpModeInCombat,
                        toggleAction = delegate { jumpModeInCombat = !jumpModeInCombat; }
                    };
                }
            }
        }

        if (def.cloakMode != null)
        {
            var command = new Command_Toggle
            {
                defaultLabel = "NS.CloakMode".Translate(),
                defaultDesc = "NS.CloakModeDesc",
                hotKey = NS_DefOf.NS_CloakModeKey,
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/cloak"),
                isActive = () => Wearer.health.hediffSet.GetFirstHediffOfDef(NS_DefOf.NS_CloakMode) != null,
                toggleAction = delegate
                {
                    var hediff = Wearer.health.hediffSet.GetFirstHediffOfDef(NS_DefOf.NS_CloakMode);
                    if (hediff != null)
                    {
                        Wearer.health.RemoveHediff(hediff);
                    }
                    else
                    {
                        hediff = HediffMaker.MakeHediff(NS_DefOf.NS_CloakMode, Wearer);
                        Wearer.health.AddHediff(hediff);
                    }
                }
            };
            if (def.cloakMode.energyConsumptionPerTick > 0 && Energy < def.cloakMode.energyConsumptionPerTick)
            {
                command.Disable("NS.LowEnergy".Translate());
            }

            yield return command;
        }

        if (def.transceiverDevice != null)
        {
            var command = new Command_Action
            {
                defaultLabel = "NS.HackShields".Translate(),
                defaultDesc = "NS.HackShieldsDesc",
                hotKey = NS_DefOf.NS_HackShields,
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/transceiverDevice1"),
                action = delegate
                {
                    Find.Targeter.BeginTargeting(forHackableShields(Wearer),
                        delegate(LocalTargetInfo x) { disableShield(x.Thing); }, null, null);
                }
            };
            if (def.transceiverDevice.energyConsumption > 0 && Energy < def.transceiverDevice.energyConsumption)
            {
                command.Disable("NS.LowEnergy".Translate());
            }

            yield return command;
        }

        if (def.symbiosis != null)
        {
            yield return new Command_Toggle
            {
                defaultLabel = "NS.SymbiosysMode".Translate(),
                defaultDesc = "NS.SymbiosysModeDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Buttons/symbiosis"),
                isActive = () => symbiosisActive,
                hotKey = NS_DefOf.NS_SymbiosysMode,
                toggleAction = delegate { symbiosisActive = !symbiosisActive; }
            };
        }
    }

    private void disableShield(Thing target)
    {
        if (target is Pawn pawn)
        {
            var shieldBelts = pawn.apparel?.WornApparel?.Where(apparel => apparel.def.HasComp(typeof(CompShield)));
            if (shieldBelts != null)
            {
                foreach (var shield in shieldBelts)
                {
                    breakMethod.Invoke(shield.GetComp<CompShield>(), null);
                }
            }

            foreach (var comp in pawn.AllComps)
            {
                if (isCustomShieldBeltComp(comp))
                {
                    Traverse.Create(comp).Method("Break").GetValue();
                }
            }

            var list = pawn.apparel?.WornApparel ?? [];
            for (var num = list.Count - 1; num >= 0; num--)
            {
                var apparel = list[num];
                foreach (var comp in apparel.AllComps)
                {
                    if (isCustomShieldBeltComp(comp))
                    {
                        Traverse.Create(comp).Method("Break").GetValue();
                    }
                }

                if (isCustomShieldBelt(apparel))
                {
                    Traverse.Create(apparel).Method("Break").GetValue();
                }
            }
        }
        else
        {
            var comp = target.TryGetComp<CompProjectileInterceptor>();
            Traverse.Create(comp).Field("shutDown").SetValue(true);
        }

        Energy -= def.transceiverDevice.energyConsumption;
    }

    public void Jump(IntVec3 target, float energyToConsume)
    {
        if (NanosuitMod.Settings.useSkipForJumping)
        {
            skipTo(target, energyToConsume);
        }
        else
        {
            Pawn_CarryTracker_TryDropCarriedThing.pawn = Wearer;
            jumpTo(target, energyToConsume);
        }
    }

    private void skipTo(IntVec3 target, float energyToConsume)
    {
        Pawn_Notify_Teleported.preventEndingJob = true;
        if (customTeleportComp is null)
        {
            ability = new Ability(Wearer, DefDatabase<AbilityDef>.GetNamed("Skip"));
            customTeleportComp = new CompAbilityEffect_Teleport
            {
                parent = ability
            };
            customTeleportComp.Initialize(new CompProperties_AbilityTeleport
            {
                destination = AbilityEffectDestination.Selected,
                requiresLineOfSight = true,
                range = 27.9f,
                clamorType = ClamorDefOf.Ability,
                clamorRadius = 10,
                destClamorType = ClamorDefOf.Ability,
                destClamorRadius = 10,
                stunTicks = new IntRange(18, 60),
                goodwillImpact = -15,
                applyGoodwillImpactToLodgers = false
            });
        }

        customTeleportComp.Apply(Wearer, target);
        if (energyToConsume > 0)
        {
            Energy -= energyToConsume;
        }

        skipTicks = 60;
        Pawn_Notify_Teleported.preventEndingJob = false;
    }

    private void jumpTo(IntVec3 target, float energyToConsume)
    {
        var map = Wearer?.Map;
        if (map == null || !target.IsValid)
        {
            return;
        }

        Wearer.rotationTracker.FaceCell(target);
        var pawnFlyer = PawnFlyer.MakeFlyer(ThingDefOf.PawnFlyer, Wearer, target,
            DefDatabase<EffecterDef>.GetNamedSilentFail("JumpFlightEffect"),
            DefDatabase<SoundDef>.GetNamedSilentFail("JumpPackLand"), true);
        if (pawnFlyer != null)
        {
            GenSpawn.Spawn(pawnFlyer, target, map);
        }

        if (energyToConsume > 0)
        {
            Energy -= energyToConsume;
        }
    }

    public bool IsActive(ApparelMode apparelMode)
    {
        if (!NanosuitMod.Settings.nanosuitModesAtTheSameTime)
        {
            return activeMode == apparelMode;
        }

        activeModes ??= new Dictionary<ApparelMode, bool>();

        if (activeModes.TryGetValue(apparelMode, out var value))
        {
            return value;
        }

        activeModes[apparelMode] = false;

        return false;
    }

    private void switchApparelMode(ApparelMode apparelMode)
    {
        if (NanosuitMod.Settings.nanosuitModesAtTheSameTime)
        {
            activeModes ??= new Dictionary<ApparelMode, bool>();

            if (!activeModes.TryAdd(apparelMode, true))
            {
                activeModes[apparelMode] = !activeModes[apparelMode];
            }
        }
        else
        {
            activeMode = activeMode == apparelMode ? ApparelMode.None : apparelMode;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref energy, "energy");
        Scribe_Values.Look(ref energyBonus, "energyBonus");
        Scribe_Values.Look(ref activeMode, "activeMode");
        Scribe_Collections.Look(ref activeModes, "activeModes", LookMode.Value, LookMode.Value, ref modeKeys,
            ref boolValues);
        Scribe_Values.Look(ref jumpModeOutsideCombat, "jumpModeOutsideCombat");
        Scribe_Values.Look(ref jumpModeInCombat, "jumpModeInCombat");
        Scribe_Values.Look(ref symbiosisActive, "symbiosisActive");
        Scribe_Values.Look(ref nightVisorActive, "nightVisorActive");
        Scribe_Values.Look(ref skipTicks, "skipTicks");
    }
}