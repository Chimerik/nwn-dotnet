using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Options
    {
      List<Option> positional;
      Dictionary<string, Option> named;

      public Options(List<Option> positional = null, Dictionary<string, Option> named = null)
      {
        this.positional = positional ?? new List<Option>();
        this.named = named ?? new Dictionary<string, Option>();
      }

      public class Result
      {
        public List<string> positional;
        public Dictionary<string, string> named;

        public Result ()
        {
          positional = new List<string>();
          named = new Dictionary<string, string>();
        }
      }

      public Result Parse(string[] args)
      {
        var result = new Result();

        foreach (var arg in args)
        {
          if (arg.StartsWith("--"))
          {
            var argArray = arg.Split("=");
            var argName = argArray.ElementAtOrDefault(0).Substring(2);
            var argValue = argArray.ElementAtOrDefault(1);

            Option option;
            if (!named.TryGetValue(argName, out option))
            {
              throw new Exception($"Unknown option: \"{argName}\"");
            }
            else
            {
              result.named.Add(argName, argValue);
            }
          }
          else
          {
            result.positional.Add(arg);
          }
        }

        // Add default values for positional
        if (result.positional.Count < positional.Count)
        {
          for (var i = result.positional.Count; i < positional.Count; i++)
          {
            var option = positional[i];
            if (option.isRequired)
            {
              throw new Exception("Missing a required positional option.");
            }
            else
            {
              result.positional.Add(option.defaultValue);
            }
          }
        }

        // Add default values for named
        foreach (KeyValuePair<string, Option> entry in named)
        {
          if (!result.named.ContainsKey(entry.Key))
          {
            if (entry.Value.isRequired)
            {
              throw new Exception($"Missing required option : \"{entry.Key}\"");
            } else
            {
              result.named.Add(entry.Key, entry.Value.defaultValue);
            }
          }
        }

        return result;
      }
    }
  }
}
