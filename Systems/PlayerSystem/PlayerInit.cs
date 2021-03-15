using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems.Craft;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerConnect(ModuleEvents.OnClientEnter HandlePlayerConnect)
    {
      NwPlayer oPC = HandlePlayerConnect.Player;

      oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value = (int)Feat.Invalid;
      oPC.GetLocalVariable<int>("_CONNECTING").Value = 1;
      oPC.GetLocalVariable<int>("_DISCONNECTING").Delete();

      if (!Players.TryGetValue(oPC, out Player player))
      {
        player = new Player(oPC);
        Players.Add(oPC, player);
      }
      else
        player.oid = oPC;
      
      if (oPC.IsDM)
        return;

      string pcAccount = player.CheckDBPlayerAccount();
      if (pcAccount != oPC.PlayerName)
      {
        oPC.BootPlayer($"Attention - Ce personnage est enregistré sous le compte {pcAccount}, or vous venez de vous connecter sous {oPC.PlayerName}, veuillez vous reconnecter avec le bon compte !");
        Utils.LogMessageToDMs($"Attention - {oPC.PlayerName} vient de se connecter avec un personnage enregistré sous le compte : {pcAccount} !");
        return;
      }

      if (player.currentHP <= 0)
        oPC.ApplyEffect(EffectDuration.Instant, API.Effect.Death());
      else
        oPC.HP = player.currentHP;

      if (player.craftJob.IsActive()
      && player.location.Area.GetLocalVariable<int>("_AREA_LEVEL")?.Value == 0)
      {
        player.CraftJobProgression();
        player.craftJob.CreateCraftJournalEntry();
      }

      if (player.currentSkillJob != (int)Feat.Invalid)
      {
        switch (player.currentSkillType)
        {
          case SkillSystem.SkillType.Skill:
            if (player.learnableSkills.ContainsKey(player.currentSkillJob))
              player.learnableSkills[player.currentSkillJob].currentJob = true;
            else
            {
              if (!Convert.ToBoolean(CreaturePlugin.GetKnowsFeat(player.oid, player.currentSkillJob)))
                CreaturePlugin.AddFeat(player.oid, player.currentSkillJob);
              player.currentSkillJob = (int)Feat.Invalid;
            }
            break;
          case SkillSystem.SkillType.Spell:
            if (player.learnableSpells.ContainsKey(player.currentSkillJob))
              player.learnableSpells[player.currentSkillJob].currentJob = true;
            else
              player.currentSkillJob = (int)Feat.Invalid;
            break;
        }

        player.AcquireSkillPoints();
        oPC.GetLocalVariable<int>("_CONNECTING").Delete();
        player.isAFK = false;

        if (player.currentSkillJob != (int)Feat.Invalid)
        {
          switch (player.currentSkillType)
          {
            case SkillSystem.SkillType.Skill:
              player.learnableSkills[player.currentSkillJob].CreateSkillJournalEntry();
              break;
            case SkillSystem.SkillType.Spell:
              player.learnableSpells[player.currentSkillJob].CreateSkillJournalEntry();
              break;
          }
        }
      }

      int improvedConst = CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ImprovedConstitution);
      if (improvedConst == (int)Feat.Invalid)
        improvedConst = 0;
      else
        improvedConst = Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", improvedConst));

      int improvedHealth = CreaturePlugin.GetHighestLevelOfFeat(player.oid, (int)Feat.ImprovedHealth);
      if (improvedHealth == (int)Feat.Invalid)
        improvedHealth = 0;
      else
        improvedHealth = Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", improvedHealth));

      //NWScript.SendMessageToPC(player.oid, $"pv : {Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43)) + (1 + 3 * ((NWScript.GetAbilityScore(oTarget, NWScript.ABILITY_CONSTITUTION, 1) + improvedConst - 10) / 2 + CreaturePlugin.GetKnowsFeat(oTarget, (int)Feat.Toughness))) * Int32.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(oTarget, (int)Feat.ImprovedHealth)))}");

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43))
        + (1 + 3 * ((NWScript.GetAbilityScore(player.oid, NWScript.ABILITY_CONSTITUTION, 1)
        + improvedConst - 10) / 2
        + CreaturePlugin.GetKnowsFeat(player.oid, (int)Feat.Toughness))) * improvedHealth);

      Task waitForTorilNecklaceChange = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Neck)?.Tag != "amulettorillink");
        ItemSystem.OnTorilNecklaceRemoved(oPC);
      });


      Task waitForArmorChange = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Chest) == null);
        ItemSystem.OnArmorRemoved(oPC);
      });

      Task waitForHelmetChange = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.Head) == null);
        ItemSystem.OnHelmetRemoved(oPC);
      });

      Task waitForShieldChange = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => oPC.GetItemInSlot(InventorySlot.LeftHand) == null && (oPC.GetItemInSlot(InventorySlot.RightHand) == null || ItemUtils.GetItemCategory((BaseItemType)oPC.GetItemInSlot(InventorySlot.RightHand)?.BaseItemType) == ItemUtils.ItemCategory.OneHandedMeleeWeapon));
        ItemSystem.OnShieldRemoved(oPC);
      });

      oPC.GetLocalVariable<int>("_CONNECTING").Delete();
      player.isAFK = false;
      player.DoJournalUpdate = false;
      player.dateLastSaved = DateTime.Now;
      player.setValue = Config.invalidInput;
      player.setString = "";
      Log.Info("End of player init.");
    }
    private static void InitializeNewPlayer(NwPlayer newPlayer)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT rowid FROM PlayerAccounts WHERE accountName = @accountName");
      NWScript.SqlBindString(query, "@accountName", newPlayer.PlayerName);

      if (!Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        if (Config.env == Config.Env.Prod)
        {
          (Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Toute première connexion de {newPlayer.Name}. Accueillons le comme il se doit !");
          (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Toute première connexion de {newPlayer.Name} => nouveau joueur à accueillir !");

          NWScript.DelayCommand(4.0f, () => newPlayer.PostString("a", 40, 15, ScreenAnchor.TopLeft, 0f, API.Color.WHITE, API.Color.WHITE, 9999, "fnt_my_gui"));
          EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", newPlayer);
        }

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO PlayerAccounts (accountName, cdKey, bonusRolePlay) VALUES (@name, @cdKey, @brp)");
        NWScript.SqlBindInt(query, "@brp", 1);
        NWScript.SqlBindString(query, "@name", newPlayer.PlayerName);
        NWScript.SqlBindString(query, "@cdKey", newPlayer.CDKey);
        NWScript.SqlStep(query);

        query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
        NWScript.SqlStep(query);
      }

      switch (newPlayer.RacialType)
      {
        case RacialType.Dwarf:
          CreaturePlugin.AddFeat(newPlayer, (int)Feat.Nain);
          break;
        case RacialType.Elf:
        case RacialType.HalfElf:
          CreaturePlugin.AddFeat(newPlayer, (int)Feat.Elfique);
          break;
        case RacialType.Halfling:
          CreaturePlugin.AddFeat(newPlayer, (int)Feat.Halfelin);
          break;
        case RacialType.Gnome:
          CreaturePlugin.AddFeat(newPlayer, (int)Feat.Gnome);
          break;
        case RacialType.HalfOrc:
          CreaturePlugin.AddFeat(newPlayer, (int)Feat.Orc);
          break;
      }

      ObjectPlugin.SetInt(newPlayer, "accountId", NWScript.SqlGetInt(query, 0), 1);
    }
    private static void InitializeNewCharacter(Player newCharacter)
    {
      if (Config.env == Config.Env.Prod)
        (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{newCharacter.oid.PlayerName} vient de créer un nouveau personnage : {newCharacter.oid.Name}");

      int startingSP = 5000;
      if (Convert.ToBoolean(CreaturePlugin.GetKnowsFeat(newCharacter.oid, (int)Feat.QuickToMaster)))
        startingSP += 500;

      ObjectPlugin.SetInt(newCharacter.oid, "_STARTING_SKILL_POINTS", startingSP, 1);

      NwArea arrivalArea;
      NwWaypoint arrivalPoint = null;

      if (Config.env == Config.Env.Prod || Config.env == Config.Env.Chim)
      {
        arrivalArea = NwArea.Create("intro_galere", $"entry_scene_{newCharacter.oid.CDKey}", $"La galère de {newCharacter.oid.Name} (Bienvenue !)");
        arrivalArea.OnExit += AreaSystem.OnAreaExit;
        arrivalPoint = arrivalArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT");

        foreach (NwObject fog in arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "intro_brouillard"))
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, fog, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

        NwPlaceable introMirror = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(o => o.Tag == "intro_mirror");
        introMirror.OnUsed += DialogSystem.StartIntroMirrorDialog;
        introMirror.ApplyEffect(EffectDuration.Permanent, API.Effect.CutsceneGhost());

        Task waitDefaultMapLoaded = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => newCharacter.oid.Location.Area != null);
          newCharacter.oid.Location = arrivalPoint.Location;
        });
        
        Task allPointsSpent = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => ObjectPlugin.GetInt(newCharacter.oid, "_STARTING_SKILL_POINTS") <= 0);
          arrivalArea.GetLocalVariable<int>("_GO").Value = 1;
        });
      }
      else
      {
        arrivalArea = newCharacter.oid.Area;
      }

      Utils.DestroyInventory(newCharacter.oid);

      var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"INSERT INTO playerCharacters (accountId , characterName, dateLastSaved, currentSkillType, currentSkillJob, currentCraftJob, currentCraftObject, frostAttackOn, areaTag, position, facing, menuOriginLeft, currentHP) VALUES (@accountId, @name, @dateLastSaved, @currentSkillType, @currentSkillJob, @currentCraftJob, @currentCraftObject, @frostAttackOn, @areaTag, @position, @facing, @menuOriginLeft, @currentHP)");
      NWScript.SqlBindInt(query, "@accountId", newCharacter.accountId);
      NWScript.SqlBindString(query, "@name", NWScript.GetName(newCharacter.oid));
      NWScript.SqlBindString(query, "@dateLastSaved", DateTime.Now.ToString());
      NWScript.SqlBindInt(query, "@currentSkillType", (int)SkillSystem.SkillType.Invalid);
      NWScript.SqlBindInt(query, "@currentSkillJob", (int)Feat.Invalid);
      NWScript.SqlBindInt(query, "@currentCraftJob", -10);
      NWScript.SqlBindString(query, "@currentCraftObject", "");
      NWScript.SqlBindInt(query, "@frostAttackOn", 0);
      NWScript.SqlBindString(query, "@areaTag", NWScript.GetTag(arrivalArea));

      if (arrivalPoint.IsValid)
      {
        NWScript.SqlBindVector(query, "@position", arrivalPoint.Position);
        NWScript.SqlBindFloat(query, "@facing", arrivalPoint.Rotation);
      }
      else
      {
        NWScript.SqlBindVector(query, "@position", NwModule.Instance.StartingLocation.Position);
        NWScript.SqlBindFloat(query, "@facing", NwModule.Instance.StartingLocation.Rotation);
      }

      NWScript.SqlBindInt(query, "@menuOriginLeft", 50);
      NWScript.SqlBindInt(query, "@currentHP", newCharacter.oid.MaxHP);
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
      NWScript.SqlStep(query);

      ObjectPlugin.SetInt(newCharacter.oid, "characterId", NWScript.SqlGetInt(query, 0), 1);

      for (int spellLevel = 0; spellLevel < 10; spellLevel++)
        while (CreaturePlugin.GetKnownSpellCount(newCharacter.oid, 43, spellLevel) > 0)
          CreaturePlugin.RemoveKnownSpell(newCharacter.oid, 43, spellLevel, CreaturePlugin.GetKnownSpell(newCharacter.oid, 43, spellLevel, 0));

      InitializeNewPlayerLearnableSkills(newCharacter);
      InitializeNewCharacterStorage(newCharacter);
    }
    public static void InitializeNewPlayerLearnableSkills(Player player)
    {
      player.learnableSkills.Add((int)Feat.ImprovedHealth, new SkillSystem.Skill((int)Feat.ImprovedHealth, 0.0f, player));
      player.learnableSkills.Add((int)Feat.Toughness, new SkillSystem.Skill((int)Feat.Toughness, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedAttackBonus, new SkillSystem.Skill((int)Feat.ImprovedAttackBonus, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedCasterLevel, new SkillSystem.Skill((int)Feat.ImprovedCasterLevel, 0.0f, player));
      player.learnableSkills.Add((int)Feat.WeaponProficiencySimple, new SkillSystem.Skill((int)Feat.WeaponProficiencySimple, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ArmorProficiencyLight, new SkillSystem.Skill((int)Feat.ArmorProficiencyLight, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ShieldProficiency, new SkillSystem.Skill((int)Feat.ShieldProficiency, 0.0f, player));
      player.learnableSkills.Add((int)Feat.WeaponFinesse, new SkillSystem.Skill((int)Feat.WeaponFinesse, 0.0f, player));
      player.learnableSkills.Add((int)Feat.TwoWeaponFighting, new SkillSystem.Skill((int)Feat.TwoWeaponFighting, 0.0f, player));
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
      player.learnableSkills.Add((int)Feat.ImprovedMoveSilently, new SkillSystem.Skill((int)Feat.ImprovedMoveSilently, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedListen, new SkillSystem.Skill((int)Feat.ImprovedListen, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedHide, new SkillSystem.Skill((int)Feat.ImprovedHide, 0.0f, player));
      player.learnableSkills.Add((int)Feat.ImprovedOpenLock, new SkillSystem.Skill((int)Feat.ImprovedOpenLock, 0.0f, player));
    }
    private static void InitializeDM(Player player)
    {
      player.playerJournal = new PlayerJournal();
    }
    private static void InitializePlayer(Player player)
    {
      InitializePlayerEvents(player.oid);
      InitializePlayerAccount(player);
      InitializePlayerCharacter(player);
      InitializePlayerLearnableSkills(player);
      InitializePlayerLearnableSpells(player);
      InitializeCharacterMapPins(player);
      InitializeCharacterAreaExplorationState(player);
    }
    private static void InitializePlayerEvents(uint player)
    {
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_BROADCAST_CAST_SPELL_AFTER", "a_spellbroadcast", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CAST_SPELL_BEFORE", "b_spellcast", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CAST_SPELL_AFTER", "a_spellcast", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "a_detection", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "a_start_combat", player);
    }
    private static void InitializePlayerAccount(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT bonusRolePlay from PlayerAccounts where rowid = @accountId");
      NWScript.SqlBindInt(query, "@accountId", player.accountId);
      NWScript.SqlStep(query);

      player.bonusRolePlay = NWScript.SqlGetInt(query, 0);
    }
    private static void InitializePlayerCharacter(Player player)
    {
      Log.Info("Initialisation from database");

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT areaTag, position, facing, currentHP, bankGold, dateLastSaved, currentSkillJob, currentCraftJob, currentCraftObject, currentCraftJobRemainingTime, currentCraftJobMaterial, frostAttackOn, menuOriginTop, menuOriginLeft, currentSkillType from playerCharacters where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlStep(query);
      Log.Info($"got current craft job : {NWScript.SqlGetInt(query, 7)}");
      player.playerJournal = new PlayerJournal();
      player.loadedQuickBar = QuickbarType.Invalid;
      player.location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 0), NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2));

      Log.Info($"got location : {player.location.Area}");

      player.currentHP = NWScript.SqlGetInt(query, 3);
      player.bankGold = NWScript.SqlGetInt(query, 4);
      player.dateLastSaved = DateTime.Parse(NWScript.SqlGetString(query, 5));
      player.currentSkillJob = NWScript.SqlGetInt(query, 6);
      player.craftJob = new Job(NWScript.SqlGetInt(query, 7), NWScript.SqlGetString(query, 10), NWScript.SqlGetFloat(query, 9), player, NWScript.SqlGetString(query, 8));
      player.isFrostAttackOn = Convert.ToBoolean(NWScript.SqlGetInt(query, 11));
      player.menu.originTop = NWScript.SqlGetInt(query, 12);
      player.menu.originLeft = NWScript.SqlGetInt(query, 13);
      player.currentSkillType = (SkillSystem.SkillType)NWScript.SqlGetInt(query, 14);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT materialName, materialStock from playerMaterialStorage where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.materialStock.Add(NWScript.SqlGetString(query, 0), NWScript.SqlGetInt(query, 1));
    }
    private static void InitializePlayerLearnableSkills(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT skillId, skillPoints from playerLearnableSkills where characterId = @characterId and trained = 0");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.learnableSkills.Add(NWScript.SqlGetInt(query, 0), new SkillSystem.Skill(NWScript.SqlGetInt(query, 0), NWScript.SqlGetInt(query, 1), player));
    }
    private static void InitializePlayerLearnableSpells(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT skillId, skillPoints from playerLearnableSpells where characterId = @characterId and trained = 0");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.learnableSpells.Add(NWScript.SqlGetInt(query, 0), new SkillSystem.LearnableSpell(NWScript.SqlGetInt(query, 0), NWScript.SqlGetInt(query, 1), player));
    }
    private static void InitializeNewCharacterStorage(Player player)
    {
      NwPlaceable storage = NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == "entrepotpersonnel").FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(s => s.Tag == "ps_entrepot");

      if(storage == null)
      {
        Utils.LogMessageToDMs($"Could not initialize new character storage for player {player.oid.PlayerName}, character {player.oid.Name}");
        return;
      }
      
      Utils.DestroyInventory(storage);
      NwItem oItem = NwItem.Create("bad_armor", storage.Location);
      storage.AcquireItem(oItem);
      oItem = NwItem.Create("bad_club", storage);
      storage.AcquireItem(oItem);
      oItem = NwItem.Create("bad_shield", storage);
      storage.AcquireItem(oItem);
      oItem = NwItem.Create("bad_sling", storage);
      storage.AcquireItem(oItem);
      oItem = NwItem.Create("NW_WAMBU001", storage, 99);
      storage.AcquireItem(oItem);

      storage.Name = $"Entrepôt de {player.oid.Name}";

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", ObjectPlugin.GetInt(player.oid, "characterId"));
      NWScript.SqlBindObject(query, "@storage", storage);
      NWScript.SqlStep(query);
    }
    
    private static void InitializeCharacterMapPins(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT mapPinId, areaTag, x, y, note from playerMapPins where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        MapPin mapPin = new MapPin(NWScript.SqlGetInt(query, 0), NWScript.SqlGetString(query, 1), NWScript.SqlGetFloat(query, 2), NWScript.SqlGetFloat(query, 3), NWScript.SqlGetString(query, 4));
        player.mapPinDictionnary.Add(NWScript.SqlGetInt(query, 0), mapPin);

        NWScript.SetLocalString(player.oid, "NW_MAP_PIN_NTRY_" + mapPin.id.ToString(), mapPin.note);
        NWScript.SetLocalFloat(player.oid, "NW_MAP_PIN_XPOS_" + mapPin.id.ToString(), mapPin.x);
        NWScript.SetLocalFloat(player.oid, "NW_MAP_PIN_YPOS_" + mapPin.id.ToString(), mapPin.y);
        NWScript.SetLocalObject(player.oid, "NW_MAP_PIN_AREA_" + mapPin.id.ToString(), NWScript.GetObjectByTag(mapPin.areaTag));
      }

      if (player.mapPinDictionnary.Count > 0)
        NWScript.SetLocalInt(player.oid, "NW_TOTAL_MAP_PINS", player.mapPinDictionnary.Max(v => v.Key));
    }
    private static void InitializeCharacterAreaExplorationState(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT areaTag, explorationState from playerAreaExplorationState where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.areaExplorationStateDictionnary.Add(NWScript.SqlGetString(query, 0), NWScript.SqlGetString(query, 1));
    }
  }
}
