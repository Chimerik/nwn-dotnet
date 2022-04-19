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

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private ScriptHandleFactory scriptHandleFactory;
    public int[] lowEnchantements = new int[] { 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554 };
    public int[] mediumEnchantements = new int[] { 555, 556, 557, 558, 559, 560, 561, 562 };
    public int[] highEnchantements = new int[] { 563, 564, 565, 566, 567, 568 };

    public static Effect frog;
    public AreaSystem areaSystem;

    public SpellSystem(ScriptHandleFactory scriptFactory, AreaSystem areaSystem)
    {
      scriptHandleFactory = scriptFactory;
      this.areaSystem = areaSystem;
      InitializeCustomEffects();
    }

    private async void InitializeCustomEffects()
    {
      frog = CreateCustomEffect("CUSTOM_EFFECT_FROG", ApplyFrogEffectToTarget, RemoveFrogEffectFromTarget, EffectIcon.Curse);

      /*await NwTask.WaitUntil(() => NwModule.Instance.Players.FirstOrDefault()?.LoginCreature?.Area != null);
      ApplyCustomEffectToTarget(frog, NwModule.Instance.Players.FirstOrDefault().LoginCreature, TimeSpan.FromSeconds(10));*/
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

    public void RegisterMetaMagicOnSpellInput(OnSpellAction onSpellAction)
    {
      if(onSpellAction.Spell.ImpactScript == "on_ench_cast")
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

      if (onSpellAction.MetaMagic == MetaMagic.Silent)
        onSpellAction.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").Value = 1;
    }
    public void OnSpellBroadcast(OnSpellBroadcast onSpellBroadcast)
    {
      if (!(onSpellBroadcast.Caster is NwCreature { IsPlayerControlled: true } oPC))
        return;

      ClassType castingClass = GetCastingClass(onSpellBroadcast.Spell);
     
      if (castingClass != (ClassType)43) // 43 = aventurier
      {
        Task resetClassOnNextFrame = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.7));
          CreaturePlugin.SetClassByPosition(oPC, 0, (int)castingClass);
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task spellCast = NwTask.WaitUntil(() => oPC.GetObjectVariable<LocalVariableInt>("_SPELLCAST").HasValue, tokenSource.Token);
          Task timeOut = NwTask.Delay(TimeSpan.FromSeconds(0.1), tokenSource.Token);

          await NwTask.WhenAny(spellCast, timeOut);
          tokenSource.Cancel();

          CreaturePlugin.SetClassByPosition(oPC, 0, 43);
          oPC.GetObjectVariable<LocalVariableInt>("_SPELLCAST").Delete();
        });
      }

      if (oPC.ControllingPlayer.IsDM ||
        !oPC.ActiveEffects.Any(e => e.EffectType == EffectType.Invisibility || e.EffectType == EffectType.ImprovedInvisibility)
        || onSpellBroadcast.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").HasValue)
      {
        onSpellBroadcast.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").Delete();
        return;
      }

      foreach (NwCreature spotter in oPC.Area.FindObjectsOfTypeInArea<NwCreature>().Where(p => p.IsPlayerControlled && p.Distance(oPC) < 20.0f))
      {
        if (!spotter.IsCreatureSeen(oPC))
        {
          spotter.ControllingPlayer.SendServerMessage("Quelqu'un d'invisible est en train de lancer un sort à proximité !", ColorConstants.Cyan);
          spotter.ControllingPlayer.ShowVisualEffect(VfxType.FnfLosNormal10, oPC.Position);
        }
      }
    }

    public static ClassType GetCastingClass(NwSpell Spell)
    {
      byte clericCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Cleric));
      byte druidCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Druid));
      byte paladinCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Paladin));
      byte rangerCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Ranger));
      byte bardCastLevel = Spell.GetSpellLevelForClass(NwClass.FromClassType(ClassType.Bard));

      Dictionary<ClassType, byte> classSorter = new Dictionary<ClassType, byte>()
      {
        { ClassType.Cleric, clericCastLevel },
        { ClassType.Druid, druidCastLevel },
        { ClassType.Paladin, paladinCastLevel },
        { ClassType.Ranger, rangerCastLevel },
        { ClassType.Bard, bardCastLevel },
      };

      try
      {
        ClassType castingClass = classSorter.Where(c => c.Value < 255).Max().Key;
        return castingClass;
      }
      catch (Exception)
      {
        return (ClassType)43;
      }
    }

    [ScriptHandler("spellhook")]
    private void HandleSpellHook(CallInfo callInfo)
    {
      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();
      uint spellId = onSpellCast.Spell.Id;

      HandleSpellDamageLocalisation(onSpellCast.Spell.SpellType, onSpellCast.Caster);

      if (!(callInfo.ObjectSelf is NwCreature { IsPlayerControlled: true } oPC) || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
      {
          if (callInfo.ObjectSelf is NwCreature castingCreature && castingCreature.Master != null && PlayerSystem.Players.TryGetValue(castingCreature.Master, out PlayerSystem.Player master))
            NWScript.DelayCommand(0.0f, () => DelayedTagAoESummon(castingCreature, master, spellId));

          return;
      }

      CreaturePlugin.SetClassByPosition(oPC, 0, 43);
      
      oPC.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_REFLEX").Value = oPC.GetBaseSavingThrow(SavingThrow.Reflex);
      oPC.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_WILL").Value = oPC.GetBaseSavingThrow(SavingThrow.Will);
      oPC.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_FORT").Value = oPC.GetBaseSavingThrow(SavingThrow.Fortitude);
      
      if (player.learnableSkills.ContainsKey(CustomSkill.ImprovedCasterLevel))
        CreaturePlugin.SetLevelByPosition(oPC, 0, player.learnableSkills[CustomSkill.ImprovedCasterLevel].totalPoints);

      ClassType castingClass = GetCastingClass(onSpellCast.Spell.SpellType);

      if ((int)castingClass == 43 && oPC.GetAbilityScore(Ability.Charisma) > oPC.GetAbilityScore(Ability.Intelligence))
        castingClass = ClassType.Sorcerer;

      CreaturePlugin.SetClassByPosition(oPC, 0, (int)castingClass);

      if(onSpellCast.Spell.ImpactScript == "on_ench_cast")
      {
        Enchantement(onSpellCast);
        oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
      }


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

      NWScript.DelayCommand(0.0f, () => DelayedSpellHook(oPC));
      NWScript.DelayCommand(0.0f, () => DelayedTagAoE(player, spellId));
    }
    private void DelayedSpellHook(NwCreature player)
    {
      CreaturePlugin.SetLevelByPosition(player, 0, 1);
      CreaturePlugin.SetClassByPosition(player, 0, 43);
      player.SetBaseSavingThrow(SavingThrow.Reflex, (sbyte)player.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_REFLEX").Value);
      player.SetBaseSavingThrow(SavingThrow.Will, (sbyte)player.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_WILL").Value);
      player.SetBaseSavingThrow(SavingThrow.Fortitude, (sbyte)player.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_FORT").Value);
    }
    private void DelayedTagAoE(PlayerSystem.Player player, uint spellId)
    {
      NwAreaOfEffect aoe = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();
      
      if (aoe == null || aoe.GetObjectVariable<LocalVariableBool>("TAGGED").HasValue || aoe.Creator != player.oid.ControlledCreature)
        return;

      aoe.Tag = player.oid.CDKey;
      aoe.GetObjectVariable<LocalVariableBool>("TAGGED").Value = true;

      if (player.openedWindows.ContainsKey("aoeDispel"))
        ((PlayerSystem.Player.AoEDispelWindow)player.windows["aoeDispel"]).UpdateAoEList();
    }
    private void DelayedTagAoESummon(NwCreature castingCreature, PlayerSystem.Player master, uint spellId)
    {
      NwAreaOfEffect aoe = UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>();

      if (aoe == null || aoe.GetObjectVariable<LocalVariableBool>("TAGGED").HasValue || aoe.Creator != castingCreature)
        return;

      aoe.Tag = master.oid.CDKey;
      aoe.GetObjectVariable<LocalVariableBool>("TAGGED").Value = true;

      if (master.openedWindows.ContainsKey("aoeDispel"))
        ((PlayerSystem.Player.AoEDispelWindow)master.windows["aoeDispel"]).UpdateAoEList();
    }
    public void HandleBeforeSpellCast(OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC))
        return;

      if (oPC.Classes.Any(c => c.Class.Id != 43))
        oPC.GetObjectVariable<LocalVariableInt>("_SPELLCAST").Value = 1;

      if (oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").HasValue && oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Value != onSpellCast.Spell.Id)
      {
        oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Delete();
        oPC.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Delete();
        oPC.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
      }

      if (onSpellCast.Spell.SpellSchool == SpellSchool.Divination)
      {
        onSpellCast.PreventSpellCast = true;
        oPC.ControllingPlayer.SendServerMessage("Un cliquetis lointain de chaînes résonne dans votre esprit. Quelque chose vient de se mettre en mouvement ...");
        Utils.LogMessageToDMs($"{oPC.Name} ({oPC.ControllingPlayer.PlayerName}) vient de lancer un sort de divination. Faire intervenir les apôtres pour jugement.");
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

      if(playerDisconnecting.IsCompletedSuccessfully)
      {
        if(silhouette != null)
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

      if (!(NWScript.GetEnteringObject().ToNwObject<NwGameObject>() is NwCreature oTarget) || oTarget == elecAoE.Creator)
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
      if (!(onExit.ExitingObject is NwCreature creature) || !PlayerSystem.Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
        if (!PlayerSystem.Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out player))
          return;
        else // cas de déconnexion du joueur
          Arena.Utils.ResetPlayerLocation(player);

      if (player.pveArena.currentRound == 0) // S'il s'agit d'un spectateur
      {
        player.oid.LoginCreature.OnSpellCast += HandleBeforeSpellCast;
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
