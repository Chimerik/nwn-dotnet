using System;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    private static int HandlePlayerConnect(uint oidSelf)
    {
      var oPC = NWScript.GetEnteringObject();

      //TODO : système de BANLIST

      //oPC.AsCreature().AddFeat(NWN.Enums.Feat.PlayerTool01);

      //if (NWScript.GetLocalInt(oidSelf, "_LANGUE_ACTIVE") != 0)
      //{
      /*      NWScript.SendMessageToPC(oPC, $"langue = {NWScript.GetLocalInt(oPC, "_LANGUE_ACTIVE")}");
              NWScript.DeleteLocalInt(oPC, "_LANGUE_ACTIVE");
            NWScript.SendMessageToPC(oPC, $"langue = {NWScript.GetLocalInt(oPC, "_LANGUE_ACTIVE")}");

            NWScript.DelayCommand(4.5f, () => NWScript.SetTextureOverride("icon_elf", "", oPC));
              NWScript.DelayCommand(5.0f, () => RefreshQBS(oPC));
       */     //}
              // else
              //{
              //NWScript.SetTextureOverride("icon_elf", "", oidSelf);

      //RefreshQBS(oidSelf, 0);
      // }

      Player player;
      if (!Players.ContainsKey(oPC))
      {
        player = new Player(oPC);
        Players.Add(oPC, player);
      }
      else
        player = Players[oPC];

      // TODO : Système de sauvegarde et de chargement de quickbar
      //EventsPlugin.AddObjectToDispatchList("NWNX_ON_QUICKBAR_SET_BUTTON_AFTER", "event_qbs", oPC);

      if (NWScript.GetIsDM(oPC) != 1)
      {
        /*if (NWScript.GetIsObjectValid(NWScript.GetItemPossessedBy(oPC, "pj_lycan_curse")) == 1) // TODO : revoir système de métamorphose et de malédiction lycanthropique
        {
          CreaturePlugin.AddFeat(oPC, NWScript.FEAT_PLAYER_TOOL_02);
          NWScript.DestroyObject(NWScript.GetItemPossessedBy(oPC, "pj_lycan_curse"));
        }*/

        // Initialisation de la faim (TODO : récupérer la faim en BDD)
        // TODO : système de faim en pause pour le moment. A revoir. Je pense qu'il doit être moins punitif et impacter uniquement le taux d'apprentissage des skills
        /*float fNourriture = 200.0f;

        if (fNourriture < 100.0f)
        {
          int nLoss = 100 - Convert.ToInt32(fNourriture);
          Effect eHunger = NWScript.EffectAbilityDecrease(NWScript.ABILITY_STRENGTH, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_STRENGTH) * nLoss / 100);
          eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease(NWScript.ABILITY_DEXTERITY, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_DEXTERITY) * nLoss / 100));
          eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease(NWScript.ABILITY_CONSTITUTION, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_CONSTITUTION) * nLoss / 100));
          eHunger = NWScript.EffectLinkEffects(eHunger, NWScript.EffectAbilityDecrease(NWScript.ABILITY_CHARISMA, NWScript.GetAbilityScore(oPC, NWScript.ABILITY_CHARISMA) * nLoss / 100));
          eHunger = NWScript.SupernaturalEffect(eHunger);
          eHunger = NWScript.TagEffect(eHunger, "Effect_Hunger");
          NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_PERMANENT, eHunger, oPC);
        }*/

        if (player.location != null)
        {
          NWScript.DelayCommand(1.0f, () => NWScript.AssignCommand(player.oid, () => NWScript.ClearAllActions()));
          NWScript.DelayCommand(1.1f, () => NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(player.location)));
        }

        if (player.craftJob.isActive && NWScript.GetLocalInt(NWScript.GetArea(player.oid), "_REST") != 0)
          player.CraftJobProgression();

        if (player.currentSkillJob != (int)Feat.Invalid)
        {
          player.learnableSkills[player.currentSkillJob].currentJob = true;
          player.AcquireSkillPoints();
        }
        else
          NWScript.DelayCommand(10.0f, () => player.PlayNoCurrentTrainingEffects());

        player.dateLastSaved = DateTime.Now;
      }

      return 0;
    }
    private static void InitializeNewPlayer(uint newPlayer)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT rowid FROM PlayerAccounts WHERE accountName = @accountName");
      NWScript.SqlBindString(query, "@accountName", NWScript.GetPCPlayerName(newPlayer));

      if (!Convert.ToBoolean(NWScript.SqlStep(query)))
       {

         WebhookSystem.StartSendingAsyncDiscordMessage($"Toute première connexion de {NWScript.GetPCPlayerName(newPlayer)}", "AoA notification service - Nouveau joueur !");

         query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO PlayerAccounts (accountName , bonusRolePlay) VALUES (@name, @brp)");
         NWScript.SqlBindInt(query, "@brp", 1);
         NWScript.SqlBindString(query, "@name", NWScript.GetPCPlayerName(newPlayer));
         NWScript.SqlStep(query);

         query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT last_insert_rowid()");
         NWScript.SqlStep(query);
       }

       ObjectPlugin.SetInt(newPlayer, "accountId", NWScript.SqlGetInt(query, 0), 1);
    }
    private static void InitializeNewCharacter(Player newCharacter)
    {
      WebhookSystem.StartSendingAsyncDiscordMessage($"{NWScript.GetPCPlayerName(newCharacter.oid)} vient de créer un nouveau personnage : {NWScript.GetName(newCharacter.oid)}", "AoA notification service - Nouveau personnage !");
      ObjectPlugin.SetInt(newCharacter.oid, "_STARTING_SKILL_POINTS", 5000, 1);

      uint arrivalArea = NWScript.CopyArea(Module.areaDictionnary.Where(v => v.Value.tag == "entry_scene").FirstOrDefault().Value.oid);
      Module.areaDictionnary.Add(NWScript.GetObjectUUID(arrivalArea), new Area(arrivalArea));
      NWScript.SetName(arrivalArea, $"La galère de {NWScript.GetName(newCharacter.oid)} (Bienvenue !)");
      NWScript.SetTag(arrivalArea, $"entry_scene_{NWScript.GetPCPublicCDKey(newCharacter.oid)}");
      uint arrivalPoint = NWScript.GetNearestObjectByTag("ENTRY_POINT", NWScript.GetFirstObjectInArea(arrivalArea));

      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO playerCharacters (accountId , characterName, dateLastSaved, currentSkillJob, currentCraftJob, currentCraftObject, frostAttackOn, areaTag, position, facing) VALUES (@accountId, @name, @dateLastSaved, @currentSkillJob, @currentCraftJob, @currentCraftObject, @frostAttackOn, @areaTag, @position, @facing)");
      NWScript.SqlBindInt(query, "@accountId", newCharacter.accountId);
      NWScript.SqlBindString(query, "@name", NWScript.GetName(newCharacter.oid));
      NWScript.SqlBindString(query, "@dateLastSaved", DateTime.Now.ToString());
      NWScript.SqlBindInt(query, "@currentSkillJob", (int)Feat.Invalid);
      NWScript.SqlBindString(query, "@currentCraftJob", "");
      NWScript.SqlBindString(query, "@currentCraftObject", "");
      NWScript.SqlBindInt(query, "@frostAttackOn", 0);
      NWScript.SqlBindString(query, "@areaTag", NWScript.GetTag(arrivalArea));
      NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(arrivalPoint));
      NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(arrivalPoint));
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT last_insert_rowid()");
      NWScript.SqlStep(query);
      
      ObjectPlugin.SetInt(newCharacter.oid, "characterId", NWScript.SqlGetInt(query, 0), 1);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"INSERT INTO playerMaterialStorage (characterId) VALUES (@characterId)");
      NWScript.SqlBindInt(query, "@characterId", ObjectPlugin.GetInt(newCharacter.oid, "characterId"));
      NWScript.SqlStep(query);

      for (int spellLevel = 0; spellLevel < 10; spellLevel++)
        while (CreaturePlugin.GetKnownSpellCount(newCharacter.oid, 43, spellLevel) > 0)
          CreaturePlugin.RemoveKnownSpell(newCharacter.oid, 43, spellLevel, CreaturePlugin.GetKnownSpell(newCharacter.oid, 43, spellLevel, 0));

      InitializeNewPlayerLearnableSkills(newCharacter);
    }
    public static void InitializeNewPlayerLearnableSkills(Player player)
    {
      player.learnableSkills.Add((int)Feat.ImprovedHealth, new SkillSystem.Skill((int)Feat.ImprovedHealth, 0.0f, player));
      player.learnableSkills.Add((int)Feat.Toughness, new SkillSystem.Skill((int)Feat.Toughness, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedAttackBonus, new SkillSystem.Skill((int)Feat.ImprovedAttackBonus, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedCasterLevel, new SkillSystem.Skill((int)Feat.ImprovedCasterLevel, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ArmorProficiencyLight, new SkillSystem.Skill((int)Feat.ArmorProficiencyLight, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ShieldProficiency, new SkillSystem.Skill((int)Feat.ShieldProficiency, 0.0f, player));
      player.learnableSkills.Add((int)Feat.WeaponFinesse, new SkillSystem.Skill((int)Feat.WeaponFinesse, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSavingThrowFortitude, new SkillSystem.Skill((int)Feat.ImprovedSavingThrowFortitude, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSavingThrowReflex, new SkillSystem.Skill((int)Feat.ImprovedSavingThrowReflex, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSavingThrowWill, new SkillSystem.Skill((int)Feat.ImprovedSavingThrowWill, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSpellSlot0_1, new SkillSystem.Skill((int)Feat.ImprovedSpellSlot0_1, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSpellSlot1_1, new SkillSystem.Skill((int)Feat.ImprovedSpellSlot1_1, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedStrength, new SkillSystem.Skill((int)Feat.ImprovedStrength, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedDexterity, new SkillSystem.Skill((int)Feat.ImprovedDexterity, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedConstitution, new SkillSystem.Skill((int)Feat.ImprovedConstitution, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedIntelligence, new SkillSystem.Skill((int)Feat.ImprovedIntelligence, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedWisdom, new SkillSystem.Skill((int)Feat.ImprovedWisdom, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedCharisma, new SkillSystem.Skill((int)Feat.ImprovedCharisma, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedAnimalEmpathy, new SkillSystem.Skill((int)Feat.ImprovedAnimalEmpathy, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedConcentration, new SkillSystem.Skill((int)Feat.ImprovedConcentration, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedDisableTraps, new SkillSystem.Skill((int)Feat.ImprovedDisableTraps, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedDiscipline, new SkillSystem.Skill((int)Feat.ImprovedDiscipline, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSkillParry, new SkillSystem.Skill((int)Feat.ImprovedSkillParry, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedPerform, new SkillSystem.Skill((int)Feat.ImprovedPerform, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedPickpocket, new SkillSystem.Skill((int)Feat.ImprovedPickpocket, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSearch, new SkillSystem.Skill((int)Feat.ImprovedSearch, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSetTrap, new SkillSystem.Skill((int)Feat.ImprovedSetTrap, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSpellcraft, new SkillSystem.Skill((int)Feat.ImprovedSpellcraft, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedSpot, new SkillSystem.Skill((int)Feat.ImprovedSpot, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedTaunt, new SkillSystem.Skill((int)Feat.ImprovedTaunt, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedUseMagicDevice, new SkillSystem.Skill((int)Feat.ImprovedUseMagicDevice, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedTumble, new SkillSystem.Skill((int)Feat.ImprovedTumble, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedBluff, new SkillSystem.Skill((int)Feat.ImprovedBluff, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedIntimidate, new SkillSystem.Skill((int)Feat.ImprovedIntimidate, 0.0f, player));
    }
    private static void InitializeDM(Player player)
    {

    }
    private static void InitializePlayer(Player player)
    {
      InitializePlayerEvents(player.oid);
      InitializePlayerAccount(player);
      InitializePlayerCharacter(player);
      InitializePlayerLearnableSkills(player);      
    }
    private static void InitializePlayerEvents(uint player)
    {
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_FEAT_BEFORE", "event_feat_used", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ADD_ASSOCIATE_AFTER", "summon_add_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_REMOVE_ASSOCIATE_AFTER", "summon_remove_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "event_spellbroadcast_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_SPELL_INTERRUPTED_AFTER", "_onspellinterrupted_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CAST_SPELL_BEFORE", "_onspellcast_before", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CAST_SPELL_AFTER", "_onspellcast_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_equip_items_before", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_unequip_items_before", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_VALIDATE_ITEM_EQUIP_BEFORE", "event_validate_equip_items_before", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_VALIDATE_USE_ITEM_BEFORE", "event_validate_equip_items_before", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_ITEM_BEFORE", "event_use_item_before", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "event_detection_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_start_combat_after", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "player_exit_before", player);
      NWScript.SetEventScript(player, NWScript.EVENT_SCRIPT_CREATURE_ON_NOTICE, "on_perceived_pc");
    }
    private static void InitializePlayerAccount(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT bonusRolePlay from PlayerAccounts where rowid = @accountId");
      NWScript.SqlBindInt(query, "@accountId", player.accountId);
      NWScript.SqlStep(query);

      player.bonusRolePlay = NWScript.SqlGetInt(query, 0);
    }
    private static void InitializePlayerCharacter(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT areaTag, position, facing, currentHP, bankGold, dateLastSaved, currentSkillJob, currentCraftJob, currentCraftObject, currentCraftJobRemainingTime, currentCraftJobMaterial, frostAttackOn, menuOriginTop, menuOriginLeft from playerCharacters where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlStep(query);

      player.location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 0), NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2));
      player.currentHP = NWScript.SqlGetInt(query, 3);
      player.bankGold = NWScript.SqlGetInt(query, 4);
      player.dateLastSaved = DateTime.Parse(NWScript.SqlGetString(query, 5));
      player.currentSkillJob = NWScript.SqlGetInt(query, 6);
      player.craftJob = new CraftJob(NWScript.SqlGetString(query, 7), NWScript.SqlGetString(query, 10), NWScript.SqlGetFloat(query, 9), player, NWScript.SqlGetString(query, 8));
      player.isFrostAttackOn = Convert.ToBoolean(NWScript.SqlGetInt(query, 11));
      player.menu.originTop = NWScript.SqlGetInt(query, 12);
      player.menu.originLeft = NWScript.SqlGetInt(query, 13);
      player.previousArea = NWScript.GetAreaFromLocation(player.location);

      query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT Veldspar, Scordite, Pyroxeres, Tritanium, Pyerite, Mexallon, Noxcium from playerMaterialStorage where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      if (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        player.materialStock.Add("Veldspar", NWScript.SqlGetInt(query, 0));
        player.materialStock.Add("Scordite", NWScript.SqlGetInt(query, 1));
        player.materialStock.Add("Pyroxeres", NWScript.SqlGetInt(query, 2));
        player.materialStock.Add("Tritanium", NWScript.SqlGetInt(query, 3));
        player.materialStock.Add("Pyerite", NWScript.SqlGetInt(query, 4));
        player.materialStock.Add("Mexallon", NWScript.SqlGetInt(query, 5));
        player.materialStock.Add("Noxcium", NWScript.SqlGetInt(query, 6));
      }

      if (player.isFrostAttackOn)
      {
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_ATTACK_OBJECT_BEFORE", "event_auto_spell", player.oid);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_FORCE_MOVE_TO_OBJECT_BEFORE", "event_auto_spell", player.oid);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "_onspellcast", player.oid);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_KEYBOARD_BEFORE", "event_auto_spell", player.oid);
        EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_WALK_TO_WAYPOINT_BEFORE", "event_auto_spell", player.oid);
      }

      //Appliquer la distance de perception du chat en fonction de la compétence Listen du joueur
      ChatPlugin.SetChatHearingDistance(ChatPlugin.GetChatHearingDistance(player.oid, ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK) + NWScript.GetSkillRank(NWScript.SKILL_LISTEN, player.oid) / 5, player.oid, ChatPlugin.NWNX_CHAT_CHANNEL_PLAYER_TALK);
      ChatPlugin.SetChatHearingDistance(ChatPlugin.GetChatHearingDistance(player.oid, ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER) + NWScript.GetSkillRank(NWScript.SKILL_LISTEN, player.oid) / 10, player.oid, ChatPlugin.NWNX_CHAT_CHANNEL_DM_WHISPER);
      player.craftJob.isCancelled = false;
      player.isConnected = true;
      player.isAFK = true;
      player.DoJournalUpdate = false;
      
      player.playerJournal = new PlayerJournal();
    }
    private static void InitializePlayerLearnableSkills(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT skillId, skillPoints from playerLearnableSkills where characterId = @characterId and trained = 0");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.learnableSkills.Add(NWScript.SqlGetInt(query, 0), new SkillSystem.Skill(NWScript.SqlGetInt(query, 0), NWScript.SqlGetInt(query, 1), player, true));
    }
  }
}
