using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using Microsoft.Data.Sqlite;

using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerConnect(ModuleEvents.OnClientEnter HandlePlayerConnect)
    {
      NwPlayer oPC = HandlePlayerConnect.Player;

      LogUtils.LogMessage($"{oPC.PlayerName} vient de connecter {oPC.LoginCreature.Name} ({NwModule.Instance.PlayerCount} joueurs)", LogUtils.LogType.PlayerConnections);

      if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
        player = new Player(oPC, areaSystem, spellSystem, feedbackService, scheduler, eventService);
      else
      {
        player.oid = oPC;
        player.characterId = oPC.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value;

        if (player.location.Area == null)
        {
          var query = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "location" } },
          new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

          foreach(var result in query)
            player.location = SqLiteUtils.DeserializeLocation(result[0]);

          player.TeleportPlayerToSavedLocation();
        }

        Task playerInitialized = NwTask.Run(async () =>
        {
          await NwTask.NextFrame();
          player.FinalizePlayerData();
        });
      }

      player.oid.SetGuiPanelDisabled(GUIPanel.ExamineItem, true);
      player.oid.SetGuiPanelDisabled(GUIPanel.Journal, true);
      player.oid.SetGuiPanelDisabled(GUIPanel.PlayerList, true);

      if (player.IsDm())
      {
        player.oid.SetGuiPanelDisabled(GUIPanel.ExaminePlaceable, true);
        player.oid.SetGuiPanelDisabled(GUIPanel.ExamineCreature, true);
      }

      player.currentLanguage = 0;

      if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableBool>("_ALWAYS_WALK").HasValue)
        player.oid.ControlledCreature.AlwaysWalk = true;

      if (player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand) is not null)
        player.oid.LoginCreature.BaseAttackCount = ItemUtils.GetWeaponAttackPerRound(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand).BaseItem.ItemType);
      else
        player.oid.LoginCreature.BaseAttackCount = 3;

      if (oPC.IsDM)
        return;

      Utils.ResetVisualTransform(player.oid.ControlledCreature);
      player.pcState = Player.PcState.Offline;

      foreach (Player connectedPlayer in Players.Values)
        if(connectedPlayer.pcState != Player.PcState.Offline && connectedPlayer.TryGetOpenedWindow("playerList", out Player.PlayerWindow playerListWindow))
          ((Player.PlayerListWindow)connectedPlayer.windows["playerList"]).UpdatePlayerList();

      player.mapLoadingTime = DateTime.Now;

      player.HandleReinit();
    }
    public partial class Player
    {
      public void InitializeNewPlayer()
      {
        var result = SqLiteUtils.SelectQuery
          ("PlayerAccounts",
          new List<string>() { { "rowid" } },
          new List<string[]>() { { new string[] { "accountName", oid.PlayerName } } });

        if (result == null || result.Count < 1)
        {
          if (Config.env == Config.Env.Prod)
          {
            Bot.playerGeneralChannel.SendMessageAsync($"Toute première connexion de {oid.LoginCreature.Name}. Accueillons le comme il se doit !");
            Bot.staffGeneralChannel.SendMessageAsync($"{Bot.discordServer.EveryoneRole.Mention} Toute première connexion de {oid.LoginCreature.Name} => nouveau joueur à accueillir !");

            windows.Add("introWelcome", new IntroWelcomeWindow(this));
          }

          SqLiteUtils.InsertQuery("PlayerAccounts",
            new List<string[]>() { new string[] { "accountName", oid.PlayerName }, new string[] { "cdKey", oid.CDKey }, new string[] { "bonusRolePlay", "1" }, new string[] { "hideFromPlayerList", oid.IsDM ? 1.ToString() : 0.ToString() } });

          var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
          query.Execute();

          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value = query.Result.GetInt(0);
        }
        else
          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value = int.Parse(result.FirstOrDefault()[0]);
      }
      public void InitializeNewCharacter()
      {
        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_REINITIALISATION_DONE").Value = 1;

        LogUtils.LogMessage($"{oid.PlayerName} vient de créer un nouveau personnage : {oid.LoginCreature.Name}", LogUtils.LogType.PlayerConnections);

        int startingSP = 5000;
        if (oid.LoginCreature.KnowsFeat(Feat.QuickToMaster))
          startingSP += 500;

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value = startingSP;

        Location arrivalLocation = NwModule.Instance.StartingLocation;

        if (NwModule.Instance.Areas.Any(a => a.Tag == "entry_scene"))
        {
          NwArea arrivalArea = NwArea.Create("intro_galere", $"entry_scene_{oid.CDKey}", $"La galère de {oid.LoginCreature.Name} (Bienvenue !)");
          arrivalArea.OnExit += areaSystem.OnIntroAreaExit;
          arrivalLocation = arrivalArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT").Location;

          AreaSystem.ScheduleRockSpawn(arrivalArea, 0);
          AreaSystem.ScheduleRockSpawn(arrivalArea, 1);

          arrivalArea.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);

          foreach (NwPlaceable recif in arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "intro_recif"))
            recif.VisibilityOverride = VisibilityMode.Hidden;

          NwPlaceable tourbillon = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(c => c.Tag == "intro_tourbillon");
          tourbillon.VisibilityOverride = VisibilityMode.Hidden;
          tourbillon.VisualTransform.Translation = new Vector3(tourbillon.VisualTransform.Translation.X, 115, tourbillon.VisualTransform.Translation.Z);

          NwPlaceable introMirror = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(o => o.Tag == "intro_mirror");
          introMirror.OnUsed += PlaceableSystem.StartIntroMirrorDialog;

          Task waitDefaultMapLoaded = NwTask.Run(async () =>
          {
            await NwTask.WaitUntilValueChanged(() => oid.LoginCreature.Location.Area);
            oid.LoginCreature.Location = arrivalLocation;
          });
        }

        Utils.DestroyInventory(oid.LoginCreature);

        if (oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin) == null)
        {
          Task waitSkinCreated = NwTask.Run(async () =>
          {
            NwItem pcSkin = await NwItem.Create("peaudejoueur", oid.LoginCreature);
            pcSkin.Name = $"Propriétés de {oid.LoginCreature.Name}";
            oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
          });
        }

        SqLiteUtils.InsertQuery("playerCharacters",
            new List<string[]>() { new string[] { "accountId", accountId.ToString() }, new string[] { "characterName", oid.LoginCreature.Name }, new string[] { "location", SqLiteUtils.SerializeLocation(arrivalLocation) }, new string[] { "menuOriginLeft", "50" }, new string[] { "currentHP", oid.LoginCreature.MaxHP.ToString() } });

        var rowQuery = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT last_insert_rowid()");
        rowQuery.Execute();

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value = rowQuery.Result.GetInt(0);

        for (byte spellLevel = 0; spellLevel < 10; spellLevel++)
          foreach (NwSpell spell in oid.LoginCreature.GetClassInfo((ClassType)43).GetKnownSpells(spellLevel))
            oid.LoginCreature.GetClassInfo((ClassType)43).RemoveKnownSpell(spellLevel, spell);

        InitializeNewPlayerLearnableSkills();
      }
      private void InitializeNewPlayerLearnableSkills()
      {
        if (learnableSkills.ContainsKey(CustomSkill.ImprovedStrength))
          return;

        int startingLanguage = -1;

        switch (oid.LoginCreature.Race.RacialType)
        {
          case RacialType.Dwarf:
            startingLanguage = CustomSkill.Nain;
            break;
          case RacialType.Elf:
          case RacialType.HalfElf:
            startingLanguage = CustomSkill.Elfique;
            break;
          case RacialType.Halfling:
            startingLanguage = CustomSkill.Halfelin;
            break;
          case RacialType.Gnome:
            startingLanguage = CustomSkill.Gnome;
            break;
          case RacialType.HalfOrc:
            startingLanguage = CustomSkill.Orc;
            break;
        }

        if (startingLanguage > -1)
        {
          LearnableSkill language = new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[startingLanguage]);
          learnableSkills.Add(startingLanguage, language);
          language.LevelUp(this);
        }

        learnableSkills.Add(CustomSkill.ImprovedStrength, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedStrength]));
        learnableSkills.Add(CustomSkill.ImprovedDexterity, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedDexterity]));
        learnableSkills.Add(CustomSkill.ImprovedConstitution, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedConstitution]));
        learnableSkills.Add(CustomSkill.ImprovedIntelligence, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedIntelligence]));
        learnableSkills.Add(CustomSkill.ImprovedWisdom, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedWisdom]));
        learnableSkills.Add(CustomSkill.ImprovedCharisma, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedCharisma]));

        learnableSkills.Add(CustomSkill.ImprovedHealth, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedHealth]));
        learnableSkills.Add(CustomSkill.Toughness, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Toughness]));

        //learnableSkills.Add(CustomSkill.ImprovedAttackBonus, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedAttackBonus]));
        //learnableSkills.Add(CustomSkill.ImprovedCasterLevel, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedCasterLevel]));
        learnableSkills.Add(CustomSkill.ImprovedSpellSlot0, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedSpellSlot0]));
        learnableSkills.Add(CustomSkill.ImprovedSpellSlot1, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedSpellSlot1]));

        learnableSkills.Add(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightArmorProficiency]));
        learnableSkills.Add(CustomSkill.LightShieldProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightShieldProficiency]));

        learnableSkills.Add(CustomSkill.ClubProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ClubProficiency]));
        learnableSkills.Add(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightFlailProficiency]));
        learnableSkills.Add(CustomSkill.ShortBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortBowProficiency]));
        learnableSkills.Add(CustomSkill.LightCrossBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightCrossBowProficiency]));
        learnableSkills.Add(CustomSkill.LightMaceProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightMaceProficiency]));
        learnableSkills.Add(CustomSkill.DaggerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DaggerProficiency]));
        learnableSkills.Add(CustomSkill.DartProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DartProficiency]));
        learnableSkills.Add(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightHammerProficiency]));
        learnableSkills.Add(CustomSkill.QuarterStaffProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.QuarterStaffProficiency]));
        learnableSkills.Add(CustomSkill.MorningStarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.MorningStarProficiency]));
        learnableSkills.Add(CustomSkill.ShortSpearProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortSpearProficiency]));
        learnableSkills.Add(CustomSkill.SlingProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SlingProficiency]));
        learnableSkills.Add(CustomSkill.SickleProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SickleProficiency]));

        //learnableSkills.Add(CustomSkill.TwoWeaponFighting, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.TwoWeaponFighting]));
        //learnableSkills.Add(CustomSkill.WeaponFinesse, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.WeaponFinesse]));

        learnableSkills.Add(CustomSkill.Athletics, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Athletics]));
        learnableSkills.Add(CustomSkill.Acrobatics, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Acrobatics]));
        learnableSkills.Add(CustomSkill.Escamotage, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Escamotage]));
        learnableSkills.Add(CustomSkill.Stealth, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Stealth]));
        learnableSkills.Add(CustomSkill.Concentration, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Concentration]));
        learnableSkills.Add(CustomSkill.Arcana, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Arcana]));
        learnableSkills.Add(CustomSkill.History, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.History]));
        learnableSkills.Add(CustomSkill.Nature, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Nature]));
        learnableSkills.Add(CustomSkill.Religion, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Religion]));
        learnableSkills.Add(CustomSkill.Investigation, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Investigation]));
        learnableSkills.Add(CustomSkill.Dressage, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Dressage]));
        learnableSkills.Add(CustomSkill.Insight, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Insight]));
        learnableSkills.Add(CustomSkill.Medicine, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Medicine]));
        learnableSkills.Add(CustomSkill.Perception, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Perception]));
        learnableSkills.Add(CustomSkill.Survival, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Survival]));
        learnableSkills.Add(CustomSkill.Deception, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Deception]));
        learnableSkills.Add(CustomSkill.Intimidation, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Intimidation]));
        learnableSkills.Add(CustomSkill.Performance, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Performance]));
        learnableSkills.Add(CustomSkill.Persuasion, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Persuasion]));
        learnableSkills.Add(CustomSkill.OpenLock, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.OpenLock]));
        learnableSkills.Add(CustomSkill.TrapExpertise, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.TrapExpertise]));
        learnableSkills.Add(CustomSkill.Taunt, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Taunt]));
      }
      public void InitializeDM()
      {
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireItem;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquireItem;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnItemEquipBefore;
        oid.LoginCreature.OnUseFeat += FeatSystem.OnUseFeatBefore;
        oid.LoginCreature.OnSpellCast += spellSystem.HandleAutoSpellBeforeSpellCast;
        oid.OnNuiEvent += HandleGenericNuiEvents;

        eventService.Subscribe<OnDMSpawnObject, DMEventFactory>(oid.LoginCreature, areaSystem.InitializeEventsAfterDMSpawnCreature, EventCallbackType.After);
      }
      public void InitializePlayer()
      {
        InitializePlayerEvents();
        InitializeSpellEvents();
        InitializePlayerAccount();
        InitializePlayerCharacter();
      }
      private void InitializePlayerEvents()
      {
        oid.OnServerCharacterSave += HandleBeforePlayerSave;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireItem;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquireItem;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnItemEquipBefore;
        oid.LoginCreature.OnItemUse += ItemSystem.OnItemUseBefore;
        oid.OnPlayerDeath += HandlePlayerDeath;
        oid.LoginCreature.OnUseFeat += FeatSystem.OnUseFeatBefore;
        oid.OnCombatStatusChange += OnCombatStarted;
        oid.LoginCreature.OnCombatRoundStart += OnCombatRoundStart;
        oid.OnPartyEvent += Party.HandlePartyEvent;
        oid.OnClientLevelUpBegin += HandleOnClientLevelUp;
        oid.LoginCreature.OnItemValidateEquip += ItemSystem.NoEquipRuinedItem;
        oid.LoginCreature.OnItemValidateUse += ItemSystem.NoUseRuinedItem;
        oid.LoginCreature.OnCombatModeToggle += HandleCombatModeOff;
        oid.LoginCreature.OnInventoryGoldAdd += HandleGainedGold;
        oid.LoginCreature.OnInventoryGoldRemove += HandleLostGold;
        oid.LoginCreature.OnItemScrollLearn += HandleBeforeScrollLearn;
        oid.LoginCreature.OnItemUnequip += ItemSystem.HandleUnequipItemBefore;
        oid.LoginCreature.OnUseSkill += HandleBeforeSkillUsed;
        oid.OnNuiEvent += HandleGenericNuiEvents;
        oid.OnMapPinAddPin += HandleMapPinAdded;
        oid.OnMapPinChangePin += HandleMapPinChanged;
        oid.OnMapPinDestroyPin += HandleMapPinDestroyed;
        eventService.Subscribe<OnDMSpawnObject, DMEventFactory>(oid.LoginCreature, areaSystem.InitializeEventsAfterDMSpawnCreature, EventCallbackType.After);
      }
      private void InitializeSpellEvents()
      {
        oid.LoginCreature.OnSpellBroadcast += spellSystem.SetCastingClassOnSpellBroadcast;
        oid.LoginCreature.OnSpellBroadcast += spellSystem.HandleHearingSpellBroadcast;
        oid.LoginCreature.OnSpellCast += spellSystem.HandleAutoSpellBeforeSpellCast;
        oid.LoginCreature.OnSpellCast += spellSystem.CheckIsDivinationBeforeSpellCast;
        oid.LoginCreature.OnSpellCast += spellSystem.HandleCraftEnchantementCast;
        oid.LoginCreature.OnSpellAction += spellSystem.HandleCraftOnSpellInput;
        oid.LoginCreature.OnSpellAction += spellSystem.RegisterMetaMagicOnSpellInput;
      }
      private void InitializePlayerAccount()
      {
        var query = SqLiteUtils.SelectQuery("PlayerAccounts",
            new List<string>() { { "bonusRolePlay" }, { "mapPins" }, { "chatColors" }, { "mutedPlayers" }, { "windowRectangles" }, { "customDMVisualEffects" }, { "hideFromPlayerList" } },
            new List<string[]>() { { new string[] { "rowid", accountId.ToString() } } });

        foreach (var result in query)
        {
          bonusRolePlay = int.TryParse(result[0], out int brp) ? brp : 1;
          string serializedMapPins = result[1];
          string serializedChatColors = result[2];
          string serializedMutedPlayers = result[3];
          string serializedWindowRectangles = result[4];
          string serializedCustomDMVisualEffects = result[5];
          hideFromPlayerList = result[6].ParseIntBool();
          InitializeAccountMapPins(serializedMapPins);
          InitializeAccountChatColors(serializedChatColors);
          InitializeAccountMutedPlayers(serializedMutedPlayers);
          InitializeAccountWindowRectanglesPlayers(serializedWindowRectangles);
          InitializeAccountCustomDMVisualEffects(serializedCustomDMVisualEffects);
        }
      }
      private void InitializePlayerCharacter()
      {
        var query = SqLiteUtils.SelectQuery("playerCharacters",
            new List<string>() { { "location" }, { "currentHP" }, { "bankGold" }, { "menuOriginTop" }, { "menuOriginLeft" }, { "pveArenaCurrentPoints" },
              { "alchemyCauldron" }, { "serializedLearnableSkills" }, { "serializedLearnableSpells" }, { "explorationState" }, { "materialStorage" }, { "craftJob" },
              { "grimoires" }, { "quickbars" }, { "itemAppearances" }, { "descriptions" }, { "currentSkillPoints" }, { "mails" }, { "subscriptions" }, { "endurance" } },
            new List<string[]>() { { new string[] { "rowid", characterId.ToString() } } });

        foreach (var result in query)
        {
          location = SqLiteUtils.DeserializeLocation(result[0]);
          oid.LoginCreature.HP = int.TryParse(result[1], out int hp) ? hp : 1;
          bankGold = int.TryParse(result[2], out int gold) ? gold : 0;
          menu.originTop = int.TryParse(result[3], out int top) ? top : 0;
          menu.originLeft = int.TryParse(result[4], out int left) ? left : 0;
          pveArena.totalPoints = uint.TryParse(result[5], out uint points) ? points : 0;
          string serializedCauldron = result[6];
          string serializedLearnableSkills = result[7];
          string serializedLearnableSpells = result[8];
          string serializedExploration = result[9];
          string serializedCraftResources = result[10];
          string serializedCraftJob = result[11];
          string serializedGrimoires = result[12];
          string serializedQuickbars = result[13];
          string serializedItemAppearances = result[14];
          string serializedDescriptions = result[15];
          tempCurrentSkillPoint = int.TryParse(result[16], out int skill) ? skill : 0;
          string serializedMails = result[17];
          string serializedSubscriptions = result[18];
          string serializedEndurance = result[19];

          InitializePlayerAsync(serializedCauldron, serializedExploration, serializedLearnableSkills, serializedLearnableSpells, serializedCraftResources, serializedCraftJob, serializedGrimoires, serializedQuickbars, serializedItemAppearances, serializedDescriptions, serializedMails, serializedSubscriptions, serializedEndurance);
        }
      }
      private async void InitializePlayerAsync(string serializedCauldron, string serializedExploration, string serializedLearnableSkills, string serializedLearnableSpells, string serializedCraftResources, string serializedCraftJob, string serializedGrimoires, string serializedQuickbars, string serializedItemAppearances, string serializedDescriptions, string serializedMails, string serializedSubscriptions, string serializedEndurance)
      {
        Task loadCauldronTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedCauldron) || serializedCauldron == "null")
            return;

          alchemyCauldron = JsonConvert.DeserializeObject<Alchemy.Cauldron>(serializedCauldron);
        });

        Task loadExplorationTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedExploration) || serializedExploration == "null")
            return;

          areaExplorationStateDictionnary = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(serializedExploration);
        });

        Task loadSkillsTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedLearnableSkills) || serializedLearnableSkills == "null")
            return;

          Dictionary<int, LearnableSkill.SerializableLearnableSkill> serializableSkills = JsonConvert.DeserializeObject<Dictionary<int, LearnableSkill.SerializableLearnableSkill>>(serializedLearnableSkills);

          foreach (var kvp in serializableSkills)
          {
            if (SkillSystem.learnableDictionary.ContainsKey(kvp.Key))
            {
              LearnableSkill skill = new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[kvp.Key], kvp.Value);

              if (skill.active)
                activeLearnable = skill;

              learnableSkills.TryAdd(kvp.Key, skill);
            }
            else
              Utils.LogMessageToDMs($"SKILL SYSTEM - INVALID SKILL KEY {kvp.Key} REMOVED FROM {this.characterId} ({this.accountId})");
          }
        });

        Task loadSpellsTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedLearnableSpells) || serializedLearnableSpells == "null")
            return;

          Dictionary<int, LearnableSpell.SerializableLearnableSpell> serializableSpells = JsonConvert.DeserializeObject<Dictionary<int, LearnableSpell.SerializableLearnableSpell>>(serializedLearnableSpells);

          foreach (var kvp in serializableSpells)
          {
            LearnableSpell spell = new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[kvp.Key], kvp.Value);

            if (spell.active)
              activeLearnable = spell;

            learnableSpells.Add(kvp.Key, spell);
          }
        });

        Task loadMateriaTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedCraftResources) || serializedCraftResources == "null")
            return;

          List<CraftResource.SerializableCraftResource> serializableCraftResource = JsonConvert.DeserializeObject<List<CraftResource.SerializableCraftResource>>(serializedCraftResources);

          foreach (var serializedMateria in serializableCraftResource)
            craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == (ResourceType)serializedMateria.type && r.grade == serializedMateria.grade), serializedMateria.quantity));
        });

        Task loadCraftJobTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedCraftJob) || serializedCraftJob == "null")
            return;

          craftJob = new CraftJob(JsonConvert.DeserializeObject<CraftJob.SerializableCraftJob>(serializedCraftJob));
        });

        Task loadGrimoiresTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedGrimoires) || serializedGrimoires == "null")
            return;

          grimoires = JsonConvert.DeserializeObject<List<Grimoire>>(serializedGrimoires);
        });

        Task loadEnduranceTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedEndurance) || serializedEndurance == "null")
          {
            endurance = new();
            return;
          }

          endurance = JsonConvert.DeserializeObject<Endurance>(serializedEndurance);
        });

        Task loadQuickbarsTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedQuickbars) || serializedQuickbars == "null")
            return;

          quickbars = JsonConvert.DeserializeObject<List<Quickbar>>(serializedQuickbars);
        });

        Task loadItemAppearancesTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedItemAppearances) || serializedItemAppearances == "null")
            return;

          itemAppearances = JsonConvert.DeserializeObject<List<ItemAppearance>>(serializedItemAppearances);
        });

        Task loadDescriptionsTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedDescriptions) || serializedDescriptions == "null")
            return;

          descriptions = JsonConvert.DeserializeObject<List<CharacterDescription>>(serializedDescriptions);
        });

        Task loadSubscriptionsTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedSubscriptions) || serializedSubscriptions == "null")
            return;

          List<Subscription.SerializableSubscription> serializedSubscription = JsonConvert.DeserializeObject<List<Subscription.SerializableSubscription>>(serializedSubscriptions);

          foreach (var subscription in serializedSubscription)
            subscriptions.Add(new Subscription(subscription));
        });

        Task loadMailsTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedMails) || serializedMails == "null")
            return;

          List<Mail.SerializableMail> serializedMail = JsonConvert.DeserializeObject<List<Mail.SerializableMail>>(serializedMails);

          foreach (var mail in serializedMail)
            if (!mail.expirationDate.HasValue || mail.expirationDate > DateTime.Now)
              mails.Add(new Mail(mail));
        });

        await Task.WhenAll(loadSkillsTask, loadSpellsTask, loadExplorationTask, loadCauldronTask, loadCraftJobTask, loadGrimoiresTask, loadQuickbarsTask, loadItemAppearancesTask, loadDescriptionsTask, loadMailsTask, loadSubscriptionsTask, loadEnduranceTask);
        await NwTask.SwitchToMainThread();
        FinalizePlayerData();
      }
      public void FinalizePlayerData()
      {
        if (oid == null || !oid.IsValid || oid.LoginCreature == null)
          return;

        HandleHealthPointInit();
        HandleLearnableInit();
        HandleJobInit();
        CheckPlayerConnectionInfo();
        HandleMailNotification();
        pcState = PcState.Online;
        oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
      }
      private void HandleHealthPointInit()
      {
        if (oid.LoginCreature.HP < 1)
          oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Death());

        if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag == "_CORE_EFFECT"))
          return;

        int improvedHealth = learnableSkills.ContainsKey(CustomSkill.ImprovedHealth) ? learnableSkills[CustomSkill.ImprovedHealth].currentLevel : 0;
        int toughness = learnableSkills.ContainsKey(CustomSkill.Toughness) ? learnableSkills[CustomSkill.Toughness].currentLevel : 0;

        int conModifier = ((oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);
        oid.LoginCreature.LevelInfo[0].HitDie = (byte)(endurance.maxHP
        + improvedHealth * (toughness + conModifier));

        Effect runAction = Effect.RunAction(null, ItemSystem.removeCoreHandle);
        runAction = Effect.LinkEffects(runAction, Effect.Icon(EffectIcon.TemporaryHitpoints));
        runAction.Tag = "_CORE_EFFECT";
        runAction.SubType = EffectSubType.Supernatural;

        TimeSpan duration = endurance.expirationDate - DateTime.Now;

        oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, runAction, TimeSpan.FromSeconds(duration.TotalSeconds > 0 ? duration.TotalSeconds : 0));
        LogUtils.LogMessage($"{oid.LoginCreature.Name} application des effets du Mélange à la connexion : HP endurance {endurance.maxHP}, max HP {oid.LoginCreature.LevelInfo[0].HitDie + conModifier}, HP régénérable {endurance.regenerableHP}, mana régénérable {endurance.regenerableMana}, se dissipe le {endurance.expirationDate}", LogUtils.LogType.EnduranceSystem);

        energyRegen = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType == BaseItemType.MagicStaff ? 4 : 2;

        if (!windows.ContainsKey("healthBar"))windows.Add("healthBar", new HealthBarWindow(this));
        else ((HealthBarWindow)windows["healthBar"]).CreateWindow();
      }
      private void HandleLearnableInit()
      {
        if (activeLearnable != null && activeLearnable.active && activeLearnable.spLastCalculation.HasValue)
        {
          activeLearnable.acquiredPoints += (DateTime.Now - activeLearnable.spLastCalculation).Value.TotalSeconds * GetSkillPointsPerSecond(activeLearnable);
          if (!windows.ContainsKey("activeLearnable")) windows.Add("activeLearnable", new ActiveLearnableWindow(this));
          else ((ActiveLearnableWindow)windows["activeLearnable"]).CreateWindow();
        }
      }
      private void HandleJobInit()
      {
        if (craftJob != null)
        {
          if ((craftJob.type == JobType.Mining || craftJob.type == JobType.WoodCutting || craftJob.type == JobType.Pelting) && craftJob.progressLastCalculation.HasValue)
          {
            craftJob.remainingTime -= (DateTime.Now - craftJob.progressLastCalculation.Value).TotalSeconds;
            craftJob.progressLastCalculation = null;
          }
          if (!windows.ContainsKey("activeCraftJob")) windows.Add("activeCraftJob", new ActiveCraftJobWindow(this));
          else ((ActiveCraftJobWindow)windows["activeCraftJob"]).CreateWindow();
        }
      }
      private void HandleMailNotification()
      {
        if (!subscriptions.Any(s => s.type == Utils.SubscriptionType.MailNotification))
          return;
        int unreadCount = mails.Count(m => m.fromCharacterId < 1 && !m.read);
        if (unreadCount > 0)
          oid.SendServerMessage($"Votre pièce vibre. Vous avez reçu {StringUtils.ToWhitecolor(unreadCount)} nouvelle(s) missive(s) de la banque", ColorConstants.Orange);
        unreadCount = mails.Count(m => m.fromCharacterId > 0 && !m.read && m.fromCharacterId != characterId);

        if (unreadCount > 0)
          oid.SendServerMessage($"Votre pièce vibre. Vous avez reçu {StringUtils.ToWhitecolor(unreadCount)} nouvelle(s) missive(s) personnelle(s)", ColorConstants.Orange);

        if (location?.Area?.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1)
          bankGold -= location.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value * 5;
      }
      private async void CheckPlayerConnectionInfo()
      {
        string cdKey = oid.CDKey;
        string playerName = oid.PlayerName;
        string ipAdress = oid.IPAddress;

        await SqLiteUtils.InsertQueryAsync("playerConnectionInfo",
          new List<string[]>() {
            new string[] { "playerAccount", playerName },
            new string[] { "cdKey", cdKey },
            new string[] { "ipAdress", ipAdress },
            new string[] { "lastConnection", DateTime.Now.ToString() } },
          new List<string>() { "playerAccount", "cdKey", "ipAdress" },
          new List<string[]>() { new string[] { "lastConnection" } });

        string queryString = "select playerAccount from playerConnectionInfo where playerAccount != @playerAccount and cdKey = @cdKey";

        try
        {
          using var connection = new SqliteConnection(Config.dbPath);
          connection.Open();

          var command = connection.CreateCommand();
          command.CommandText = queryString;

          command.Parameters.AddWithValue($"@playerAccount", (object)playerName ?? DBNull.Value);
          command.Parameters.AddWithValue($"@cdKey", (object)cdKey ?? DBNull.Value);

          using var reader = await command.ExecuteReaderAsync();
          while (reader.Read())
          {
            LogUtils.LogMessage($"WARNING - {playerName} vient de se connecter avec la clef {cdKey} également utilisée par {reader.GetString(0)}", LogUtils.LogType.PlayerConnections);
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Select Query - {e.Message}");
          Utils.LogMessageToDMs(queryString);
        }

        try
        {
          queryString = "select playerAccount from playerConnectionInfo where playerAccount != @playerAccount and ipAdress = @ipAdress";

          using var connection = new SqliteConnection(Config.dbPath);
          connection.Open();

          var command = connection.CreateCommand();
          command.CommandText = queryString;

          command.Parameters.AddWithValue($"@playerAccount", (object)playerName ?? DBNull.Value);
          command.Parameters.AddWithValue($"@ipAdress", (object)ipAdress ?? DBNull.Value);

          using var reader = await command.ExecuteReaderAsync();
          while (reader.Read())
          {
            LogUtils.LogMessage($"WARNING - {playerName} vient de se connecter avec l'adresse ip {ipAdress} également utilisée par {reader.GetString(0)}", LogUtils.LogType.PlayerConnections);
          }
        }
        catch (Exception e)
        {
          Utils.LogMessageToDMs($"Select Query - {e.Message}");
          Utils.LogMessageToDMs(queryString);
        }
      }
      private async void InitializeAccountMapPins(string serializedMapPins)
      {
        using var stream = await StringUtils.GenerateStreamFromString(serializedMapPins);

        try
        {
          mapPinDictionnary = await JsonSerializer.DeserializeAsync<Dictionary<int, MapPin>>(stream);
        }
        catch (Exception)
        {
          return;
        }

        await NwTask.SwitchToMainThread();

        foreach (var pin in mapPinDictionnary.Values)
        {
          oid.LoginCreature.GetObjectVariable<LocalVariableString>($"NW_MAP_PIN_NTRY_{pin.id}").Value = pin.note;
          oid.LoginCreature.GetObjectVariable<LocalVariableFloat>($"NW_MAP_PIN_XPOS_{pin.id}").Value = pin.x;
          oid.LoginCreature.GetObjectVariable<LocalVariableFloat>($"NW_MAP_PIN_YPOS_{pin.id}").Value = pin.y;
          oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwArea>>($"NW_MAP_PIN_AREA_{pin.id}").Value = NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == pin.areaTag);
        }

        if (mapPinDictionnary.Count > 0)
          oid.LoginCreature.GetObjectVariable<LocalVariableInt>("NW_TOTAL_MAP_PINS").Value = mapPinDictionnary.Max(v => v.Key);
      }
      private async void InitializeAccountChatColors(string serializedChatColors)
      {
        if (string.IsNullOrEmpty(serializedChatColors))
          return;

        await Task.Run(() => chatColors = JsonConvert.DeserializeObject<Dictionary<int, byte[]>>(serializedChatColors));
      }
      private async void InitializeAccountMutedPlayers(string serializedMutedPlayers)
      {
        if (string.IsNullOrEmpty(serializedMutedPlayers))
          return;

        await Task.Run(() => mutedList = JsonConvert.DeserializeObject<List<int>>(serializedMutedPlayers));
      }
      private async void InitializeAccountWindowRectanglesPlayers(string serializedWindowRectangles)
      {
        if (string.IsNullOrEmpty(serializedWindowRectangles))
          return;

        windowRectangles = await Task.Run(() => JsonConvert.DeserializeObject<Dictionary<string, NuiRect>>(serializedWindowRectangles));
      }
      private async void InitializeAccountCustomDMVisualEffects(string serializedCustomDMVisualEffects)
      {
        if (string.IsNullOrEmpty(serializedCustomDMVisualEffects))
          return;

        customDMVisualEffects = await Task.Run(() => JsonConvert.DeserializeObject<List<CustomDMVisualEffect>>(serializedCustomDMVisualEffects));
      }
      private void OnCombatStarted(OnCombatStatusChange onCombatStatusChange)
      {
        onCombatStatusChange.Player.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;

        if (onCombatStatusChange.CombatStatus == CombatStatus.ExitCombat)
          return;

        Effect effPC = onCombatStatusChange.Player.ControlledCreature.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.CutsceneGhost);
        if (effPC != null)
          onCombatStatusChange.Player.ControlledCreature.RemoveEffect(effPC);
      }
      private void HandleMapPinAdded(OnMapPinAddPin onAdd)
      {
        int id = oid.LoginCreature.GetObjectVariable<LocalVariableInt>("NW_TOTAL_MAP_PINS").Value;
        mapPinDictionnary.Add(id, new MapPin(id, oid.ControlledCreature.Area.Tag, onAdd.Position.X, onAdd.Position.Y, onAdd.Note));
        SaveMapPinsToDatabase();
      }
      private void HandleMapPinChanged(OnMapPinChangePin onChange)
      {
        MapPin updatedMapPin = mapPinDictionnary[onChange.Id];
        updatedMapPin.x = onChange.Position.X;
        updatedMapPin.y = onChange.Position.Y;
        updatedMapPin.note = onChange.Note;
        SaveMapPinsToDatabase();
      }
      private void HandleMapPinDestroyed(OnMapPinDestroyPin onDestroy)
      {
        mapPinDictionnary.Remove(onDestroy.Id);
        SaveMapPinsToDatabase();
      }
      public void HandleReinit()
      {
        LearnableSkill oldSkill;

        if(learnableSkills.ContainsKey(CustomSkill.ImprovedClubProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedClubProficiency];
          learnableSkills.Add(CustomSkill.ClubProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ClubProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedClubProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightFlailProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedLightFlailProficiency];
          learnableSkills.Add(CustomSkill.LightFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightFlailProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedLightFlailProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedShortBowProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedShortBowProficiency];
          learnableSkills.Add(CustomSkill.ShortBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortBowProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedShortBowProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightCrossBowProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedLightCrossBowProficiency];
          learnableSkills.Add(CustomSkill.LightCrossBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightCrossBowProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedLightCrossBowProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightMaceProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedLightMaceProficiency];
          learnableSkills.Add(CustomSkill.LightMaceProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightMaceProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedLightMaceProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedDaggerProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedDaggerProficiency];
          learnableSkills.Add(CustomSkill.DaggerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DaggerProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedDaggerProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedDartProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedDartProficiency];
          learnableSkills.Add(CustomSkill.DartProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.DartProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedDartProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightHammerProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedLightHammerProficiency];
          learnableSkills.Add(CustomSkill.LightHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightHammerProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedLightHammerProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedQuarterStaffProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedQuarterStaffProficiency];
          learnableSkills.Add(CustomSkill.QuarterStaffProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.QuarterStaffProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedQuarterStaffProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedMorningStarProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedMorningStarProficiency];
          learnableSkills.Add(CustomSkill.MorningStarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.MorningStarProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedMorningStarProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedShortSpearProficiency) && !learnableSkills.ContainsKey(CustomSkill.ShortSpearProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedShortSpearProficiency];
          learnableSkills.Add(CustomSkill.ShortSpearProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ShortSpearProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedShortSpearProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedSlingProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedSlingProficiency];
          learnableSkills.Add(CustomSkill.SlingProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SlingProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedSlingProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedSickleProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedSickleProficiency];
          learnableSkills.Add(CustomSkill.SickleProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.SickleProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedSickleProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightArmorProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedLightArmorProficiency];
          learnableSkills.Add(CustomSkill.LightArmorProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightArmorProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedLightArmorProficiency);
        }

        if (learnableSkills.ContainsKey(CustomSkill.ImprovedLightShieldProficiency))
        {
          oldSkill = learnableSkills[CustomSkill.ImprovedLightShieldProficiency];
          learnableSkills.Add(CustomSkill.LightShieldProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.LightShieldProficiency], false, oldSkill.acquiredPoints, oldSkill.currentLevel, oldSkill.bonusPoints));
          learnableSkills.Remove(CustomSkill.ImprovedLightShieldProficiency);
        }

        if (oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_REINITIALISATION_DONE").HasNothing)
        {
          /*foreach(var item in oid.LoginCreature.Inventory.Items)
            item.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;*/

          foreach (var feat in oid.LoginCreature.Feats)
            if (feat.Id > 1116)
              oid.LoginCreature.RemoveFeat(feat);

          foreach (Skill skillType in (Skill[])Enum.GetValues(typeof(Skill)))
            try { oid.LoginCreature.SetSkillRank(NwSkill.FromSkillType(skillType), 0); }
            catch(Exception) { }

          oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).Destroy();

          Task waitSkinCreated = NwTask.Run(async () =>
          {
            NwItem pcSkin = await NwItem.Create("peaudejoueur", oid.LoginCreature);
            //pcSkin.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
            pcSkin.Name = $"Propriétés de {oid.LoginCreature.Name}";
            oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
          });

          InitializeNewPlayerLearnableSkills();

          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_REINITIALISATION_DONE").Value = 1;
        }
      }
    }
  }
}
