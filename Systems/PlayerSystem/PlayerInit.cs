using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Discord;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System.Threading;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerConnect(ModuleEvents.OnClientEnter HandlePlayerConnect)
    {
      Log.Info("Player connecting");
      NwPlayer oPC = HandlePlayerConnect.Player;

      if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
        player = new Player(oPC, areaSystem, spellSystem, feedbackService, scheduler);
      else
      {
        player.oid = oPC;
        player.characterId = oPC.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value;

        if (player.location.Area == null)
        {
          var result = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "location" } },
          new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

          if (result.Result != null)
            player.location = SqLiteUtils.DeserializeLocation(result.Result.GetString(0));

          player.TeleportPlayerToSavedLocation();
        }
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

      if (oPC.IsDM)
        return;

      string pcAccount = player.CheckDBPlayerAccount();
      if (pcAccount != oPC.PlayerName)
      {
        oPC.BootPlayer($"Attention - Ce personnage est enregistré sous le compte {pcAccount}, or vous venez de vous connecter sous {oPC.PlayerName}, veuillez vous reconnecter avec le bon compte !");
        Utils.LogMessageToDMs($"Attention - {oPC.PlayerName} vient de se connecter avec un personnage enregistré sous le compte : {pcAccount} !");
        return;
      }

      Utils.ResetVisualTransform(player.oid.ControlledCreature);
      player.pcState = Player.PcState.Offline;

      player.InitializePlayerLearnableJobs();

      if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Sit))
        player.oid.LoginCreature.AddFeat(CustomFeats.Sit);

      foreach (Player connectedPlayer in Players.Values.Where(p => p.pcState != Player.PcState.Offline && p.TryGetOpenedWindow("playerList", out Player.PlayerWindow playerListWindow)))
        ((Player.PlayerListWindow)connectedPlayer.windows["playerList"]).UpdatePlayerList();

      player.mapLoadingTime = DateTime.Now;

      Log.Info("End of player init.");
    }
    public partial class Player
    {
      public void InitializeNewPlayer()
      {
        var result = SqLiteUtils.SelectQuery("PlayerAccounts",
          new List<string>() { { "rowid" } },
          new List<string[]>() { { new string[] { "accountName", oid.PlayerName } } });

        if (result.Result == null)
        {
          if (Config.env == Config.Env.Prod)
          {
            (Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Toute première connexion de {oid.LoginCreature.Name}. Accueillons le comme il se doit !");
            (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Toute première connexion de {oid.LoginCreature.Name} => nouveau joueur à accueillir !");

            windows.Add("introWelcome", new IntroWelcomeWindow(this));
          }

          SqLiteUtils.InsertQuery("PlayerAccounts",
            new List<string[]>() { new string[] { "accountName", oid.PlayerName }, new string[] { "cdKey", oid.CDKey }, new string[] { "bonusRolePlay", "1" } });

          var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
          query.Execute();

          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value = query.Result.GetInt(0);
        }
        else
          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value = result.Result.GetInt(0);

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
      }
      public void InitializeNewCharacter()
      {
        if (Config.env == Config.Env.Prod)
        {
          Task waitBotMessage = NwTask.Run(async () =>
          {
            await (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{oid.PlayerName} vient de créer un nouveau personnage : {oid.LoginCreature.Name}");
          });
        }

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
            await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);
            oid.LoginCreature.Location = arrivalLocation;
          });
        }

        Utils.DestroyInventory(oid.LoginCreature);

        if (oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin) == null)
        {
          Task waitSkinCreated = NwTask.Run(async () =>
          {
            NwItem pcSkin = await NwItem.Create("peaudejoueur", oid.LoginCreature);
            pcSkin.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
            pcSkin.Name = $"Propriétés de {oid.LoginCreature.Name}";
            oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
          });
        }

        SqLiteUtils.InsertQuery("playerCharacters",
            new List<string[]>() { new string[] { "accountId", accountId.ToString() }, new string[] { "characterName", oid.LoginCreature.Name },  new string[] { "location", SqLiteUtils.SerializeLocation(arrivalLocation) }, new string[] { "menuOriginLeft", "50" }, new string[] { "currentHP", oid.LoginCreature.MaxHP.ToString() } });

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
        learnableSkills.Add(CustomSkill.ImprovedStrength, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedStrength]));
        learnableSkills.Add(CustomSkill.ImprovedDexterity, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedDexterity]));
        learnableSkills.Add(CustomSkill.ImprovedConstitution, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedConstitution]));
        learnableSkills.Add(CustomSkill.ImprovedIntelligence, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedIntelligence]));
        learnableSkills.Add(CustomSkill.ImprovedWisdom, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedWisdom]));
        learnableSkills.Add(CustomSkill.ImprovedCharisma, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedCharisma]));
       
        learnableSkills.Add(CustomSkill.ImprovedHealth, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedHealth]));
        learnableSkills.Add(CustomSkill.Toughness, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.Toughness]));

        learnableSkills.Add(CustomSkill.ImprovedAttackBonus, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedAttackBonus]));
        learnableSkills.Add(CustomSkill.ImprovedCasterLevel, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedCasterLevel]));
        learnableSkills.Add(CustomSkill.ImprovedSpellSlot0, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedSpellSlot0]));
        learnableSkills.Add(CustomSkill.ImprovedSpellSlot1, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedSpellSlot1]));

        learnableSkills.Add(CustomSkill.ImprovedLightArmorProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedLightArmorProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedLightShieldProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedLightShieldProficiency]));

        learnableSkills.Add(CustomSkill.ImprovedClubProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedClubProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedLightFlailProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedLightFlailProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedShortBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedShortBowProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedLightCrossBowProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedLightCrossBowProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedLightMaceProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedLightMaceProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedDaggerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedDaggerProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedDartProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedDartProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedLightHammerProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedLightHammerProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedQuarterStaffProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedQuarterStaffProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedMorningStarProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedMorningStarProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedShortSpearProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedShortSpearProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedSlingProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedSlingProficiency]));
        learnableSkills.Add(CustomSkill.ImprovedSickleProficiency, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.ImprovedSickleProficiency]));

        learnableSkills.Add(CustomSkill.TwoWeaponFighting, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.TwoWeaponFighting]));
        learnableSkills.Add(CustomSkill.WeaponFinesse, new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[CustomSkill.WeaponFinesse]));

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

        ItemSystem.feedbackService.AddCombatLogMessageFilter(CombatLogMessage.ComplexAttack, oid);
      }
      public void InitializePlayer()
      {
        InitializePlayerEvents();
        InitializeSpellEvents();
        InitializePlayerAccount();
        InitializePlayerCharacter();

        switch (oid.LoginCreature.Race.RacialType)
        {
          case RacialType.Dwarf:
            if (!oid.LoginCreature.KnowsFeat(CustomFeats.Nain))
              oid.LoginCreature.AddFeat(CustomFeats.Nain);
            break;
          case RacialType.Elf:
          case RacialType.HalfElf:
            if (!oid.LoginCreature.KnowsFeat(CustomFeats.Elfique))
              oid.LoginCreature.AddFeat(CustomFeats.Elfique);
            break;
          case RacialType.Halfling:
            if (!oid.LoginCreature.KnowsFeat(CustomFeats.Halfelin))
              oid.LoginCreature.AddFeat(CustomFeats.Halfelin);
            break;
          case RacialType.Gnome:
            if (!oid.LoginCreature.KnowsFeat(CustomFeats.Gnome))
              oid.LoginCreature.AddFeat(CustomFeats.Gnome);
            break;
          case RacialType.HalfOrc:
            if (!oid.LoginCreature.KnowsFeat(CustomFeats.Orc))
              oid.LoginCreature.AddFeat(CustomFeats.Orc);
            break;
        }

        //CheckForAFKStatus();
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
        oid.OnPlayerGuiEvent += HandleGuiEvents;
        oid.OnNuiEvent += HandleGenericNuiEvents;
        oid.OnMapPinAddPin += HandleMapPinAdded;
        oid.OnMapPinChangePin += HandleMapPinChanged;
        oid.OnMapPinDestroyPin += HandleMapPinDestroyed;
        oid.OnDMSpawnObjectAfter += areaSystem.InitializeEventsAfterDMSpawnCreature;
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
        var result = SqLiteUtils.SelectQuery("PlayerAccounts",
            new List<string>() { { "bonusRolePlay" }, { "mapPins" }, { "chatColors" }, { "mutedPlayers" }, { "windowRectangles" }, { "customDMVisualEffects" } },
            new List<string[]>() { { new string[] { "rowid", accountId.ToString() } } });

        if (result.Result != null)
        {
          bonusRolePlay = result.Result.GetInt(0);
          string serializedMapPins = result.Result.GetString(1);
          string serializedChatColors = result.Result.GetString(2);
          string serializedMutedPlayers = result.Result.GetString(3);
          string serializedWindowRectangles = result.Result.GetString(4);
          string serializedCustomDMVisualEffects = result.Result.GetString(5);
          InitializeAccountMapPins(serializedMapPins);
          InitializeAccountChatColors(serializedChatColors);
          InitializeAccountMutedPlayers(serializedMutedPlayers);
          InitializeAccountWindowRectanglesPlayers(serializedWindowRectangles);
          InitializeAccountCustomDMVisualEffects(serializedCustomDMVisualEffects);
        }
      }
      private void InitializePlayerCharacter()
      {
        var result = SqLiteUtils.SelectQuery("playerCharacters",
            new List<string>() { { "location" }, { "currentHP" }, { "bankGold" }, { "menuOriginTop" }, { "menuOriginLeft" }, { "pveArenaCurrentPoints" }, { "alchemyCauldron" }, { "serializedLearnableSkills" }, { "serializedLearnableSpells" }, { "explorationState" }, { "materialStorage" }, { "craftJob" }, { "grimoires" }, { "quickbars" }, { "itemAppearances" }, { "descriptions" } },
            new List<string[]>() { { new string[] { "rowid", characterId.ToString() } } });

        if (result.Result == null)
          return;

        location = SqLiteUtils.DeserializeLocation(result.Result.GetString(0));
        oid.LoginCreature.HP = result.Result.GetInt(1);
        bankGold = result.Result.GetInt(2);
        menu.originTop = result.Result.GetInt(3);
        menu.originLeft = result.Result.GetInt(4);
        pveArena.totalPoints = (uint)result.Result.GetInt(5);
        string serializedCauldron = result.Result.GetString(6);
        string serializedLearnableSkills = result.Result.GetString(7);
        string serializedLearnableSpells = result.Result.GetString(8);
        string serializedExploration = result.Result.GetString(9);
        string serializedCraftResources = result.Result.GetString(10);
        string serializedCraftJob = result.Result.GetString(11);
        string serializedGrimoires = result.Result.GetString(12);
        string serializedQuickbars = result.Result.GetString(13);
        string serializedItemAppearances = result.Result.GetString(14);
        string serializedDescriptions = result.Result.GetString(15);

        InitializePlayerAsync(serializedCauldron, serializedExploration, serializedLearnableSkills, serializedLearnableSpells, serializedCraftResources, serializedCraftJob, serializedGrimoires, serializedQuickbars, serializedItemAppearances, serializedDescriptions);
      }
      private async void InitializePlayerAsync(string serializedCauldron, string serializedExploration, string serializedLearnableSkills, string serializedLearnableSpells, string serializedCraftResources, string serializedCraftJob, string serializedGrimoires, string serializedQuickbars, string serializedItemAppearances, string serializedDescriptions)
      {
        Log.Info("starting async init");

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
            LearnableSkill skill = new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[kvp.Key], kvp.Value);

            if (skill.active)
              activeLearnable = skill;

            learnableSkills.Add(kvp.Key, skill);
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

          List<CraftResource.SerializableCraftResource> serializableCraftResource = JsonConvert.DeserializeObject<List<CraftResource.SerializableCraftResource>> (serializedCraftResources);
          
          foreach (var serializedMateria in serializableCraftResource)
            craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == (ResourceType)serializedMateria.type && r.grade == serializedMateria.grade), serializedMateria.quantity));
        });

        Task loadCraftJobTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedCraftJob) || serializedCraftJob == "null")
            return;

          craftJob = new CraftJob(JsonConvert.DeserializeObject<CraftJob.SerializableCraftJob>(serializedCraftJob), this);
        });

        Task loadGrimoiresTask = Task.Run(() =>
        {
          if (string.IsNullOrEmpty(serializedGrimoires) || serializedGrimoires == "null")
            return;

          grimoires = JsonConvert.DeserializeObject<List<Grimoire>>(serializedGrimoires);
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

        await Task.WhenAll(loadSkillsTask, loadSpellsTask, loadExplorationTask, loadCauldronTask, loadCraftJobTask, loadGrimoiresTask, loadQuickbarsTask, loadItemAppearancesTask, loadDescriptionsTask);
        Log.Info("async init done");
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
    }
  }
}
