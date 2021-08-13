using NWN.Core;
using NWN.Core.NWNX;
using Anvil.Services;
using Anvil.API;
using System.Linq;
using Discord;
using NLog;
using Anvil.API.Events;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static int[] lowEnchantements = new int[] { 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554 };
    public static int[] mediumEnchantements = new int[] { 555, 556, 557, 558, 559, 560, 561, 562 };
    public static int[] highEnchantements = new int[] { 563, 564, 565, 566, 567, 568 };

    public static void RegisterMetaMagicOnSpellInput(OnSpellAction onSpellAction)
    {
      if (onSpellAction.MetaMagic == MetaMagic.Silent)
        onSpellAction.Caster.GetObjectVariable<LocalVariableInt>("_IS_SILENT_SPELL").Value = 1;
    }
    public static void OnSpellBroadcast(OnSpellBroadcast onSpellBroadcast)
    {
      if (!(onSpellBroadcast.Caster is NwCreature { IsPlayerControlled: true } oPC))
        return;

      ClassType castingClass = Spells2da.spellsTable.GetSpellDataEntry(onSpellBroadcast.Spell).castingClass;

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
    [ScriptHandler("spellhook")]
    private void HandleSpellHook(CallInfo callInfo)
    {
      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();

      HandleSpellDamageLocalisation(onSpellCast.Spell, onSpellCast.Caster);

      if (!(callInfo.ObjectSelf is NwCreature { IsPlayerControlled: true } oPC) || !PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      CreaturePlugin.SetClassByPosition(oPC, 0, 43);
      
      oPC.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_REFLEX").Value = oPC.GetBaseSavingThrow(SavingThrow.Reflex);
      oPC.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_WILL").Value = oPC.GetBaseSavingThrow(SavingThrow.Will);
      oPC.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_FORT").Value = oPC.GetBaseSavingThrow(SavingThrow.Fortitude);
      
      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedCasterLevel))
        CreaturePlugin.SetLevelByPosition(oPC, 0, SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedCasterLevel, player.learntCustomFeats[CustomFeats.ImprovedCasterLevel]) + 1);

      ClassType castingClass = Spells2da.spellsTable.GetSpellDataEntry(onSpellCast.Spell).castingClass;

      if ((int)castingClass == 43 && oPC.GetAbilityScore(Ability.Charisma) > oPC.GetAbilityScore(Ability.Intelligence))
        castingClass = ClassType.Sorcerer;

      CreaturePlugin.SetClassByPosition(oPC, 0, (int)castingClass);

      switch (onSpellCast.Spell)
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
          new Invisibility(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ImprovedInvisibility:
          new ImprovedInvisibility(onSpellCast);
          oPC.GetObjectVariable<LocalVariableInt>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      NWScript.DelayCommand(0.0f, () => DelayedSpellHook(oPC));
    }
    private void DelayedSpellHook(NwCreature player)
    {
      CreaturePlugin.SetLevelByPosition(player, 0, 1);
      CreaturePlugin.SetClassByPosition(player, 0, 43);
      player.SetBaseSavingThrow(SavingThrow.Reflex, (sbyte)player.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_REFLEX").Value);
      player.SetBaseSavingThrow(SavingThrow.Will, (sbyte)player.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_WILL").Value);
      player.SetBaseSavingThrow(SavingThrow.Fortitude, (sbyte)player.GetObjectVariable<LocalVariableInt>("_DELAYED_SPELLHOOK_FORT").Value);
    }
    public static void HandleBeforeSpellCast(OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oPC))
        return;

      if (oPC.Classes.Any(c => (int)c != 43))
        oPC.GetObjectVariable<LocalVariableInt>("_SPELLCAST").Value = 1;

      if (oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").HasValue && oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Value != (int)onSpellCast.Spell)
      {
        oPC.GetObjectVariable<LocalVariableInt>("_AUTO_SPELL").Delete();
        oPC.GetObjectVariable<LocalVariableObject<NwGameObject>>("_AUTO_SPELL_TARGET").Delete();
        oPC.OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
      }

      SpellsTable.Entry entry = Spells2da.spellsTable.GetSpellDataEntry(onSpellCast.Spell);

      if (entry.school == SpellSchool.Divination && oPC.GetItemInSlot(InventorySlot.Neck).Tag != "amulettorillink")
      {
        (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync(
          $"{oPC.Name} " +
          $"vient de lancer un sort de divination ({entry.name})" +
          $" en portant l'amulette de traçage. L'Amiral s'apprête à punir l'impudent !");
      }
    }
    [ScriptHandler("invi_hb")]
    private void HandleInvisibiltyHeartBeat(CallInfo callInfo)
    {
      NwAreaOfEffect inviAoE = (NwAreaOfEffect)callInfo.ObjectSelf;

      if (!(inviAoE.Creator is NwCreature { IsPlayerControlled: true } oInvi))
        return;

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
            VisibilityPlugin.SetVisibilityOverride(oSpotter, invisMarker, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
          continue;
        }

        if (invisMarker == null)
        {
          invisMarker = NwPlaceable.Create("silhouette", oInvi.Location, false, $"invis_marker_{oInvi.ControllingPlayer.PlayerName}");
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, invisMarker, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
          OnInvisMarkerPositionChanged(oInvi, invisMarker);
        }

        VisibilityPlugin.SetVisibilityOverride(oSpotter, invisMarker, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
        listenTriggered = true;
      }

      if (!listenTriggered && invisMarker != null)
        invisMarker.Destroy();
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
        eLink = Effect.LinkEffects(eLink, Effect.AreaOfEffect(193, null, "invi_hb"));  // 193 = AoE 20 m

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

        foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.EffectType == EffectType.Invisibility && e.Spell == Spell.InvisibilitySphere && e.Creator == inviSphere.Creator))
          oTarget.RemoveEffect(eff);
      }
    }

    [ScriptHandler("frog_on")]
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

    [ScriptHandler("mechaura_enter")]
    private void HandleMechAuraHeartOnEnter(CallInfo callInfo)
    {
      NwAreaOfEffect elecAoE = (NwAreaOfEffect)callInfo.ObjectSelf;
      elecAoE.Creator.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value += 5;
      elecAoE.Creator.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitElectrical));

      if (!(NWScript.GetEnteringObject().ToNwObject<NwGameObject>() is NwCreature oTarget) || oTarget == elecAoE.Creator)
        return;

      if (NwRandom.Roll(Utils.random, 100) > elecAoE.Creator.GetObjectVariable<LocalVariableInt>("_SPARK_LEVEL").Value + 20)
        return;

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
    [ScriptHandler("mechaura_hb")]
    private void HandleMechAuraHeartBeat(CallInfo callInfo)
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
  }
}
