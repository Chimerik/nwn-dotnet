using System.Collections.Generic;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Options
    {
      List<Option> positional;
      Dictionary<string, Option> named;

      public Options (List<Option> positional, Dictionary<string, Option> named)
      {
        this.positional ??= new List<Option>();
        this.named ??= new Dictionary<string, Option>();
      }

      void Parse (string args)
      {
        // TODO
      }
    }
  }
}
