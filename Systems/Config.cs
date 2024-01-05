using System;
using System.Collections.Generic;
using System.IO;

using Anvil.API;
using Anvil.Services;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace NWN.Systems
{
  public static partial class Config
  {
    public static readonly string database = Environment.GetEnvironmentVariable("DB_NAME");
    public static readonly string dbPath = "Data Source=" + Environment.GetEnvironmentVariable("DB_PATH");
    public static readonly string googleDriveCredentials = Environment.GetEnvironmentVariable("GOOGLE_DRIVE_CREDENTIALS");
    //public static readonly string itemKey = Environment.GetEnvironmentVariable("ITEM_KEY");
    public const int InvalidInput = -999999;
    public const int MaxSerializeTimeMs = 10;
    public enum Env
    {
      Prod,
      Bigby,
      Chim,
    }

    public static readonly Env env = InitEnv();

    private static Env InitEnv()
    {
      var env = Environment.GetEnvironmentVariable("ENV");

      return env switch
      {
        "production" => Env.Prod,
        "Bigby" => Env.Bigby,
        "Chim" => Env.Chim,
        _ => Env.Prod,
      };
    }

    // CREATURES

    public static Dictionary<string, CreatureStats> creatureStats = new();

    // LOOT

    public static readonly int baseCreatureDropChance = 4;
    public static readonly int minCreatureGoldDrop = 12;
    public static readonly int maxCreatureGoldDrop = 17;
    public static readonly int creatureGoldDropAreaMultiplier = 8;
    public static readonly int mobQualityRollMultiplier = 30;
    public static readonly int bossQualityRollMultiplier = 60;

    // MATERIA

    public static readonly int baseMateriaGrowth = 360;
    public static readonly double baseMateriaGrowthMultiplier = 0.2;
    public static readonly int materiaSpawnChance = 15;
    public static readonly int minMateriaSpawnYield = 5000;
    public static readonly int maxMateriaSpawnYield = 15001;
    public static readonly int minSmallMateriaSpawnYield = 500;
    public static readonly int maxSmallMateriaSpawnYield = 5001;
    public static Dictionary<int, int[]> materiaSpawnGradeChance = new()
    {
      { 2, new int[] {80, 100} },
      { 3, new int[] {30, 80, 100} },
      { 4, new int[] {10, 40, 80, 100} },
      { 5, new int[] {13, 30, 50, 80, 100} },
      { 6, new int[] {10, 24, 40, 58, 73, 100} },
      { 7, new int[] {4, 10, 20, 38, 58, 73, 100} },
      { 8, new int[] {1, 5, 15, 26, 40, 58, 73, 100} },
      { 9, new int[] {0, 0, 10, 22, 38, 56, 75, 100} },
    };

    public static readonly double scanBaseDuration = 120;

    // EXTRACTION

    public static readonly double extractionBaseDuration = 60;
    public static readonly double extractionBaseYield = 16 * extractionBaseDuration;
    // CRAFT

    public static readonly int baseCraftToolDurability = 100;

    public static DriveService AuthenticateServiceAccount()
    {
      try
      {
        // These are the scopes of permissions you need. It is best to request only what you need and not all of them
        string[] scopes = new string[] { DriveService.Scope.DriveReadonly };

        GoogleCredential credential;
        using (var stream = new FileStream(googleDriveCredentials, FileMode.Open, FileAccess.Read))
        {
          credential = GoogleCredential.FromStream(stream)
                .CreateScoped(scopes);
        }

        // Create the  Analytics service.
        return new DriveService(new BaseClientService.Initializer()
        {
          HttpClientInitializer = credential,
          ApplicationName = "Les Larmes des Erylies",
        });
      }
      catch (Exception ex)
      {
        Console.WriteLine("Create service account DriveService failed" + ex.Message);
        throw new Exception("CreateServiceAccountDriveFailed", ex);
      }
    }

    public static readonly TargetModeSettings selectItemTargetMode = new()
    {
      ValidTargets = ObjectTypes.Item,
      CursorType = MouseCursor.Use,
      BadCursorType = MouseCursor.Nouse
    };

    public static readonly TargetModeSettings attackCreatureTargetMode = new()
    {
      ValidTargets = ObjectTypes.Creature,
      CursorType = MouseCursor.Attack,
      BadCursorType = MouseCursor.NoAttack
    };

    public static readonly TargetModeSettings selectCreatureTargetMode = new()
    {
      ValidTargets = ObjectTypes.Creature,
      CursorType = MouseCursor.Action,
      BadCursorType = MouseCursor.NoAction
    };

    public static readonly TargetModeSettings selectLocationTargetMode = new()
    {
      ValidTargets = ObjectTypes.Creature | ObjectTypes.Door | ObjectTypes.Waypoint | ObjectTypes.Trigger | ObjectTypes.AreaOfEffect | ObjectTypes.Placeable | ObjectTypes.Placeable | ObjectTypes.Tile,
      CursorType = MouseCursor.Attack,
      BadCursorType = MouseCursor.NoAttack
    };
  }
}
