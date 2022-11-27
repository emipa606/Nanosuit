namespace Nanosuit;

public class SpeedMode
{
    public bool canJumpInCombat;
    public bool canJumpOutsideCombat;
    public float energyConsumptionPerTickWhenActive;

    public float jumpChance;
    public float jumpEnergyConsumption;

    public float jumpMaxDistance;

    public float? meleeCooldownFactor;
    public float meleeCooldownFactorEnergyConsumption;
    public float movementSpeedFactor;
}