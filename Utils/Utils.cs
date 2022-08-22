using Discord;
using NWN.Systems;
using System;
using System.Numerics;
using System.Linq;
using Anvil.API;
using System.Collections.Generic;
using NLog;
using NWN.Core;
using System.Threading.Tasks;

namespace NWN
{
  public static class Utils
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static readonly Random random = new ();
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

      if (position.Contains(','))
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
      return nAnimation switch
      {
        0 => Animation.LoopingPause,
        52 => Animation.LoopingPause2,
        30 => Animation.LoopingListen,
        32 => Animation.LoopingMeditate,
        33 => Animation.LoopingWorship,
        48 => Animation.LoopingLookFar,
        36 => Animation.LoopingSitChair,
        47 => Animation.LoopingSitCross,
        38 => Animation.LoopingTalkNormal,
        39 => Animation.LoopingTalkPleading,
        40 => Animation.LoopingTalkForceful,
        41 => Animation.LoopingTalkLaughing,
        59 => Animation.LoopingGetLow,
        60 => Animation.LoopingGetMid,
        57 => Animation.LoopingPauseTired,
        58 => Animation.LoopingPauseDrunk,
        6 => Animation.LoopingDeadFront,
        8 => Animation.LoopingDeadBack,
        15 => Animation.LoopingConjure1,
        16 => Animation.LoopingConjure2,
        93 => Animation.LoopingCustom1,
        98 => Animation.LoopingCustom2,
        101 => Animation.LoopingCustom3,
        102 => Animation.LoopingCustom4,
        103 => Animation.LoopingCustom5,
        104 => Animation.LoopingCustom6,
        105 => Animation.LoopingCustom7,
        106 => Animation.LoopingCustom8,
        107 => Animation.LoopingCustom9,
        108 => Animation.LoopingCustom10,
        109 => Animation.LoopingCustom11,
        110 => Animation.LoopingCustom12,
        111 => Animation.LoopingCustom13,
        112 => Animation.LoopingCustom14,
        113 => Animation.LoopingCustom15,
        114 => Animation.LoopingCustom16,
        115 => Animation.LoopingCustom17,
        116 => Animation.LoopingCustom18,
        117 => Animation.LoopingCustom19,
        118 => Animation.LoopingCustom20,
        119 => Animation.Mount1,
        120 => Animation.Dismount1,
        53 => Animation.FireForgetHeadTurnLeft,
        54 => Animation.FireForgetHeadTurnRight,
        55 => Animation.FireForgetPauseScratchHead,
        56 => Animation.FireForgetPauseBored,
        34 => Animation.FireForgetSalute,
        35 => Animation.FireForgetBow,
        37 => Animation.FireForgetSteal,
        29 => Animation.FireForgetGreeting,
        28 => Animation.FireForgetTaunt,
        44 => Animation.FireForgetVictory1,
        45 => Animation.FireForgetVictory2,
        46 => Animation.FireForgetVictory3,
        71 => Animation.FireForgetRead,
        70 => Animation.FireForgetDrink,
        90 => Animation.FireForgetDodgeSide,
        91 => Animation.FireForgetDodgeDuck,
        23 => Animation.LoopingSpasm,
        _ => Animation.FireForgetPauseBored,
      };
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
            icon = ItemPropertySpells2da.ipSpellTable[oItem.ItemProperties.FirstOrDefault(ip => ip.Property.PropertyType == ItemPropertyType.CastSpell).SubType.RowIndex].icon;

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

    public static readonly Dictionary<string, MainMenuCommand> mainMenuCommands = new ()
    {
      { "dm", new MainMenuCommand("Mode DM", "", CommandRank.Admin) },
      { "creaturePalette", new MainMenuCommand("Palette des créatures", "", CommandRank.DM) },
      { "itemPalette", new MainMenuCommand("Palette des objets", "", CommandRank.DM) },
      { "placeablePalette", new MainMenuCommand("Palette des placeables", "", CommandRank.DM) },
      { "placeableManager", new MainMenuCommand("Gérer les placeable de la zone", "", CommandRank.DM) },
      { "sit", new MainMenuCommand("S'asseoir n'importe où", "Permet de s'asseoir partout. Attention, seule la position affichée change. La position réelle du personnage reste la même.", CommandRank.Public) },
      { "touch", new MainMenuCommand("Mode toucher", "Permet d'éviter les collisions entre personnages (non utilisable en combat)", CommandRank.Public) },
      { "walk", new MainMenuCommand("Mode marche", "Permet d'avoir l'air moins ridicule en ville", CommandRank.Public) },
      { "follow", new MainMenuCommand("Suivre", "Suivre une créature ciblée (pour les feignasses !)", CommandRank.Public) },
      { "examineArea", new MainMenuCommand("Examiner les environs", "Obtenir une description de la zone", CommandRank.Public) },
      { "learnables", new MainMenuCommand("Journal d'apprentissage", "Ouvrir le journal d'apprentissage", CommandRank.Public) },
      { "currentJob", new MainMenuCommand("Carnet d'artisanat", "Ouvrir mon carnet d'artisanat", CommandRank.Public) },
      { "effectDispel", new MainMenuCommand("Dissiper mes effets de sorts", "", CommandRank.Public) },
      { "dispelAoE", new MainMenuCommand("Dissiper mes zones d'effets", "", CommandRank.Public) },
      { "grimoire", new MainMenuCommand("Gérer les grimoires", "Enregistrer ou charger un grimoire de sorts", CommandRank.Public) },
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
      { "dmRename", new MainMenuCommand("Changer le nom de la cible", "Permet de modifier le nom de n'importe quel objet", CommandRank.DM) }, // TODO : Ajouter à OnExamine Custom pour DM
      { "persistentPlaceables", new MainMenuCommand("Placeable persistant", "Permettre de rendre persistant les placeables créés, même après reboot", CommandRank.DM) },
      { "visualEffects", new MainMenuCommand("Gérer mes effets visuels", "Permet d'utiliser et de gérer les effets visuels personnalisés", CommandRank.DM) },
      { "reboot", new MainMenuCommand("Reboot", "", CommandRank.Admin) },
      { "refill", new MainMenuCommand("Refill ressources", "", CommandRank.Admin) },
      { "instantLearn", new MainMenuCommand("Instant Learn", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "instantCraft", new MainMenuCommand("Instant Craft", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "giveResources", new MainMenuCommand("Don de ressources", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "giveSkillbook", new MainMenuCommand("Don de skillbook", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
    };

    public static List<NuiComboEntry> appearanceEntries = new();
    public static List<NuiComboEntry> placeableEntries = new();
    public static List<NuiComboEntry> raceList = new();
    public static readonly List<NuiComboEntry> genderList = new();
    public static readonly List<NuiComboEntry> soundSetList = new();
    public static readonly List<NuiComboEntry> factionList = new();
    public static readonly List<NuiComboEntry> movementRateList = new();
    public static readonly List<NuiComboEntry> sizeList = new();
    public static readonly List<NuiComboEntry> creaturePaletteCreatorsList = new();
    public static List<PaletteEntry> creaturePaletteList = new();
    public static readonly List<NuiComboEntry> itemPaletteCreatorsList = new();
    public static List<PaletteEntry> itemPaletteList = new();
    public static readonly List<NuiComboEntry> placeablePaletteCreatorsList = new();
    public static List<PaletteEntry> placeablePaletteList = new();
    public static NuiBind<string>[] paletteColorBindings = new NuiBind<string>[256];

    public static readonly List<string> colorPaletteLeather = new();
    public static readonly List<string> colorPaletteMetal = new();

    public static readonly List<NuiComboEntry> variableTypes = new()
        {
          new NuiComboEntry("int", 1),
          new NuiComboEntry("string", 2),
          new NuiComboEntry("float", 3),
          new NuiComboEntry("date", 4)
        };

    public static void ConvertLocalVariable(string localName, string localValue, int localType, NwGameObject target, NwPlayer player)
    {
      switch (localType)
      {
        case 1:
          if (int.TryParse(localValue, out int parsedInt))
          {
            LocalVarCleaning(target, localName);
            target.GetObjectVariable<LocalVariableInt>(localName).Value = parsedInt;
          }
          else
            player.SendServerMessage($"{localName.ColorString(ColorConstants.White)} : la valeur {localValue.ColorString(ColorConstants.White)} n'est pas un entier.", ColorConstants.Red);
          break;

        case 2:

          LocalVarCleaning(target, localName);
          target.GetObjectVariable<LocalVariableString>(localName).Value = localValue;
          break;

        case 3:
          if (float.TryParse(localValue, out float parsedFloat))
          {
            LocalVarCleaning(target, localName);
            target.GetObjectVariable<LocalVariableFloat>(localName).Value = parsedFloat;
          }
          else
            player.SendServerMessage($"{localName.ColorString(ColorConstants.White)} : la valeur {localValue.ColorString(ColorConstants.White)} n'est pas un float.", ColorConstants.Red);
          break;

        case 4:
          if (DateTime.TryParse(localValue, out DateTime parsedDate))
          {
            LocalVarCleaning(target, localName);
            target.GetObjectVariable<DateTimeLocalVariable>(localName).Value = parsedDate;
          }
          else
            player.SendServerMessage($"{localName.ColorString(ColorConstants.White)} : la valeur {localValue.ColorString(ColorConstants.White)} n'est pas une date.", ColorConstants.Red);
          break;
      }
    }

    private static void LocalVarCleaning(NwGameObject target, string localName)
    {
      List<ObjectVariable> toDelete = new();
      foreach (var local in target.LocalVariables.Where(v => v.Name == localName))
        toDelete.Add(local);

      foreach (var local in toDelete)
        local.Delete();
    }

    public static double GetDamageMultiplier(double targetAC)
    {
      return Math.Pow(0.5, (targetAC - 60) / 40);
    }

    public static int GetDodgeChance(NwCreature creature)
    {
      int skillBonusDodge = PlayerSystem.Players.TryGetValue(creature, out PlayerSystem.Player player) && player.learnableSkills.ContainsKey(CustomSkill.ImprovedDodge) ? 2 * player.learnableSkills[CustomSkill.ImprovedDodge].totalPoints : 0;
      skillBonusDodge += creature.KnowsFeat(Feat.Dodge) ? 2 : 0;
      skillBonusDodge += creature.GetAbilityModifier(Ability.Dexterity) - creature.ArmorCheckPenalty - creature.ShieldCheckPenalty;
      return skillBonusDodge < 0 ? 0 : skillBonusDodge;
    }

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
