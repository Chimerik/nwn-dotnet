using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Anvil.API;

using Newtonsoft.Json;

namespace NWN
{
  public static class StringUtils
  {
    public static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

    public static string FirstCharToUpper(this string input)
    {
      switch (input)
      {
        case null: throw new ArgumentNullException(nameof(input));
        case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
        default: return input.First().ToString().ToUpper() + input.Substring(1);
      }
    }

    public static int NthIndexOf(string s, char c, int n)
    {
      var takeCount = s.TakeWhile(x => (n -= (x == c ? 1 : 0)) > 0).Count();
      return takeCount == s.Length ? -1 : takeCount;
    }
    public static string ToDescription(this Enum value)
    {
      FieldInfo field = value.GetType().GetField(value.ToString());
      DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
      return attribute == null ? value.ToString().Replace("_", " ") : attribute.Description.Replace("_", " ");
    }
    public static string TranslateAttributeToFrench(Ability ability)
    {
      switch(ability)
      {
        case Ability.Strength:
          return "Force";
        case Ability.Dexterity:
          return "Dextérité";
        case Ability.Wisdom:
          return "Sagesse";
        case Ability.Charisma:
          return "Charisme";
      }

      return ability.ToString();
    }
    public async static Task<Stream> GenerateStreamFromString(string s)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      await writer.WriteAsync(s);
      await writer.FlushAsync();
      stream.Position = 0;
      return stream;
    }
  }
}
