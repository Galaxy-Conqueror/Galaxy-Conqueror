namespace Galaxy.Conqueror.API.Utils;

public class Calculations
{

    // TODO: USE CONFIG VALUES INSTEAD OF HARDCODED

    // ***** TURRETS *****
    public static int GetTurretHealth(int turretLevel) => turretLevel * 40;
    public static int GetTurretDamage(int turretLevel) => turretLevel * 20;
    public static int GetTurretUpgradeCost(int turretLevel) => turretLevel * 200;
    public static int GetTurretUpgradeEffect(int turretLevel) => turretLevel * 20;


    // ***** SPACESHIPS *****
    public static int GetSpaceshipMaxResources(int spaceshipLevel) => spaceshipLevel * 100;
    public static int GetSpaceshipMaxHealth(int spaceshipLevel) => spaceshipLevel * 100;
    public static int GetSpaceshipDamage(int spaceshipLevel) => spaceshipLevel * 10;
    public static int GetSpaceshipUpgradeCost(int spaceshipLevel) => spaceshipLevel * 200;
    public static int GetSpaceshipUpgradeEffect(int spaceshipLevel) => spaceshipLevel * 20;
    public static int GetSpaceshipMaxFuel(int spaceshipLevel) => spaceshipLevel * 30;
    public static int GetSpaceshipFuelCost(int spaceshipLevel, int fuelAmount) => spaceshipLevel * fuelAmount * 5;
    public static int GetSpaceshipRefuelCost(int spaceshipLevel, int currentFuel) => GetSpaceshipFuelCost(spaceshipLevel, GetSpaceshipMaxFuel(spaceshipLevel) - currentFuel);
    public static int GetSpaceshipHealthCost(int spaceshipLevel, int healthAmount) => spaceshipLevel * healthAmount * 2;
    public static int GetSpaceshipRepairCost(int spaceshipLevel, int currentHealth) => GetSpaceshipHealthCost(spaceshipLevel, GetSpaceshipMaxHealth(spaceshipLevel) - currentHealth);



    // ***** RESOURCE EXTRACTORS *****
    public static int GetResourceGenAmount(int extractorLevel) => extractorLevel * 50;

}