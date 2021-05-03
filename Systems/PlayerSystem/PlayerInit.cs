using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems.Craft;
using NWNX.API.Events;
using NWNX.Services;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerConnect(ModuleEvents.OnClientEnter HandlePlayerConnect)
    {
      NwPlayer oPC = HandlePlayerConnect.Player;

      oPC.GetLocalVariable<int>("_ACTIVE_LANGUAGE").Value = (int)CustomFeats.Invalid;
      oPC.GetLocalVariable<int>("_CONNECTING").Value = 1;
      oPC.GetLocalVariable<int>("_DISCONNECTING").Delete();

      if (!Players.TryGetValue(oPC, out Player player))
      {
        player = new Player(oPC);
        Players.Add(oPC, player);
      }
      else
      {
        player.oid = oPC;

        if (player.location.Area == null)
        {
          var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT areaTag, position, facing from playerCharacters where rowid = @characterId");
          NWScript.SqlBindInt(query, "@characterId", player.characterId);
          NWScript.SqlStep(query);

          player.location = Utils.GetLocationFromDatabase(NWScript.SqlGetString(query, 0), NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2));

          Task waitAreaLoaded = NwTask.Run(async () =>
          {
            await NwTask.WaitUntil(() => oPC.Location.Area != null);
            oPC.Location = player.location;
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
              if (!Convert.ToBoolean(CreaturePlugin.GetKnowsFeat(player.oid, player.currentSkillJob)))
                CreaturePlugin.AddFeat(player.oid, player.currentSkillJob);
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
        oPC.GetLocalVariable<int>("_CONNECTING").Delete();
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

        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat.Key), out int nameValue))
          PlayerPlugin.SetTlkOverride(player.oid, nameValue, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
          //player.oid.SetTlkOverride(nameValue, $"{customFeat.name} - {SkillSystem.GetCustomFeatLevelFromSkillPoints(feat.Key, feat.Value)}");
        else
          Utils.LogMessageToDMs($"CUSTOM SKILL SYSTEM ERROR - Skill {customFeat.name} : no available custom name StrRef");
      
        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat.Key), out int descriptionValue))
           PlayerPlugin.SetTlkOverride(player.oid, descriptionValue, customFeat.description);
       // player.oid.SetTlkOverride(descriptionValue, customFeat.description);
        else
        {
          Utils.LogMessageToDMs($"CUSTOM SKILL SYSTEM ERROR - Skill {customFeat.name} : no available custom description StrRef");
        }
      }

      int improvedHealth = 0;
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedHealth))
        improvedHealth = SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedHealth, player.learntCustomFeats[CustomFeats.ImprovedHealth]);

      CreaturePlugin.SetMaxHitPointsByLevel(player.oid, 1, Int32.Parse(NWScript.Get2DAString("classes", "HitDie", 43))
        + (1 + 3 * ((player.oid.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + CreaturePlugin.GetKnowsFeat(player.oid, (int)Feat.Toughness)) * improvedHealth);

      if (player.currentHP <= 0)
        oPC.ApplyEffect(EffectDuration.Instant, API.Effect.Death());
      else
        oPC.HP = player.currentHP;

      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedAttackBonus))
        CreaturePlugin.SetBaseAttackBonus(player.oid, player.oid.BaseAttackBonus + SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedAttackBonus, player.learntCustomFeats[CustomFeats.ImprovedAttackBonus]));

      if (!player.oid.KnowsFeat(CustomFeats.Sit))
        player.oid.AddFeat(CustomFeats.Sit);

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

      WaitForPartyChange(oPC);

      oPC.GetLocalVariable<int>("_CONNECTING").Delete();
      player.isAFK = false;
      player.DoJournalUpdate = false;
      player.dateLastSaved = DateTime.Now;

      Log.Info("End of player init.");
    }
    private static void WaitForPartyChange(NwPlayer oPC)
    {
      Task waitForPartyChange = NwTask.Run(async () =>
      {
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task playerGetsDMRights = NwTask.WaitUntil(() => oPC.IsDM, tokenSource.Token);
        Task partyChanged = NwTask.WaitUntilValueChanged(() => oPC.PartyMembers.Count<NwPlayer>(p => !p.IsDM), tokenSource.Token);
        await NwTask.WhenAny(playerGetsDMRights, partyChanged);
        tokenSource.Cancel();

        if (playerGetsDMRights.IsCompletedSuccessfully)
        {
          Task playerLosesDMRights = NwTask.Run(async () =>
          {
            await NwTask.WaitUntil(() => !oPC.IsDM);
            WaitForPartyChange(oPC);
          });
          return;
        }

        Party.HandlePartyChange(oPC);
      });
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
          CreaturePlugin.AddFeat(newPlayer, (int)CustomFeats.Nain);
          break;
        case RacialType.Elf:
        case RacialType.HalfElf:
          CreaturePlugin.AddFeat(newPlayer, (int)CustomFeats.Elfique);
          break;
        case RacialType.Halfling:
          CreaturePlugin.AddFeat(newPlayer, (int)CustomFeats.Halfelin);
          break;
        case RacialType.Gnome:
          CreaturePlugin.AddFeat(newPlayer, (int)CustomFeats.Gnome);
          break;
        case RacialType.HalfOrc:
          CreaturePlugin.AddFeat(newPlayer, (int)CustomFeats.Orc);
          break;
      }

      ObjectPlugin.SetInt(newPlayer, "accountId", NWScript.SqlGetInt(query, 0), 1);
    }
    private static void InitializeNewCharacter(Player newCharacter)
    {
      if (Config.env == Config.Env.Prod)
        (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync($"{newCharacter.oid.PlayerName} vient de créer un nouveau personnage : {newCharacter.oid.Name}");

      int startingSP = 5000;
      if (newCharacter.oid.KnowsFeat(Feat.QuickToMaster))
        startingSP += 500;

      ObjectPlugin.SetInt(newCharacter.oid, "_STARTING_SKILL_POINTS", startingSP, 1);
      ObjectPlugin.SetInt(newCharacter.oid, "_REINIT_DONE", 1, 1);

      NwArea arrivalArea;
      NwWaypoint arrivalPoint = null;

      if (Config.env == Config.Env.Prod || Config.env == Config.Env.Chim)
      {
        arrivalArea = NwArea.Create("intro_galere", $"entry_scene_{newCharacter.oid.CDKey}", $"La galère de {newCharacter.oid.Name} (Bienvenue !)");
        arrivalArea.OnExit += AreaSystem.OnIntroAreaExit;
        arrivalPoint = arrivalArea.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(o => o.Tag == "ENTRY_POINT");

        foreach (NwObject fog in arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "intro_brouillard"))
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, fog, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

        foreach (NwObject recif in arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "intro_recif")) 
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, recif, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);

        NwPlaceable tourbillon = arrivalArea.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(c => c.Tag == "intro_tourbillon");
        VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, tourbillon, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
        VisualTransform transfo = tourbillon.VisualTransform;
        transfo.Translation.Y = 115;
        tourbillon.VisualTransform = transfo;

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

      if (newCharacter.oid.GetItemInSlot(InventorySlot.CreatureSkin) == null)
      {
        NwItem pcSkin = NwItem.Create("peaudejoueur", newCharacter.oid);
        pcSkin.Name = $"Propriétés de {newCharacter.oid.Name}";

        Task waitSkinEquipped = NwTask.Run(async () =>
        {
          await newCharacter.oid.ClearActionQueue();
          await newCharacter.oid.ActionEquipItem(pcSkin, InventorySlot.CreatureSkin);
        });
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"INSERT INTO playerCharacters (accountId , characterName, dateLastSaved, currentSkillType, currentSkillJob, currentCraftJob, currentCraftObject, areaTag, position, facing, menuOriginLeft, currentHP) VALUES (@accountId, @name, @dateLastSaved, @currentSkillType, @currentSkillJob, @currentCraftJob, @currentCraftObject, @areaTag, @position, @facing, @menuOriginLeft, @currentHP)");
      NWScript.SqlBindInt(query, "@accountId", newCharacter.accountId);
      NWScript.SqlBindString(query, "@name", NWScript.GetName(newCharacter.oid));
      NWScript.SqlBindString(query, "@dateLastSaved", DateTime.Now.ToString());
      NWScript.SqlBindInt(query, "@currentSkillType", (int)SkillSystem.SkillType.Invalid);
      NWScript.SqlBindInt(query, "@currentSkillJob", (int)CustomFeats.Invalid);
      NWScript.SqlBindInt(query, "@currentCraftJob", -10);
      NWScript.SqlBindString(query, "@currentCraftObject", "");
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

      eventService.Subscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(player.oid, ItemSystem.OnItemUseBefore)
        .Register<ItemEvents.OnItemUseBefore>();
      eventService.Subscribe<ModuleEvents.OnAcquireItem, GameEventFactory>(player.oid, ItemSystem.OnAcquireItem)
        .Register<ModuleEvents.OnAcquireItem>(NwModule.Instance);
      eventService.Subscribe<ModuleEvents.OnUnacquireItem, GameEventFactory>(player.oid, ItemSystem.OnUnacquireItem)
        .Register<ModuleEvents.OnUnacquireItem>(NwModule.Instance);

      player.oid.OnUseFeat += FeatSystem.OnUseFeatBefore;
      player.oid.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      player.oid.OnExamineObject += ExamineSystem.OnExamineBefore;
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

      switch (player.oid.RacialType)
      {
        case RacialType.Dwarf:
          if(!player.oid.KnowsFeat(CustomFeats.Nain))
            player.oid.AddFeat(CustomFeats.Nain);
          break;
        case RacialType.Elf:
        case RacialType.HalfElf:
          if (!player.oid.KnowsFeat(CustomFeats.Elfique))
            player.oid.AddFeat(CustomFeats.Elfique);
          break;
        case RacialType.Halfling:
          if (!player.oid.KnowsFeat(CustomFeats.Halfelin))
            player.oid.AddFeat(CustomFeats.Halfelin);
          break;
        case RacialType.Gnome:
          if (!player.oid.KnowsFeat(CustomFeats.Gnome))
            player.oid.AddFeat(CustomFeats.Gnome);
          break;
        case RacialType.HalfOrc:
          if (!player.oid.KnowsFeat(CustomFeats.Orc))
            player.oid.AddFeat(CustomFeats.Orc);
          break;
      }
    }
    private static void InitializePlayerEvents(NwPlayer player)
    {
      player.OnServerCharacterSave += HandleBeforePlayerSave;
      
      eventService.Subscribe<ItemEvents.OnItemEquipBefore, NWNXEventFactory>(player, ItemSystem.OnItemEquipBefore)
        .Register<ItemEvents.OnItemEquipBefore>();
      eventService.Subscribe<ItemEvents.OnItemUseBefore, NWNXEventFactory>(player, ItemSystem.OnItemUseBefore)
        .Register<ItemEvents.OnItemUseBefore>();
      eventService.Subscribe<ModuleEvents.OnAcquireItem, GameEventFactory>(player, ItemSystem.OnAcquireItem)
        .Register<ModuleEvents.OnAcquireItem>(NwModule.Instance);
      eventService.Subscribe<ModuleEvents.OnUnacquireItem, GameEventFactory>(player, ItemSystem.OnUnacquireItem)
        .Register<ModuleEvents.OnUnacquireItem>(NwModule.Instance);

      player.OnPlayerDeath += HandlePlayerDeath;
      player.OnUseFeat += FeatSystem.OnUseFeatBefore;
      player.OnSpellCast += SpellSystem.HandleBeforeSpellCast;
      player.OnExamineObject += ExamineSystem.OnExamineBefore;
      player.OnPerception += HandlePlayerPerception;
      player.OnCombatStatusChange += OnCombatStarted;
      player.OnCombatRoundStart += OnCombatRoundStart;
      player.OnSpellBroadcast += SpellSystem.OnSpellBroadcast;
      player.OnSpellAction += SpellSystem.RegisterMetaMagicOnSpellInput;
      player.OnCreatureAttack += AttackSystem.HandleAttackEvent;
      player.OnPhysicalAttacked += AttackSystem.HandlePlayerAttackedEvent;
      player.OnCreatureDamage += AttackSystem.HandleDamageEvent;

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_COMBAT_MODE_OFF", "event_combatmode", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", player);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_DO_LISTEN_DETECTION_AFTER", "a_detection", player);
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

      if (ObjectPlugin.GetInt(player.oid, "_REINIT_DONE") == 0 && player.currentSkillType == SkillSystem.SkillType.Skill)
        player.currentSkillJob = (int)CustomFeats.Invalid;

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT materialName, materialStock from playerMaterialStorage where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        player.materialStock.Add(NWScript.SqlGetString(query, 0), NWScript.SqlGetInt(query, 1));
    }
    private static void InitializePlayerLearnableSkills(Player player)
    {
      // TEMP REINIT POUR JOUEURS EXISTANTS AVANT LE NOUVEAU SYSTEME DE DONS
      if (ObjectPlugin.GetInt(player.oid, "_REINIT_DONE") == 0)
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

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT skillPoints from playerLearnableSkills where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);

      int skillPoints = 0;
      
      for(int i = 0; i < CreaturePlugin.GetFeatCount(player.oid); i++)
      {
        int feat = CreaturePlugin.GetFeatByIndex(player.oid, i);

        if (feat != (int)Feat.KeenSense && feat != (int)Feat.QuickToMaster && feat != (int)Feat.Lucky && feat != (int)Feat.BattleTrainingVersusGiants && feat != (int)Feat.BattleTrainingVersusGoblins && feat != (int)Feat.BattleTrainingVersusOrcs && feat != (int)Feat.BattleTrainingVersusReptilians && feat != (int)Feat.Darkvision && feat != (int)Feat.Lowlightvision && feat != (int)Feat.Fearless && feat != (int)Feat.GoodAim && feat != (int)Feat.HardinessVersusEnchantments && feat != (int)Feat.HardinessVersusIllusions && feat != (int)Feat.HardinessVersusPoisons && feat != (int)Feat.HardinessVersusSpells && feat != (int)Feat.ImmunityToSleep && feat != (int)Feat.PartialSkillAffinityListen && feat != (int)Feat.PartialSkillAffinitySearch && feat != (int)Feat.PartialSkillAffinitySpot && feat != (int)Feat.SkillAffinityConcentration
           && feat != (int)Feat.SkillAffinityListen && feat != (int)Feat.SkillAffinityLore && feat != (int)Feat.SkillAffinityMoveSilently && feat != (int)Feat.SkillAffinitySearch && feat != (int)Feat.SkillAffinitySpot && feat != (int)Feat.Stonecunning && feat != (int)Feat.WeaponProficiencyElf)
        {
          Task waitLoopEndToRemove = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
            CreaturePlugin.RemoveFeat(player.oid, feat);
          });
        }
      }

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
        skillPoints += NWScript.SqlGetInt(query, 0);

      query = NWScript.SqlPrepareQueryCampaign(Config.database, $"DELETE from playerLearnableSkills where characterId = @characterId");
      NWScript.SqlBindInt(query, "@characterId", player.characterId);
      NWScript.SqlStep(query);

      skillPoints += 5000;

      if (player.oid.KnowsFeat(Feat.QuickToMaster))
        skillPoints += 500;

      InitializeNewPlayerLearnableSkills(player);

      ObjectPlugin.SetInt(player.oid, "_STARTING_SKILL_POINTS", skillPoints, 1);
      ObjectPlugin.SetInt(player.oid, "_REINIT_DONE", 1, 1);
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
      NwStore storage = NwStore.Create("generic_shop_res", NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == "entrepotpersonnel").FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(s => s.Tag == "ps_entrepot").Location);

      if(storage == null)
      {
        Utils.LogMessageToDMs($"Could not initialize new character storage for player {player.oid.PlayerName}, character {player.oid.Name}");
        return;
      }
      
      Utils.DestroyInventory(storage);
      NwItem oItem = NwItem.Create("bad_armor", storage);
      oItem = NwItem.Create("bad_club", storage);
      oItem = NwItem.Create("bad_shield", storage);
      oItem = NwItem.Create("bad_sling", storage);
      oItem = NwItem.Create("NW_WAMBU001", storage, 99);

      storage.Name = $"Entrepôt de {player.oid.Name}";

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
      NWScript.SqlBindInt(query, "@characterId", ObjectPlugin.GetInt(player.oid, "characterId"));
      NWScript.SqlBindObject(query, "@storage", storage);
      NWScript.SqlStep(query);

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
    private static void InitializePlayerChatColors(Player player)
    {
      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT channel, color from chatColors where accountId = @accountId");
      NWScript.SqlBindInt(query, "@accountId", player.accountId);

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        byte[] colorConverter = BitConverter.GetBytes(NWScript.SqlGetInt(query, 1));
        player.chatColors.Add(NWScript.SqlGetInt(query, 0), new API.Color(colorConverter[3], colorConverter[2], colorConverter[1], colorConverter[0]));
      }
    }
  }
}
