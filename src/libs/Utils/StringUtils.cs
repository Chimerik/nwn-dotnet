using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Utils
{
  public static class StringUtils
  {
    public static string FirstCharToUpper(this string input)
    {
      switch (input)
      {
        case null: throw new ArgumentNullException(nameof(input));
        case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
        default: return input.First().ToString().ToUpper() + input.Substring(1);
      }
    }
    public static string ToDescription(this Enum value)
    {
      FieldInfo field = value.GetType().GetField(value.ToString());
      DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
      return attribute == null ? value.ToString().Replace("_", " ") : attribute.Description.Replace("_", " ");
    }
  }
}
