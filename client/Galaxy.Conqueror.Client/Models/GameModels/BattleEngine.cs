using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;

namespace Battle;

public static class BattleEngine
{
    public static int MAP_WIDTH = StateManager.MAP_SCREEN_WIDTH;
    public static int MAP_HEIGHT = StateManager.MAP_SCREEN_HEIGHT;

    public static Spaceship Spaceship;
    private static Turret Turret;

    private static List<Bullet> Bullets;
    private static float SpaceshipBulletSpeed;

    private static float TurretBulletSpeed;
    private static float TurretFiringRate;
    private static float TurretMoveRate;

    private static float LastUpdateTime;
    private static float LastTurretBulletFiredTime;
    private static float LastTurretMoveTime;

    private static readonly Random Random = new(0);

    public static bool GameRunning = false;

    private static Action<BattleResult>? BattleConcludedCallback;

    private static float BattleStartTime;

    private static DateTime BattleStartDateTime;

    private static int DamageToSpaceship;
    private static int DamageToTurret;

    private static int PlanetResourceReserve;

    private static float MIN_BULLET_SPEED = 8f;
    private static float BULLET_SPEED_MULTIPLIER = 0.5f;
    private static float MIN_FIRING_RATE = 0.2f;
    private static float FIRING_RATE_MULTIPLIER = 0.02f;
    private static float TURRET_MOVE_RATE_MULTIPLIER = 0.2f;
    private static float MIN_TURRET_MOVE_RATE = 1f;

    public static void Initialise(Spaceship playerSpaceship, Turret enemyTurret, int planetResourceReserve)
    {
        Spaceship = playerSpaceship;
        Spaceship.Position = new Vector2I(MAP_WIDTH / 2, MAP_HEIGHT - 1);
        SpaceshipBulletSpeed = (Spaceship.Level * BULLET_SPEED_MULTIPLIER + MIN_BULLET_SPEED) * -1f;

        Turret = enemyTurret;
        Turret.Position = new Vector2I(MAP_WIDTH / 2, 0);
        TurretBulletSpeed = Turret.Level * BULLET_SPEED_MULTIPLIER + MIN_BULLET_SPEED;
        TurretFiringRate = Turret.Level * FIRING_RATE_MULTIPLIER + MIN_FIRING_RATE;
        TurretMoveRate = Turret.Level * TURRET_MOVE_RATE_MULTIPLIER + MIN_TURRET_MOVE_RATE;

        Bullets = new List<Bullet>();

        LastUpdateTime = GetCurrentTime();
        LastTurretBulletFiredTime = GetCurrentTime();
        LastTurretMoveTime = GetCurrentTime();

        BattleStartTime = GetCurrentTime();
        BattleStartDateTime = DateTime.Now;

        DamageToSpaceship = 0;
        DamageToTurret = 0;

        PlanetResourceReserve = planetResourceReserve;

        GameRunning = true;
    }

    public static void OnBattleConcluded(Action<BattleResult> callback)
    {
        BattleConcludedCallback = callback;
    }

    private static float GetCurrentTime()
    {
        return (float)Environment.TickCount / 1000;
    }

    public static void MoveSpaceshipLeft()
    {
        if (Spaceship.Position.X > 0)
            Spaceship.Position.X--;
    }

    public static void MoveSpaceshipRight()
    {
        if (Spaceship.Position.X < MAP_WIDTH - 1)
            Spaceship.Position.X++;
    }

    public static void MoveSpaceshipUp()
    {
        if (Spaceship.Position.Y > MAP_HEIGHT / 2)
            Spaceship.Position.Y--;
    }

    public static void MoveSpaceshipDown()
    {
        if (Spaceship.Position.Y < MAP_HEIGHT - 1)
            Spaceship.Position.Y++;
    }

    public static void ShootFromSpaceship()
    {
        bool bulletDirectlyAhead = Bullets.Any(b =>
            b.Speed < 0 &&
            b.X == Spaceship.Position.X &&
            (b.Y <= Spaceship.Position.Y && b.Y >= Spaceship.Position.Y - 1));

        if (!bulletDirectlyAhead)
        {
            Bullets.Add(new Bullet(Spaceship.Position.X, Spaceship.Position.Y - 1, SpaceshipBulletSpeed, new Glyph('|', ConsoleColor.Yellow), Spaceship.Damage));
        }
    }

    public static void ShootFromTurret(float currentTime)
    {
        float timeBetweenTurretShots = 1f / TurretFiringRate;
        if (currentTime - LastTurretBulletFiredTime >= timeBetweenTurretShots)
        {
            Bullets.Add(new Bullet(Turret.Position.X, Turret.Position.Y + 1, TurretBulletSpeed, new Glyph('|', ConsoleColor.Red), Turret.Damage));
            LastTurretBulletFiredTime = currentTime;
        }
    }

    private static void MoveTurret(float currentTime)
    {
        float timeBetweenMoves = 1f / TurretMoveRate;
        if (currentTime - LastTurretMoveTime < timeBetweenMoves) return;

        var incomingBullets = Bullets.Where(b =>
            b.Speed < 0 &&
            Math.Abs(b.X - Turret.Position.X) <= 1 &&
            b.Y > Turret.Position.Y &&
            b.Y - Turret.Position.Y < 8
        ).ToList();

        if (incomingBullets.Any())
        {
            int dx = DetermineBestDodgeDirection(incomingBullets);

            int dy = 0;
            if (Random.NextDouble() < 0.4)
            {
                dy = Random.NextDouble() < 0.8 ? 1 : -1;
            }

            int newX = Turret.Position.X + dx;
            int newY = Turret.Position.Y + dy;

            if (newX >= 0 && newX < MAP_WIDTH) Turret.Position.X = newX;
            if (newY >= 0 && newY < MAP_HEIGHT / 2) Turret.Position.Y = newY;

            Turret.DirectionX = dx;
            LastTurretMoveTime = currentTime;
            return;
        }

        DetermineStrategicPosition(currentTime, out int targetX, out int targetY);

        int moveX = Math.Sign(targetX - Turret.Position.X);
        int moveY = Math.Sign(targetY - Turret.Position.Y);

        if (Random.NextDouble() < 0.15) moveX = 0;
        if (Random.NextDouble() < 0.25) moveY = 0;

        int newPosX = Turret.Position.X + moveX;
        int newPosY = Turret.Position.Y + moveY;

        if (newPosX >= 0 && newPosX < MAP_WIDTH) Turret.Position.X = newPosX;
        if (newPosY >= 0 && newPosY < MAP_HEIGHT / 2) Turret.Position.Y = newPosY;

        Turret.DirectionX = moveX;
        LastTurretMoveTime = currentTime;
    }

    private static int DetermineBestDodgeDirection(List<Bullet> incomingBullets)
    {
        int detectionWidth = 3;

        int leftBulletCount = 0;
        int rightBulletCount = 0;

        for (int i = 1; i <= detectionWidth; i++)
        {
            int leftX = Turret.Position.X - i;
            if (leftX >= 0)
            {
                leftBulletCount += incomingBullets.Count(b => Math.Abs(b.X - leftX) <= 0.5f);
            }
            else
            {
                leftBulletCount += 999;
            }

            int rightX = Turret.Position.X + i;
            if (rightX < MAP_WIDTH)
            {
                rightBulletCount += incomingBullets.Count(b => Math.Abs(b.X - rightX) <= 0.5f);
            }
            else
            {
                rightBulletCount += 999;
            }
        }

        if (leftBulletCount < rightBulletCount)
        {
            return -1;
        }
        else if (rightBulletCount < leftBulletCount)
        {
            return 1;
        }
        else
        {
            if (Turret.Position.X < MAP_WIDTH / 4)
                return 1;
            else if (Turret.Position.X > 3 * MAP_WIDTH / 4)
                return -1;
            else
                return Random.Next(0, 2) * 2 - 1;
        }
    }

    private static void DetermineStrategicPosition(float currentTime, out int targetX, out int targetY)
    {
        var bulletColumns = new bool[MAP_WIDTH];
        foreach (var bullet in Bullets.Where(b => b.Speed < 0))
        {
            int column = (int)bullet.X;
            if (column >= 0 && column < MAP_WIDTH)
            {
                bulletColumns[column] = true;
            }
        }

        targetX = Spaceship.Position.X;

        if (targetX >= 0 && targetX < MAP_WIDTH && bulletColumns[targetX])
        {
            int leftDist = 1;
            int rightDist = 1;
            bool foundSafe = false;

            while (!foundSafe && (targetX - leftDist >= 0 || targetX + rightDist < MAP_WIDTH))
            {
                if (targetX - leftDist >= 0)
                {
                    if (!bulletColumns[targetX - leftDist])
                    {
                        targetX = targetX - leftDist;
                        foundSafe = true;
                        break;
                    }
                    leftDist++;
                }

                if (targetX + rightDist < MAP_WIDTH)
                {
                    if (!bulletColumns[targetX + rightDist])
                    {
                        targetX = targetX + rightDist;
                        foundSafe = true;
                        break;
                    }
                    rightDist++;
                }
            }
        }

        int positionPhase = (int)(currentTime / 7) % 3;

        switch (positionPhase)
        {
            case 0:
                targetY = MAP_HEIGHT / 3;
                break;
            case 1:
                targetY = MAP_HEIGHT / 4;
                break;
            case 2:
                targetY = MAP_HEIGHT / 8;
                break;

            default:
                targetY = 0;
                break;
        }

        if (Random.NextDouble() < 0.2)
        {
            targetX += Random.Next(-2, 3);
            targetY += Random.Next(-1, 2);
        }

        targetX = Math.Max(2, Math.Min(MAP_WIDTH - 3, targetX));
    }

    public static void Update()
    {
        if (!GameRunning) return;

        float currentTime = GetCurrentTime();
        float deltaTime = currentTime - LastUpdateTime;
        LastUpdateTime = currentTime;

        MoveTurret(currentTime);
        ShootFromTurret(currentTime);

        foreach (var b in Bullets) b.Update(deltaTime);

        DetectCollisions();

        Bullets.RemoveAll(b => (b.Speed < 0 && b.Y < 0) || (b.Speed > 0 && b.Y >= MAP_HEIGHT));
    }

    public static void DetectCollisions()
    {
        List<Bullet> bulletsToRemove = new List<Bullet>();

        for (int i = 0; i < Bullets.Count; i++)
        {
            for (int j = i + 1; j < Bullets.Count; j++)
            {
                if (Bullets[i].Speed * Bullets[j].Speed < 0)
                {
                    if (Bullets[i].X == Bullets[j].X && Math.Abs(Bullets[i].Y - Bullets[j].Y) <= 1)
                    {
                        bulletsToRemove.Add(Bullets[i]);
                        bulletsToRemove.Add(Bullets[j]);
                    }
                }
            }
        }

        foreach (var bullet in Bullets)
        {
            if (bullet.Speed > 0)
            {
                if (bullet.X == Spaceship.Position.X && Math.Abs(bullet.Y - Spaceship.Position.Y) <= 1)
                {
                    bulletsToRemove.Add(bullet);
                    OnSpaceshipHit(bullet);
                }
            }
        }

        foreach (var bullet in Bullets)
        {
            if (bullet.Speed < 0)
            {
                if (bullet.X == Turret.Position.X && Math.Abs(bullet.Y - Turret.Position.Y) <= 1)
                {
                    bulletsToRemove.Add(bullet);
                    OnTurretHit(bullet);
                }
            }
        }

        foreach (var bullet in bulletsToRemove)
        {
            Bullets.Remove(bullet);
        }
    }

    private static void OnSpaceshipHit(Bullet bullet)
    {
        DamageToSpaceship += bullet.Damage;
        Spaceship.TakeDamage(bullet);

        if (Spaceship.IsDestroyed()) ConcludeBattle();
    }

    private static void OnTurretHit(Bullet bullet)
    {
        DamageToTurret += bullet.Damage;
        Turret.TakeDamage(bullet);

        if (Turret.IsDestroyed()) ConcludeBattle();
    }

    public static Dictionary<Vector2I, Glyph> GetMap()
    {
        var map = new Dictionary<Vector2I, Glyph>();

        map.Add(Turret.GetPosition(), Turret.Glyph);

        foreach (var bullet in Bullets)
        {
            if (!map.Remove(bullet.GetPosition()))
            {
                map.Add(bullet.GetPosition(), bullet.Glyph);
            }
        }

        return map;
    }

    public static Tuple<Vector2I, Glyph> GetSpaceship()
    {
        return new Tuple<Vector2I, Glyph>(new Vector2I(Spaceship.Position), Spaceship.Glyph);
    }

    private static void PrintMessageCenterMap(string message, ConsoleColor color)
    {
        Console.Clear();
        int messageLength = message.Length;
        Console.SetCursorPosition(MAP_WIDTH - (messageLength / 2), MAP_HEIGHT / 2);
        ConsolePrinter.PrintLine(message, color);
    }

    private static void ConcludeBattle()
    {
        GameRunning = false;

        BattleResult battleResult = GetBattleResult();

        Thread.Sleep(1000);
        PrintMessageCenterMap("BATTLE OVER!", ConsoleColor.Yellow);
        Thread.Sleep(2000);

        if (Turret.IsDestroyed())
            PrintMessageCenterMap("YOU WIN!", ConsoleColor.Green);
        else if (Spaceship.IsDestroyed())
            PrintMessageCenterMap("YOU LOSE!", ConsoleColor.Red);
        else
            PrintMessageCenterMap("DRAW!", ConsoleColor.Yellow);

        Thread.Sleep(2000);
        PrintMessageCenterMap("PLANET RESOURCES LOOTED: " + battleResult.ResourcesLooted.ToString(), ConsoleColor.Yellow);
        Thread.Sleep(2000);

        Console.Clear();

        BattleConcludedCallback?.Invoke(battleResult);
    }

    public static BattleResult GetBattleResult()
    {
        int loot = 0;

        if (Turret.IsDestroyed())
        {
            loot = Math.Min(PlanetResourceReserve, Spaceship.MaxResources - Spaceship.ResourceReserve);
        }

        return new BattleResult(
            BattleStartDateTime,
            DateTime.Now,
            DamageToSpaceship,
            DamageToTurret,
            loot
        );
    }

    public static BattleState GetBattleState()
    {
        return new BattleState(
            Spaceship.CurrentHealth,
            Turret.CurrentHealth,
            GetCurrentTime() - BattleStartTime
        );
    }
}