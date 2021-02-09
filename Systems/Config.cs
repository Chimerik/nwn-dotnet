using System;

namespace NWN.Systems
{
  public static class Config
  {
    public static string db_path = "Data Source=" + Environment.GetEnvironmentVariable("DB_DIRECTORY");
    public static string database = Environment.GetEnvironmentVariable("DB_NAME");
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
