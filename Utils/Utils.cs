﻿using Discord;
using NWN.Systems;
using System;
using System.Numerics;
using System.Linq;
using Anvil.API;
using System.Collections.Generic;
using NLog;
using NWN.Core;

namespace NWN
{
  public static class Utils
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static Random random = new Random();
    public static void LogMessageToDMs(string message)
    {
      Log.Info(message);

      switch (Config.env)
      {
        case Config.Env.Prod:
          (Bot._client.GetChannel(703964971549196339) as IMessageChannel).SendMessageAsync(message);
          break;
        case Config.Env.Bigby:
          Bot._client.GetUser(225961076448034817).SendMessageAsync(message);
          break;
        case Config.Env.Chim:
          Bot._client.GetUser(232218662080086017).SendMessageAsync(message);
          break;
      }
    }
    public static void DestroyInventory(NwCreature oContainer)
    {
      foreach (NwItem item in oContainer.Inventory.Items)
        item.Destroy();
    }
    public static void DestroyInventory(NwPlaceable oContainer)
    {
      foreach (NwItem item in oContainer.Inventory.Items)
        item.Destroy();
    }
    public static void DestroyInventory(NwStore oContainer)
    {
      foreach (NwItem item in oContainer.Items)
        item.Destroy();
    }
    public static void DestroyEquippedItems(NwCreature oCreature)
    {
      for (int i = 0; i < 17; i++)
        oCreature.GetItemInSlot((InventorySlot)i)?.Destroy();
    }

    public static double ScaleToRange(double value, double originalMin, double originalMax, double destMin, double destMax)
    {
      double result = (value - originalMin) / (originalMax - originalMin) * (destMax - destMin);
      return result + destMin;
    }

    public static Location GetLocationFromDatabase(string areaTag, string position, float facing)
    {
      Vector3 pos;

      if (position.Contains(","))
      {
        position = position.Replace("<", "");
        position = position.Replace(">", "");
        string[] splitString = position.Split(",");
        pos = new Vector3(float.TryParse(splitString[0], out float X) ? X : 0, float.TryParse(splitString[1], out float Y) ? Y : 0, float.TryParse(splitString[2], out float Z) ? Z : 0);
      }
      else
      {
        string[] splitString = position.Split(":");
        pos = new Vector3(float.TryParse(splitString[0], out float X) ? X : 0, float.TryParse(splitString[1], out float Y) ? Y : 0, float.TryParse(splitString[2], out float Z) ? Z : 0);
      }

      return Location.Create(NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == areaTag), pos, facing);
    }
    public static void BootAllPC()
    {
      foreach (NwPlayer oPC in NwModule.Instance.Players)
        oPC.BootPlayer("Le serveur redémarre. Vous pourrez vous reconnecter dans une minute.");
    }
    public static void RemoveTaggedEffect(NwGameObject oTarget, string Tag)
    {
      foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.Tag == Tag))
        oTarget.RemoveEffect(eff);
    }
    public static TimeSpan StripTimeSpanMilliseconds(TimeSpan timespan)
    {
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds);
    }
    public static string FormatTimeSpan(TimeSpan timespan)
    {
      string formattedTimespan = "";

      if (timespan.TotalSeconds < 1)
        return "Immédiat";

      if (timespan.TotalDays >= 1)
        formattedTimespan = $"{timespan.TotalDays}j ";

      if (timespan.TotalHours >= 1)
        formattedTimespan += $"{timespan.Hours}h ";

      if (timespan.TotalMinutes >= 1)
        formattedTimespan += $"{timespan.Minutes}m ";

      if (timespan.TotalHours < 1)
        formattedTimespan += $"{timespan.Seconds}s ";

      return formattedTimespan;
    }
    public static Animation TranslateEngineAnimation(int nAnimation)
    {
      switch (nAnimation)
      {
        case 0: return Animation.LoopingPause;
        case 52: return Animation.LoopingPause2;
        case 30: return Animation.LoopingListen;
        case 32: return Animation.LoopingMeditate;
        case 33: return Animation.LoopingWorship;
        case 48: return Animation.LoopingLookFar;
        case 36: return Animation.LoopingSitChair;
        case 47: return Animation.LoopingSitCross;
        case 38: return Animation.LoopingTalkNormal;
        case 39: return Animation.LoopingTalkPleading;
        case 40: return Animation.LoopingTalkForceful;
        case 41: return Animation.LoopingTalkLaughing;
        case 59: return Animation.LoopingGetLow;
        case 60: return Animation.LoopingGetMid;
        case 57: return Animation.LoopingPauseTired;
        case 58: return Animation.LoopingPauseDrunk;
        case 6: return Animation.LoopingDeadFront;
        case 8: return Animation.LoopingDeadBack;
        case 15: return Animation.LoopingConjure1;
        case 16: return Animation.LoopingConjure2;
        case 93: return Animation.LoopingCustom1;
        case 98: return Animation.LoopingCustom2;
        case 101: return Animation.LoopingCustom3;
        case 102: return Animation.LoopingCustom4;
        case 103: return Animation.LoopingCustom5;
        case 104: return Animation.LoopingCustom6;
        case 105: return Animation.LoopingCustom7;
        case 106: return Animation.LoopingCustom8;
        case 107: return Animation.LoopingCustom9;
        case 108: return Animation.LoopingCustom10;
        case 109: return Animation.LoopingCustom11;
        case 110: return Animation.LoopingCustom12;
        case 111: return Animation.LoopingCustom13;
        case 112: return Animation.LoopingCustom14;
        case 113: return Animation.LoopingCustom15;
        case 114: return Animation.LoopingCustom16;
        case 115: return Animation.LoopingCustom17;
        case 116: return Animation.LoopingCustom18;
        case 117: return Animation.LoopingCustom19;
        case 118: return Animation.LoopingCustom20;
        case 119: return Animation.Mount1;
        case 120: return Animation.Dismount1;
        case 53: return Animation.FireForgetHeadTurnLeft;
        case 54: return Animation.FireForgetHeadTurnRight;
        case 55: return Animation.FireForgetPauseScratchHead;
        case 56: return Animation.FireForgetPauseBored;
        case 34: return Animation.FireForgetSalute;
        case 35: return Animation.FireForgetBow;
        case 37: return Animation.FireForgetSteal;
        case 29: return Animation.FireForgetGreeting;
        case 28: return Animation.FireForgetTaunt;
        case 44: return Animation.FireForgetVictory1;
        case 45: return Animation.FireForgetVictory2;
        case 46: return Animation.FireForgetVictory3;
        case 71: return Animation.FireForgetRead;
        case 70: return Animation.FireForgetDrink;
        case 90: return Animation.FireForgetDodgeSide;
        case 91: return Animation.FireForgetDodgeDuck;
        case 23: return Animation.LoopingSpasm;
        default: return Animation.FireForgetPauseBored;
      }
    }
    public static async void SendMailToPC(int characterId, string senderName, string title, string message)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      await SqLiteUtils.InsertQueryAsync("messenger",
          new List<string[]>() {
            new string[] { "characterId", characterId.ToString() },
            new string[] { "senderName", senderName },
            new string[] { "title", title },
          new string[] { "message", message },
          new string[] { "sentDate", DateTime.Now.ToLongDateString() },
          new string[] { "read", "0" } });
    }
    public static async void SendDiscordPMToPlayer(int characterId, string message)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      var result = await SqLiteUtils.SelectQueryAsync("PlayerAccounts",
          new List<string>() { { "discordId" } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

      if (result != null && result.Count > 0)
      {
        await Bot._client.GetUser(ulong.Parse(result[0][0])).SendMessageAsync(message);
      }
    }
    public static async void SendItemToPCStorage(int characterId, NwItem item)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      var result = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "storage" } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

      if (result.Result != null)
      {
        NwStore storage = SqLiteUtils.StoreSerializationFormatProtection(result.Result, 0, NwModule.Instance.StartingLocation);
        item.Clone(storage);
        item.Destroy();

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "storage", storage.Serialize().ToBase64EncodedString() } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

        storage.Destroy();
      }
      else
      {
        LogMessageToDMs($"Impossible de trouver le storage du pj {characterId} et d'y déposer un objet !");
      }
    }
    public static void ResetVisualTransform(NwCreature creature)
    {
      creature.VisualTransform.Rotation = new Vector3(0, 0, 0);
      creature.VisualTransform.Translation = new Vector3(0, 0, 0);

      if (creature.IsPlayerControlled)
        creature.ControllingPlayer.CameraHeight = 0;
    }

    public static string[] GetIconResref(NwItem oItem/*, int id*/)
    {
      string icon = oItem.BaseItem.DefaultIcon;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.Cloak: // Cloaks use PLTs so their default icon doesn't really work
          icon = "iit_cloak";
          break;
        case BaseItemType.SpellScroll: // Scrolls get their icon from the cast spell property
        case BaseItemType.EnchantedScroll:

          if (oItem.HasItemProperty(ItemPropertyType.CastSpell))
            icon = ItemPropertySpells2da.spellsTable.GetSpellDataEntry(oItem.ItemProperties.FirstOrDefault(ip => ip.PropertyType == ItemPropertyType.CastSpell).SubType).icon;

          break;

        default:

          switch (oItem.BaseItem.ModelType)
          {
            case BaseItemModelType.Simple:
              icon = Util_GetSimpleIconData(oItem);
              break;

            case BaseItemModelType.Composite:
              return Util_GetComplexIconData(oItem/*, id*/);
          }

          break;
      }

      /*List<NuiDrawListItem> iconDrawListItems = new List<NuiDrawListItem>();
      NuiSpacer spacer = new NuiSpacer() { Id = $"examine_{id}", DrawList = iconDrawListItems, Width = 75, Height = 125 };

      if (NWScript.ResManGetAliasFor(icon, NWScript.RESTYPE_TGA) != "")
        iconDrawListItems.Add(new NuiDrawListImage(icon, new NuiRect(0, 0, 25, 25)));*/

      return new string[3] { icon, "", "" };
    }

    public static string Util_GetSimpleIconData(NwItem item)
    {
      string sSimpleModelId = item.Appearance.GetSimpleModel().ToString().PadLeft(3, '0');
      string sDefaultIcon = item.BaseItem.DefaultIcon;

      switch (item.BaseItem.ItemType)
      {
        case BaseItemType.MiscSmall:
        case BaseItemType.CraftMaterialSmall:
          sDefaultIcon = "iit_smlmisc_" + sSimpleModelId;
          break;
        case BaseItemType.MiscMedium:
        case BaseItemType.CraftMaterialMedium:
        case (BaseItemType)112:/* Crafting Base Material */
          sDefaultIcon = "iit_midmisc_" + sSimpleModelId;
          break;
        case BaseItemType.MiscLarge:
          sDefaultIcon = "iit_talmisc_" + sSimpleModelId;
          break;
        case BaseItemType.MiscThin:
          sDefaultIcon = "iit_thnmisc_" + sSimpleModelId;
          break;
      }

      int nLength = sDefaultIcon.Length;
      if (sDefaultIcon.Substring(nLength - 4, 1) == "_")// Some items have a default icon of xx_yyy_001, we strip the last 4 symbols if that is the case
        sDefaultIcon = sDefaultIcon.Remove(nLength - 4);
      string sIcon = sDefaultIcon + "_" + sSimpleModelId;

      if (NWScript.ResManGetAliasFor(sIcon, NWScript.RESTYPE_TGA) != "")// Check if the icon actually exists, if not, we'll fall through and return the default icon
        return sIcon;
      else
        return "";
    }
    public static string[] Util_GetComplexIconData(NwItem item/*, int id*/)
    {
      //List<NuiDrawListItem> iconDrawListItems = new List<NuiDrawListItem>();
      //NuiSpacer spacer = new NuiSpacer() { Id = $"examine_{id}", DrawList = iconDrawListItems, Width = 75, Height = 125 };

      string topIcon = item.BaseItem.DefaultIcon + "_t_" + item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Top).ToString().PadLeft(3, '0');
      string midIcon = item.BaseItem.DefaultIcon + "_m_" + item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Middle).ToString().PadLeft(3, '0');
      string botIcon = item.BaseItem.DefaultIcon + "_b_" + item.Appearance.GetWeaponModel(ItemAppearanceWeaponModel.Bottom).ToString().PadLeft(3, '0');

      /*if (NWScript.ResManGetAliasFor(botIcon, NWScript.RESTYPE_TGA) != "")
        iconDrawListItems.Add(new NuiDrawListImage(botIcon, new NuiRect(0, 0, 25, 25)));

      if (NWScript.ResManGetAliasFor(midIcon, NWScript.RESTYPE_TGA) != "")
        iconDrawListItems.Add(new NuiDrawListImage(midIcon, new NuiRect(0, 0, 25, 25)));

      if (NWScript.ResManGetAliasFor(topIcon, NWScript.RESTYPE_TGA) != "")
        iconDrawListItems.Add(new NuiDrawListImage(topIcon, new NuiRect(0, 0, 25, 25)));*/

      return new string[3] { topIcon, midIcon, botIcon };
    }

    public static Dictionary<string, MainMenuCommand> mainMenuCommands = new Dictionary<string, MainMenuCommand>()
    {
      { "dm", new MainMenuCommand("Mode DM", "", CommandRank.Admin) },
      { "touch", new MainMenuCommand("Mode toucher", "Permet d'éviter les collisions entre personnages (non utilisable en combat)", CommandRank.Public) },
      { "walk", new MainMenuCommand("Mode marche", "Permet d'avoir l'air moins ridicule en ville", CommandRank.Public) },
      { "examineArea", new MainMenuCommand("Examine les environs", "Obtenir une description de la zone", CommandRank.Public) },
      { "grimoire", new MainMenuCommand("Gérer les grimoires", "Enregistrer ou charger un grimoire de sorts", CommandRank.Public)  },
      { "quickbars", new MainMenuCommand("Gérer les barres de raccourcis", "Enregistrer ou charger une barre de raccourcis", CommandRank.Public) },
      { "commend", new MainMenuCommand("Recommander un joueur", "Recommander un joueur pour la qualité de son roleplay et son implication sur le module", CommandRank.Public) }, // TODO : Ajouter à OnExamine Player
      { "itemAppearance", new MainMenuCommand("Gestion des apparences d'objets", "Enregistrer ou charger une apparence d'objet", CommandRank.Public) },
      { "description", new MainMenuCommand("Gérer les descriptions", "Enregistrer ou charger une description de personnage", CommandRank.Public) },
      { "chat", new MainMenuCommand("Gestion des couleurs du chat", "Personnaliser les couleurs du chat", CommandRank.Public) },
      { "unstuck", new MainMenuCommand("Déblocage du décor", "Tentative de déblocage du décor (succès non garanti)", CommandRank.Public) },
      { "reinitPositionDisplay", new MainMenuCommand("Réinitialiser la position affichée", "Réinitialise la position affichée du personnage (à utiliser en cas de problème avec le système d'assise)", CommandRank.Public) },
      { "publicKey", new MainMenuCommand("Afficher ma clé publique", "Permet d'obtenir la clé publique de votre compte, utile pour lier le compte Discord au compte Never", CommandRank.Public) },
      { "delete", new MainMenuCommand("Supprimer ce personnage", "Attention, la suppression est définitive", CommandRank.Public) },
      { "wind", new MainMenuCommand("Gestion du vent", "Permet de modifier la configuration du vent de cette zone", CommandRank.DM) },
      { "listenAll", new MainMenuCommand("Ecoute globale", "Permet d'écouter tous les joueurs, où qu'ils fussent", CommandRank.DM) },
      { "listen", new MainMenuCommand("Ecoute ciblée", "Permet d'écouter le joueur sélectionné, où qu'il soit", CommandRank.DM)}, // TODO : Ajouter à OnExamine Player
      { "dmRename", new MainMenuCommand("Changer le nom de la cible", "Permet de modifier le nom de n'importe quel objet", CommandRank.DM) }, // TODO : Ajouter à OnExamine Custom pour DM
      { "persistentPlaceables", new MainMenuCommand("Placeable persistant", "Permettre de rendre persistant les placeables créés, même après reboot", CommandRank.DM) },
      { "visualEffects", new MainMenuCommand("Gérer mes effets visuels", "Permet d'utiliser et de gérer les effets visuels personnalisés", CommandRank.DM) },
      { "reboot", new MainMenuCommand("Reboot", "", CommandRank.Admin) },
      { "refill", new MainMenuCommand("Refill ressources", "", CommandRank.Admin) },
      { "instantLearn", new MainMenuCommand("Instant Learn", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "instantCraft", new MainMenuCommand("Instant Craft", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "giveResources", new MainMenuCommand("Don de ressources", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "giveSkillbook", new MainMenuCommand("Don de skillbook", "", CommandRank.Admin) } // TODO : Ajouter à OnExamine Player
    };

    public enum CommandRank
    {
      Public,
      DM,
      Admin
    }
    public class MainMenuCommand
    {
      public string label;
      public string tooltip;
      public CommandRank rank;

      public MainMenuCommand(string label, string tooltip, CommandRank rank)
      {
        this.label = label;
        this.tooltip = tooltip;
        this.rank = rank;
      }
    }
  }
}
