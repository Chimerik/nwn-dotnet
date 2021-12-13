using System;
using System.IO;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace NWN.Systems
{
  public static partial class Config
  {
    public static string database = Environment.GetEnvironmentVariable("DB_NAME");
    public static string dbPath = "Data Source=" + Environment.GetEnvironmentVariable("DB_PATH");
    public static string googleDriveCredentials = Environment.GetEnvironmentVariable("GOOGLE_DRIVE_CREDENTIALS");
    public const int invalidInput = -999999;
    public enum Env
    {
      Prod,
      Bigby,
      Chim,
    }

    public static Env env = InitEnv();

    private static Env InitEnv()
    {
      var env = Environment.GetEnvironmentVariable("ENV");

      switch (env)
      {
        default: return Env.Prod;

        case "production": return Env.Prod;
        case "Bigby": return Env.Bigby;
        case "Chim": return Env.Chim;
      }
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
