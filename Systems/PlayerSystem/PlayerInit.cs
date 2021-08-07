using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Discord;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using Anvil.Services;
using NWN.Systems.Craft;
using Color = Anvil.API.Color;
using Newtonsoft.Json;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerConnect(ModuleEvents.OnClientEnter HandlePlayerConnect)
    {
      NwPlayer oPC = HandlePlayerConnect.Player;

      oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_ACTIVE_LANGUAGE").Value = (int)CustomFeats.Invalid;
      oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_CONNECTING").Value = 1;
      oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_PLAYER_INPUT_CANCELLED").Delete();

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
      player.pcState = Player.PcState.Offline;

      if (player.craftJob.IsActive()
      && player.location.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL")?.Value == 0)
      {
        player.CraftJobProgression();
        player.craftJob.CreateCraftJournalEntry();
      }

      if (player.learnables.Any(l => l.Value.active))
      {
        Learnable learnable = player.learnables.First(l => l.Value.active).Value;
        player.AwaitPlayerStateChangeToCalculateSPGain(learnable);

        Task waitForOnlineNotice = NwTask.Run(async () =>
        {
          await NwTask.WaitUntil(() => player.pcState == Player.PcState.Online && player.oid.LoginCreature.Area != null);
          player.CreateSkillJournalEntry(learnable);
        });
      }

      foreach(KeyValuePair<Feat, int> feat in player.learntCustomFeats)
      {
        CustomFeat customFeat = SkillSystem.customFeatsDictionnary[feat.Key];
        FeatTable.Entry featEntry = Feat2da.featTable.GetFeatDataEntry(feat.Key);
        player.oid.SetTlkOverride((int)featEntry.tlkName, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        player.oid.SetTlkOverride((int)featEntry.tlkDescription, customFeat.description);
      }

      int improvedHealth = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedHealth))
        improvedHealth = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedHealth, player.learntCustomFeats[CustomFeats.ImprovedHealth]);

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + Convert.ToInt32(player.oid.LoginCreature.KnowsFeat(Feat.Toughness))) * improvedHealth);
      
      if (oPC.LoginCreature.HP <= 0)
        oPC.LoginCreature.ApplyEffect(EffectDuration.Instant, Effect.Death());

      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedAttackBonus))
        player.oid.LoginCreature.BaseAttackBonus = (byte)(player.oid.LoginCreature.BaseAttackBonus + SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedAttackBonus, player.learntCustomFeats[CustomFeats.ImprovedAttackBonus]));

      if (!player.oid.LoginCreature.KnowsFeat(CustomFeats.Sit))
        player.oid.LoginCreature.AddFeat(CustomFeats.Sit);

      oPC.LoginCreature.GetObjectVariable<LocalVariableInt>("_CONNECTING").Delete();
      player.isAFK = false;
      player.DoJournalUpdate = false;
      player.dateLastSaved = DateTime.Now;

      player.pcState = Player.PcState.Online;

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

        newPlayer.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value = query.Result.GetInt(0);
      }
      else
        newPlayer.LoginCreature.GetObjectVariable<PersistentVariableInt>("accountId").Value = result.Result.GetInt(0);

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

      newCharacter.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value = startingSP;
      newCharacter.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_REINIT_DONE").Value = 1;

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
          
          await NwTask.WaitUntil(() => newCharacter.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value <= 0);
          arrivalArea.GetObjectVariable<LocalVariableInt>("_GO").Value = 1;
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

      newCharacter.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value = rowQuery.Result.GetInt(0);

      for (byte spellLevel = 0; spellLevel < 10; spellLevel++)
        foreach(Spell spell in newCharacter.oid.LoginCreature.GetClassInfo((ClassType)43).GetKnownSpells(spellLevel))
          newCharacter.oid.LoginCreature.GetClassInfo((ClassType)43).RemoveKnownSpell(spellLevel, spell);
      
      InitializeNewPlayerLearnableSkills(newCharacter);
      InitializeNewCharacterStorage(newCharacter);
    }
    public static void InitializeNewPlayerLearnableSkills(Player player)
    {
      player.learnables.Add($"F{(int)CustomFeats.ImprovedHealth}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedHealth, 0.0f, player));
      player.learnables.Add($"F{(int)Feat.Toughness}", new Learnable(LearnableType.Feat, (int)Feat.Toughness, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedAttackBonus}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedAttackBonus, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedCasterLevel}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedCasterLevel, 0.0f, player));
      player.learnables.Add($"F{(int)Feat.WeaponProficiencySimple}", new Learnable(LearnableType.Feat, (int)Feat.WeaponProficiencySimple, 0.0f, player));
      player.learnables.Add($"F{(int)Feat.ArmorProficiencyLight}", new Learnable(LearnableType.Feat, (int)Feat.ArmorProficiencyLight, 0.0f, player));
      player.learnables.Add($"F{(int)Feat.ShieldProficiency}", new Learnable(LearnableType.Feat, (int)Feat.ShieldProficiency, 0.0f, player));
      player.learnables.Add($"F{(int)Feat.WeaponFinesse}", new Learnable(LearnableType.Feat, (int)Feat.WeaponFinesse, 0.0f, player));
      player.learnables.Add($"F{(int)Feat.TwoWeaponFighting}", new Learnable(LearnableType.Feat, (int)Feat.TwoWeaponFighting, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSavingThrowFortitude}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSavingThrowFortitude, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSavingThrowReflex}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSavingThrowReflex, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSavingThrowWill}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSavingThrowWill, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSpellSlot0}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSpellSlot0, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSpellSlot1}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSpellSlot1, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedStrength}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedStrength, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedDexterity}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedDexterity, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedConstitution}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedConstitution, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedIntelligence}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedIntelligence, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedWisdom}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedWisdom, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedCharisma}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedCharisma, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedAnimalEmpathy}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedAnimalEmpathy, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedConcentration}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedConcentration, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedDisableTraps}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedDisableTraps, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedDiscipline}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedDiscipline, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSkillParry}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSkillParry, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedPerform}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedPerform, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedPickpocket}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedPickpocket, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSearch}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSearch, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSetTrap}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSetTrap, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSpellcraft}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSpellcraft, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedSpot}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedSpot, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedTaunt}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedTaunt, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedUseMagicDevice}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedUseMagicDevice, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedTumble}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedTumble, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedBluff}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedBluff, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedIntimidate}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedIntimidate, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedMoveSilently}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedMoveSilently, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedListen}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedListen, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedHide}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedHide, 0.0f, player));
      player.learnables.Add($"F{(int)CustomFeats.ImprovedOpenLock}", new Learnable(LearnableType.Feat, (int)CustomFeats.ImprovedOpenLock, 0.0f, player));
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
      InitializePlayerMutedPM(player);

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
          new List<string>() { { "areaTag" }, { "position" }, { "facing" }, { "currentHP" }, { "bankGold" }, { "dateLastSaved" }, { "currentCraftJob" }, { "currentCraftObject" }, { "currentCraftJobRemainingTime" }, { "currentCraftJobMaterial" }, { "menuOriginTop" }, { "menuOriginLeft" }, { "pveArenaCurrentPoints" }, { "alchemyCauldron" }, { "previousSPCalculation" } },
          new List<string[]>() { { new string[] { "rowid", player.characterId.ToString() } } });

      if (result.Result == null)
        return;

      player.playerJournal = new PlayerJournal();
      player.loadedQuickBar = QuickbarType.Invalid;
      player.location = Utils.GetLocationFromDatabase(result.Result.GetString(0), result.Result.GetString(1), result.Result.GetFloat(2));
      player.oid.LoginCreature.HP = result.Result.GetInt(3);
      player.bankGold = result.Result.GetInt(4);
      player.dateLastSaved = DateTime.Parse(result.Result.GetString(5));
      player.craftJob = new Job(result.Result.GetInt(6), result.Result.GetString(9), result.Result.GetFloat(8), player, result.Result.GetString(7));
      player.menu.originTop = result.Result.GetInt(10);
      player.menu.originLeft = result.Result.GetInt(11);
      player.pveArena.totalPoints = (uint)result.Result.GetInt(12);
      player.alchemyCauldron = JsonConvert.DeserializeObject<Alchemy.Cauldron>(result.Result.GetString(13));
      player.previousSPCalculation = DateTime.TryParse(result.Result.GetString(14), out DateTime previousSPDate) ? previousSPDate : null;

      result = SqLiteUtils.SelectQuery("playerMaterialStorage",
        new List<string>() { { "materialName" }, { "materialStock" } },
        new List<string[]>() { { new string[] { "characterId", player.accountId.ToString() } } });

      foreach(var material in result.Results)
        player.materialStock.Add(material.GetString(0), material.GetInt(1));
    }
    private static void InitializePlayerLearnableSkills(Player player)
    {
      // TEMP REINIT POUR JOUEURS EXISTANTS AVANT LE NOUVEAU SYSTEME DE DONS
      if (player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_REINIT_DONE").HasNothing)
        TempFeatReinit(player);
      else
      {
        var result = SqLiteUtils.SelectQuery("playerLearnableSkills",
          new List<string>() { { "skillId" }, { "skillPoints" }, { "trained" }, { "active" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } } });

        foreach (var skill in result.Results)
        {
          int id = skill.GetInt(0);
          Feat skillId = (Feat)id;
          int currentSkillPoints = skill.GetInt(1);
          bool active = skill.GetInt(3) == 1 ? true : false;

          if (SkillSystem.customFeatsDictionnary.ContainsKey(skillId))
            player.learntCustomFeats.Add(skillId, currentSkillPoints);

          if (skill.GetInt(2) == 0)
            player.learnables.Add($"F{id}", new Learnable(LearnableType.Feat, id, currentSkillPoints, player, active));
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

      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_STARTING_SKILL_POINTS").Value = skillPoints;
      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_REINIT_DONE").Value = 1;
    }
    private static void InitializePlayerLearnableSpells(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerLearnableSpells",
          new List<string>() { { "skillId" }, { "skillPoints" }, { "nbScrolls" }, { "active" } },
          new List<string[]>() { { new string[] { "characterId", player.characterId.ToString() } }, { new string[] { "trained", "0" } } } );

      foreach (var spell in result.Results)
      {
        bool active = spell.GetInt(3) == 1 ? true : false;
        player.learnables.Add($"S{spell.GetInt(0)}", new Learnable(LearnableType.Spell, spell.GetInt(0), spell.GetInt(1), player, active, spell.GetInt(2)));
      }
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
        new List<string[]>() { { new string[] { "rowid", player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value.ToString() } } });

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

          player.oid.LoginCreature.GetObjectVariable<LocalVariableString>($"NW_MAP_PIN_NTRY_{mapPin.id}").Value = mapPin.note;
          player.oid.LoginCreature.GetObjectVariable<LocalVariableFloat>($"NW_MAP_PIN_XPOS_{mapPin.id}").Value = mapPin.x;
          player.oid.LoginCreature.GetObjectVariable<LocalVariableFloat>($"NW_MAP_PIN_YPOS_{mapPin.id}").Value = mapPin.y;
          player.oid.LoginCreature.GetObjectVariable<LocalVariableObject<NwArea>>($"NW_MAP_PIN_AREA_{mapPin.id}").Value = NwObject.FindObjectsWithTag<NwArea>(mapPin.areaTag).FirstOrDefault();
      }

      if (player.mapPinDictionnary.Count > 0)
        player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("NW_TOTAL_MAP_PINS").Value = player.mapPinDictionnary.Max(v => v.Key);
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
        player.chatColors.Add((ChatChannel)color.GetInt(0), new Color(colorConverter[3], colorConverter[2], colorConverter[1], colorConverter[0]));
      }
    }
    private static void InitializePlayerMutedPM(Player player)
    {
      var result = SqLiteUtils.SelectQuery("playerMutedPM",
          new List<string>() { { "mutedAccountId" }},
          new List<string[]>() { { new string[] { "accountId", player.accountId.ToString() } } });

      foreach (var mute in result.Results)
        player.mutedList.Add(mute.GetInt(0));
    }
  }
}
