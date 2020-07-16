using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Options
    {
      List<Option> positional;
      Dictionary<string, Option> named;

      public Options (List<Option> positional = null, Dictionary<string, Option> named = null)
      {
        this.positional = positional ?? new List<Option>();
        this.named = named ?? new Dictionary<string, Option>();
      }

      public class Result
      {
        public List<string> positional;
        public Dictionary<string, string> named;
      }

      public Result Parse (string[] args)
      {
        // TODO
      }
    }
  }
}
