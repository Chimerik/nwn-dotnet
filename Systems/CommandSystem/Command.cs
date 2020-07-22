using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private class Command
    {
      public string name { get; }
      public Action<ChatSystem.Context, Options.Result> execute { get; }
      public Description description { get; }
      public Options options { get; }

      public Command(
        string name,
        Action<ChatSystem.Context, Options.Result> execute,
        Description description,
        Options options = null
      )
      {
        this.name = name;
        this.execute = execute;
        this.description = description;
        this.options = options ?? new Options();
      }

      public struct Description
      {
        public string title { get; }
        public string[] examples { get; }

        public Description (string title = "", string[] examples = null)
        {
          this.title = title;
          this.examples = examples ?? new string[] { "" };
        }
      }

      public string shortDesc
      {
        get
        {
          return $"* {PREFIX}{name} : {description.title}";
        }
      }

      public string longDesc
      {
        get
        {
          return $"\nUsage: {usageDesc}\n\n" +
            $"{description.title}\n\n" +
            $"{optionsDesc}\n" +
            $"{exampleDesc}";
        }
      }

      public string optionsDesc
      {
        get
        {
          if (options.positional.Count == 0 &&
            options.named.Count == 0  
          )
          {
            return "";
          } else
          {
            return $"Options:\n" +
            string.Join("", options.positional.Select((option) => getOptionDesc(option) + '\n')) +
            string.Join("", options.named.Select((entree) => getOptionDesc(entree.Value) + '\n'));
          }
        }
      }

      string getOptionDesc (Option option)
      {
        var desc = $"* {option.name} - {option.description}\n";
        desc += $"Required: {option.isRequired}";

        if (!option.isRequired)
        {
          desc += $" - Default Value: \"{option.defaultValue}\"";
        }

        return desc;
      }

      public string usageDesc
      {
        get
        {
          var desc = $"{PREFIX}{name}";

          foreach (var option in options.positional)
          {
            var bfrBracket = option.isRequired ? "" : "[";
            var aftBracket = option.isRequired ? "" : "]";

            desc += $" {bfrBracket}{option.name}{aftBracket}";
          }
          foreach (var entry in options.named)
          {
            var bfrBracket = entry.Value.isRequired ? "" : "[";
            var aftBracket = entry.Value.isRequired ? "" : "]";

            desc += $" {bfrBracket}--{entry.Value.name}{aftBracket}";
          }
          return desc;
        }
      }

      public string exampleDesc
      {
        get
        {
          if (description.examples.Length == 0)
          {
            return "";
          } else if (description.examples.Length == 1)
          {
            return $"Example : {PREFIX}{name} {description.examples[0]}";
          } else
          {
            return "Examples :\n" +
              string.Join("", description.examples
                .Select((example) => $"* {PREFIX}{name} {example}\n")
              );
          }
        }
      }
    }
  }
}
