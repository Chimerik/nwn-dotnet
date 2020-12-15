using System;

namespace NWN.Systems
{
  public static class Config
  {
    public enum Env
    {
      Dev,
      Prod,
      Bigby,
      Chim,
    }

    public static Env env = InitEnv();

    private static Env InitEnv ()
    {
      var env = Environment.GetEnvironmentVariable("ENV");

      switch (env)
      {
        default: return Env.Prod;

        case "production": return Env.Prod;
        case "development": return Env.Dev;
        case "Bigby": return Env.Bigby;
        case "Chim": return Env.Chim;
      }
    }
  }
}
