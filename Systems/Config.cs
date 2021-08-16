using System;

namespace NWN.Systems
{
  public static partial class Config
  {
    public static string database = Environment.GetEnvironmentVariable("DB_NAME");
    public static string dbPath = "Data Source=" + Environment.GetEnvironmentVariable("DB_PATH");
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
  }
}
