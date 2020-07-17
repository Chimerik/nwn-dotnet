using System;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    public class Command
    {
      public string name { get; }
      public Action<ChatSystem.Context> execute { get; }
      public Description description { get; }

      public Command (string name, Action<ChatSystem.Context> execute, Description description)
      {
        this.name = name;
        this.execute = execute;
        this.description = description;
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
