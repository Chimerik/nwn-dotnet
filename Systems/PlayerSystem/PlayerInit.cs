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
          var result = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "areaTag" }, { "position" }, { "facing" } },
          new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

          if(result.Result != null)
            player.location = Utils.GetLocationFromDatabase(result.Result.GetString(0), result.Result.GetString(1), result.Result.GetFloat(2));

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

      Utils.ResetVisualTransform(player.oid.ControlledCreature);

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
        //PlayerPlugin.SetTlkOverride(player.oid.LoginCreature, (int)featEntry.tlkName, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        //PlayerPlugin.SetTlkOverride(player.oid.LoginCreature, (int)featEntry.tlkDescription, customFeat.description);
        player.oid.SetTlkOverride((int)featEntry.tlkName, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        player.oid.SetTlkOverride((int)featEntry.tlkDescription, customFeat.description);
      }

      int improvedHealth = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedHealth))
        improvedHealth = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedHealth, player.learntCustomFeats[CustomFeats.ImprovedHealth]);

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + Convert.ToInt32(player.oid.LoginCreature.KnowsFeat(Feat.Toughness))) * improvedHealth);
      
      if (player.currentHP <= 0)
        oPC.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Death());
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
      var result = SqLiteUtils.SelectQuery("PlayerAccounts",
        new List<string>() { { "rowid" } },
        new List<string[]>() { { new string[] { "accountName", newPlayer.PlayerName } } });

      if (result.Result == null)
      {
        if (Config.env == Config.Env.Prod)
        {
          (Bot._client.GetChannel(786218144296468481) as IMessageChannel).SendMessageAsync($"Toute première connexion de {newPlayer.LoginCreature.Name}. Accueillons le comme il se doit !");
          (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{Bot._client.GetGuild(680072044364562528).EveryoneRole.Mention} Toute première connexion de {newPlayer.LoginCreature.Name} => nouveau joueur à accueillir !");

          Task displayWelcomeScroll = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(3));
            newPlayer.PostString("a", 40, 15, ScreenAnchor.TopLeft, 0f, ColorConstants.White, ColorConstants.White, 9999, "fnt_my_gui");
          });

          EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", newPlayer.LoginCreature);
        }

        SqLiteUtils.InsertQuery("PlayerAccounts",
          new List<string[]>() { new string[] { "accountName", newPlayer.PlayerName }, new string[] { "cdKey", newPlayer.CDKey }, new string[] { "bonusRolePlay", "1" } });

        var query = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, $"SELECT last_insert_rowid()");
        query.Execute();

        ObjectPlugin.SetInt(newPlayer.LoginCreature, "accountId", query.Result.GetInt(0), 1);
      }
      else
        ObjectPlugin.SetInt(newPlayer.LoginCreature, "accountId", result.Result.GetInt(0), 1);

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
        introMirror.ApplyEffect(EffectDuration.Permanent, Effect.CutsceneGhost());

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

      string position = "";
      string facing = "";

      if (arrivalPoint.IsValid)
      {
        position = arrivalPoint.Position.ToString();
        facing = arrivalPoint.Rotation.ToString();
      }
      else
      {
        position = NwModule.Instance.StartingLocation.Position.ToString();
        facing = NwModule.Instance.StartingLocation.Rotation.ToString();
      }

      SqLiteUtils.InsertQuery("playerCharacters",
          new List<string[]>() { new string[] { "accountId", newCharacter.accountId.ToString() }, new string[] { "characterName", newCharacter.oid.LoginCreature.Name }, new string[] { "dateLastSaved", DateTime.Now.ToString() }, new string[] { "currentSkillType", ((int)SkillSystem.SkillType.Invalid).ToString() }, new string[] { "currentSkillJob", ((int)CustomFeats.Invalid).ToString() }, new string[] { "currentCraftJob", "-10" }, new string[] { "currentCraftObject", "" }, new string[] { "areaTag", arrivalArea.Tag }, new string[] { "position", position }, new string[] { "facing", facing}, new string[] { "menuOriginLeft", "50" }, new string[] { "currentHP", newCharacter.oid.LoginCreature.MaxHP.ToString() } });
     
      var rowQuery = NwModule.Instance.PrepareCampaignSQLQuery(Config.database, "SELECT last_insert_rowid()");
      rowQuery.Execute();

      ObjectPlugin.SetInt(newCharacter.oid.LoginCreature, "characterId", rowQuery.Result.GetInt(0), 1);
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
      var result = SqLiteUtils.SelectQuery("PlayerAccounts",
          new List<string>() { { "bonusRolePlay" } },
          new List<string[]>() { { new string[] { "rowid", player.accountId.ToString() } } });

      if (result.Result != null)
        player.bonusRolePlay = result.Result.GetInt(0);
    }
    private static void InitializePlayerCharacter(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerCharacters",
          new List<string>() { { "areaTag" }, { "position" }, { "facing" }, { "currentHP" }, { "bankGold" }, { "dateLastSaved" }, { "currentSkillJob" }, { "currentCraftJob" }, { "currentCraftObject" }, { "currentCraftJobRemainingTime" }, { "currentCraftJobMaterial" }, { "menuOriginTop" }, { "menuOriginLeft" }, { "currentSkillType" }, { "pveArenaCurrentPoints" } },
          new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

      if (result.Result == null)
        return;

      player.playerJournal = new PlayerJournal();
      player.loadedQuickBar = QuickbarType.Invalid;
      player.location = Utils.GetLocationFromDatabase(result.Result.GetString(0), result.Result.GetString(1), result.Result.GetFloat(2));
      player.currentHP = result.Result.GetInt(3);
      player.bankGold = result.Result.GetInt(4);
      player.dateLastSaved = DateTime.Parse(result.Result.GetString(5));
      player.currentSkillJob = result.Result.GetInt(6);
      player.craftJob = new Job(result.Result.GetInt(7), result.Result.GetString(10), result.Result.GetFloat(9), player, result.Result.GetString(8));
      player.menu.originTop = result.Result.GetInt(11);
      player.menu.originLeft = result.Result.GetInt(12);
      player.currentSkillType = (SkillSystem.SkillType)result.Result.GetInt(13);
      player.pveArena.totalPoints = (uint)result.Result.GetInt(14);

      if (ObjectPlugin.GetInt(player.oid.LoginCreature, "_REINIT_DONE") == 0 && player.currentSkillType == SkillSystem.SkillType.Skill)
        player.currentSkillJob = (int)CustomFeats.Invalid;

      result = SqLiteUtils.SelectQuery("playerMaterialStorage",
        new List<string>() { { "materialName" }, { "materialStock" } },
        new List<string[]>() { { new string[] { "characterId", player.accountId.ToString() } } });

      foreach(var material in result.Results)
        player.materialStock.Add(material.GetString(0), material.GetInt(1));
    }
    private static void InitializePlayerLearnableSkills(Player player)
    {
      // TEMP REINIT POUR JOUEURS EXISTANTS AVANT LE NOUVEAU SYSTEME DE DONS
      if (ObjectPlugin.GetInt(player.oid.LoginCreature, "_REINIT_DONE") == 0)
        TempFeatReinit(player);
      else
      {
        var result = SqLiteUtils.SelectQuery("playerLearnableSkills",
          new List<string>() { { "skillId" }, { "skillPoints" }, { "trained" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });

        foreach (var skill in result.Results)
        {
          Feat skillId = (Feat)skill.GetInt(0);
          int currentSkillPoints = skill.GetInt(1);

          if (SkillSystem.customFeatsDictionnary.ContainsKey(skillId))
            player.learntCustomFeats.Add(skillId, currentSkillPoints);

          if (skill.GetInt(2) == 0)
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

      var result = SqLiteUtils.SelectQuery("playerLearnableSkills",
          new List<string>() { { "skillPoints" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });

      int skillPoints = 0;

      foreach (var skill in result.Results)
        skillPoints += skill.GetInt(0);

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
      var result = SqLiteUtils.SelectQuery("playerLearnableSpells",
          new List<string>() { { "skillId" }, { "skillPoints" }, { "nbScrolls" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } }, { new string[] { "trained", "0" } } } );

      foreach(var spell in result.Results)
        player.learnableSpells.Add(spell.GetInt(0), new SkillSystem.LearnableSpell(spell.GetInt(0), spell.GetInt(1), player, spell.GetInt(2)));
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
        new List<string[]>() { { new string[] { "storage", storage.Serialize().ToBase64EncodedString() } }, { new string[] { "trained", "0" } } },
        new List<string[]>() { { new string[] { "rowid", ObjectPlugin.GetInt(player.oid.LoginCreature, "characterId").ToString() } } });

      storage.Destroy();
    }
    
    private static void InitializeCharacterMapPins(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerMapPins",
          new List<string>() { { "mapPinId" }, { "areaTag" }, { "x" }, { "y" }, { "note" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });

        foreach(var pin in result.Results)
        {
          MapPin mapPin = new MapPin(pin.GetInt(0), pin.GetString(1), pin.GetFloat(2), pin.GetFloat(3), pin.GetString(4));
          player.mapPinDictionnary.Add(pin.GetInt(0), mapPin);

          player.oid.LoginCreature.GetLocalVariable<string>($"NW_MAP_PIN_NTRY_{mapPin.id}").Value = mapPin.note;
          player.oid.LoginCreature.GetLocalVariable<float>($"NW_MAP_PIN_XPOS_{mapPin.id}").Value = mapPin.x;
          player.oid.LoginCreature.GetLocalVariable<float>($"NW_MAP_PIN_YPOS_{mapPin.id}").Value = mapPin.y;
          player.oid.LoginCreature.GetLocalVariable<NwObject>($"NW_MAP_PIN_AREA_{mapPin.id}").Value = NwObject.FindObjectsWithTag(mapPin.areaTag).FirstOrDefault();
        }

      if (player.mapPinDictionnary.Count > 0)
        player.oid.LoginCreature.GetLocalVariable<int>("NW_TOTAL_MAP_PINS").Value = player.mapPinDictionnary.Max(v => v.Key);
    }
    private static void InitializeCharacterAreaExplorationState(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerAreaExplorationState",
          new List<string>() { { "areaTag" }, { "explorationState" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });

      foreach(var explo in result.Results)
        player.areaExplorationStateDictionnary.Add(explo.GetString(0), explo.GetString(1).ToByteArray());
    }
    private static void InitializePlayerChatColors(Player player)
    {
      var result = SqLiteUtils.SelectQuery("chatColors",
          new List<string>() { { "channel" }, { "color" } },
          new List<string[]>() { { new string[] { "accountId", player.accountId.ToString() } } });

      foreach (var color in result.Results)
      {
        byte[] colorConverter = BitConverter.GetBytes(color.GetInt(1));
        player.chatColors.Add((ChatChannel)color.GetInt(0), new API.Color(colorConverter[3], colorConverter[2], colorConverter[1], colorConverter[0]));
      }
    }
  }
}
