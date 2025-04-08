namespace Galaxy.Conqueror.API.Utils;

public class Calculations
{

    // TODO: USE CONFIG VALUES INSTEAD OF HARDCODED

    // ***** TURRETS *****
    public static int getTurretHealth(int turretLevel) => turretLevel * 40;
    public static int getTurretDamage(int turretLevel) => turretLevel * 20;
    public static int getTurretUpgradeCost(int turretLevel) => turretLevel * 200;
    public static int getTurretUpgradeEffect(int turretLevel) => turretLevel * 20;


    // ***** SPACESHIPS *****
    public static int getSpaceshipMaxResources(int spaceshipLevel) => spaceshipLevel * 100;
    public static int getSpaceshipMaxHealth(int spaceshipLevel) => spaceshipLevel * 100;
    public static int getSpaceshipDamage(int spaceshipLevel) => spaceshipLevel * 10;
    public static int getSpaceshipUpgradeCost(int spaceshipLevel) => spaceshipLevel * 200;
    public static int getSpaceshipUpgradeEffect(int spaceshipLevel) => spaceshipLevel * 20;
    public static int getSpaceshipMaxFuel(int spaceshipLevel) => spaceshipLevel * 30;


    // ***** RESOURCE EXTRACTORS *****
    public static int getResourceGenAmount(int extractorLevel) => extractorLevel * 50;

}