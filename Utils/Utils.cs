using Discord;
using NWN.Systems;
using System;
using System.Numerics;
using System.Linq;
using Anvil.API;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NWN
{
  public static class Utils
  {
    public static readonly Random random = new();
    public enum SubscriptionType
    {
      MailNotification = 1,
      MailDistantAccess = 2
    }
    public static int RollAdvantage(int advantage, bool displayLogs = true)
    {
      int attackRoll = 0;
      int tempAttackRoll;

      if (advantage == 0)
        attackRoll = NwRandom.Roll(random, 20);
      else if (advantage > 0)
      {
        attackRoll = NwRandom.Roll(random, 20);
        tempAttackRoll = NwRandom.Roll(random, 20);

        if(displayLogs)
          LogUtils.LogMessage($"Jet avec avantage : {attackRoll} et {tempAttackRoll}", LogUtils.LogType.Combat);
        
        attackRoll = attackRoll > tempAttackRoll ? attackRoll : tempAttackRoll;
      }
      else if (advantage < 0)
      {
        attackRoll = NwRandom.Roll(random, 20);
        tempAttackRoll = NwRandom.Roll(random, 20);

        if (displayLogs)
          LogUtils.LogMessage($"Jet avec désavantage : {attackRoll} et {tempAttackRoll}", LogUtils.LogType.Combat);
        
        attackRoll = attackRoll < tempAttackRoll ? attackRoll : tempAttackRoll;
      }

      if (displayLogs)
        LogUtils.LogMessage($"Jet : {attackRoll}", LogUtils.LogType.Combat);

      return attackRoll;
    }
    public static void LogMessageToDMs(string message)
    {
      ModuleSystem.Log.Info(message);
      SendDiscordLog(message, 0);

      //Bot.bigbyDiscordUser.SendMessageAsync(message); Bigby user
      //Bot.chimDiscordUser.SendMessageAsync(message); Chim user
    }
    private static async void SendDiscordLog(string message, int nbTry)
    {
      try
      {
        switch (Config.env)
        {
          case Config.Env.Prod: await Bot.logChannel.SendMessageAsync(message); break;
          case Config.Env.Bigby: await Bot.logChannel.SendMessageAsync("Bigby test : " + message); break;
          case Config.Env.Chim: await Bot.logChannel.SendMessageAsync("Chim test : " + message); break;
        }
      }
      catch (Exception)
      {
        await Task.Delay(10000);

        if (nbTry < 5)
        {
          ModuleSystem.Log.Info($"Retrying ({nbTry}) to send Discord message");
          SendDiscordLog(message, nbTry++);
        }
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
    public static bool In(this SpellSchool me, params SpellSchool[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this EffectType me, params EffectType[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this ClassType me, params ClassType[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this RacialType me, params RacialType[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this Native.API.RacialType me, params Native.API.RacialType[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this int me, params int[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this DamageType me, params DamageType[] set)
    {
      return set.Contains(me);
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

      if (result != null)
      {
        NwStore storage = SqLiteUtils.StoreSerializationFormatProtection(result.FirstOrDefault()[0], NwModule.Instance.StartingLocation);
        item.Clone(storage);
        item.Destroy();

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "storage", storage.Serialize().ToBase64EncodedString() } },
          new List<string[]>() { new string[] { "ROWID", characterId.ToString() } });

        storage.Destroy();
      }
      else
      {
        LogUtils.LogMessage($"Impossible de trouver le storage du pj {characterId} et d'y déposer un objet !", LogUtils.LogType.PlayerConnections);
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

          ItemProperty ip = oItem.ItemProperties.FirstOrDefault(ip => ip.Property.PropertyType == ItemPropertyType.CastSpell);

          if (ip != null)
            icon = ip.SubTypeTable.GetString(ip.SubType.RowIndex, "Icon");

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
        return sDefaultIcon;
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

    public static readonly Dictionary<string, MainMenuCommand> mainMenuCommands = new()
    {
      { "shortRest", new MainMenuCommand("Simuler un repos court (alpha)", "", CommandRank.Public) },
      { "longRest", new MainMenuCommand("Simuler un repos long (alpha)", "", CommandRank.Public) },
      { "sacrificeHP", new MainMenuCommand("Perdre 20 % de points de vie (alpha)", "", CommandRank.Public) },
      { "dm", new MainMenuCommand("Mode DM", "", CommandRank.Admin) },
      { "spellBook", new MainMenuCommand("Livre de sorts", "", CommandRank.Public) },
      { "chatimentLevelSelection", new MainMenuCommand("Châtiment - Niveau de sort", "Ouvrir ou fermer la fenêtre de sélection du niveau de sorts de vos châtiments", CommandRank.Public) },
      { "addClass", new MainMenuCommand("Test Chim", "", CommandRank.Admin) },
      { "creaturePalette", new MainMenuCommand("Palette des créatures", "", CommandRank.DM) },
      { "itemPalette", new MainMenuCommand("Palette des objets", "", CommandRank.DM) },
      { "placeablePalette", new MainMenuCommand("Palette des placeables", "", CommandRank.DM) },
      { "placeableManager", new MainMenuCommand("Gérer les placeable de la zone", "", CommandRank.DM) },
      { "mailBox", new MainMenuCommand("Corbeau Messager", "Le corbeau Skalsgard vous apporte vos missives où que vous vous trouviez", CommandRank.Public) },
      { "sit", new MainMenuCommand("S'asseoir n'importe où", "Permet de s'asseoir partout. Attention, seule la position affichée change. La position réelle du personnage reste la même.", CommandRank.Public) },
      { "touch", new MainMenuCommand("Mode toucher", "Permet d'éviter les collisions entre personnages (non utilisable en combat)", CommandRank.Public) },
      { "walk", new MainMenuCommand("Mode marche", "Permet d'avoir l'air moins ridicule en ville", CommandRank.Public) },
      { "follow", new MainMenuCommand("Suivre", "Suivre une créature ciblée (pour les feignasses !)", CommandRank.Public) },
      { "examineArea", new MainMenuCommand("Examiner les environs", "Obtenir une description de la zone", CommandRank.Public) },
      { "learnables", new MainMenuCommand("Journal d'apprentissage", "Ouvrir le journal d'apprentissage", CommandRank.Public) },
      { "currentJob", new MainMenuCommand("Carnet d'artisanat", "Ouvrir mon carnet d'artisanat", CommandRank.Public) },
      { "language", new MainMenuCommand("Langue", "Choisir la langue actuellement parlée par mon personnage", CommandRank.Public) },
      { "effectDispel", new MainMenuCommand("Dissiper mes effets de sorts", "", CommandRank.Public) },
      { "dispelAoE", new MainMenuCommand("Dissiper mes zones d'effets", "", CommandRank.Public) },
      //{ "healthManaBars", new MainMenuCommand("Dé/Verrouiller l'affichage des barres de statuts", "", CommandRank.Public) },
      { "grimoire", new MainMenuCommand("Gérer les grimoires", "Enregistrer ou charger un grimoire de sorts", CommandRank.Public) },
      { "quickbars", new MainMenuCommand("Gérer les barres de raccourcis", "Enregistrer ou charger une barre de raccourcis", CommandRank.Public) },
      { "commend", new MainMenuCommand("Recommander un joueur", "Recommander un joueur pour la qualité de son roleplay et son implication sur le module", CommandRank.Public) }, // TODO : Ajouter à OnExamine Player
      { "itemAppearance", new MainMenuCommand("Gestion des apparences d'objets", "Enregistrer ou charger une apparence d'objet", CommandRank.Public) },
      { "description", new MainMenuCommand("Gérer les descriptions", "Enregistrer ou charger une description de personnage", CommandRank.Public) },
      { "chat", new MainMenuCommand("Gestion des couleurs du chat", "Personnaliser les couleurs du chat", CommandRank.Public) },
      { "cooldownPosition", new MainMenuCommand("Gérer l'affichage des cooldowns", "Personnaliser l'affichage des cooldowns", CommandRank.Public) },
      { "unstuck", new MainMenuCommand("Déblocage du décor", "Tentative de déblocage du décor (succès non garanti)", CommandRank.Public) },
      { "reinitPositionDisplay", new MainMenuCommand("Réinitialiser la position affichée", "Réinitialise la position affichée du personnage (à utiliser en cas de problème avec le système d'assise)", CommandRank.Public) },
      { "publicKey", new MainMenuCommand("Afficher ma clé publique", "Permet d'obtenir la clé publique de votre compte, utile pour lier le compte Discord au compte Never", CommandRank.Public) },
      { "delete", new MainMenuCommand("Supprimer ce personnage", "Attention, la suppression est définitive", CommandRank.Public) },
      { "wind", new MainMenuCommand("Gestion du vent", "Permet de modifier la configuration du vent de cette zone", CommandRank.DM) },
      { "dmRename", new MainMenuCommand("Changer le nom de la cible", "Permet de modifier le nom de n'importe quel objet", CommandRank.DM) }, // TODO : Ajouter à OnExamine Custom pour DM
      { "visualEffects", new MainMenuCommand("Gérer mes effets visuels - Alpha", "Permet d'utiliser et de gérer les effets visuels personnalisés", CommandRank.Public) },
      { "aoeVisualEffects", new MainMenuCommand("Gérer mes effets visuels en AOE - Alpha", "Permet d'utiliser et de gérer les effets visuels personnalisés", CommandRank.Public) },
      { "areaMusicEditor", new MainMenuCommand("Modifier la sélection musicale de la zone", "", CommandRank.DM) },
      { "areaLoadScreenEditor", new MainMenuCommand("Modifier l'écran de chargement de la zone", "", CommandRank.DM) },
      { "reboot", new MainMenuCommand("Reboot", "", CommandRank.Admin) },
      { "refill", new MainMenuCommand("Refill ressources", "", CommandRank.Admin) },
      { "instantLearn", new MainMenuCommand("Activer/Désactiver Instant Learn (alpha)", "", CommandRank.Public) }, // TODO : Ajouter à OnExamine Player => Temporairement accessible à tout le monde pendant la BETA
      { "instantCraft", new MainMenuCommand("Instant Craft", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "giveResources", new MainMenuCommand("Don de ressources", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "giveSkillbook", new MainMenuCommand("Don de skillbook", "", CommandRank.Admin) }, // TODO : Ajouter à OnExamine Player
      { "lootEditor", new MainMenuCommand("Modifier les listes de loots", "", CommandRank.Admin) }
    };

    public static readonly List<NuiComboEntry> tradeMaterialList = new();

    public static readonly List<NuiComboEntry> appearanceEntries = new();
    public static readonly List<NuiComboEntry> placeableEntries = new();
    public static readonly List<NuiComboEntry> mailReceiverEntries = new();
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
    public static List<Rumor> rumors = new();
    public static readonly List<NuiComboEntry> skillList = new();
    public static readonly List<NuiComboEntry> mageCanTripList = new();

    public static readonly List<string> colorPaletteLeather = new();
    public static readonly List<string> colorPaletteMetal = new();

    public static readonly List<NuiComboEntry> variableTypes = new()
        {
          new NuiComboEntry("int", 1),
          new NuiComboEntry("string", 2),
          new NuiComboEntry("float", 3),
          new NuiComboEntry("date", 4),
          new NuiComboEntry("persistInt", 5),
          new NuiComboEntry("persistString", 6),
          new NuiComboEntry("persistFloat", 7)
        };

    public static readonly IEnumerable<LoadScreenTableEntry> loadScreensResRefList = NwGameTables.LoadScreenTable.Where(l => l.BMPResRef != null);

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

    public static double GetDamageMultiplier(double targetAC, double strikeLevel)
    {
      return Math.Pow(2, (strikeLevel - targetAC) / 40);
    }
    public static NuiRect GetDrawListTextScaleFromPlayerUI(PlayerSystem.Player player)
    {
      return new(0, 20/*player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiScale) * 0.234f*/, 500, 60);
    }
    public static int GetResTypeFromFileExtension(string extension, string fileName)
    {
      switch (extension.ToLower())
      {
        case "2da": return NWScript.RESTYPE_2DA;
        case "4pc": return NWScript.RESTYPE_4PC;
        case "are": return NWScript.RESTYPE_ARE;
        case "bak": return NWScript.RESTYPE_BAK;
        case "bic": return NWScript.RESTYPE_BIC;
        case "bif": return NWScript.RESTYPE_BIF;
        case "bmp": return NWScript.RESTYPE_BMP;
        case "btc": return NWScript.RESTYPE_BTC;
        case "btd": return NWScript.RESTYPE_BTD;
        case "bte": return NWScript.RESTYPE_BTE;
        case "btg": return NWScript.RESTYPE_BTG;
        case "bti": return NWScript.RESTYPE_BTI;
        case "btm": return NWScript.RESTYPE_BTM;
        case "btp": return NWScript.RESTYPE_BTP;
        case "bts": return NWScript.RESTYPE_BTS;
        case "btt": return NWScript.RESTYPE_BTT;
        case "caf": return NWScript.RESTYPE_CAF;
        case "ccs": return NWScript.RESTYPE_CCS;
        case "css": return NWScript.RESTYPE_CSS;
        case "dat": return NWScript.RESTYPE_DAT;
        case "dds": return NWScript.RESTYPE_DDS;
        case "dft": return NWScript.RESTYPE_DFT;
        case "dlg": return NWScript.RESTYPE_DLG;
        case "dwk": return NWScript.RESTYPE_DWK;
        case "erf": return NWScript.RESTYPE_ERF;
        case "fac": return NWScript.RESTYPE_FAC;
        case "fnt": return NWScript.RESTYPE_FNT;
        case "gff": return NWScript.RESTYPE_GFF;
        case "gic": return NWScript.RESTYPE_GIC;
        case "gif": return NWScript.RESTYPE_GIF;
        case "git": return NWScript.RESTYPE_GIT;
        case "gui": return NWScript.RESTYPE_GUI;
        case "hak": return NWScript.RESTYPE_HAK;
        case "ids": return NWScript.RESTYPE_IDS;
        case "ifo": return NWScript.RESTYPE_IFO;
        case "ini": return NWScript.RESTYPE_INI;
        case "itp": return NWScript.RESTYPE_ITP;
        case "jpg": return NWScript.RESTYPE_JPG;
        case "jrl": return NWScript.RESTYPE_JRL;
        case "jui": return NWScript.RESTYPE_JUI;
        case "key": return NWScript.RESTYPE_KEY;
        case "ktx": return NWScript.RESTYPE_KTX;
        case "lod": return NWScript.RESTYPE_LOD;
        case "ltr": return NWScript.RESTYPE_LTR;
        case "lua": return NWScript.RESTYPE_LUA;
        case "mdl": return NWScript.RESTYPE_MDL;
        case "mod": return NWScript.RESTYPE_MOD;
        case "mp3": return NWScript.RESTYPE_MP3;
        case "mpg": return NWScript.RESTYPE_MPG;
        case "mtr": return NWScript.RESTYPE_MTR;
        case "mve": return NWScript.RESTYPE_MVE;
        case "ncs": return NWScript.RESTYPE_NCS;
        case "ndb": return NWScript.RESTYPE_NDB;
        case "nss": return NWScript.RESTYPE_NSS;
        case "nwm": return NWScript.RESTYPE_NWM;
        case "plh": return NWScript.RESTYPE_PLH;
        case "plt": return NWScript.RESTYPE_PLT;
        case "png": return NWScript.RESTYPE_PNG;
        case "ptm": return NWScript.RESTYPE_PTM;
        case "ptt": return NWScript.RESTYPE_PTT;
        case "pwk": return NWScript.RESTYPE_PWK;
        case "res": return NWScript.RESTYPE_RES;
        case "sav": return NWScript.RESTYPE_SAV;
        case "set": return NWScript.RESTYPE_SET;
        case "shd": return NWScript.RESTYPE_SHD;
        case "slt": return NWScript.RESTYPE_SLT;
        case "sq3": return NWScript.RESTYPE_SQ3;
        case "sql": return NWScript.RESTYPE_SQL;
        case "ssf": return NWScript.RESTYPE_SSF;
        case "tex": return NWScript.RESTYPE_TEX;
        case "tga": return NWScript.RESTYPE_TGA;
        case "thg": return NWScript.RESTYPE_THG;
        case "tlk": return NWScript.RESTYPE_TLK;
        case "tml": return NWScript.RESTYPE_TML;
        case "ttf": return NWScript.RESTYPE_TTF;
        case "txi": return NWScript.RESTYPE_TXI;
        case "txt": return NWScript.RESTYPE_TXT;
        case "utc": return NWScript.RESTYPE_UTC;
        case "utd": return NWScript.RESTYPE_UTD;
        case "ute": return NWScript.RESTYPE_UTE;
        case "utg": return NWScript.RESTYPE_UTG;
        case "uti": return NWScript.RESTYPE_UTI;
        case "utm": return NWScript.RESTYPE_UTM;
        case "utp": return NWScript.RESTYPE_UTP;
        case "uts": return NWScript.RESTYPE_UTS;
        case "utt": return NWScript.RESTYPE_UTT;
        case "utw": return NWScript.RESTYPE_UTW;
        case "wav": return NWScript.RESTYPE_WAV;
        case "wbm": return NWScript.RESTYPE_WBM;
        case "wfx": return NWScript.RESTYPE_WFX;
        case "wok": return NWScript.RESTYPE_WOK;
        case "xbc": return NWScript.RESTYPE_XBC;
        default:
          LogUtils.LogMessage($"WARNING - type {extension} non reconnu - fichier {fileName}", LogUtils.LogType.ModuleAdministration);
          return NWScript.RESTYPE_MDL;
      }
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
    public static readonly Feat[] racialFeats = new Feat[]
    {
      Feat.BattleTrainingVersusGiants,
      Feat.BattleTrainingVersusGoblins,
      Feat.BattleTrainingVersusOrcs,
      Feat.BattleTrainingVersusReptilians,
      Feat.Lowlightvision,
      Feat.Lucky,
      Feat.Darkvision,
      Feat.Fearless,
      Feat.GoodAim,
      Feat.PartialSkillAffinityListen,
      Feat.PartialSkillAffinitySearch,
      Feat.PartialSkillAffinitySpot,
      Feat.QuickToMaster,
      Feat.HardinessVersusEnchantments,
      Feat.HardinessVersusIllusions,
      Feat.HardinessVersusPoisons,
      Feat.HardinessVersusSpells,
      Feat.ImmunityToSleep,
      Feat.KeenSense,
      Feat.SkillAffinityConcentration,
      Feat.SkillAffinityListen,
      Feat.SkillAffinityLore,
      Feat.SkillAffinityMoveSilently,
      Feat.SkillAffinitySearch,
      Feat.SkillAffinitySpot,
      Feat.Stonecunning, 
    };
    public static async Task<Location> GetRandomLocationInArea(NwArea area)
    {
      NwGameObject transition;

      try
      {
        transition = area.FindObjectsOfTypeInArea<NwDoor>().First(d => d.TransitionTarget != null);
      }
      catch(Exception)
      {
        try
        {
          transition = area.FindObjectsOfTypeInArea<NwTrigger>().First(t => t.TransitionTarget != null);
        }
        catch(Exception)
        {
          LogUtils.LogMessage($"Impossible de trouver une transition valide dans la zone {area.Name}", LogUtils.LogType.MateriaSpawn);
          return null;
        }
      }

      int areaWidth = NWScript.GetAreaSize((int)AreaSizeDimension.Width, area);
      int areaHeigth = NWScript.GetAreaSize((int)AreaSizeDimension.Height, area);
      Vector3 randomPosition = new Vector3(random.Next(areaWidth * 8), random.Next(areaHeigth * 8), 10);
      int nbTry = 1;
      Stopwatch stopwatch = Stopwatch.StartNew();
      
      while (AreaPlugin.GetPathExists(area, transition.Position, randomPosition, areaWidth * areaHeigth) < 1 && stopwatch.Elapsed.TotalSeconds < 2)
      {
        nbTry++;
        randomPosition = new Vector3(random.Next(areaWidth * 8), random.Next(areaHeigth * 8), 10);
        
        while (AreaPlugin.GetPathExists(area, transition.Position, randomPosition, areaWidth * areaHeigth) < 1 && stopwatch.Elapsed.TotalMilliseconds < Config.MaxSerializeTimeMs)
        {
          nbTry++;
          randomPosition = new Vector3(random.Next(areaWidth * 8), random.Next(areaHeigth * 8), 10);
        }

        await NwTask.NextFrame();
      }

      if (stopwatch.Elapsed.TotalSeconds > 2)
      {
        LogUtils.LogMessage($"Could not find valid path in {area.Name}", LogUtils.LogType.MateriaSpawn);
        return null;
      }

      LogUtils.LogMessage($"Path found in {area.Name} after {nbTry} tries ({stopwatch.Elapsed.TotalMilliseconds} ms)", LogUtils.LogType.MateriaSpawn);

      ModuleSystem.placeholderTemplate.Location = Location.Create(area, new Vector3(randomPosition.X, randomPosition.Y, 0), random.Next(360));
      Vector3 safePosition = CreaturePlugin.ComputeSafeLocation(ModuleSystem.placeholderTemplate, randomPosition);

      if(safePosition == Vector3.Zero)
      {
        LogUtils.LogMessage($"Could not find safePosition in {area.Name}", LogUtils.LogType.MateriaSpawn);
        return null;
      }

      return Location.Create(area, safePosition, random.Next(360));
    }
    public static int GetSpawnedMateriaGrade(int areaLevel)
    {
      int materiaGrade = 1;
      int roll = NwRandom.Roll(random, 100);

      foreach (int chance in Config.materiaSpawnGradeChance[areaLevel])
      {
        if (roll < chance)
          return materiaGrade;

        materiaGrade++;
      }

      return materiaGrade;
    }
    public static void SetResourceBlockData(NwPlaceable block)
    {
      switch(block.ResRef) 
      {
        case "mineable_rock":
          block.Name = "Matéria - Dépôt minéral";
          block.Useable = true;
          block.Description = "Un phénomène mystérieux provoque l'agglomération d'influx à certains matériaux bruts, qu'on appelle alors 'matéria'.\n\nAvant de pouvoir être utilisée par un artisan, cette matéria brute doit être extraite, puis raffinée avec une technique spéciale qui permet de rendre le matériau malléable.";
          break;
        case "mineable_animal":
          block.Name = "Matéria - Dépôt animal";
          block.Useable = true;
          block.Description = "Un phénomène mystérieux provoque l'agglomération d'influx à certains matériaux bruts, qu'on appelle alors 'matéria'.\n\nAvant de pouvoir être utilisée par un artisan, cette matéria brute doit être extraite, puis raffinée avec une technique spéciale qui permet de rendre le matériau malléable.";
          block.Appearance = NwGameTables.PlaceableTable.GetRow(4964);
          break;
        case "mineable_tree":
          block.Name = "Matéria - Dépôt végétal";
          block.Useable = true;
          block.Description = "Un phénomène mystérieux provoque l'agglomération d'influx à certains matériaux bruts, qu'on appelle alors 'matéria'.\n\nAvant de pouvoir être utilisée par un artisan, cette matéria brute doit être extraite, puis raffinée avec une technique spéciale qui permet de rendre le matériau malléable.";
          block.Appearance = NwGameTables.PlaceableTable.GetRow(30557);
          break;
      }
    }
  }
}
