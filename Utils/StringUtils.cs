using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

using NWN.Systems;

namespace NWN
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
    public async static Task<Stream> GenerateStreamFromString(string s)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      await writer.WriteAsync(s);
      await writer.FlushAsync();
      stream.Position = 0;
      return stream;
    }
    public async static Task<string> SerializeObjectToJsonString(Systems.Alchemy.Cauldron alchemyCauldron)
    {
      using (var stream = new MemoryStream())
      {
        if (alchemyCauldron != null)
        {
          await JsonSerializer.SerializeAsync(stream, alchemyCauldron);
          stream.Position = 0;
          using var reader = new StreamReader(stream);
          return await reader.ReadToEndAsync();
        }
        else
          return "";
      }
    }
    public async static Task<string> SerializeObjectToJsonString(Dictionary<string, Learnable> learnables)
    {
      using (var stream = new MemoryStream())
      {
        if (learnables.Count > 0)
        {
          await JsonSerializer.SerializeAsync(stream, learnables);
          stream.Position = 0;
          using var reader = new StreamReader(stream);
          return await reader.ReadToEndAsync();
        }
        else
          return "";
      }
    }
    public async static Task<string> SerializeObjectToJsonString(Dictionary<string, byte[]> explorationState)
    {
      using (var stream = new MemoryStream())
      {
        if (explorationState.Count > 0)
        {
          await JsonSerializer.SerializeAsync(stream, explorationState);
          stream.Position = 0;
          using var reader = new StreamReader(stream);
          return await reader.ReadToEndAsync();
        }
        else
          return "";
      }
    }
  }
}
