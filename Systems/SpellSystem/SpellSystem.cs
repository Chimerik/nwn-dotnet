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
using static NWN.Systems.PlayerSystem;

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
        SkillSystem.learnableDictionary.Add(spell.Id, new LearnableSpell(spell.Id, 
          spell.Name.Override is null ? spell.Name.ToString() : spell.Name.Override, 
          spell.Description.Override is null ? spell.Description.ToString() : spell.Name.Override, spell.IconResRef, spell.InnateSpellLevel + 1,
          Ability.Intelligence,
          Ability.Charisma));
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
        runAction = Effect.LinkEffects(runAction, Effect.Icon(NwGameTables.EffectIconTable.GetRow((int)icon)));

      return runAction;
    }
    public void HandleSpellInput(OnSpellAction onSpellAction)
    {
      onSpellAction.Caster.GetObjectVariable<LocalVariableInt>(SpellConfig.CurrentSpellVariable).Value = onSpellAction.Spell.Id;

      //if (!Players.TryGetValue(onSpellAction.Caster, out Player player))
      //return;

      //if (onSpellAction.IsFake || onSpellAction.IsInstant)
      //return;

      //onSpellAction.PreventSpellCast = true;

      //double energyCost = SpellUtils.spellCostDictionary[onSpellAction.Spell][(int)SpellUtils.SpellData.EnergyCost];
      /*int remainingCooldown = (int)Math.Round((player.oid.LoginCreature.GetObjectVariable<DateTimeLocalVariable>($"_SPELL_COOLDOWN_{onSpellAction.Spell.Id}").Value - DateTime.Now).TotalSeconds, MidpointRounding.ToEven);

      if (remainingCooldown > 0)
      {
        player.oid.DisplayFloatingTextStringOnCreature(player.oid.ControlledCreature, $"{StringUtils.ToWhitecolor(onSpellAction.Spell.Name.ToString())} - Temps restant : {StringUtils.ToWhitecolor(remainingCooldown.ToString())}".ColorString(ColorConstants.Red));
        return;
      }*/

      /*if((SkillSystem.Type)SpellUtils.spellCostDictionary[onSpellAction.Spell][(int)SpellUtils.SpellData.Type] == SkillSystem.Type.Enchantement)
      {
        int mysticismLevel = player.GetAttributeLevel(SkillSystem.Attribut.Mysticism);
        energyCost *= mysticismLevel * ((onSpellAction.Caster.GetAbilityScore(Ability.Charisma, true) - 10) / 2) / 100;
      }

      int missingEnergy = (int)(energyCost - Math.Round(player.endurance.currentMana, MidpointRounding.ToZero));

      if (missingEnergy > 0)
      {
        player.oid.DisplayFloatingTextStringOnCreature(player.oid.ControlledCreature, $"{onSpellAction.Spell.Name.ToString()} - Energie manquante : {missingEnergy.ToString().ColorString(ColorConstants.Red)}");
        return;
      }*/

      //_ = onSpellAction.Caster.ClearActionQueue();

      //player.endurance.currentMana -= (int)Math.Round(energyCost, MidpointRounding.ToZero);

      //HandleCastTime(onSpellAction, player);
    }
    /*private static async void HandleCastTime(OnSpellAction spellAction, Player castingPlayer)
      {
        Location targetLocation = spellAction.IsAreaTarget ? Location.Create(spellAction.Caster.Area, spellAction.TargetPosition, spellAction.Caster.Rotation) : null;
        Vector3 previousPosition = spellAction.Caster.Position;

        double castTime = GetCastTime(castingPlayer, spellAction.Spell);

        if (spellAction.IsAreaTarget)
          await spellAction.Caster.ActionCastFakeSpellAt(spellAction.Spell, targetLocation);
        else
          await spellAction.Caster.ActionCastFakeSpellAt(spellAction.Spell, spellAction.TargetObject);

        foreach (NwPlayer player in NwModule.Instance.Players)
          if (player?.ControlledCreature?.Area == spellAction.Caster?.Area && player.ControlledCreature.IsCreatureHeard(spellAction.Caster))
            player.DisplayFloatingTextStringOnCreature(spellAction.Caster, StringUtils.ToWhitecolor($"{spellAction.Caster.Name.ColorString(ColorConstants.Cyan)} incante {spellAction.Spell.Name.ToString().ColorString(ColorConstants.Purple)}"));


        CancellationTokenSource tokenSource = new CancellationTokenSource();

        Task spellinterrupted = NwTask.WaitUntil(() => !spellAction.Caster.IsValid 
        || spellAction.Caster.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").HasValue
        || spellAction.Caster.GetObjectVariable<LocalVariableInt>("_INTERRUPTED").HasValue
        || (spellAction.TargetObject is null && !spellAction.IsAreaTarget)
        || previousPosition.X != spellAction.Caster.Position.X || previousPosition.Y != spellAction.Caster.Position.Y
        || (spellAction.TargetObject is not null && spellAction.TargetObject.Area != spellAction.Caster.Area), tokenSource.Token);
        Task castTimer = NwTask.Delay(TimeSpan.FromMilliseconds(castTime), tokenSource.Token);

        await NwTask.WhenAny(spellinterrupted, castTimer);
        tokenSource.Cancel();

        if (spellinterrupted.IsCompletedSuccessfully)
        {
          if(spellAction.Caster.IsValid)
            spellAction.Caster.GetObjectVariable<LocalVariableInt>("_INTERRUPTED").Delete();

          return;
        }

        if (spellAction.IsAreaTarget)
          await spellAction.Caster.ActionCastSpellAt(spellAction.Spell, targetLocation, spellAction.MetaMagic, true, ProjectilePathType.Default, true);
        else
          await spellAction.Caster.ActionCastSpellAt(spellAction.Spell, spellAction.TargetObject, spellAction.MetaMagic, true, 0, ProjectilePathType.Default, true);

        spellAction.Caster.GetObjectVariable<LocalVariableInt>("_CURRENT_SPELL").Value = spellAction.Spell.Id;
      }*/
      /*private static double GetCastTime(Player player, NwSpell spell)
      {
        double castTime = spell.ConjureTime.TotalMilliseconds;

        if (SpellUtils.spellCostDictionary[spell][3] != (int)SkillSystem.Type.Signet)
        {
          foreach (var eff in player.oid.LoginCreature.ActiveEffects)
            if (eff.Tag == "CUSTOM_CONDITION_DAZED")
            {
              castTime *= 2;
              break;
            }

          if (ItemUtils.GetItemHalvesCastTime(spell, player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand)))
            castTime /= 2;

          if (ItemUtils.GetItemHalvesCastTime(spell, player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand)))
            castTime /= 2;

          if (spell.SpellSchool == SpellSchool.Necromancy) // TODO : configurer tous les sorts exploitant des cadavres comme étant de l'école nécromancie
          {
            castTime -= 0.02 * SpellUtils.GetReduceCastTimeFromItem(player.oid.LoginCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Ensanglanté);
            castTime -= 0.02 * SpellUtils.GetReduceCastTimeFromItem(player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Ensanglanté);
            castTime -= 0.02 * SpellUtils.GetReduceCastTimeFromItem(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Ensanglanté);
          }

          if (spell.SpellSchool == SpellSchool.Conjuration) // TODO : configurer tous les sorts d'invocation comme étant de l'école conjuration
          {
            castTime -= 0.02 * SpellUtils.GetReduceCastTimeFromItem(player.oid.LoginCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Invocateur);
            castTime -= 0.02 * SpellUtils.GetReduceCastTimeFromItem(player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Invocateur);
            castTime -= 0.02 * SpellUtils.GetReduceCastTimeFromItem(player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Invocateur);
          }
        }

        int fastCastLevel = player.GetAttributeLevel(SkillSystem.Attribut.FastCasting);

        if(fastCastLevel > 0 && (castTime > 2000 
          || SpellUtils.spellCostDictionary[spell][(int)SpellUtils.SpellData.Attribute] == (int)SkillSystem.Attribut.DominationMagic
          || SpellUtils.spellCostDictionary[spell][(int)SpellUtils.SpellData.Attribute] == (int)SkillSystem.Attribut.FastCasting
          || SpellUtils.spellCostDictionary[spell][(int)SpellUtils.SpellData.Attribute] == (int)SkillSystem.Attribut.InspirationMagic
          || SpellUtils.spellCostDictionary[spell][(int)SpellUtils.SpellData.Attribute] == (int)SkillSystem.Attribut.IllusionMagic))
        {
          castTime -= fastCastLevel * (player.oid.LoginCreature.GetAbilityScore(Ability.Charisma) - 10) / 400;
        }

        return castTime / 1.5;      
      }*/
      public static void HandleCraftOnSpellInput(OnSpellAction onSpellAction)
    {
      if (onSpellAction.Spell.Id == CustomSpell.Calligraphie)
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
    public static void NoArmorShieldProficiencyOnSpellInput(OnSpellAction onSpellAction)
    {
      onSpellAction.Caster.LoginPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort tant que vous êtes équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas.", ColorConstants.Red);
      onSpellAction.PreventSpellCast = true;
    }

    [ScriptHandler("spellhook")] // TODO : Dans le cas des sorts de contact, appliquer un désavantage si le lancer est menacé en mêlée
    private void HandleSpellHook(CallInfo callInfo)
    {
      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();
      
      NwGameObject oCaster = onSpellCast.Caster;
      NwSpell spell = onSpellCast.Spell;

      oCaster.GetObjectVariable<LocalVariableObject<NwGameObject>>(CreatureUtils.CurrentAttackTarget).Delete();

      if (onSpellCast.TargetObject is not null)
        LogUtils.LogMessage($"----- {oCaster.Name} lance {spell.Name.ToString()} (id {spell.Id}) sur {onSpellCast.TargetObject.Name} -----", LogUtils.LogType.Combat);
      else
        LogUtils.LogMessage($"----- {oCaster.Name} lance {spell.Name.ToString()} (id {spell.Id}) en mode AoE -----", LogUtils.LogType.Combat);

      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      if (onSpellCast.Caster is NwCreature caster)
      {
        if (!SpellUtils.CanCastSpell(caster, onSpellCast.TargetObject, spell, spellEntry))
          return;

        SpellUtils.CheckDispelConcentration(caster, onSpellCast.Spell, spellEntry);
        SpellUtils.HandlePhlegetos(caster, spellEntry);

        if (!caster.KnowsFeat((Feat)CustomSkill.WizardIllusionAmelioree) && spell.SpellType != (Spell)CustomSpell.IllusionMineure)
          EffectUtils.RemoveEffectType(oCaster, EffectType.Invisibility, EffectType.ImprovedInvisibility);
      }
      else
        EffectUtils.RemoveEffectType(oCaster, EffectType.Invisibility, EffectType.ImprovedInvisibility);
      
        

      var castingClass = onSpellCast.SpellCastClass is not null ? onSpellCast.SpellCastClass : NwClass.FromClassId(CustomClass.Adventurer);

      SpellUtils.SpellSwitch(oCaster, spell, NwFeat.FromFeatId(NWScript.GetSpellFeatId()), spellEntry, onSpellCast.TargetObject, 
        onSpellCast.TargetLocation, castingClass);
    }
    public void HandleCraftEnchantementCast(OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oPC || !Players.TryGetValue(oPC, out Player player) || onSpellCast.Spell.Id != CustomSpell.Calligraphie)
        return;

      Enchantement(onSpellCast, player);
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
        silhouette?.Destroy();
        return;
      }

      silhouette.Location = oPC.Location;
      OnInvisMarkerPositionChanged(oPC, silhouette);
    }

    /*[ScriptHandler("nw_s0_invspha")]
    private void OnInviSphereEnter(CallInfo callInfo)
    {
      if (NWScript.GetEnteringObject().ToNwObject() is NwCreature oTarget)
      {
        Effect eInvis = Effect.Invisibility(InvisibilityTypef.Normal);
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
    }*/

    /*[ScriptHandler("nw_s0_invsphb")]
    private void OnInviSphereExit(CallInfo callInfo)
    {
      if (NWScript.GetExitingObject().ToNwObject() is NwCreature oTarget)
      {
        NwAreaOfEffect inviSphere = (NwAreaOfEffect)callInfo.ObjectSelf;

        foreach (Effect eff in oTarget.ActiveEffects.Where(e => e.EffectType == EffectType.Invisibility && e.Spell.SpellType == Spell.InvisibilitySphere && e.Creator == inviSphere.Creator))
          oTarget.RemoveEffect(eff);
      }
    }*/

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

      if (Players.TryGetValue(oPC.LoginCreature, out Player player))
      {
        if (player.oid.LoginCreature.Area.FindObjectsOfTypeInArea<NwCreature>().Any(c => c.GetObjectVariable<LocalVariableInt>("_IS_PVE_ARENA_CREATURE").HasValue))
          ArenaMenu.DrawRunAwayPage(player);
        else
          ArenaMenu.DrawNextFightPage(player, this);
      }
    }
    public void OnExitArena(AreaEvents.OnExit onExit)
    {
      if (onExit.ExitingObject is not NwCreature creature || !Players.TryGetValue(onExit.ExitingObject, out PlayerSystem.Player player))
        return;

      AreaSystem.CloseWindows(player);

      if (creature.IsPlayerControlled) // Cas normal de changement de zone
        if (!PlayerSystem.Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out player))
          return;
        else // cas de déconnexion du joueur
          Arena.Utils.ResetPlayerLocation(player);

      if (player.pveArena.currentRound == 0) // S'il s'agit d'un spectateur
      {
        player.oid.LoginCreature.OnSpellCast += OnSpellCastCancelDivination;
        player.oid.LoginCreature.OnSpellCast -= Arena.Utils.NoMagicMalus;
        return;
      }

      // A partir de là, il s'agit du gladiateur
      player.oid.OnPlayerDeath -= Arena.Utils.HandleArenaDeath;
      player.oid.OnPlayerDeath += HandlePlayerDeath;

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
    [ScriptHandler("on_cantrip_cast")]
    private void OnCantripCastRefundUse(CallInfo callInfo)
    {
      int classPosition = int.Parse(EventsPlugin.GetEventData("CLASS"));

      if (callInfo.ObjectSelf is not NwCreature caster
        || (MetaMagic)int.Parse(EventsPlugin.GetEventData("METAMAGIC")) != MetaMagic.None
        || classPosition == 254) //spell like ability
        return;

      var castingClass = caster.Classes[classPosition].Class.ClassType;
      var spell = NwSpell.FromSpellId(int.Parse(EventsPlugin.GetEventData("SPELL_ID")));
      var spellEntry = Spells2da.spellTable[spell.Id];

      if (spell.GetSpellLevelForClass(caster.Classes[classPosition].Class) < 1)
      {
        EventsPlugin.SkipEvent();
        return;
      }

      if (spellEntry.ritualSpell && !caster.IsInCombat)
      {
        LogUtils.LogMessage("Sort lancé en mode rituel", LogUtils.LogType.Combat);
        EventsPlugin.SkipEvent();
        return;
      }

      switch(spell.Id)
      {
        case (int)Spell.EpicDragonKnight:

          if(caster.KnowsFeat((Feat)CustomSkill.EnsoCompagnonDraconique)
             && (caster.GetObjectVariable<PersistentVariableString>("_COMPAGNON_DRACONIQUE_COOLDOWN").HasNothing
            || (DateTime.TryParse(caster.GetObjectVariable<PersistentVariableString>("_COMPAGNON_DRACONIQUE_COOLDOWN").Value, out var cooldown)
                && cooldown < DateTime.Now)))

          caster.GetObjectVariable<PersistentVariableString>("_COMPAGNON_DRACONIQUE_COOLDOWN").Value = DateTime.Now.AddDays(1).ToString();
          EventsPlugin.SkipEvent();

          return;

        case (int)Spell.CallLightning:

          if(caster.GetObjectVariable<LocalVariableInt>("_FREE_SPELL").HasValue)
          {
            caster.GetObjectVariable<LocalVariableInt>("_FREE_SPELL").Delete();
            EventsPlugin.SkipEvent();
          }
          return;

        case (int)Spell.SeeInvisibility:

          if (castingClass == ClassType.Wizard && caster.KnowsFeat((Feat)CustomSkill.IllusionVoirLinvisible)
            && caster.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").HasNothing)
          {
            caster.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").Value = 1;
            EventsPlugin.SkipEvent();
          }
          return;

        case CustomSpell.MarqueDuChasseur:

          if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.FreeMarqueDuChasseurTag))
          {
            EffectUtils.RemoveTaggedEffect(caster, EffectSystem.FreeMarqueDuChasseurTag);
            EventsPlugin.SkipEvent();
          }

          return;

        case CustomSpell.Terrassement: EventsPlugin.SkipEvent(); return;
      }

      if (castingClass == ClassType.Wizard)
      {
        if (Players.TryGetValue(caster, out var player) && player.learnableSpells.TryGetValue(spell.Id, out var masterSpell) && masterSpell.mastery)
        {
          EventsPlugin.SkipEvent();
          return;
        }
      }

      if(castingClass == ClassType.Druid)
      {
        if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.EconomieNaturelleEffectTag)
         && Players.TryGetValue(caster, out var druidPlayer) && druidPlayer.learnableSpells.TryGetValue(spell.Id, out var druidSpell) 
         && druidSpell.alwaysPrepared)
        {
          EventsPlugin.SkipEvent();
          EffectUtils.RemoveTaggedEffect(caster, EffectSystem.EconomieNaturelleEffectTag);
          return;
        }
      }
    }
  }
}
