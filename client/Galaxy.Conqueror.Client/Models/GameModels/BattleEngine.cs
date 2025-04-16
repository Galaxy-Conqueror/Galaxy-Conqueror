using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;

namespace Battle;

public static class BattleEngine
{
    public static int MAP_WIDTH;
    public static int MAP_HEIGHT;

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
    private static float BULLET_SPEED_MULTIPLIER = 0.2f;
    private static float MIN_FIRING_RATE = 0.2f;
    private static float FIRING_RATE_MULTIPLIER = 0.2f;

    public static void Initialise(int width, int height, Spaceship playerSpaceship, Turret enemyTurret, int planetResourceReserve)
    {
        MAP_WIDTH = width;
        MAP_HEIGHT = height;

        Spaceship = playerSpaceship;
        Spaceship.Position = new Vector2I(MAP_WIDTH / 2, MAP_HEIGHT);
        SpaceshipBulletSpeed = (Spaceship.Level * BULLET_SPEED_MULTIPLIER + MIN_BULLET_SPEED) * -1f;

        Turret = enemyTurret;
        Turret.Position = new Vector2I(MAP_WIDTH / 2, 0);
        TurretBulletSpeed = Turret.Level * BULLET_SPEED_MULTIPLIER + MIN_BULLET_SPEED;
        TurretFiringRate = Turret.Level * FIRING_RATE_MULTIPLIER + MIN_FIRING_RATE;

        TurretMoveRate = 2f;

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
        if (Spaceship.Position.Y > 0)
            Spaceship.Position.Y--;
    }

    public static void MoveSpaceshipDown()
    {
        if (Spaceship.Position.Y < MAP_HEIGHT - 1)
            Spaceship.Position.Y++;
    }

    public static void ShootFromSpaceship()
    {
        Bullets.Add(new Bullet(Spaceship.Position.X, Spaceship.Position.Y - 1, SpaceshipBulletSpeed, new Glyph('|', ConsoleColor.Yellow), Spaceship.Damage));
    }

    public static void ShootFromTurret()
    {
        Bullets.Add(new Bullet(Turret.Position.X, Turret.Position.Y + 1, TurretBulletSpeed, new Glyph('|', ConsoleColor.Red), Turret.Damage));
    }

    private static void MoveTurret()
    {
        int direction = Random.Next(-1, 2);

        int newX = Turret.Position.X + direction;

        if (newX >= 0 && newX < MAP_WIDTH)
        {
            Turret.Position.X = newX;
        }
    }

    public static void Update()
    {
        if (!GameRunning) return;

        float currentTime = GetCurrentTime();
        float deltaTime = currentTime - LastUpdateTime;
        LastUpdateTime = currentTime;

        float timeBetweenTurretMoves = 1f / TurretMoveRate;
        if (currentTime - LastTurretMoveTime >= timeBetweenTurretMoves)
        {
            MoveTurret();
            LastTurretMoveTime = currentTime;
        }

        float timeBetweenTurretShots = 1f / TurretFiringRate;
        if (currentTime - LastTurretBulletFiredTime >= timeBetweenTurretShots)
        {
            ShootFromTurret();
            LastTurretBulletFiredTime = currentTime;
        }

        foreach (var bullet in Bullets)
        {
            bullet.Update(deltaTime);
        }

        DetectCollisions();

        Bullets.RemoveAll(b =>
            (b.Speed < 0 && b.Y < 0) ||
            (b.Speed > 0 && b.Y >= MAP_HEIGHT)
        );
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

    private static void ConcludeBattle()
    {
        GameRunning = false;
        BattleConcludedCallback?.Invoke(GetBattleResult());
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