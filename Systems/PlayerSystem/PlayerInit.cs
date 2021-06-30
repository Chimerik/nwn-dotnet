using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Discord;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems.Craft;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerConnect(ModuleEvents.OnClientEnter HandlePlayerConnect)
    {
      NwPlayer oPC = HandlePlayerConnect.Player;

      oPC.LoginCreature.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value = (int)CustomFeats.Invalid;
      oPC.LoginCreature.GetLocalVariable<int>("_CONNECTING").Value = 1;
      oPC.LoginCreature.GetLocalVariable<int>("_DISCONNECTING").Delete();
      oPC.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();

      if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
      {
        player = new Player(oPC);
        Players.Add(oPC.LoginCreature, player);
      }
      else
      {
        player.oid = oPC;

        if (player.location.Area == null)
        {
          var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT areaTag, position, facing from playerCharacters where rowid = @characterId");
          query.BindParam("@characterId", player.characterId);
          query.Execute();
          player.location = Utils.GetLocationFromDatabase(query.Result.GetString(0), query.Result.GetVector3(1), query.Result.GetFloat(2));

          Task waitAreaLoaded = NwTask.Run(async () =>
          {
            await NwTask.WaitUntil(() => oPC.LoginCreature.Location.Area != null);
            oPC.LoginCreature.Location = player.location;
          });
        }
      }
      
      if (oPC.IsDM)
        return;

      string pcAccount = player.CheckDBPlayerAccount();
      if (pcAccount != oPC.PlayerName)
      {
        oPC.BootPlayer($"Attention - Ce personnage est enregistré sous le compte {pcAccount}, or vous venez de vous connecter sous {oPC.PlayerName}, veuillez vous reconnecter avec le bon compte !");
        Utils.LogMessageToDMs($"Attention - {oPC.PlayerName} vient de se connecter avec un personnage enregistré sous le compte : {pcAccount} !");
        return;
      }

      if (player.craftJob.IsActive()
      && player.location.Area.GetLocalVariable<int>("_AREA_LEVEL")?.Value == 0)
      {
        player.CraftJobProgression();
        player.craftJob.CreateCraftJournalEntry();
      }

      if (player.currentSkillJob != (int)CustomFeats.Invalid)
      {
        switch (player.currentSkillType)
        {
          case SkillSystem.SkillType.Skill:
            if (player.learnableSkills.ContainsKey((Feat)player.currentSkillJob))
              player.learnableSkills[(Feat)player.currentSkillJob].currentJob = true;
            else
            {
              if (!player.oid.LoginCreature.KnowsFeat((Feat)player.currentSkillJob))
                player.oid.LoginCreature.AddFeat((Feat)player.currentSkillJob);
              player.currentSkillJob = (int)CustomFeats.Invalid;
            }
            break;
          case SkillSystem.SkillType.Spell:
            if (player.learnableSpells.ContainsKey(player.currentSkillJob))
              player.learnableSpells[player.currentSkillJob].currentJob = true;
            else
              player.currentSkillJob = (int)CustomFeats.Invalid;
            break;
        }

        player.AcquireSkillPoints();
        oPC.LoginCreature.GetLocalVariable<int>("_CONNECTING").Delete();
        player.isAFK = false;

        if (player.currentSkillJob != (int)CustomFeats.Invalid)
        {
          switch (player.currentSkillType)
          {
            case SkillSystem.SkillType.Skill:
              player.learnableSkills[(Feat)player.currentSkillJob].CreateSkillJournalEntry();
              break;
            case SkillSystem.SkillType.Spell:
              player.learnableSpells[player.currentSkillJob].CreateSkillJournalEntry();
              break;
          }
        }
      }

      foreach(KeyValuePair<Feat, int> feat in player.learntCustomFeats)
      {
        CustomFeat customFeat = SkillSystem.customFeatsDictionnary[feat.Key];
        FeatTable.Entry featEntry = Feat2da.featTable.GetFeatDataEntry(feat.Key);
        PlayerPlugin.SetTlkOverride(player.oid.LoginCreature, featEntry.tlkName, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        PlayerPlugin.SetTlkOverride(player.oid.LoginCreature, featEntry.tlkDescription, customFeat.description);
        //player.oid.SetTlkOverride(nameValue, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        //player.oid.SetTlkOverride(descriptionValue, customFeat.description);
      }

      int improvedHealth = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedHealth))
        improvedHealth = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedHealth, player.learntCustomFeats[CustomFeats.ImprovedHealth]);

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + Convert.ToInt32(player.oid.LoginCreature.KnowsFeat(Feat.Toughness))) * improvedHealth);
      
      if (player.currentHP <= 0)
        oPC.LoginCreature.ApplyEffect(EffectDuration.Instant, API.Effect.Death());
      else
        oPC.LoginCreature.HP = player.currentHP;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedAttackBonus))
        player.oid.LoginCreature.BaseAttackBonus = (byte)(player.oid.LoginCreature.BaseAttackBonus + SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedAttackBonus, player.learntCustomFeats[CustomFeats.ImprovedAttackBonus]));

      if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Sit))
        player.oid.LoginCreature.AddFeat(CustomFeats.Sit);

      oPC.LoginCreature.GetLocalVariable<int>("_CONNECTING").Delete();
      player.isAFK = false;
      player.DoJournalUpdate = false;
      player.dateLastSaved = DateTime.Now;

      Task waitForTorilNecklaceChange = NwTask.Run(async () =>
      {
        await NwTask.WaitUntil(() => oPC.LoginCreature == null || oPC.LoginCreature.GetItemInSlot(InventorySlot.Neck)?.Tag != "amulettorillink");

        if (oPC.LoginCreature == null)
          return;

        ItemSystem.OnTorilNecklaceRemoved(oPC);
      });

      player.mapLoadingTime = DateTime.Now;
      Log.Info("End of player init.");
    }
    private static void InitializeNewPlayer(NwPlayer newPlayer)
    {
      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT rowid FROM PlayerAccounts WHERE accountName = @accountName");
      query.BindParam("@accountName", newPlayer.PlayerName);
      query.Execute();

      if(query.Result == null)
      {
        if (Config.env == Config.Env.Prod)
        {
          (Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Toute première connexion de {newPlayer.LoginCreature.Name}. Accueillons le comme il se doit !");
          (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Toute première connexion de {newPlayer.LoginCreature.Name} => nouveau joueur à accueillir !");

          NWScript.DelayCommand(4.0f, () => newPlayer.PostString("a", 40, 15, ScreenAnchor.TopLeft, 0f, API.ColorConstants.White, API.ColorConstants.White, 9999, "fnt_my_gui"));
          EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", newPlayer.LoginCreature);
        }

        query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"INSERT INTO PlayerAccounts (accountName, cdKey, bonusRolePlay) VALUES (@name, @cdKey, @brp)");
        query.BindParam("@brp", 1);
        query.BindParam("@name", newPlayer.PlayerName);
        query.BindParam("@cdKey", newPlayer.CDKey);
        query.Execute();

        query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
        query.Execute();
      }

      switch (newPlayer.LoginCreature.RacialType)
      {
        case RacialType.Dwarf:
          newPlayer.LoginCreature.AddFeat(CustomFeats.Nain);
          break;
        case RacialType.Elf:
        case RacialType.HalfElf:
          newPlayer.LoginCreature.AddFeat(CustomFeats.Elfique);
          break;
        case RacialType.Halfling:
          newPlayer.LoginCreature.AddFeat(CustomFeats.Halfelin);
          break;
        case RacialType.Gnome:
          newPlayer.LoginCreature.AddFeat(CustomFeats.Gnome);
          break;
        case RacialType.HalfOrc:
          newPlayer.LoginCreature.AddFeat(CustomFeats.Orc);
          break;
      }
      
      ObjectPlugin.SetInt(newPlayer.LoginCreature, "accountId", query.Result.GetInt(0), 1);
    }
    private static void InitializeNewCharacter(Player newCharacter)
    {
      if (Config.env == Config.Env.Prod)
      {
        Task waitBotMessage = NwTask.Run(async () =>
        {
          await (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{newCharacter.oid.PlayerName} vient de créer un nouveau personnage : {newCharacter.oid.LoginCreature.Name}");
        });
      }

      int startingSP = 5000;
      if (newCharacter.oid.LoginCreature.KnowsFeat(Feat.QuickToMaster))
        startingSP += 500;

      ObjectPlugin.SetInt(newCharacter.oid.LoginCreature, "_STARTING_SKILL_POINTS", startingSP, 1);
      ObjectPlugin.SetInt(newCharacter.oid.LoginCreature, "_REINIT_DONE", 1, 1);

      NwArea arrivalArea;
      NwWaypoint arrivalPoint = null;

      if (Config.env == Config.Env.Prod || Config.env == Config.Env.Chim)
      {
        arrivalArea = NwArea.Create("intro_galere", $"entry_scene_{newCharacter.oid.CDKey}", $"La galère de {newCharacter.oid.LoginCreature.Name} (Bienvenue !)");
        arrivalArea.OnExit += AreaSystem.OnIntroAreaExit;
        arrivalPoint = arrivalArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT");

        arrivalArea.SetAreaWind(new Vector3(1, 0, 0), 4, 0, 0);

        foreach (NwObject recif in arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "intro_recif")) 
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, recif, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

        NwPlaceable tourbillon = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(c => c.Tag == "intro_tourbillon");
        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, tourbillon, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
        tourbillon.VisualTransform.Translation = new Vector3(tourbillon.VisualTransform.Translation.X, 115, tourbillon.VisualTransform.Translation.Z);

        NwPlaceable introMirror = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(o => o.Tag == "intro_mirror");
        introMirror.OnUsed += DialogSystem.StartIntroMirrorDialog;
        introMirror.ApplyEffect(EffectDuration.Permanent, API.Effect.CutsceneGhost());

        Task waitDefaultMapLoaded = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => newCharacter.oid.LoginCreature.Location.Area != null);
          newCharacter.oid.LoginCreature.Location = arrivalPoint.Location;
        });
        
        Task allPointsSpent = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => ObjectPlugin.GetInt(newCharacter.oid.LoginCreature, "_STARTING_SKILL_POINTS") <= 0);
          arrivalArea.GetLocalVariable<int>("_GO").Value = 1;
        });
      }
      else
      {
        arrivalArea = newCharacter.oid.LoginCreature.Area;
      }

      Utils.DestroyInventory(newCharacter.oid.LoginCreature);

      if (newCharacter.oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin) == null)
      {
        Task waitSkinCreated = NwTask.Run(async () =>
        {
          NwItem pcSkin = await NwItem.Create("peaudejoueur", newCharacter.oid.LoginCreature);
          pcSkin.Name = $"Propriétés de {newCharacter.oid.LoginCreature.Name}";
          newCharacter.oid.LoginCreature.RunEquip(pcSkin, InventorySlot.CreatureSkin);
        });
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerCharacters (accountId , characterName, dateLastSaved, currentSkillType, currentSkillJob, currentCraftJob, currentCraftObject, areaTag, position, facing, menuOriginLeft, currentHP) VALUES (@accountId, @name, @dateLastSaved, @currentSkillType, @currentSkillJob, @currentCraftJob, @currentCraftObject, @areaTag, @position, @facing, @menuOriginLeft, @currentHP)");
      NWScript.SqlBindInt(query, "@accountId", newCharacter.accountId);
      NWScript.SqlBindString(query, "@name", newCharacter.oid.LoginCreature.Name);
      NWScript.SqlBindString(query, "@dateLastSaved", DateTime.Now.ToString());
      NWScript.SqlBindInt(query, "@currentSkillType", (int)SkillSystem.SkillType.Invalid);
      NWScript.SqlBindInt(query, "@currentSkillJob", (int)CustomFeats.Invalid);
      NWScript.SqlBindInt(query, "@currentCraftJob", -10);
      NWScript.SqlBindString(query, "@currentCraftObject", "");
      NWScript.SqlBindString(query, "@areaTag", arrivalArea.Tag);

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
      NWScript.SqlBindInt(query, "@currentHP", newCharacter.oid.LoginCreature.MaxHP);
      NWScript.SqlStep(query);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT last_insert_rowid()");
      NWScript.SqlStep(query);

      ObjectPlugin.SetInt(newCharacter.oid.LoginCreature, "characterId", NWScript.SqlGetInt(query, 0), 1);
      for (byte spellLevel = 0; spellLevel < 10; spellLevel++)
        foreach(Spell spell in newCharacter.oid.LoginCreature.GetClassInfo((ClassType)43).GetKnownSpells(spellLevel))
          newCharacter.oid.LoginCreature.GetClassInfo((ClassType)43).RemoveKnownSpell(spellLevel, spell);
      
      InitializeNewPlayerLearnableSkills(newCharacter);
      InitializeNewCharacterStorage(newCharacter);
    }
    public static void InitializeNewPlayerLearnableSkills(Player player)
    {
      player.learnableSkills.Add(CustomFeats.ImprovedHealth, new SkillSystem.Skill(CustomFeats.ImprovedHealth, 0.0f, player));
      player.learnableSkills.Add(Feat.Toughness, new SkillSystem.Skill(Feat.Toughness, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedAttackBonus, new SkillSystem.Skill(CustomFeats.ImprovedAttackBonus, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedCasterLevel, new SkillSystem.Skill(CustomFeats.ImprovedCasterLevel, 0.0f, player));
      player.learnableSkills.Add(Feat.WeaponProficiencySimple, new SkillSystem.Skill(Feat.WeaponProficiencySimple, 0.0f, player));
      player.learnableSkills.Add(Feat.ArmorProficiencyLight, new SkillSystem.Skill(Feat.ArmorProficiencyLight, 0.0f, player));
      player.learnableSkills.Add(Feat.ShieldProficiency, new SkillSystem.Skill(Feat.ShieldProficiency, 0.0f, player));
      player.learnableSkills.Add(Feat.WeaponFinesse, new SkillSystem.Skill(Feat.WeaponFinesse, 0.0f, player));
      player.learnableSkills.Add(Feat.TwoWeaponFighting, new SkillSystem.Skill(Feat.TwoWeaponFighting, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSavingThrowFortitude, new SkillSystem.Skill(CustomFeats.ImprovedSavingThrowFortitude, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSavingThrowReflex, new SkillSystem.Skill(CustomFeats.ImprovedSavingThrowReflex, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSavingThrowWill, new SkillSystem.Skill(CustomFeats.ImprovedSavingThrowWill, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSpellSlot0, new SkillSystem.Skill(CustomFeats.ImprovedSpellSlot0, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSpellSlot1, new SkillSystem.Skill(CustomFeats.ImprovedSpellSlot1, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedStrength, new SkillSystem.Skill(CustomFeats.ImprovedStrength, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedDexterity, new SkillSystem.Skill(CustomFeats.ImprovedDexterity, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedConstitution, new SkillSystem.Skill(CustomFeats.ImprovedConstitution, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedIntelligence, new SkillSystem.Skill(CustomFeats.ImprovedIntelligence, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedWisdom, new SkillSystem.Skill(CustomFeats.ImprovedWisdom, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedCharisma, new SkillSystem.Skill(CustomFeats.ImprovedCharisma, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedAnimalEmpathy, new SkillSystem.Skill(CustomFeats.ImprovedAnimalEmpathy, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedConcentration, new SkillSystem.Skill(CustomFeats.ImprovedConcentration, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedDisableTraps, new SkillSystem.Skill(CustomFeats.ImprovedDisableTraps, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedDiscipline, new SkillSystem.Skill(CustomFeats.ImprovedDiscipline, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSkillParry, new SkillSystem.Skill(CustomFeats.ImprovedSkillParry, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedPerform, new SkillSystem.Skill(CustomFeats.ImprovedPerform, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedPickpocket, new SkillSystem.Skill(CustomFeats.ImprovedPickpocket, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSearch, new SkillSystem.Skill(CustomFeats.ImprovedSearch, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSetTrap, new SkillSystem.Skill(CustomFeats.ImprovedSetTrap, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSpellcraft, new SkillSystem.Skill(CustomFeats.ImprovedSpellcraft, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedSpot, new SkillSystem.Skill(CustomFeats.ImprovedSpot, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedTaunt, new SkillSystem.Skill(CustomFeats.ImprovedTaunt, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedUseMagicDevice, new SkillSystem.Skill(CustomFeats.ImprovedUseMagicDevice, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedTumble, new SkillSystem.Skill(CustomFeats.ImprovedTumble, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedBluff, new SkillSystem.Skill(CustomFeats.ImprovedBluff, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedIntimidate, new SkillSystem.Skill(CustomFeats.ImprovedIntimidate, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedMoveSilently, new SkillSystem.Skill(CustomFeats.ImprovedMoveSilently, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedListen, new SkillSystem.Skill(CustomFeats.ImprovedListen, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedHide, new SkillSystem.Skill(CustomFeats.ImprovedHide, 0.0f, player));
      player.learnableSkills.Add(CustomFeats.ImprovedOpenLock, new SkillSystem.Skill(CustomFeats.ImprovedOpenLock, 0.0f, player));
    }
    private static void InitializeDM(Player player)
    {
      player.playerJournal = new PlayerJournal();
      player.oid.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireItem;
      player.oid.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquireItem;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnItemEquipBefore;
      player.oid.LoginCreature.OnUseFeat += FeatSystem.OnUseFeatBefore;
      player.oid.LoginCreature.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      player.oid.OnExamineObject += ExamineSystem.OnExamineBefore;

      ItemSystem.feedbackService.AddCombatLogMessageFilter(CombatLogMessage.ComplexAttack, player.oid);
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
      InitializePlayerChatColors(player);

      switch (player.oid.LoginCreature.RacialType)
      {
        case RacialType.Dwarf:
          if(!player.oid.LoginCreature.KnowsFeat(CustomFeats.Nain))
            player.oid.LoginCreature.AddFeat(CustomFeats.Nain);
          break;
        case RacialType.Elf:
        case RacialType.HalfElf:
          if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Elfique))
            player.oid.LoginCreature.AddFeat(CustomFeats.Elfique);
          break;
        case RacialType.Halfling:
          if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Halfelin))
            player.oid.LoginCreature.AddFeat(CustomFeats.Halfelin);
          break;
        case RacialType.Gnome:
          if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Gnome))
            player.oid.LoginCreature.AddFeat(CustomFeats.Gnome);
          break;
        case RacialType.HalfOrc:
          if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Orc))
            player.oid.LoginCreature.AddFeat(CustomFeats.Orc);
          break;
      }
    }
    private static void InitializePlayerEvents(NwPlayer player)
    {
      player.OnServerCharacterSave += HandleBeforePlayerSave;
      player.LoginCreature.OnAcquireItem += ItemSystem.OnAcquireItem;
      player.LoginCreature.OnUnacquireItem += ItemSystem.OnUnacquireItem;
      player.LoginCreature.OnItemEquip += ItemSystem.OnItemEquipBefore;
      player.LoginCreature.OnItemUse += ItemSystem.OnItemUseBefore;
      player.OnPlayerDeath += HandlePlayerDeath;
      player.LoginCreature.OnUseFeat += FeatSystem.OnUseFeatBefore;
      player.LoginCreature.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      player.OnExamineObject += ExamineSystem.OnExamineBefore;
      player.LoginCreature.OnPerception += HandlePlayerPerception;
      player.OnCombatStatusChange += OnCombatStarted;
      player.LoginCreature.OnCombatRoundStart += OnCombatRoundStart;
      player.LoginCreature.OnSpellBroadcast += SpellSystem.OnSpellBroadcast;
      player.LoginCreature.OnSpellAction += SpellSystem.RegisterMetaMagicOnSpellInput;
      player.OnPartyEvent += Party.HandlePartyEvent;
      player.OnClientLevelUpBegin += HandleOnClientLevelUp;
      player.LoginCreature.OnItemValidateEquip += ItemSystem.NoEquipRuinedItem;
      player.LoginCreature.OnItemValidateUse += ItemSystem.NoUseRuinedItem;
      player.LoginCreature.OnCombatModeToggle += HandleCombatModeOff;

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip", player.LoginCreature);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", player.LoginCreature);
    }
    private static void InitializePlayerAccount(Player player)
    {
      var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT bonusRolePlay from PlayerAccounts where rowid = @accountId");
      query.BindParam("@accountId", player.accountId);
      query.Execute();
      player.bonusRolePlay = query.Result.GetInt(0); ;
    }
    private static void InitializePlayerCharacter(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT areaTag, position, facing, currentHP, bankGold, dateLastSaved, currentSkillJob, currentCraftJob, currentCraftObject, currentCraftJobRemainingTime, currentCraftJobMaterial, menuOriginTop, menuOriginLeft, currentSkillType, pveArenaCurrentPoints from playerCharacters where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlStep(query);
      player.playerJournal = new PlayerJournal();
      player.loadedQuickBar = QuickbarType.Invalid;
      player.location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 0), NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2));
      player.currentHP = NWScript.SqlGetInt(query, 3);
      player.bankGold = NWScript.SqlGetInt(query, 4);
      player.dateLastSaved = DateTime.Parse(NWScript.SqlGetString(query, 5));
      player.currentSkillJob = NWScript.SqlGetInt(query, 6);
      player.craftJob = new Job(NWScript.SqlGetInt(query, 7), NWScript.SqlGetString(query, 10), NWScript.SqlGetFloat(query, 9), player, NWScript.SqlGetString(query, 8));
      player.menu.originTop = NWScript.SqlGetInt(query, 11);
      player.menu.originLeft = NWScript.SqlGetInt(query, 12);
      player.currentSkillType = (SkillSystem.SkillType)NWScript.SqlGetInt(query, 13);
      player.pveArena.totalPoints = (uint)NWScript.SqlGetInt(query, 14);

      if (ObjectPlugin.GetInt(player.oid.LoginCreature, "_REINIT_DONE") == 0 && player.currentSkillType == SkillSystem.SkillType.Skill)
        player.currentSkillJob = (int)CustomFeats.Invalid;

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT materialName, materialStock from playerMaterialStorage where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.materialStock.Add(NWScript.SqlGetString(query, 0), NWScript.SqlGetInt(query, 1));
    }
    private static void InitializePlayerLearnableSkills(Player player)
    {
      // TEMP REINIT POUR JOUEURS EXISTANTS AVANT LE NOUVEAU SYSTEME DE DONS
      if (ObjectPlugin.GetInt(player.oid.LoginCreature, "_REINIT_DONE") == 0)
        TempFeatReinit(player);
      else
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT skillId, skillPoints, trained from playerLearnableSkills where characterId = @characterId");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);

        while (Convert.ToBoolean(NWScript.SqlStep(query)))
        {
          Feat skillId = (Feat)NWScript.SqlGetInt(query, 0);
          int currentSkillPoints = NWScript.SqlGetInt(query, 1);

          if (SkillSystem.customFeatsDictionnary.ContainsKey(skillId))
            player.learntCustomFeats.Add(skillId, currentSkillPoints);

          if (NWScript.SqlGetInt(query, 2) == 0)
            player.learnableSkills.Add(skillId, new SkillSystem.Skill(skillId, currentSkillPoints, player));
        }
      }
    }
    private static void TempFeatReinit(Player player)
    {
      Log.Info("starting temp feat reinit");

      foreach (Feat feat in player.oid.LoginCreature.Feats.Where(f => f != Feat.KeenSense && f != Feat.QuickToMaster && f != Feat.Lucky && f != Feat.BattleTrainingVersusGiants && f != Feat.BattleTrainingVersusGoblins && f != Feat.BattleTrainingVersusOrcs && f != Feat.BattleTrainingVersusReptilians && f != Feat.Darkvision && f != Feat.Lowlightvision && f != Feat.Fearless && f != Feat.GoodAim && f != Feat.HardinessVersusEnchantments && f != Feat.HardinessVersusIllusions && f != Feat.HardinessVersusPoisons && f != Feat.HardinessVersusSpells && f != Feat.ImmunityToSleep && f != Feat.PartialSkillAffinityListen && f != Feat.PartialSkillAffinitySearch && f != Feat.PartialSkillAffinitySpot && f != Feat.SkillAffinityConcentration
            && f != Feat.SkillAffinityListen && f != Feat.SkillAffinityLore && f != Feat.SkillAffinityMoveSilently && f != Feat.SkillAffinitySearch && f != Feat.SkillAffinitySpot && f != Feat.Stonecunning && f != Feat.WeaponProficiencyElf))
      {
        Task waitLoopEndToRemove = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
          player.oid.LoginCreature.RemoveFeat(feat);
        });
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT skillPoints from playerLearnableSkills where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      int skillPoints = 0;

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        skillPoints += NWScript.SqlGetInt(query, 0);

      SqLiteUtils.DeletionQuery("playerLearnableSkills", 
        new Dictionary<string, string>() { { "characterId", player.characterId.ToString() } });

      skillPoints += 5000;

      if (player.oid.LoginCreature.KnowsFeat(Feat.QuickToMaster))
        skillPoints += 500;

      InitializeNewPlayerLearnableSkills(player);

      ObjectPlugin.SetInt(player.oid.LoginCreature, "_STARTING_SKILL_POINTS", skillPoints, 1);
      ObjectPlugin.SetInt(player.oid.LoginCreature, "_REINIT_DONE", 1, 1);
    }
    private static void InitializePlayerLearnableSpells(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT skillId, skillPoints, nbScrolls from playerLearnableSpells where characterId = @characterId and trained = 0");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.learnableSpells.Add(NWScript.SqlGetInt(query, 0), new SkillSystem.LearnableSpell(NWScript.SqlGetInt(query, 0), NWScript.SqlGetInt(query, 1), player, NWScript.SqlGetInt(query, 2)));
    }
    private static async void InitializeNewCharacterStorage(Player player)
    {
      NwStore storage = NwStore.Create("generic_shop_res", NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == "entrepotpersonnel").FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(s => s.Tag == "ps_entrepot").Location);

      if(storage == null)
      {
        Utils.LogMessageToDMs($"Could not initialize new character storage for player {player.oid.PlayerName}, character {player.oid.LoginCreature.Name}");
        return;
      }
      
      Utils.DestroyInventory(storage);
      NwItem oItem = await NwItem.Create("bad_armor", storage);
      oItem = await NwItem.Create("bad_club", storage);
      oItem = await NwItem.Create("bad_shield", storage);
      oItem = await NwItem.Create("bad_sling", storage);
      oItem = await NwItem.Create("NW_WAMBU001", storage, 99);

      storage.Name = $"Entrepôt de {player.oid.LoginCreature.Name}";

      SqLiteUtils.UpdateQuery("playerCharacters",
          new Dictionary<string, string>() { { "storage", storage.Serialize().ToBase64EncodedString() } },
          new Dictionary<string, string>() { { "rowid", ObjectPlugin.GetInt(player.oid.LoginCreature, "characterId").ToString() } });

      storage.Destroy();
    }
    
    private static void InitializeCharacterMapPins(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT mapPinId, areaTag, x, y, note from playerMapPins where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        MapPin mapPin = new MapPin(NWScript.SqlGetInt(query, 0), NWScript.SqlGetString(query, 1), NWScript.SqlGetFloat(query, 2), NWScript.SqlGetFloat(query, 3), NWScript.SqlGetString(query, 4));
        player.mapPinDictionnary.Add(NWScript.SqlGetInt(query, 0), mapPin);

        NWScript.SetLocalString(player.oid.LoginCreature, "NW_MAP_PIN_NTRY_" + mapPin.id.ToString(), mapPin.note);
        NWScript.SetLocalFloat(player.oid.LoginCreature, "NW_MAP_PIN_XPOS_" + mapPin.id.ToString(), mapPin.x);
        NWScript.SetLocalFloat(player.oid.LoginCreature, "NW_MAP_PIN_YPOS_" + mapPin.id.ToString(), mapPin.y);
        NWScript.SetLocalObject(player.oid.LoginCreature, "NW_MAP_PIN_AREA_" + mapPin.id.ToString(), NWScript.GetObjectByTag(mapPin.areaTag));
      }

      if (player.mapPinDictionnary.Count > 0)
        NWScript.SetLocalInt(player.oid.LoginCreature, "NW_TOTAL_MAP_PINS", player.mapPinDictionnary.Max(v => v.Key));
    }
    private static void InitializeCharacterAreaExplorationState(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT areaTag, explorationState from playerAreaExplorationState where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.areaExplorationStateDictionnary.Add(NWScript.SqlGetString(query, 0), NWScript.SqlGetString(query, 1).ToByteArray());
    }
    private static void InitializePlayerChatColors(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT channel, color from chatColors where accountId = @accountId");
      NWScript.SqlBindInt(query, "@accountId", player.accountId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        byte[] colorConverter = BitConverter.GetBytes(NWScript.SqlGetInt(query, 1));
        player.chatColors.Add((ChatChannel)NWScript.SqlGetInt(query, 0), new API.Color(colorConverter[3], colorConverter[2], colorConverter[1], colorConverter[0]));
      }
    }
  }
}
