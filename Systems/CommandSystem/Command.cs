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
        public string usage { get; }
        public List<string> examples { get; }

        public Description (string title = "", string usage = "", List<string> examples = null)
        {
          this.title = title;
          this.usage = usage;
          this.examples = examples != null ? examples : new List<string>();
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
          return $"{description.title}\n" +
            $"{usageDesc}\n" +
            $"{exampleDesc}";
        }
      }

      public string usageDesc
      {
        get
        {
          return $"Usage: {PREFIX}{name} {description.usage}";
        }
      }

      public string exampleDesc
      {
        get
        {
          if (description.examples.Count == 0)
          {
            return "";
          } else if (description.examples.Count == 1)
          {
            return $"Example : {PREFIX}{name} {description.examples[0]}";
          } else
          {
            return string.Join("\n",
              "Examples :\n" + description.examples
                .Select((example) => $"* {PREFIX}{name} {example}")
              );
          }
        }
      }
    }
  }
}
