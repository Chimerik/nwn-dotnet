﻿using System;
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
  public static partial class StringUtils
  {
    public static readonly string shieldArmorDisadvantageEffectTag = "_DISADVANTAGE_SHIELD_ARMOR_PROFICIENCY";
    public static readonly Native.API.CExoString shieldArmorDisadvantageEffectExoTag = "_DISADVANTAGE_SHIELD_ARMOR_PROFICIENCY".ToExoString();
    public static readonly string threatenedEffectTag = "_THREATENED_EFFECT";
    public static readonly Native.API.CExoString threatenedEffectExoTag = "_THREATENED_EFFECT".ToExoString();
    public static readonly string frightenedEffectTag = "FRIGHTENED_";
    public static readonly Native.API.CExoString frightenedEffectExoTag = "FRIGHTENED_".ToExoString();
    public static readonly Native.API.CExoString exoDelimiter = "_".ToExoString();

    public static JsonSerializerSettings settings = new() { TypeNameHandling = TypeNameHandling.All };
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
    public static void DisplayStringToAllPlayersNearTarget(NwCreature target, string message, Color color, bool includeSelf = false)
    {
      foreach (NwPlayer player in NwModule.Instance.Players)
        if ((player != target.ControllingPlayer || (includeSelf && player == target.ControllingPlayer)) 
          && player?.ControlledCreature?.Area == target?.Area && player?.ControlledCreature.DistanceSquared(target) < 1225)
          player.DisplayFloatingTextStringOnCreature(target, message.ColorString(color));
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

      Color color = player.chatColors.TryGetValue(102, out byte[] colorArray) ? new(colorArray[0], colorArray[1], colorArray[2], colorArray[3]) 
        : ColorConstants.Red;

      player.oid.PostString(adrenaline.ToString(), player.cooldownPositions.xPos + slotId * player.cooldownPositions.spacing, 100, ScreenAnchor.TopLeft, 25, color, color, featId + 10000);
    }
  }
}