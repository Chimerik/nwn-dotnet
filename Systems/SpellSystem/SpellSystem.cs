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
using static NWN.Systems.PlayerSystem;
using NLog.Targets;
using System.Numerics;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private static ScriptHandleFactory scriptHandleFactory;
    public int[] lowEnchantements = new int[] { 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554 };
    public int[] mediumEnchantements = new int[] { 555, 556, 557, 558, 559, 560, 561, 562 };
    public int[] highEnchantements = new int[] { 563, 564, 565, 566, 567, 568 };

    public static Effect frog;
    public AreaSystem areaSystem;

    public SpellSystem(ScriptHandleFactory scriptFactory, AreaSystem areaSystem, Spells2da spells2Da)
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
        int multiplier = level < 1 ? 1 : (int)level + 1;

        SkillSystem.learnableDictionary.Add(spell.Id, new LearnableSpell(spell.Id, spell.Name.Override is null ? spell.Name.ToString() : spell.Name.Override, spell.Description.Override is null ? spell.Description.ToString() : spell.Name.Override, spell.IconResRef, multiplier, castClass == ClassType.Druid || castClass == ClassType.Cleric || castClass == ClassType.Ranger ? Ability.Wisdom : Ability.Intelligence, Ability.Charisma));
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

    private static Effect CreateCustomEffect(string tag, Func<CallInfo, ScriptHandleResult> onApply, Func<CallInfo, ScriptHandleResult> onRemoved, EffectIcon icon = EffectIcon.Invalid, Func<CallInfo, ScriptHandleResult> onInterval = null, TimeSpan interval = default, EffectSubType subType = EffectSubType.Supernatural, string effectData = "")
    {
      ScriptCallbackHandle applyHandle = scriptHandleFactory.CreateUniqueHandler(onApply);
      ScriptCallbackHandle removeHandle = scriptHandleFactory.CreateUniqueHandler(onRemoved);
      ScriptCallbackHandle intervalHandle = scriptHandleFactory.CreateUniqueHandler(onInterval);

      Effect runAction = Effect.RunAction(applyHandle, removeHandle, intervalHandle, interval, effectData);
      runAction.Tag = tag;
      runAction.SubType = subType;

      if (icon != EffectIcon.Invalid)
        runAction = Effect.LinkEffects(runAction, Effect.Icon(icon));

      return runAction;
    }
    public async void HandleSpellInput(OnSpellAction onSpellAction)
    {
      onSpellAction.Caster.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;

      if (!(Players.TryGetValue(onSpellAction.Caster, out Player player)))
        return;

      if (onSpellAction.IsFake || onSpellAction.IsInstant)
        return;

      onSpellAction.PreventSpellCast = true;

      int[] spellCost = SpellUtils.spellCostDictionary[onSpellAction.Spell];
      int energyCost = spellCost[0];
      int remainingCooldown = (int)Math.Round((player.oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>($"_SPELL_COOLDOWN_{onSpellAction.Spell.Id}").Value - DateTime.Now).TotalSeconds, MidpointRounding.ToEven);

      if (remainingCooldown > 0)
      {
        player.oid.DisplayFloatingTextStringOnCreature(player.oid.ControlledCreature, $"{StringUtils.ToWhitecolor(onSpellAction.Spell.Name.ToString())} - Temps restant : {StringUtils.ToWhitecolor(remainingCooldown.ToString())}".ColorString(ColorConstants.Red));
        return;
      }

      int missingEnergy = energyCost - (int)Math.Round(player.endurance.currentMana, MidpointRounding.ToZero);

      if (missingEnergy > 0)
      {
        player.oid.DisplayFloatingTextStringOnCreature(player.oid.ControlledCreature, $"{onSpellAction.Spell.Name.ToString()} - Energie manquante : {missingEnergy.ToString().ColorString(ColorConstants.Red)}");
        return;
      }

      await onSpellAction.Caster.ClearActionQueue();

      player.endurance.currentMana -= energyCost;

      HandleCastTime(onSpellAction);
    }
    private static async void HandleCastTime(OnSpellAction spellAction)
    {
      Location targetLocation = spellAction.IsAreaTarget ? Location.Create(spellAction.Caster.Area, spellAction.TargetPosition, spellAction.Caster.Rotation) : null;
      Vector3 previousPosition = spellAction.Caster.Position;

      if (spellAction.IsAreaTarget)
        await spellAction.Caster.ActionCastFakeSpellAt(spellAction.Spell, targetLocation);
      else
        await spellAction.Caster.ActionCastFakeSpellAt(spellAction.Spell, spellAction.TargetObject);

      foreach (NwPlayer player in NwModule.Instance.Players)
        if (player?.ControlledCreature?.Area == spellAction.Caster?.Area && player.ControlledCreature.IsCreatureHeard(spellAction.Caster))
          player.DisplayFloatingTextStringOnCreature(spellAction.Caster, StringUtils.ToWhitecolor($"{spellAction.Caster.Name.ColorString(ColorConstants.Blue)} incante {spellAction.Spell.Name.ToString().ColorString(ColorConstants.Purple)}"));

      await NwTask.Delay(TimeSpan.FromMilliseconds(spellAction.Spell.ConjureTime.TotalMilliseconds / 1.5));

      if (spellAction.Caster.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").HasValue || (spellAction.TargetObject is null && !spellAction.IsAreaTarget)
        || previousPosition.X != spellAction.Caster.Position.X || previousPosition.Y != spellAction.Caster.Position.Y
        || spellAction.Caster.GetObjectVariable<LocalVariableInt>("_INTERRUPTED").HasValue)
      {
        spellAction.Caster.GetObjectVariable<LocalVariableInt>("_INTERRUPTED").Delete();
        return;
      }

      if (spellAction.IsAreaTarget)
        await spellAction.Caster.ActionCastSpellAt(spellAction.Spell, targetLocation, spellAction.MetaMagic, true, ProjectilePathType.Default, true);
      else
        await spellAction.Caster.ActionCastSpellAt(spellAction.Spell, spellAction.TargetObject, spellAction.MetaMagic, true, 0, ProjectilePathType.Default, true);

      spellAction.Caster.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").Value = spellAction.Spell.Id;
    }
    public void HandleCraftOnSpellInput(OnSpellAction onSpellAction)
    {
      if (onSpellAction.Spell.ImpactScript == "on_ench_cast")
      {
        if (!(Players.TryGetValue(onSpellAction.Caster, out Player player)) || player.craftJob != null)
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

    [ScriptHandler("spellhook")]
    private void HandleSpellHook(CallInfo callInfo)
    {
      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();
      HandleSpellDamageLocalisation(onSpellCast.Spell.SpellType, onSpellCast.Caster);

      if (callInfo.ObjectSelf is not NwCreature castingCreature)
        return;

      if (!(callInfo.ObjectSelf is NwCreature { IsPlayerControlled: true } oPC) || !Players.TryGetValue(oPC, out Player player))
      {
        if (castingCreature.Master != null && Players.TryGetValue(castingCreature.Master, out Player master))
          NWScript.DelayCommand(0.0f, () => DelayedTagAoESummon(castingCreature, master));

        return;
      }

      /*if (onSpellCast.Spell.ImpactScript == "on_ench_cast")
      {
        Enchantement(onSpellCast, player);
        oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
      }*/

      HandleCasterLevel(onSpellCast.Caster, onSpellCast.Spell, player);

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
          RayOfFrost(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Resistance:
          new Resistance(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Virtue:
          HealingBreeze(onSpellCast);
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

      castingCreature.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").Delete();
      NWScript.DelayCommand(0.0f, () => DelayedTagAoE(player));
    }
    private void HandleCasterLevel(NwGameObject caster, NwSpell spell, Player player)
    {
      if (caster is not NwCreature castingCreature)
        return;

      ClassType castingClass = SpellUtils.GetCastingClass(spell);

      if ((int)castingClass == 43 && castingCreature.GetAbilityScore(Ability.Charisma) > castingCreature.GetAbilityScore(Ability.Intelligence))
        castingClass = ClassType.Sorcerer;

      CreaturePlugin.SetClassByPosition(castingCreature, 0, (int)castingClass);
      CreaturePlugin.SetCasterLevelOverride(castingCreature, (int)castingClass, player.learnableSpells[spell.Id].currentLevel);

      if(castingCreature.IsLoginPlayerCharacter)
      {
        if(castingCreature.ControllingPlayer.IsDM && !castingCreature.IsDMPossessed)
          CreaturePlugin.SetCasterLevelOverride(castingCreature, (int)castingClass, 15);
      }
      else if(castingCreature.IsPlayerControlled)
      {
        if(castingCreature.GetObjectVariable<LocalVariableInt>("_CREATURE_CASTER_LEVEL").HasValue)
          CreaturePlugin.SetCasterLevelOverride(castingCreature, (int)castingClass, castingCreature.GetObjectVariable<LocalVariableInt>("_CREATURE_CASTER_LEVEL").Value);
        else
          CreaturePlugin.SetCasterLevelOverride(castingCreature, (int)castingClass, (int)castingCreature.ChallengeRating);
      }




      NWScript.DelayCommand(0.0f, () => DelayedSpellHook(castingCreature, spell, player));
    }
    private void DelayedSpellHook(NwCreature caster, NwSpell spell, Player player)
    {
      if (!caster.IsValid)
        return;

      CreaturePlugin.SetClassByPosition(caster, 0, 43);

      foreach (var spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spell.InnateSpellLevel))
        if (spellSlot.Spell == spell)
          spellSlot.IsReady = false;

      int cooldown = SpellUtils.spellCostDictionary[spell][1];

      StartSpellCooldown(caster, spell, cooldown, player);
      WaitCooldownToRestoreSpell(caster, spell, cooldown);
    }
    private void DelayedTagAoE(Player player)
    {
      NwAreaOfEffect aoe = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      if (aoe == null || aoe.GetObjectVariable<LocalVariableBool>("TAGGED").HasValue || aoe.Creator != player.oid.ControlledCreature)
        return;

      aoe.Tag = $"_PLAYER_{player.characterId}";
      aoe.GetObjectVariable<LocalVariableBool>("TAGGED").Value = true;

      if (player.TryGetOpenedWindow("aoeDispel", out Player.PlayerWindow aoeWindow))
        ((Player.AoEDispelWindow)aoeWindow).UpdateAoEList();
    }
    private void DelayedTagAoESummon(NwCreature castingCreature, Player master)
    {
      NwAreaOfEffect aoe = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      if (aoe == null || aoe.GetObjectVariable<LocalVariableBool>("TAGGED").HasValue || aoe.Creator != castingCreature)
        return;
      
      aoe.Tag = $"_PLAYER_{master.characterId}";
      aoe.GetObjectVariable<LocalVariableBool>("TAGGED").Value = true;

      if (master.TryGetOpenedWindow("aoeDispel", out PlayerSystem.Player.PlayerWindow aoeWindow))
        ((Player.AoEDispelWindow)aoeWindow).UpdateAoEList();
    }
    public void HandleCraftEnchantementCast(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oPC || !Players.TryGetValue(oPC, out Player player) || onSpellCast.Spell.ImpactScript != "on_ench_cast")
        return;

      Enchantement(onSpellCast, player);
    }
    private void StartSpellCooldown(NwCreature caster, NwSpell spell, int cooldown, Player player)
    {
      byte slotId;

      for (slotId = 0; slotId < 13; slotId++)
      {
        var slot = caster.GetQuickBarButton(slotId);

        if (slot.ObjectType == QuickBarButtonType.Spell && slot.Param1 == spell.Id)
          break;
      }

      if (slotId > 11)
        return;

      Color color;

      if (player.chatColors.TryGetValue(102, out byte[] colorArray))
        color = new(colorArray[0], colorArray[1], colorArray[2], colorArray[3]);
      else
        color = ColorConstants.Red;

      DecreaseSpellCooldown(caster, spell.Id, cooldown, GetUIScaledPosition(caster.ControllingPlayer.GetDeviceProperty(PlayerDeviceProperty.GuiScale), slotId), color);
    }
    private async void DecreaseSpellCooldown(NwCreature caster, int spell, int cooldown, int xPos, Color color)
    {
      if (!caster.IsValid)
        return;

      caster.LoginPlayer.PostString(cooldown.ToString(), xPos, 100, ScreenAnchor.TopLeft, cooldown, color, color, spell);

      await NwTask.Delay(TimeSpan.FromSeconds(1));
      cooldown--;
      
      if(cooldown > 0)
        DecreaseSpellCooldown(caster, spell, cooldown, xPos, color);
    }
    private static int GetUIScaledPosition(int uiScale, int slotId)
    {
      return uiScale switch
      {
        110 => slotId switch
        {
          1 => 51,
          2 => 56,
          3 => 64,
          4 => 71,
          5 => 77,
          6 => 84,
          7 => 91,
          8 => 97,
          9 => 104,
          10 => 111,
          11 => 117,
          _ => 44,
        },
        120 => slotId switch
        {
          1 => 44,
          2 => 50,
          3 => 57,
          4 => 64,
          5 => 70,
          6 => 77,
          7 => 84,
          8 => 90,
          9 => 97,
          10 => 104,
          11 => 110,
          _ => 37,
        },
        130 => slotId switch
        {
          1 => 38,
          2 => 44,
          3 => 51,
          4 => 58,
          5 => 65,
          6 => 71,
          7 => 78,
          8 => 84,
          9 => 91,
          10 => 98,
          11 => 104,
          _ => 31,
        },
        140 => slotId switch
        {
          1 => 33,
          2 => 39,
          3 => 46,
          4 => 53,
          5 => 60,
          6 => 66,
          7 => 73,
          8 => 79,
          9 => 86,
          10 => 93,
          11 => 99,
          _ => 26,
        },
        150 => slotId switch
        {
          1 => 29,
          2 => 35,
          3 => 42,
          4 => 48,
          5 => 55,
          6 => 62,
          7 => 68,
          8 => 75,
          9 => 82,
          10 => 88,
          11 => 95,
          _ => 22,
        },
        _ => slotId switch
        {
          1 => 59,
          2 => 65,
          3 => 72,
          4 => 79,
          5 => 86,
          6 => 92,
          7 => 99,
          8 => 106,
          9 => 112,
          10 => 119,
          11 => 125,
          _ => 52,
        },
      };
    }
    private async void WaitCooldownToRestoreSpell(NwCreature caster, NwSpell spell, int cooldown)
    {
      caster.GetObjectVariable<DateTimeLocalVariable>($"_SPELL_COOLDOWN_{spell.Id}").Value = DateTime.Now.AddSeconds(cooldown);

      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task playerLeft = NwTask.WaitUntil(() => !caster.IsValid, tokenSource.Token);
      Task cooledDown = NwTask.Delay(TimeSpan.FromSeconds(cooldown), tokenSource.Token);

      await NwTask.WhenAny(playerLeft, cooledDown);
      tokenSource.Cancel();

      if (playerLeft.IsCompletedSuccessfully || !caster.IsValid)
        return;

      RestoreSpell(caster, spell);
    }
    public void RestoreSpell(NwCreature caster, NwSpell spell)
    {
      foreach (var spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(spell.InnateSpellLevel))
        if (spellSlot.Spell == spell)
          spellSlot.IsReady = true;

      caster.GetObjectVariable<DateTimeLocalVariable>($"_SPELL_COOLDOWN_{spell.Id}").Delete();
    }
    public void CheckIsDivinationBeforeSpellCast(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster.IsLoginPlayerCharacter(out NwPlayer player) && onSpellCast.Spell.SpellSchool == SpellSchool.Divination)
      {
        onSpellCast.PreventSpellCast = true;
        player.ControlledCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwkill));
        player.SendServerMessage("La Loi même vous empêche de faire appel à ce sort. Un cliquetis lointain de chaînes résonne dans votre esprit. Quelque chose vient de se mettre en mouvement ...", ColorConstants.Red);
        LogUtils.LogMessage($"{player.ControlledCreature.Name} ({player.PlayerName}) vient de lancer un sort de divination. Faire intervenir les apôtres pour jugement.", LogUtils.LogType.ModuleAdministration);
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
    private static void HandleSpellDamageLocalisation(Spell spell, NwGameObject oCaster)
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
