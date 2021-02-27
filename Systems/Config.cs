using System;

namespace NWN.Systems
{
  public static class Config
  {
    public static string database = Environment.GetEnvironmentVariable("DB_NAME");
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
