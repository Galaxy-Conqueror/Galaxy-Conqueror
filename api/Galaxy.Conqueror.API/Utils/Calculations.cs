using System.Reflection.Metadata;

namespace Galaxy.Conqueror.API.Utils;

public class Calculations
{
    // ***** CONSTANTS *****
    public const int MinDistance = 5;
    public const int MapWidth = 100;
    public const int MapHeight = 100;

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
    public static int GetSpaceshipMaxFuel(int spaceshipLevel) => spaceshipLevel * 30;
    public static int GetSpaceshipFuelCost(int spaceshipLevel, int fuelAmount) => spaceshipLevel * fuelAmount * 1;
    public static int GetSpaceshipRefuelCost(int spaceshipLevel, int currentFuel) => 
        GetSpaceshipFuelCost(spaceshipLevel, GetSpaceshipMaxFuel(spaceshipLevel) - currentFuel);
    public static int GetSpaceshipHealthCost(int spaceshipLevel, int healthAmount) => spaceshipLevel * healthAmount * 2;
    public static int GetSpaceshipRepairCost(int spaceshipLevel, int currentHealth) => 
        GetSpaceshipHealthCost(spaceshipLevel, GetSpaceshipMaxHealth(spaceshipLevel) - currentHealth);


    // ***** RESOURCE EXTRACTORS *****
    public static int GetResourceGenAmount(int extractorLevel) => extractorLevel * 50;
    public static int GetExtractorUpgradeCost(int extractorLevel) => extractorLevel * 100;


    // ***** GENERAL *****
    public static int GetFuelUsed(int startX, int startY, int endX, int endY) => 
        (int) Math.Ceiling(Math.Sqrt(Math.Pow(endX - startX, 2) + Math.Pow(endY - startY, 2)));
    public static bool IsInRange(int x1, int y1, int x2, int y2, int maxDistance) => Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)) <= maxDistance;
}