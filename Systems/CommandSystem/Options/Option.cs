using System;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private enum OptionTypes
    {
      String,
      Int,
      Number,
      Bool
    }
    private class Option
    {
      public string name { get; }
      public string description { get; }
      public object defaultValue { get; }
      public bool isRequired { get; }
      public OptionTypes type { get; }

      public Option (
        string name,
        string description,
        object defaultValue,
        bool isRequired = false,
        OptionTypes type = OptionTypes.String
      )
      {
        this.name = name;
        this.description = description;
        this.isRequired = isRequired;
        this.defaultValue = defaultValue;
        this.type = type;
      }

      public object Parse (string value)
      {
        switch (type)
        {
          default: throw new Exception("Not implemented.");

          case OptionTypes.String: return value;
          case OptionTypes.Int: return int.Parse(value);
          case OptionTypes.Number: return double.Parse(value);
          case OptionTypes.Bool: return (value == null || value == "false")
              ? false
              : true;
        }
      }
    }
  }
}
