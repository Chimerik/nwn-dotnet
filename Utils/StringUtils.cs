using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Anvil.API;

using Newtonsoft.Json;

using NWN.Core;
using NWN.Systems;

namespace NWN
{
  public static class StringUtils
  {
    public static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
    public static string[] noReplyArray = { "Banque Skalsgard" };

    public static string FirstCharToUpper(this string input)
    {
      return input switch
      {
        null => throw new ArgumentNullException(nameof(input)),
        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        _ => string.Concat(input.First().ToString().ToUpper(), input.AsSpan(1)),
      };
    }

    public static int NthIndexOf(string s, char c, int n)
    {
      var takeCount = s.TakeWhile(x => (n -= (x == c ? 1 : 0)) > 0).Count();
      return takeCount == s.Length ? -1 : takeCount;
    }
    public static string ToDescription(this Enum value)
    {
      try
      {
        FieldInfo field = value.GetType().GetField(value.ToString());
        return Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is not DescriptionAttribute attribute ? value.ToString().Replace("_", " ") : attribute.Description.Replace("_", " ");
      }
      catch(Exception e)
      {
        Utils.LogMessageToDMs($"{e.Message}\n\n" +
          $"{e.StackTrace}\n\n" +
          $"value : {value}");

        return "";
      }
    }
    public static string TranslateAttributeToFrench(Ability ability)
    {
      return ability switch
      {
        Ability.Strength => "Force",
        Ability.Dexterity => "Dextérité",
        Ability.Wisdom => "Sagesse",
        Ability.Charisma => "Charisme",
        _ => ability.ToString(),
      };
    }
    public static string GetAttributeIcon(Ability ability)
    {
      return ability switch
      {
        Ability.Strength => "ief_inc_str",
        Ability.Dexterity => "ief_inc_dex",
        Ability.Constitution => "ief_inc_con",
        Ability.Intelligence => "ief_inc_int",
        Ability.Wisdom => "ief_inc_wis",
        Ability.Charisma => "ief_inc_cha",
        _ => ability.ToString(),
      };
    }
    public static async Task<Stream> GenerateStreamFromString(string s)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      await writer.WriteAsync(s);
      await writer.FlushAsync();
      stream.Position = 0;
      return stream;
    }

    public static async Task<string> DownloadGoogleDoc(string fileId)
    {
      var request = ModuleSystem.googleDriveService.Files.Export(fileId, "text/plain");

      using var stream = new MemoryStream();
      await request.DownloadAsync(stream);
      stream.Position = 0;
      var reader = await new StreamReader(stream).ReadToEndAsync();
      return reader.Replace("\r\n", "\n").Replace("’", "'");
    }

    public static async Task<string> DownloadGoogleDocFromName(string fileName)
    {
      var request = ModuleSystem.googleDriveService.Files.List();
      request.Q = $"name = '{fileName}'";
      var files = request.Execute().Files;

      if(files.Count < 1)
      {
        Utils.LogMessageToDMs($"GDoc introuvable : {fileName}");
        return "";
      }
      
      var exportRequest = ModuleSystem.googleDriveService.Files.Export(files.FirstOrDefault().Id, "text/plain");
      using var stream = new MemoryStream();
      await exportRequest.DownloadAsync(stream);
      stream.Position = 0;
      var reader = await new StreamReader(stream).ReadToEndAsync();
      return reader.Replace("\r\n", "\n").Replace("’", "'");
    }

    public static string GetMetalPaletteResRef(int color)
    {
      return NWScript.ResManGetAliasFor($"metal{color}", NWScript.RESTYPE_TGA) != "" ? $"metal{color}" : $"leather{color}";
    }
    public static string ConvertToUTF8(string toConvert)
    {
      if (string.IsNullOrEmpty(toConvert))
        return "";
      
      return Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(toConvert));
    }
    public static string ToWhitecolor(int toColorWhite)
    {
      return toColorWhite.ToString().ColorString(ColorConstants.White);
    }
    public static string ToWhitecolor(string toColorWhite)
    {
      return toColorWhite.ColorString(ColorConstants.White);
    }
    public static void DisplayStringToAllPlayersNearTarget(NwCreature target, string message)
    {
      foreach (NwPlayer player in NwModule.Instance.Players)
        if (player?.ControlledCreature?.Area == target?.Area && player?.ControlledCreature.DistanceSquared(target) < 1225)
          player.DisplayFloatingTextStringOnCreature(target, message);
    }
    public static double GetDrawListTextPositionScaledToUI(int uiScale)
    {
      if (uiScale < 160)
        return Math.Round(2 - (0.14 * ((uiScale / 100 - 1) * 10)), 2, MidpointRounding.ToEven);
      else
      {
        return uiScale switch
        {
          160 => 1.22,
          170 => 1.2,
          180 => 1.14,
          190 => 1.1,
          _ => 1,
        };
      } 
    }
    public static void UpdateQuickbarPostring(PlayerSystem.Player player, int featId, int adrenaline)
    {
      byte slotId;

      for (slotId = 0; slotId < 13; slotId++)
      {
        var slot = player.oid.LoginCreature.GetQuickBarButton(slotId);

        if (slot.ObjectType == QuickBarButtonType.Feat && slot.Param1 == featId)
          break;
      }

      if (slotId > 11)
        return;

      Color color;

      if (player.chatColors.TryGetValue(102, out byte[] colorArray))
        color = new(colorArray[0], colorArray[1], colorArray[2], colorArray[3]);
      else
        color = ColorConstants.Red;

      player.oid.PostString(adrenaline.ToString(), GetUIScaledPosition(player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale), slotId), 100, ScreenAnchor.TopLeft, 25, color, color, featId + 10000);
    }
    public static int GetUIScaledPosition(int uiScale, int slotId)
    {
      return uiScale switch
      {
        110 => slotId switch
        {
          1 => 51,
          2 => 57,
          3 => 64,
          4 => 71,
          5 => 77,
          6 => 84,
          7 => 91,
          8 => 97,
          9 => 104,
          10 => 111,
          11 => 117,
          _ => 44,
        },
        120 => slotId switch
        {
          1 => 44,
          2 => 50,
          3 => 57,
          4 => 64,
          5 => 70,
          6 => 77,
          7 => 84,
          8 => 90,
          9 => 97,
          10 => 104,
          11 => 110,
          _ => 37,
        },
        130 => slotId switch
        {
          1 => 38,
          2 => 44,
          3 => 51,
          4 => 58,
          5 => 65,
          6 => 71,
          7 => 78,
          8 => 84,
          9 => 91,
          10 => 98,
          11 => 104,
          _ => 31,
        },
        140 => slotId switch
        {
          1 => 33,
          2 => 39,
          3 => 46,
          4 => 53,
          5 => 60,
          6 => 66,
          7 => 73,
          8 => 79,
          9 => 86,
          10 => 93,
          11 => 99,
          _ => 26,
        },
        150 => slotId switch
        {
          1 => 29,
          2 => 35,
          3 => 42,
          4 => 48,
          5 => 55,
          6 => 62,
          7 => 68,
          8 => 75,
          9 => 82,
          10 => 88,
          11 => 95,
          _ => 22,
        },
        _ => slotId switch
        {
          1 => 59,
          2 => 65,
          3 => 72,
          4 => 79,
          5 => 86,
          6 => 92,
          7 => 99,
          8 => 106,
          9 => 112,
          10 => 119,
          11 => 125,
          _ => 52,
        },
      };
    }
  }
}
