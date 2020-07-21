using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Options
    {
      public List<Option> positional { get; }
      public Dictionary<string, Option> named { get; }

      public Options(List<Option> positional = null, Dictionary<string, Option> named = null)
      {
        this.positional = positional ?? new List<Option>();
        this.named = named ?? new Dictionary<string, Option>();
      }

      public class Result
      {
        public List<object> positional;
        public Dictionary<string, object> named;

        public Result ()
        {
          positional = new List<object>();
          named = new Dictionary<string, object>();
        }
      }

      public Result Parse(string[] args)
      {
        var result = new Result();

        for (var i = 0; i < args.Length; i++)
        {
          var arg = args[i];

          if (arg.StartsWith("--"))
          {
            var argArray = arg.Split("=");
            var argName = argArray.ElementAtOrDefault(0).Substring(2);
            var argValue = argArray.ElementAtOrDefault(1) ?? "";

            Option option;
            if (!named.TryGetValue(argName, out option))
            {
              throw new Exception($"Unknown option: \"{argName}\"");
            }
            else
            {
              try
              {
                result.named.Add(argName, option.Parse(argValue));
              } catch (Exception e)
              {
                throw new Exception($"Invalid value for option \"{argName}\". Expected a {option.type}.");
              }
            }
          }
          else
          {
            var option = positional.ElementAtOrDefault(i);

            if (option == null)
            {
              throw new Exception($"Unknown option at position \"{i}\".");
            }

            try
            {
              result.positional.Add(option.Parse(arg));
            }
            catch (Exception e)
            {
              throw new Exception($"Invalid value for option \"^{option.name}\". Expected a {option.type}.");
            }
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
