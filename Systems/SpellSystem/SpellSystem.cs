using NWN.Core;
using NWN.Core.NWNX;
using Anvil.Services;
using Anvil.API;
using System.Linq;
using NLog;
using Anvil.API.Events;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using NWN.Systems.Arena;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private readonly ScriptHandleFactory scriptHandleFactory;
    public int[] lowEnchantements = new int[] { 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554 };
    public int[] mediumEnchantements = new int[] { 555, 556, 557, 558, 559, 560, 561, 562 };
    public int[] highEnchantements = new int[] { 563, 564, 565, 566, 567, 568 };

    public static Effect frog;
    public AreaSystem areaSystem;

    public SpellSystem(ScriptHandleFactory scriptFactory, AreaSystem areaSystem)
    {
      scriptHandleFactory = scriptFactory;
      this.areaSystem = areaSystem;

      InitializeLearnableSpells();
      InitializeCustomEffects();
    }

    private void InitializeCustomEffects()
    {
      frog = CreateCustomEffect("CUSTOM_EFFECT_FROG", ApplyFrogEffectToTarget, RemoveFrogEffectFromTarget, EffectIcon.Curse);

      /*await NwTask.WaitUntil(() => NwModule.Instance.Players.FirstOrDefault()?.LoginCreature?.Area != null);
      ApplyCustomEffectToTarget(frog, NwModule.Instance.Players.FirstOrDefault().LoginCreature, TimeSpan.FromSeconds(10));*/
    }
    private void InitializeLearnableSpells()
    {
      foreach (NwSpell spell in NwRuleset.Spells)
      {
        ClassType castClass;

        int clericCastLevel = spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Cleric)) < 255 ? spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Cleric)) : -1;
        int druidCastLevel = spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Druid)) < 255 ? spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Druid)) : -1;
        int paladinCastLevel = spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Paladin)) < 255 ? spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Paladin)) : -1;
        int rangerCastLevel = spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Ranger)) < 255 ? spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Ranger)) : -1;
        int bardCastLevel = spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Bard)) < 255 ? spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Bard)) : -1;

        Dictionary<ClassType, int> classSorter = new()
        {
          { ClassType.Cleric, clericCastLevel },
          { ClassType.Druid, druidCastLevel },
          { ClassType.Paladin, paladinCastLevel },
          { ClassType.Ranger, rangerCastLevel },
          { ClassType.Bard, bardCastLevel },
        };

        var sortedClass = classSorter.OrderByDescending(c => c.Value);
        castClass = sortedClass.ElementAt(0).Value > -1 ? sortedClass.ElementAt(0).Key : (ClassType)43;

        float level = spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Wizard));
        level = level < 1 ? 0.5f : level;

        SkillSystem.learnableDictionary.Add(spell.Id, new LearnableSpell(spell.Id, spell.Name.Override is null ? spell.Name.ToString() : spell.Name.Override, spell.Description.Override is null ? spell.Description.ToString() : spell.Name.Override, spell.IconResRef, level < 1 ? 1 : (int)level, level < 1 ? 0 : (int)level, castClass == ClassType.Druid || castClass == ClassType.Cleric || castClass == ClassType.Ranger ? Ability.Wisdom : Ability.Intelligence, Ability.Charisma));
      }

      //ReinitStuff();
      //RefoundSkills();
    }
    private static async void RefoundSkills()
    {
      List<string[]> results = new List<string[]>();

      using (var connection = new SqliteConnection("Data Source=/home/chim/aoadatabasebeforereinit.sqlite3"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "select characterId, SUM(skillPoints) from playerLearnableSkills group by characterId";

        using (var reader = await command.ExecuteReaderAsync())
        {
          while (reader.Read())
          {
            string[] row = new string[2];

            for (int i = 0; i < 2; i++)
              row[i] = !reader.IsDBNull(i) ? reader.GetString(i) : "";

            results.Add(row);
          }
        }
      }

      Dictionary<int, int> refundSkillDico = new();

      foreach (var result in results)
      {
        int charId = int.Parse(result[0]);
        int skillPoints = int.Parse(result[1]);

        refundSkillDico.TryAdd(charId, skillPoints);
      }

      foreach (var player in refundSkillDico)
        TempRefundSkillQuery(player.Key.ToString(), player.Value.ToString());
    }
    private static async void ReinitSpells()
    {
      List<string[]> results = new List<string[]>();

      using (var connection = new SqliteConnection("Data Source=/home/chim/aoadatabasebeforereinit.sqlite3"))
      {
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "select characterId, skillId, skillPoints, trained from playerLearnableSpells where trained < 1";

        using (var reader = await command.ExecuteReaderAsync())
        {
          while (reader.Read())
          {
            string[] row = new string[4];

            for (int i = 0; i < 4; i++)
              row[i] = !reader.IsDBNull(i) ? reader.GetString(i) : "";

            results.Add(row);
          }
        }
      }

      Dictionary<int, Dictionary<int, LearnableSpell>> reinitSpellDico = new();

      foreach (var result in results)
      {
        int charId = int.Parse(result[0]);
        int spellId = int.Parse(result[1]);
        LearnableSpell newSpell = (LearnableSpell)SkillSystem.learnableDictionary[spellId];
        LearnableSpell.SerializableLearnableSpell newSerializedSpell = new LearnableSpell.SerializableLearnableSpell(newSpell);
        newSerializedSpell.active = false;
        newSerializedSpell.acquiredPoints = int.Parse(result[2]);
        newSerializedSpell.currentLevel = 0;
        newSerializedSpell.nbScrollUsed = 0;


        reinitSpellDico.TryAdd(charId, new Dictionary<int, LearnableSpell>());
        reinitSpellDico[charId].Add(spellId, new LearnableSpell(newSpell, newSerializedSpell));
      }

      foreach (var player in reinitSpellDico)
      {
        Dictionary<int, LearnableSpell.SerializableLearnableSpell> serializableSpells = new Dictionary<int, LearnableSpell.SerializableLearnableSpell>();
        foreach (var kvp in player.Value)
          serializableSpells.Add(kvp.Key, new LearnableSpell.SerializableLearnableSpell(kvp.Value));

        TempReinitQuery(player.Key.ToString(), JsonConvert.SerializeObject(serializableSpells));
      }
    }
    private static void TempReinitQuery(string charId, string serializedSpells)
    {
      using (var connection = new SqliteConnection("Data Source=/home/chim/aoadatabasebeforereinit.sqlite3"))
      {
        connection.Open();
        string queryString = $"UPDATE playerCharacters SET serializedLearnableSpells = $serializedLearnableSpells where rowId = {charId}";
        var command = connection.CreateCommand();
        command.CommandText = queryString;

        command.Parameters.AddWithValue("$serializedLearnableSpells", serializedSpells);
        command.ExecuteNonQuery();
      }
    }
    private static void TempRefundSkillQuery(string charId, string skillPoints)
    {
      using (var connection = new SqliteConnection("Data Source=/home/chim/aoadatabasebeforereinit.sqlite3"))
      {
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = $"UPDATE playerCharacters SET currentSkillPoints = {skillPoints} where rowId = {charId}";
        command.ExecuteNonQuery();
      }
    }

    private Effect CreateCustomEffect(string tag, Func<CallInfo, ScriptHandleResult> onApply, Func<CallInfo, ScriptHandleResult> onRemoved, EffectIcon icon = EffectIcon.Invalid, Func<CallInfo, ScriptHandleResult> onInterval = null, TimeSpan interval = default, string effectData = "")
    {
      ScriptCallbackHandle applyHandle = scriptHandleFactory.CreateUniqueHandler(onApply);
      ScriptCallbackHandle removeHandle = scriptHandleFactory.CreateUniqueHandler(onRemoved);
      ScriptCallbackHandle intervalHandle = scriptHandleFactory.CreateUniqueHandler(onInterval);

      Effect runAction = Effect.RunAction(applyHandle, removeHandle, intervalHandle, interval, effectData);
      runAction.Tag = tag;
      runAction.SubType = EffectSubType.Supernatural;

      if (icon != EffectIcon.Invalid)
        runAction = Effect.LinkEffects(runAction, Effect.Icon(icon));

      return runAction;
    }


    public void HandleCraftOnSpellInput(OnSpellAction onSpellAction)
    {
      if (onSpellAction.Spell.ImpactScript == "on_ench_cast")
      {
        if (!(PlayerSystem.Players.TryGetValue(onSpellAction.Caster, out PlayerSystem.Player player)) || player.craftJob != null)
        {
          player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
          onSpellAction.PreventSpellCast = true;
          return;
        }

        if (onSpellAction.TargetObject.GetObjectVariable<LocalVariableInt>("_AVAILABLE_ENCHANTEMENT_SLOT").HasNothing)
        {
          player.oid.SendServerMessage($"{onSpellAction.TargetObject.Name} ne dispose d'aucun emplacement d'enchantement disponible !", ColorConstants.Red);
          onSpellAction.PreventSpellCast = true;
          return;
        }
      }
    }
    public void RegisterMetaMagicOnSpellInput(OnSpellAction onSpellAction)
    {
      if (onSpellAction.MetaMagic == MetaMagic.Silent)
        onSpellAction.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").Value = 1;

      onSpellAction.Caster.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").Value = onSpellAction.Spell.Id;
    }
    public void SetCastingClassOnSpellBroadcast(OnSpellBroadcast onSpellBroadcast)
    {
      if (PlayerSystem.Players.TryGetValue(onSpellBroadcast.Caster, out PlayerSystem.Player player))
      {
        ClassType castingClass = SpellUtils.GetCastingClass(onSpellBroadcast.Spell);

        switch (castingClass)
        {
          case ClassType.Druid:

            NwItem armor = onSpellBroadcast.Caster.GetItemInSlot(InventorySlot.Chest);
            NwItem shield = onSpellBroadcast.Caster.GetItemInSlot(InventorySlot.LeftHand);

            if ((armor != null && armor.BaseACValue > 5) || (shield != null && shield.BaseACValue > 1))
            {
              onSpellBroadcast.PreventSpellCast = true;
              player.oid.SendServerMessage("Un si lourd arnachement affaiblit bien trop votre lien à la nature.", ColorConstants.Red);
            }

            break;

          case ClassType.Cleric:
          case ClassType.Paladin:
          case ClassType.Ranger:

            onSpellBroadcast.Caster.BaseArmorArcaneSpellFailure = 0;
            onSpellBroadcast.Caster.BaseShieldArcaneSpellFailure = 0;

            Task resetClassOnNextFrame = NwTask.Run(async () =>
            {
              CancellationTokenSource tokenSource = new CancellationTokenSource();

              Task spellCast = NwTask.WaitUntil(() => !onSpellBroadcast.Caster.IsValid || onSpellBroadcast.Caster.CurrentAction != Anvil.API.Action.CastSpell, tokenSource.Token);

              await NwTask.WhenAny(spellCast);
              tokenSource.Cancel();

              if (!onSpellBroadcast.Caster.IsValid)
                return;

              NwItem shield = onSpellBroadcast.Caster.GetItemInSlot(InventorySlot.LeftHand);
              NwItem armor = onSpellBroadcast.Caster.GetItemInSlot(InventorySlot.Chest);

              if (shield != null)
                onSpellBroadcast.Caster.BaseShieldArcaneSpellFailure = shield.BaseItem.ArcaneSpellFailure;

              if (armor != null)
                onSpellBroadcast.Caster.BaseArmorArcaneSpellFailure = Armor2da.GetArcaneSpellFailure(armor.BaseACValue);
            });

            break;
        }
      }
    }
    public void HandleHearingSpellBroadcast(OnSpellBroadcast onSpellBroadcast)
    {
      if (onSpellBroadcast.Caster.ControllingPlayer.IsDM ||
        !onSpellBroadcast.Caster.ActiveEffects.Any(e => e.EffectType == EffectType.Invisibility || e.EffectType == EffectType.ImprovedInvisibility)
        || onSpellBroadcast.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").HasValue)
      {
        onSpellBroadcast.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").Delete();
        return;
      }

      foreach (NwCreature spotter in onSpellBroadcast.Caster.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.DistanceSquared(onSpellBroadcast.Caster) < 400.0f))
      {
        if (!spotter.IsCreatureSeen(onSpellBroadcast.Caster))
        {
          spotter.ControllingPlayer.SendServerMessage("Quelqu'un d'invisible est en train de lancer un sort à proximité !", ColorConstants.Cyan);
          spotter.ControllingPlayer.ShowVisualEffect(VfxType.FnfLosNormal10, onSpellBroadcast.Caster.Position);
        }
      }
    }

    [ScriptHandler("spellhook")]
    private void HandleSpellHook(CallInfo callInfo)
    {
      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();
      HandleSpellDamageLocalisation(onSpellCast.Spell.SpellType, onSpellCast.Caster);

      if (callInfo.ObjectSelf is not NwCreature castingCreature)
        return;

      if (!(callInfo.ObjectSelf is NwCreature { IsPlayerControlled: true } oPC) || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
      {
        if (castingCreature.Master != null && PlayerSystem.Players.TryGetValue(castingCreature.Master, out PlayerSystem.Player master))
          NWScript.DelayCommand(0.0f, () => DelayedTagAoESummon(castingCreature, master));

        return;
      }

      /*if (onSpellCast.Spell.ImpactScript == "on_ench_cast")
      {
        Enchantement(onSpellCast, player);
        oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
      }*/

      HandleCasterLevel(onSpellCast.Caster, onSpellCast.Spell.SpellType, player);

      switch (onSpellCast.Spell.SpellType)
      {
        case Spell.AcidSplash:
          new AcidSplash(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Daze:
          new Daze(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ElectricJolt:
          new EletricJolt(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Flare:
          new Flare(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Light:
          new Light(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RayOfFrost:
          new RayOfFrost(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Resistance:
          new Resistance(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Virtue:
          new Virtue(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RaiseDead:
        case Spell.Resurrection:
          new RaiseDead(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Invisibility:
          Invisibility(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ImprovedInvisibility:
          ImprovedInvisibility(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      NWScript.DelayCommand(0.0f, () => DelayedTagAoE(player));
    }
    private void HandleCasterLevel(NwGameObject caster, Spell spell, PlayerSystem.Player player)
    {
      if (caster is not NwCreature castingCreature)
        return;

      //CreaturePlugin.SetClassByPosition(castingCreature, 0, 43);
 
      ClassType castingClass = SpellUtils.GetCastingClass(spell);

      if ((int)castingClass == 43 && castingCreature.GetAbilityScore(Ability.Charisma) > castingCreature.GetAbilityScore(Ability.Intelligence))
        castingClass = ClassType.Sorcerer;

      CreaturePlugin.SetClassByPosition(castingCreature, 0, (int)castingClass);
      CreaturePlugin.SetLevelByPosition(castingCreature, 0, player.learnableSkills.ContainsKey(CustomSkill.ImprovedCasterLevel) ? player.learnableSkills[CustomSkill.ImprovedCasterLevel].totalPoints : 1);

      NWScript.DelayCommand(0.0f, () => DelayedSpellHook(castingCreature));
    }
    private void DelayedSpellHook(NwCreature player)
    {
      if (!player.IsValid)
        return;

      CreaturePlugin.SetClassByPosition(player, 0, 43);
      CreaturePlugin.SetLevelByPosition(player, 0, 1);
    }
    private void DelayedTagAoE(PlayerSystem.Player player)
    {
      NwAreaOfEffect aoe = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      if (aoe == null || aoe.GetObjectVariable<LocalVariableBool>("TAGGED").HasValue || aoe.Creator != player.oid.ControlledCreature)
        return;

      aoe.Tag = $"_PLAYER_{player.characterId}";
      aoe.GetObjectVariable<LocalVariableBool>("TAGGED").Value = true;

      if (player.TryGetOpenedWindow("aoeDispel", out PlayerSystem.Player.PlayerWindow aoeWindow))
        ((PlayerSystem.Player.AoEDispelWindow)aoeWindow).UpdateAoEList();
    }
    private void DelayedTagAoESummon(NwCreature castingCreature, PlayerSystem.Player master)
    {
      NwAreaOfEffect aoe = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      if (aoe == null || aoe.GetObjectVariable<LocalVariableBool>("TAGGED").HasValue || aoe.Creator != castingCreature)
        return;
      
      aoe.Tag = $"_PLAYER_{master.characterId}";
      aoe.GetObjectVariable<LocalVariableBool>("TAGGED").Value = true;

      if (master.TryGetOpenedWindow("aoeDispel", out PlayerSystem.Player.PlayerWindow aoeWindow))
        ((PlayerSystem.Player.AoEDispelWindow)aoeWindow).UpdateAoEList();
    }
    public void HandleAutoSpellBeforeSpellCast(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oPC)
        return;

      if (oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").HasValue && oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Value != onSpellCast.Spell.Id)
      {
        oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Delete();
        oPC.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Delete();
        oPC.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
      }
    }
    public void HandleCraftEnchantementCast(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oPC || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player) || onSpellCast.Spell.ImpactScript != "on_ench_cast")
        return;

      Enchantement(onSpellCast, player);
    }

    [ScriptHandler("spell_dcr")]
    private void OnDecreaseSpellCount(CallInfo callInfo)
    {
      if ((MetaMagic)int.Parse(EventsPlugin.GetEventData("METAMAGIC")) != MetaMagic.None)
        return;

      //Log.Info(EventsPlugin.GetEventData("SPELL_ID"));
      //Log.Info(callInfo.ObjectSelf.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").Value);
      //Spell spell = (Spell)int.Parse(EventsPlugin.GetEventData("SPELL_ID"));
      Spell spell = (Spell)callInfo.ObjectSelf.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").Value;
      switch (spell)
      {
        case Spell.AcidSplash:
        case Spell.Daze:
        case Spell.ElectricJolt:
        case Spell.Flare:
        case Spell.Light:
        case Spell.RayOfFrost:
        case Spell.Resistance:
        case Spell.Virtue:
          EventsPlugin.SkipEvent();
          break;
      }
    }
    public void CheckIsDivinationBeforeSpellCast(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster.IsLoginPlayerCharacter(out NwPlayer player) && onSpellCast.Spell.SpellSchool == SpellSchool.Divination)
      {
        onSpellCast.PreventSpellCast = true;
        player.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
        player.SendServerMessage("La Loi même vous empêche de faire appel à ce sort. Un cliquetis lointain de chaînes résonne dans votre esprit. Quelque chose vient de se mettre en mouvement ...", ColorConstants.Red);
        Utils.LogMessageToDMs($"{player.ControlledCreature.Name} ({player.PlayerName}) vient de lancer un sort de divination. Faire intervenir les apôtres pour jugement.");
      }
    }

    public static ScriptHandleResult HandleInvisibiltyHeartBeat(CallInfo callInfo)
    {
      NwAreaOfEffect inviAoE = (NwAreaOfEffect)callInfo.ObjectSelf;

      if (!(inviAoE.Creator is NwCreature { IsPlayerControlled: true } oInvi))
        return ScriptHandleResult.Handled;

      int iMoveSilentlyCheck = oInvi.GetSkillRank(Skill.MoveSilently) + NwRandom.Roll(Utils.random, 20);
      NwPlaceable invisMarker = NwObject.FindObjectsWithTag<NwPlaceable>($"invis_marker_{oInvi.ControllingPlayer.PlayerName}").FirstOrDefault();
      bool listenTriggered = false;

      foreach (NwCreature oSpotter in inviAoE.GetObjectsInEffectArea<NwCreature>().Where(p => p.IsPlayerControlled && p.IsCreatureSeen(oInvi) && (p.DetectModeActive || p.HasFeatEffect(Feat.KeenSense))))
      {
        int iListencheck = oSpotter.GetSkillRank(Skill.Listen) + NwRandom.Roll(Utils.random, 20) - (int)oInvi.Distance(oSpotter);

        if (NWScript.GetDetectMode(oSpotter) == 2)
          iListencheck -= 10;

        if (iMoveSilentlyCheck >= iListencheck)
        {
          if (invisMarker != null)
            oSpotter.ControllingPlayer.SetPersonalVisibilityOverride(invisMarker, VisibilityMode.Hidden);
          continue;
        }

        if (invisMarker == null)
        {
          invisMarker = NwPlaceable.Create("silhouette", oInvi.Location, false, $"invis_marker_{oInvi.ControllingPlayer.PlayerName}");
          invisMarker.VisibilityOverride = VisibilityMode.Hidden;
          OnInvisMarkerPositionChanged(oInvi, invisMarker);
        }

        oSpotter.ControllingPlayer.SetPersonalVisibilityOverride(invisMarker, VisibilityMode.Visible);
        listenTriggered = true;
      }

      if (!listenTriggered && invisMarker != null)
        invisMarker.Destroy();

      return ScriptHandleResult.Handled;
    }
    private static async void OnInvisMarkerPositionChanged(NwCreature oPC, NwPlaceable silhouette)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task markerDestroyed = NwTask.WaitUntil(() => silhouette == null, tokenSource.Token);
      Task playerDisconnecting = NwTask.WaitUntil(() => !oPC.IsValid, tokenSource.Token);
      Task positionChanged = NwTask.WaitUntilValueChanged(() => oPC.Location.Position, tokenSource.Token);

      await NwTask.WhenAny(positionChanged, markerDestroyed, playerDisconnecting);
      tokenSource.Cancel();

      if (markerDestroyed.IsCompletedSuccessfully)
        return;

      if (playerDisconnecting.IsCompletedSuccessfully)
      {
        if (silhouette != null)
          silhouette.Destroy();
        return;
      }

      silhouette.Location = oPC.Location;
      OnInvisMarkerPositionChanged(oPC, silhouette);
    }

    [ScriptHandler("nw_s0_invspha")]
    private void OnInviSphereEnter(CallInfo callInfo)
    {
      if (NWScript.GetEnteringObject().ToNwObject() is NwCreature oTarget)
      {
        Effect eInvis = Effect.Invisibility(InvisibilityType.Normal);
        Effect eVis = Effect.VisualEffect(VfxType.DurInvisibility);
        Effect eDur = Effect.VisualEffect(VfxType.DurCessatePositive);
        Effect eLink = Effect.LinkEffects(eInvis, eVis);
        eLink = Effect.LinkEffects(eLink, eDur);

        eLink = Effect.LinkEffects(eLink, Effect.AreaOfEffect((PersistentVfxType)193, null, scriptHandleFactory.CreateUniqueHandler(HandleInvisibiltyHeartBeat)));  // 193 = AoE 20 m

        NwAreaOfEffect inviSphere = (NwAreaOfEffect)callInfo.ObjectSelf;

        if (inviSphere.Creator is NwCreature oCreator && oTarget.IsFriend(oCreator) && !oTarget.IsDead)
        {
          SpellUtils.SignalEventSpellCast(oTarget, oCreator, Spell.InvisibilitySphere, false);
          oTarget.ApplyEffect(EffectDuration.Permanent, eLink);
        }
      }
    }

    [ScriptHandler("nw_s0_invsphb")]
    private void OnInviSphereExit(CallInfo callInfo)
    {
      if (NWScript.GetExitingObject().ToNwObject() is NwCreature oTarget)
      {
        NwAreaOfEffect inviSphere = (NwAreaOfEffect)callInfo.ObjectSelf;

        foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.EffectType == EffectType.Invisibility && e.Spell.SpellType == Spell.InvisibilitySphere && e.Creator == inviSphere.Creator))
          oTarget.RemoveEffect(eff);
      }
    }

    /*[ScriptHandler("frog_on")]
    private void OnFrogEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Frog.ApplyFrogEffectToTarget(oTarget);
    }

    [ScriptHandler("frog_off")]
    private void OnFrogEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Frog.RemoveFrogEffectFromTarget(oTarget);
    }

    [ScriptHandler("nomagic_on")]
    private void OnNoMagicEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoMagic.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("nomagic_off")]
    private void OnNoMagicEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoMagic.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("noheal_on")]
    private void OnNoHealingMagicEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoHealMagic.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("noheal_off")]
    private void OnNoHealingMagicEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoHealMagic.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("nosummon_on")]
    private void OnNoSummonEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoSummon.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("nosummon_off")]
    private void OnNoSummonEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoSummon.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("nooffmagic_on")]
    private void OnNoOffensiveMagicEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoOffensiveMagic.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("nooffmagic_off")]
    private void OnNoOffensiveMagicEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoOffensiveMagic.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("nobuff_on")]
    private void OnNoDefensiveBuffEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoBuff.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("nobuff_off")]
    private void OnNoDefensiveBuffEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoBuff.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("noarmor_on")]
    private void OnNoArmorEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoArmor.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("noarmor_off")]
    private void OnNoArmorEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoArmor.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("noweapon_on")]
    private void OnNoWeaponEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoWeapon.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("noweapon_off")]
    private void OnNoWeaponEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoWeapon.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("noaccess_on")]
    private void OnNoAccessoryEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoAccessory.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("noaccess_off")]
    private void OnNoAccessoryEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoAccessory.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("noitem_on")]
    private void OnNoUseableItemEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoUseableItem.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("noitem_off")]
    private void OnNoUseableItemEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        NoUseableItem.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("slow_on")]
    private void OnSlowEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Slow.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("slow_off")]
    private void OnSlowItemEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Slow.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("mini_on")]
    private void OnMiniEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Mini.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("mini_off")]
    private void OnMiniEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Mini.RemoveEffectFromTarget(oTarget);
    }

    [ScriptHandler("poison_on")]
    private void OnPoisonEffectApplied(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Poison.ApplyEffectToTarget(oTarget);
    }

    [ScriptHandler("poison_off")]
    private void OnPoisonEffectRemoved(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is NwCreature oTarget)
        Poison.RemoveEffectFromTarget(oTarget);
    }
    */
    public void ApplyGnomeMechAoE(NwCreature oCreature)
    {
      Effect elecAoE = Effect.AreaOfEffect(PersistentVfxType.MobElectrical, scriptHandleFactory.CreateUniqueHandler(HandleMechAuraHeartOnEnter), scriptHandleFactory.CreateUniqueHandler(HandleMechAuraHeartBeat));
      elecAoE.Creator = oCreature;
      elecAoE.Tag = "mechaura_aoe";
      elecAoE.SubType = EffectSubType.Supernatural;
      oCreature.ApplyEffect(EffectDuration.Permanent, elecAoE);
    }

    private static ScriptHandleResult HandleMechAuraHeartOnEnter(CallInfo callInfo)
    {
      NwAreaOfEffect elecAoE = (NwAreaOfEffect)callInfo.ObjectSelf;
      elecAoE.Creator.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value += 5;
      elecAoE.Creator.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));

      if (NWScript.GetEnteringObject().ToNwObject<NwGameObject>() is not NwCreature oTarget || oTarget == elecAoE.Creator)
        return ScriptHandleResult.Handled;

      if (NwRandom.Roll(Utils.random, 100) > elecAoE.Creator.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value + 20)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamLightning, elecAoE.Creator, BodyNode.Chest), TimeSpan.FromSeconds(1.4));

      if (oTarget.GetObjectVariable<LocalVariableInt>("_IS_PVE_ARENA_CREATURE").HasValue)
        oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(1, DamageType.Electrical));
      else
      {
        if (oTarget.RollSavingThrow(SavingThrow.Reflex, 12, SavingThrowType.Electricity, elecAoE.Creator) == SavingThrowResult.Failure)
        {
          oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(1, DamageType.Electrical));
          oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult HandleMechAuraHeartBeat(CallInfo callInfo)
    {
      NwAreaOfEffect elecAoE = (NwAreaOfEffect)callInfo.ObjectSelf;
      elecAoE.Creator.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value += 5;
      elecAoE.Creator.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));

      foreach (NwCreature oTarget in elecAoE.GetObjectsInEffectArea<NwCreature>().Where(c => c != elecAoE.Creator))
      {
        if (NwRandom.Roll(Utils.random, 100) > elecAoE.Creator.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value + 20)
          continue;

        oTarget.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamLightning, elecAoE.Creator, BodyNode.Chest), TimeSpan.FromSeconds(1.4));

        if (oTarget.GetObjectVariable<LocalVariableInt>("_IS_PVE_ARENA_CREATURE").HasValue)
          oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(1, DamageType.Electrical));
        else
        {
          if (oTarget.RollSavingThrow(SavingThrow.Reflex, 12, SavingThrowType.Electricity, elecAoE.Creator) == SavingThrowResult.Failure)
          {
            oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(1, DamageType.Electrical));
            oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));
          }
        }
      }

      return ScriptHandleResult.Handled;
    }
    private void HandleSpellDamageLocalisation(Spell spell, NwGameObject oCaster)
    {
      switch (spell)
      {
        case Spell.AcidFog:
        case Spell.BallLightning:
        case Spell.BladeBarrier:
        case Spell.Cloudkill:
        case Spell.DelayedBlastFireball:
        case Spell.ShadesFireball:
        case Spell.Fireball:
        case Spell.IncendiaryCloud:
        case Spell.ShadesWallOfFire:
        case Spell.WallOfFire:
          oCaster.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").Value = 1;
          break;

        case Spell.IceStorm:
        case Spell.Bombardment:
        case Spell.CallLightning:
        case Spell.EpicRuin:
        case Spell.FireStorm:
        case Spell.FlameStrike:
        case Spell.MeteorSwarm:
        case Spell.SoundBurst:
        case Spell.StormOfVengeance:
        case Spell.Sunburst:
          oCaster.GetObjectVariable<LocalVariableInt>("_SPELL_ATTACK_POSITION").Value = 3;
          break;
      }
    }
    public static async void ApplyCustomEffectToTarget(Effect runAction, NwGameObject target, TimeSpan effectDuration = default)
    {
      await NwModule.Instance.WaitForObjectContext();
      target.ApplyEffect(EffectDuration.Temporary, runAction, effectDuration);
    }
    public void HandlePullRopeChainUse(PlaceableEvents.OnLeftClick onUsed)
    {
      NwPlayer oPC = onUsed.ClickedBy;

      if (PlayerSystem.Players.TryGetValue(oPC.LoginCreature, out PlayerSystem.Player player))
      {
        if (player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.GetObjectVariable<LocalVariableInt>("_IS_PVE_ARENA_CREATURE").HasValue))
          ArenaMenu.DrawRunAwayPage(player);
        else
          ArenaMenu.DrawNextFightPage(player, this);
      }
    }
    public void OnExitArena(AreaEvents.OnExit onExit)
    {
      if (onExit.ExitingObject is not NwCreature creature || !PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;

      AreaSystem.CloseWindows(player);

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
        if (!PlayerSystem.Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out player))
          return;
        else // cas de déconnexion du joueur
          Arena.Utils.ResetPlayerLocation(player);

      if (player.pveArena.currentRound == 0) // S'il s'agit d'un spectateur
      {
        player.oid.LoginCreature.OnSpellCast += HandleAutoSpellBeforeSpellCast;
        player.oid.LoginCreature.OnSpellCast += CheckIsDivinationBeforeSpellCast;
        player.oid.LoginCreature.OnSpellCast -= Arena.Utils.NoMagicMalus;
        return;
      }

      // A partir de là, il s'agit du gladiateur
      player.oid.OnPlayerDeath -= Arena.Utils.HandleArenaDeath;
      player.oid.OnPlayerDeath += PlayerSystem.HandlePlayerDeath;

      player.pveArena.currentPoints = 0;
      player.pveArena.currentRound = 0;

      player.pveArena.currentMalusList.Clear();

      foreach (Effect paralysis in player.oid.LoginCreature.ActiveEffects.Where(e => e.Tag == "_ARENA_CUTSCENE_PARALYZE_EFFECT"))
        player.oid.LoginCreature.RemoveEffect(paralysis);

      foreach (NwCreature spectator in onExit.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled || p.IsLoginPlayerCharacter))
      {
        spectator.ControllingPlayer.SendServerMessage($"La tentative de {player.oid.LoginCreature.Name} s'achève. Vous êtes reconduit à la salle principale.");
        spectator.Location = NwObject.FindObjectsWithTag<NwWaypoint>(Arena.Config.PVE_ENTRY_WAYPOINT_TAG).FirstOrDefault().Location;
      }

      areaSystem.AreaDestroyer(onExit.Area);
    }
  }
}
