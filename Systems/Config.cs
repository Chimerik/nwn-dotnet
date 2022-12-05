using System;
using System.IO;

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
  }
}
