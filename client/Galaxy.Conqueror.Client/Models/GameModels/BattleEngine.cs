using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;

namespace Battle;

public static class BattleEngine
{
    public static int MAP_WIDTH;
    public static int MAP_HEIGHT;

    public static Spaceship spaceship;
    private static Turret turret;

    private static List<Bullet> bullets;
    private static float playerBulletSpeed;
    private static float turretBulletSpeed;

    private static float turretFiringRate;
    private static float turretMoveRate;

    private static float lastUpdateTime;
    private static float lastTurretBulletFiredTime;
    private static float lastTurretMoveTime;

    private static readonly Random random = new(0);

    public static bool gameRunning = false;

    private static Action<BattleResult> battleConcludedCallback;

    private static float battleStartTime;


    public static void Initialise(int width, int height, Spaceship playerSpaceship, Turret enemyTurret)
    {
        MAP_WIDTH = width;
        MAP_HEIGHT = height;

        spaceship = playerSpaceship;
        spaceship.Position = new Vector2I(MAP_WIDTH / 2, MAP_HEIGHT);

        turret = enemyTurret;
        turret.Position = new Vector2I(MAP_WIDTH / 2, 0);

        bullets = new List<Bullet>();
        lastUpdateTime = GetCurrentTime();
        lastTurretBulletFiredTime = GetCurrentTime();
        lastTurretMoveTime = GetCurrentTime();

        playerBulletSpeed = spaceship.Level / -10f;
        turretBulletSpeed = turret.Level / 10f;
        turretFiringRate = turret.Level / 200f;
        turretMoveRate = 20f;

        battleStartTime = GetCurrentTime();
        gameRunning = true;
    }

    public static void OnBattleConcluded(Action<BattleResult> callback)
    {
        battleConcludedCallback = callback;
    }

    private static float GetCurrentTime()
    {
        return (float)Environment.TickCount / 1000;
    }

    public static void MoveSpaceshipLeft()
    {
        if (spaceship.Position.X > 0)
            spaceship.Position.X--;
    }

    public static void MoveSpaceshipRight()
    {
        if (spaceship.Position.X < MAP_WIDTH - 1)
            spaceship.Position.X++;
    }

    public static void MoveSpaceshipUp()
    {
        if (spaceship.Position.Y > 0)
            spaceship.Position.Y--;
    }

    public static void MoveSpaceshipDown()
    {
        if (spaceship.Position.Y < MAP_HEIGHT - 1)
            spaceship.Position.Y++;
    }

    public static void ShootFromSpaceship()
    {
        bullets.Add(new Bullet(spaceship.Position.X, spaceship.Position.Y - 1, playerBulletSpeed, new Glyph('|', ConsoleColor.Yellow), 10));
    }

    public static void ShootFromTurret()
    {
        bullets.Add(new Bullet(turret.Position.X, turret.Position.Y + 1, turretBulletSpeed, new Glyph('|', ConsoleColor.Red), 10));
    }

    private static void MoveTurret()
    {
        int direction = random.Next(-1, 2);

        int newX = turret.Position.X + direction;

        if (newX >= 0 && newX < MAP_WIDTH)
        {
            turret.Position.X = newX;
        }
    }

    public static void Update()
    {
        if (!gameRunning) return;

        float currentTime = GetCurrentTime();
        float deltaTime = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;

        float timeBetweenTurretMoves = 1f / turretMoveRate;
        if (currentTime - lastTurretMoveTime >= timeBetweenTurretMoves)
        {
            MoveTurret();
            lastTurretMoveTime = currentTime;
        }

        float timeBetweenTurretShots = 1f / turretFiringRate;
        if (currentTime - lastTurretBulletFiredTime >= timeBetweenTurretShots)
        {
            ShootFromTurret();
            lastTurretBulletFiredTime = currentTime;
        }

        foreach (var bullet in bullets)
        {
            bullet.Update(deltaTime);
        }

        DetectCollisions();

        bullets.RemoveAll(b =>
            (b.Speed < 0 && b.Y < 0) ||
            (b.Speed > 0 && b.Y >= MAP_HEIGHT)
        );
    }

    public static void DetectCollisions()
    {
        List<Bullet> bulletsToRemove = new List<Bullet>();
        bool turretHit = false;

        for (int i = 0; i < bullets.Count; i++)
        {
            for (int j = i + 1; j < bullets.Count; j++)
            {
                if (bullets[i].Speed * bullets[j].Speed < 0)
                {
                    if (bullets[i].X == bullets[j].X && Math.Abs(bullets[i].Y - bullets[j].Y) <= 1)
                    {
                        bulletsToRemove.Add(bullets[i]);
                        bulletsToRemove.Add(bullets[j]);
                    }
                }
            }
        }

        foreach (var bullet in bullets)
        {
            if (bullet.Speed > 0)
            {
                if (bullet.X == spaceship.Position.X && Math.Abs(bullet.Y - spaceship.Position.Y) <= 1)
                {
                    bulletsToRemove.Add(bullet);
                    OnSpaceshipHit(bullet);
                }
            }
        }

        foreach (var bullet in bullets)
        {
            if (bullet.Speed < 0)
            {
                if (bullet.X == turret.Position.X && Math.Abs(bullet.Y - turret.Position.Y) <= 1)
                {
                    bulletsToRemove.Add(bullet);
                    OnTurretHit(bullet);
                }
            }
        }

        foreach (var bullet in bulletsToRemove)
        {
            bullets.Remove(bullet);
        }
    }

    private static void OnSpaceshipHit(Bullet bullet)
    {
        spaceship.TakeDamage(bullet);

        if (spaceship.IsDestroyed()) ConcludeBattle();
    }

    private static void OnTurretHit(Bullet bullet)
    {
        turret.TakeDamage(bullet);

        if (turret.IsDestroyed()) ConcludeBattle();
    }

    public static Dictionary<Vector2I, Glyph> GetMap()
    {
        var map = new Dictionary<Vector2I, Glyph>();

        map.Add(turret.GetPosition(), turret.Glyph);

        foreach (var bullet in bullets)
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
        return new Tuple<Vector2I, Glyph>(new Vector2I(spaceship.Position), spaceship.Glyph);
    }

    private static void ConcludeBattle()
    {
        gameRunning = false;
        battleConcludedCallback?.Invoke(GetBattleState());
    }

    public static BattleResult GetBattleState()
    {
        string winner;
        if (spaceship.IsDestroyed() && turret.IsDestroyed())
            winner = "Draw";
        else if (turret.IsDestroyed())
            winner = "Spaceship";
        else if (spaceship.IsDestroyed())
            winner = "Turret";
        else
            winner = "Unknown";

        return new BattleResult(
            spaceship.CurrentHealth,
            turret.CurrentHealth,
            winner,
            GetCurrentTime() - battleStartTime
        );
    }
}