using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        player.TeleportToSavedLocation();

        Task playerInitialized = NwTask.Run(async () =>
        {
          await NwTask.NextFrame();
          player.FinalizePlayerData();
        });
      }

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Sprint))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Sprint);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Disengage))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Disengage);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Dodge))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Dodge);

      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Stealth))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Stealth);

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

      Utils.ResetVisualTransform(player.oid.ControlledCreature);
      player.pcState = Player.PcState.Offline;
      player.OnLoginRefreshPlayerList();
      CreatureUtils.InitThreatRange(player.oid.LoginCreature);

      player.mapLoadingTime = DateTime.Now;
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
      public void InitializeDM()
      {
        oid.LoginCreature.OnAcquireItem += ItemSystem.HandleUnacquirableItems;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireForceDurability;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquirePlayerCorpse;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireDMCreatedItem;
        oid.LoginCreature.OnAcquireItem += ItemSystem.MergeStackableItem;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireItemSavePlayer;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquirePlayerCorpse;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquireItemSavePlayer;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipCancelIfInventoryFull;
        oid.LoginCreature.OnUseFeat += FeatSystem.OnUseFeatBefore;
        oid.OnNuiEvent += HandleGenericNuiEvents;

        eventService.Subscribe<OnDMSpawnObject, DMEventFactory>(oid.LoginCreature, areaSystem.InitializeEventsAfterDMSpawnCreature, EventCallbackType.After);
      }
      public void InitializePlayer()
      {
        InitializePlayerEvents();
        InitializeItemEvents();
        InitializeSpellEvents();
        InitializePlayerAccount();
        InitializePlayerCharacter();
      }
      private void InitializePlayerEvents()
      {
        oid.OnServerCharacterSave += HandleBeforePlayerSave;
        oid.OnPlayerDeath += HandlePlayerDeath;
        oid.LoginCreature.OnUseFeat += FeatSystem.OnUseFeatBefore;
        oid.OnCombatStatusChange += OnCombatStarted;
        oid.LoginCreature.OnCombatRoundStart += OnCombatStartForceHostility;
        oid.OnPartyEvent += Party.HandlePartyEvent;
        oid.OnClientLevelUpBegin += HandleOnClientLevelUp;
        oid.LoginCreature.OnUseSkill += HandleBeforeSkillUsed;
        oid.OnNuiEvent += HandleGenericNuiEvents;
        oid.OnMapPinAddPin += HandleMapPinAdded;
        oid.OnMapPinChangePin += HandleMapPinChanged;
        oid.OnMapPinDestroyPin += HandleMapPinDestroyed;
        oid.OnDMPlayerDMLogin += OnDmLoginRemoveThreatRange;
        oid.OnDMPlayerDMLogout += OnDmLogoutRemoveThreatRange;

        //oid.LoginCreature.OnEffectApply += HandleItemPropertyChecksOnEffectApplied;
        //oid.LoginCreature.OnEffectRemove += HandleItemPropertyChecksOnEffectRemoved;
        //eventService.Subscribe<OnUseFeat, OnUseFeat.Factory>(oid.LoginCreature, FeatSystem.OnUseFeatAfter, EventCallbackType.After);
        eventService.Subscribe<OnDMSpawnObject, DMEventFactory>(oid.LoginCreature, areaSystem.InitializeEventsAfterDMSpawnCreature, EventCallbackType.After);
      }
      private void InitializeItemEvents()
      {
        oid.LoginCreature.OnAcquireItem += ItemSystem.HandleUnacquirableItems;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireForceDurability;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquirePlayerCorpse;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireDMCreatedItem;
        oid.LoginCreature.OnAcquireItem += ItemSystem.MergeStackableItem;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireItemSavePlayer;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquirePlayerCorpse;
        oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquireItemSavePlayer;
        oid.LoginCreature.OnItemValidateEquip += ItemSystem.CancelRuinedItemEquip;
        oid.LoginCreature.OnItemValidateUse += ItemSystem.CancelRuinedItemUse;
        oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipCancelIfInventoryFull;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipCancelIfInventoryFull;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipOffHandWeapon;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipCheckArmorShieldProficiency;
        oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipCheckArmorShieldProficiency;
        oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireForceEquipCreatureSkin;
        oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipForceEquipCreatureSkin;
        oid.LoginCreature.OnItemUse += ItemSystem.OnItemUse;
        oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipCheckThreatRange;
        oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnequipCheckThreatRange;
        oid.LoginCreature.OnInventoryGoldAdd += HandleGainedGold;
        oid.LoginCreature.OnInventoryGoldRemove += HandleLostGold;
        oid.LoginCreature.OnItemScrollLearn += SpellSystem.OnLearnScroll;
      }
      private void InitializeSpellEvents()
      {
        //oid.LoginCreature.OnSpellAction += spellSystem.HandleSpellInput;
        oid.LoginCreature.OnSpellAction += SpellSystem.OnCastBonusSpell;    
        oid.LoginCreature.OnSpellAction += SpellSystem.HandleCraftOnSpellInput;
        oid.LoginCreature.OnSpellAction += SpellSystem.OnSpellCastSelectTargets;
        //oid.LoginCreature.OnSpellBroadcast += spellSystem.HandleHearingSpellBroadcast;
        oid.LoginCreature.OnSpellCast += SpellSystem.OnSpellCastCancelDivination;
        oid.LoginCreature.OnSpellCast += spellSystem.HandleCraftEnchantementCast;
        oid.LoginCreature.OnSpellSlotMemorize += HandleMemorizeSpellSlot;
      }
      private void InitializePlayerAccount()
      {
        var query = SqLiteUtils.SelectQuery("PlayerAccounts",
            new List<string>() { { "bonusRolePlay" }, { "mapPins" }, { "chatColors" }, { "mutedPlayers" }, { "windowRectangles" }, { "customDMVisualEffects" },
              { "hideFromPlayerList" }, { "cooldownPosition" } },
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
          string serializedCooldownPosition = result[7];
          InitializeAccountMapPins(serializedMapPins);
          InitializeAccountChatColors(serializedChatColors);
          InitializeAccountMutedPlayers(serializedMutedPlayers);
          InitializeAccountWindowRectanglesPlayers(serializedWindowRectangles);
          InitializeAccountCustomDMVisualEffects(serializedCustomDMVisualEffects);
          InitializeAccountCooldownPosition(serializedCooldownPosition);
        }
      }
      private void InitializePlayerCharacter()
      {
        var query = SqLiteUtils.SelectQuery("playerCharacters",
            new List<string>() { { "location" }, { "currentHP" }, { "bankGold" }, { "menuOriginTop" }, { "menuOriginLeft" }, { "pveArenaCurrentPoints" },
              { "alchemyCauldron" }, { "serializedLearnableSkills" }, { "serializedLearnableSpells" }, { "explorationState" }, { "materialStorage" }, { "craftJob" },
              { "grimoires" }, { "quickbars" }, { "itemAppearances" }, { "descriptions" }, { "mails" }, { "subscriptions" }, { "endurance" },
              { "transmutationStone" } },
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
          string serializedMails = result[16];
          string serializedSubscriptions = result[17];
          string serializedEndurance = result[18];
          transmutationStone = Guid.TryParse(result[19], out Guid uuid) ? uuid : Guid.Empty;

          InitializePlayerAsync(serializedCauldron, serializedExploration, serializedLearnableSkills, serializedLearnableSpells, serializedCraftResources, serializedCraftJob, serializedGrimoires, serializedQuickbars, serializedItemAppearances, serializedDescriptions, serializedMails, serializedSubscriptions, serializedEndurance);
        }
      }
      private async void InitializePlayerAsync(string serializedCauldron, string serializedExploration, string serializedLearnableSkills, string serializedLearnableSpells, string serializedCraftResources, string serializedCraftJob, string serializedGrimoires, string serializedQuickbars, string serializedItemAppearances, string serializedDescriptions, string serializedMails, string serializedSubscriptions, string serializedEndurance)
      {
        int playerLevel = oid.LoginCreature.Level;
        //int multiClassMultiplier = oid.LoginCreature.Classes.Distinct().Count();

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
            if (SkillSystem.learnableDictionary.TryGetValue(kvp.Key, out var value))
            {
              LearnableSkill skill = new LearnableSkill((LearnableSkill)value, kvp.Value, this, playerLevel);/*, multiClassMultiplier);
                */
              if (skill.active)
                activeLearnable = skill;

              learnableSkills.TryAdd(kvp.Key, skill);
            }
            else
              LogUtils.LogMessage($"SKILL SYSTEM - INVALID SKILL KEY {kvp.Key} REMOVED FROM {characterId} ({accountId})", LogUtils.LogType.Learnables);
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
            craftResourceStock.Add(new CraftResource(Craft.Collect.System.craftResourceArray.FirstOrDefault(r => r.type == (ResourceType)serializedMateria.type), serializedMateria.quantity));
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
        if (oid == null || oid.LoginCreature == null)
          return;

        HandleHealthPointInit();
        InitializeLearnables();
        InitializeJob();
        CheckPlayerConnectionInfo();
        InitializeNumAttackPerRound();
        HandleMailNotification();
        ApplyElvenSleepImmunity();
        ApplyWoodElfSpeed();
        ApplySmallRaceSlow();
        ApplyDrowLightSensitivity();
        ApplyThieflingFireResistance();
        ApplyDwarfPoisonResistance();
        ApplyNecroticResistance();
        ApplyConstitutionInfernale();
        ApplyHumanVersatility();
        ApplyHalfOrcEndurance();
        InitializeAbilityImprovementFeat();
        InitializeBonusAbilityChoice();
        InitializeElementalistChoice();
        InitializeBonusSkillChoice();
        InitializeWeaponMasterChoice();
        InitializeFeatChoice();
        InitializeFightingStyleChoice();
        InitializeRangerArchetypeChoice();
        InitializeHunterTactiqueDefensiveChoice();
        InitializeHunterDefenseSuperieureChoice();
        InitializeBelluaireCompanionChoice();
        InitializeHunterProieChoice();
        InitializeFavoredEnemyChoice();
        InitializeSubClassChoice();
        InitializeManoeuvreChoice();
        InitializeTirArcaniqueChoice();
        InitializeEspritTotemChoice();
        InitializeAspectTotemChoice();
        InitializeLienTotemChoice();
        InitializeMaitriseDesSortsChoice();
        InitializeSpellSelection();
        InitializeTechniqueElementaireSelection();
        InitializeMagicalSecretSelection();
        InitializeSkillProficiencySelection();
        ResetFlameBlade();
        ResetSize();
        ApplyProtectionStyle();
        ApplyAmbiMaster();
        ApplyMaitreArmureIntermediaire();
        ApplyTueurDeMage();
        ApplySentinelle();
        ApplyMobile();
        ApplyBroyeur();
        ApplyPourfendeur();
        ApplyLameDoutretombe();
        ApplyChanceDebordante();
        ApplyUltimeSurvivant();
        ApplyShieldArmorMalus();
        ApplyUnarmoredDefence();
        ApplyMonkUnarmoredDefence();
        InitializeActionSurge();
        InitializeWarMasterImplacable();
        InitializeMonkPerfection();
        ApplyPuretePhysique();
        ApplyElkAspect();
        ApplyGloutonAspect();
        ApplyBerserkerRepresailles();
        ApplyPerceptionAveugle();
        ApplyAssassinate();
        ApplyFrappeMeurtriere();
        ApplyThiefReflex();
        ApplyAbjurationWard();
        ApplyMagieDeGuerre();
        ApplyFrappeOcculte();
        ApplyArmeLiee();
        ApplyContreCharme();
        ApplyInspirationSuperieure();
        ApplyMagieDeCombat();
        ApplyEscrime();
        ApplyColdWanderer();
        ApplyFireWanderer();
        ApplyPoisonWanderer();
        ApplyAcidWanderer();
        ApplyMonkOpportunist();
        ApplyDefenseAdaptative();
        ApplyTraqueurRedoutable();
        ApplyWolfAspectAura();
        ApplyAuraDeProtection();
        ApplyAuraDeCourage();
        ApplyAuraDeDevotion();
        ApplyAuraDeGarde();
        ApplyPureteDeLesprit();
        ApplySentinelleImmortelle();
        ApplyVengeurImplacable();
        ApplyAvatarDeBataille();
        ApplyElectrocution();

        //RestoreCooledDownSpells();
        //HandleAdrenalineInit();
        //oid.LoginCreature.OnHeal -= SpellSystem.PreventHeal;
        //oid.LoginCreature.OnEffectApply -= EffectSystem.CheckFaerieFire;

        oid.LoginCreature.OnHeartbeat -= OnHeartbeatDetectTrap;
        oid.LoginCreature.OnHeartbeat += OnHeartbeatDetectTrap;

        InitializePlayerTlk();
        pcState = PcState.Online;
        oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;
      }
      private void HandleHealthPointInit()
      {
        if (oid.LoginCreature.HP < 1)
          oid.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Death());

        /*if (!windows.ContainsKey("healthBar")) windows.Add("healthBar", new HealthBarWindow(this));
        else ((HealthBarWindow)windows["healthBar"]).CreateWindow();

        if (!windows.ContainsKey("energyBar")) windows.Add("energyBar", new EnergyBarWindow(this));
        else ((EnergyBarWindow)windows["energyBar"]).CreateWindow();

        if (oid.LoginCreature.ActiveEffects.Any(e => e.Tag == "_CORE_EFFECT"))
          return;

        int conModifier = ((oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2);
        SetMaxHP();

        Effect runAction = Effect.RunAction(null, ItemSystem.removeCoreHandle);
        runAction = Effect.LinkEffects(runAction, Effect.Icon(NwGameTables.EffectIconTable.GetRow(132)));
        runAction.Tag = "_CORE_EFFECT";
        runAction.SubType = EffectSubType.Supernatural;
        
        TimeSpan duration = endurance.expirationDate - DateTime.Now;

        oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, runAction, TimeSpan.FromSeconds(duration.TotalSeconds > 0 ? duration.TotalSeconds : 0));
        LogUtils.LogMessage($"{oid.LoginCreature.Name} application des effets du Mélange à la connexion : HP endurance {endurance.maxHP}, max HP {oid.LoginCreature.LevelInfo[0].HitDie + conModifier}, HP régénérable {endurance.regenerableHP}, max énergie {endurance.maxMana}, énergie régénérable {(int)endurance.regenerableMana}, se dissipe le {endurance.expirationDate}", LogUtils.LogType.EnduranceSystem);

        energyRegen = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType == BaseItemType.MagicStaff ? 4 : 2;
        wasHPGreaterThan50 = oid.LoginCreature.HP > MaxHP / 2;*/
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
      private void RestoreCooledDownSpells()
      {
        foreach(var localVar in oid.LoginCreature.LocalVariables)
          if(localVar is DateTimeLocalVariable dateVar && dateVar.Name.StartsWith("_SPELL_COOLDOWN_"))
          {
            double cooldown = (DateTime.Now - dateVar.Value).TotalSeconds;
            if (cooldown < 0)
              cooldown = 1;

            string[] split = localVar.Name.Split("_");
            WaitCooldownToRestoreSpell(NwSpell.FromSpellId(int.Parse(split[^1])), cooldown);
          }
      }
      private void HandleAdrenalineInit()
      {
        if (oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>($"_LAST_DAMAGE_ON").HasNothing)
        {
          foreach (var feat in oid.LoginCreature.Feats)
            if (feat.MaxLevel > 0 && feat.MaxLevel < 255)
            {
              oid.LoginCreature.DecrementRemainingFeatUses(feat);
              oid.LoginCreature.GetObjectVariable<LocalVariableInt>($"_ADRENALINE_{feat.Id}").Delete();
            }
        }
      }
      private async void WaitCooldownToRestoreSpell(NwSpell spell, double cooldown)
      {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task playerLeft = NwTask.WaitUntil(() => !oid.IsValid, tokenSource.Token);
        Task cooledDown = NwTask.Delay(TimeSpan.FromSeconds(cooldown), tokenSource.Token);

        await NwTask.WhenAny(playerLeft, cooledDown);
        tokenSource.Cancel();

        if (playerLeft.IsCompletedSuccessfully || !oid.IsValid)
          return;

        RestoreSpell(spell);
      }
      private void RestoreSpell(NwSpell spell)
      {
        foreach (var spellSlot in oid.LoginCreature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spell.InnateSpellLevel))
          if (spellSlot.Spell == spell)
            spellSlot.IsReady = true;

        oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>($"_SPELL_COOLDOWN_{spell.Id}").Delete();
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
      private async void InitializeAccountCooldownPosition(string serializedPosition)
      {
        cooldownPositions = string.IsNullOrEmpty(serializedPosition) || serializedPosition == "null" ?
          new CooldownPosition(52, 6) : cooldownPositions = await Task.Run(() => JsonConvert.DeserializeObject<CooldownPosition>(serializedPosition));
      }
      private void OnCombatStarted(OnCombatStatusChange onCombatStatusChange)
      {
        onCombatStatusChange.Player.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;

        if (onCombatStatusChange.CombatStatus == CombatStatus.ExitCombat)
          return;

        NwCreature creature = onCombatStatusChange.Player.ControlledCreature;

        if (!creature.ActiveEffects.Any(e => e.Tag == EffectSystem.LienTotemElanAuraEffectTag))
          EffectUtils.RemoveEffectType(creature, EffectType.CutsceneGhost);
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
      /*public void HandleReinit()
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
          //foreach(var item in oid.LoginCreature.Inventory.Items)
            //item.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;

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
      }*/
      private async void HandleMemorizeSpellSlot(OnSpellSlotMemorize onMemorize)
      {
        if (onMemorize.Creature.GetObjectVariable<DateTimeLocalVariable>($"_SPELL_COOLDOWN_{onMemorize.Spell.Id}").Value > DateTime.UnixEpoch)
          return;

        await NwTask.NextFrame();
        
        foreach (var spellSlot in onMemorize.Creature.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(onMemorize.Spell.InnateSpellLevel))
          if (spellSlot.Spell == onMemorize.Spell)
            spellSlot.IsReady = true;
      }
    }
  }
}
