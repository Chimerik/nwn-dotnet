﻿using System;
using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveConcentrationCallback;
    public static Effect concentration(int spellId)
    {
      Effect eff = Effect.LinkEffects(Effect.RunAction(onRemovedHandle: onRemoveConcentrationCallback), Effect.Icon(CustomEffectIcon.Concentration), Effect.VisualEffect(VfxType.DurCessateNeutral));
      eff.Tag = ConcentrationEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Spell = NwSpell.FromSpellId(spellId);

      return eff;
    }
    public const string ConcentrationEffectTag = "_CONCENTRATION_EFFECT";
    public const string ConcentrationTargetString = "_CONCENTRATION_TARGET_";
    public const string ConcentrationSpellIdString = "_CONCENTRATION_SPELL";
    private static StrRef tlkEntry = StrRef.FromCustomTlk(190116);

    public static async void ApplyConcentrationEffect(NwCreature caster, int spellId, List<NwGameObject> targetList, TimeSpan duration)
    {
      if (targetList.Count < 1)
        return;

      await NwTask.NextFrame();

      if(caster.IsLoginPlayerCharacter) 
        tlkEntry.SetPlayerOverride(caster.LoginPlayer, $"Concentration : {NwSpell.FromSpellId(spellId).Name}");

      if (duration.TotalSeconds > 0)
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, concentration(spellId), duration));
      else
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, concentration(spellId)));

      int targetNumber = 1;

      foreach(var target in targetList)
      {
        caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{targetNumber}").Value = target;
        targetNumber++;
      }

      caster.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value = spellId;
      caster.OnDamaged += CreatureUtils.OnDamageConcentration;
    }
    private static ScriptHandleResult OnRemoveConcentration(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
        EffectUtils.RemoveTaggedEffect(caster, ConcentrationAdvantageEffectTag);

      int i = 1;

      while (eventData.EffectTarget.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{i}").HasValue)
      {
        NwGameObject target = eventData.EffectTarget.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{i}").Value;

        if (target is null)
        {
          eventData.EffectTarget.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}_{i}").Delete();
          i++;
          continue;
        }

        if (target is NwAreaOfEffect aoe)
        {
          aoe.Destroy();
          eventData.EffectTarget.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}_{i}").Delete();
          i++;
          continue;
        }

        if (target.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value == CustomSpell.LameArdente && target is NwCreature creature)
        {
          if (creature.GetItemInSlot(InventorySlot.RightHand)?.Tag == "_TEMP_FLAME_BLADE")
            creature.GetItemInSlot(InventorySlot.RightHand).Destroy();

          creature.DecrementRemainingFeatUses((Feat)CustomSkill.FlameBlade);
          return ScriptHandleResult.Handled;
        }

        //ModuleSystem.Log.Info($"removing concentration effects on : {target.Name}");

        foreach (var eff in target.ActiveEffects)
        {
          /*ModuleSystem.Log.Info($"effect: {eff.Tag}");

          if (eff.Creator is null)
            ModuleSystem.Log.Info($"effect creator: ");
          else
            ModuleSystem.Log.Info($"effect creator: {eff.Creator.Name}");

          if (eff.Spell is null)
            ModuleSystem.Log.Info($"effect spell: ");
          else
            ModuleSystem.Log.Info($"effect spell: {NwSpell.FromSpellId(eff.Spell.Id).Name.ToString()}");*/

          if (eff.Spell is null || eff.Creator != eventData.EffectTarget
            || eventData.EffectTarget.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value != eff.Spell.Id)
            continue;

          //ModuleSystem.Log.Info($"effect removed: {eff.Tag}");
          target.RemoveEffect(eff);
          eventData.EffectTarget.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}_{i}").Delete();
        }

        i++;
      }

      eventData.EffectTarget.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Delete();
      ((NwCreature)eventData.EffectTarget).OnDamaged -= CreatureUtils.OnDamageConcentration;

      return ScriptHandleResult.Handled;
    }
    /*public static void OnRemoveConcentration(OnEffectRemove onEffect)
    {
      if(onEffect.Effect.EffectType != EffectType.RunScript)
        return;

      int i = 1;

      while(onEffect.Object.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{i}").HasValue)
      {
        NwGameObject target = onEffect.Object.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{i}").Value;

        if (target is NwAreaOfEffect aoe)
        {
          aoe.Destroy();
          onEffect.Object.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}_{i}").Delete();
          i++;
          continue;
        }
        
        if (target.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value == CustomSpell.FlameBlade && target is NwCreature creature)
        {
          if (creature.GetItemInSlot(InventorySlot.RightHand)?.Tag == "_TEMP_FLAME_BLADE")
            creature.GetItemInSlot(InventorySlot.RightHand).Destroy();

          creature.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.FlameBlade));
          return;
        }

        //ModuleSystem.Log.Info($"removing concentration effects on : {target.Name}");

        foreach (var eff in target.ActiveEffects)
        {
          //ModuleSystem.Log.Info($"effect: {eff.Tag}");

          //if (eff.Creator is null)
            //ModuleSystem.Log.Info($"effect creator: ");
          //else
            //ModuleSystem.Log.Info($"effect creator: {eff.Creator.Name}");

          //if (eff.Spell is null)
            //ModuleSystem.Log.Info($"effect spell: ");
          //else
            //ModuleSystem.Log.Info($"effect spell: {NwSpell.FromSpellId(eff.Spell.Id).Name.ToString()}");

          if (eff.Spell is null || eff.Creator != onEffect.Object
            || onEffect.Object.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value != eff.Spell.Id)
            continue;

          //ModuleSystem.Log.Info($"effect removed: {eff.Tag}");
          target.RemoveEffect(eff);
          onEffect.Object.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}_{i}").Delete();
        }

        i++;
      }

      onEffect.Object.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Delete();
      ((NwCreature)onEffect.Object).OnDamaged -= CreatureUtils.OnDamageConcentration;
    }*/
  }
}
