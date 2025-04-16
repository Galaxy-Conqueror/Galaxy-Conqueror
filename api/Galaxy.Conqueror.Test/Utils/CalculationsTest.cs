using Galaxy.Conqueror.API.Utils;

namespace Galaxy.Conqueror.Test.Utils;

public class CalculationsTests
{
    [Theory]
    [InlineData(1, 40)]
    [InlineData(3, 120)]
    public void GetTurretHealth_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetTurretHealth(level));

    [Theory]
    [InlineData(1, 20)]
    [InlineData(3, 60)]
    public void GetTurretDamage_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetTurretDamage(level));

    [Theory]
    [InlineData(1, 200)]
    [InlineData(2, 400)]
    public void GetTurretUpgradeCost_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetTurretUpgradeCost(level));

    [Theory]
    [InlineData(1, 20)]
    [InlineData(4, 80)]
    public void GetTurretUpgradeEffect_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetTurretUpgradeEffect(level));

    [Theory]
    [InlineData(1, 100)]
    [InlineData(3, 300)]
    public void GetSpaceshipMaxResources_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipMaxResources(level));

    [Theory]
    [InlineData(2, 200)]
    public void GetSpaceshipMaxHealth_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipMaxHealth(level));

    [Theory]
    [InlineData(3, 30)]
    public void GetSpaceshipDamage_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipDamage(level));

    [Theory]
    [InlineData(2, 60)]
    public void GetSpaceshipMaxFuel_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipMaxFuel(level));

    [Theory]
    [InlineData(2, 10, 20)]
    public void GetSpaceshipFuelCost_ReturnsCorrectValue(int level, int fuel, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipFuelCost(level, fuel));

    [Theory]
    [InlineData(2, 20, 80)]
    public void GetSpaceshipRefuelCost_ReturnsCorrectValue(int level, int currentFuel, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipRefuelCost(level, currentFuel));

    [Theory]
    [InlineData(2, 20, 80)]
    public void GetSpaceshipHealthCost_ReturnsCorrectValue(int level, int health, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipHealthCost(level, health));

    [Theory]
    [InlineData(2, 50, 600)]
    public void GetSpaceshipRepairCost_ReturnsCorrectValue(int level, int currentHealth, int expected) =>
        Assert.Equal(expected, Calculations.GetSpaceshipRepairCost(level, currentHealth));

    [Theory]
    [InlineData(1, 50)]
    [InlineData(2, 100)]
    public void GetResourceGenAmount_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetResourceGenAmount(level));

    [Theory]
    [InlineData(3, 300)]
    public void GetExtractorUpgradeCost_ReturnsCorrectValue(int level, int expected) =>
        Assert.Equal(expected, Calculations.GetExtractorUpgradeCost(level));

    [Theory]
    [InlineData(0, 0, 3, 4, 5)]
    [InlineData(1, 1, 4, 5, 5)]
    public void GetFuelUsed_ReturnsCorrectRoundedDistance(int x1, int y1, int x2, int y2, int expected) =>
        Assert.Equal(expected, Calculations.GetFuelUsed(x1, y1, x2, y2));

    [Theory]
    [InlineData(0, 0, 3, 4, 5, true)]
    [InlineData(0, 0, 6, 8, 9, false)]
    public void IsInRange_ReturnsExpectedResult(int x1, int y1, int x2, int y2, int maxDist, bool expected) =>
        Assert.Equal(expected, Calculations.IsInRange(x1, y1, x2, y2, maxDist));
}
