using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect concentration
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(148)), Effect.VisualEffect(VfxType.DurCessateNeutral));
        eff.Tag = ConcentrationEffectTag;
        eff.SubType = EffectSubType.Supernatural;

        return eff;
      }
    }
    public const string ConcentrationEffectTag = "_CONCENTRATION_EFFECT";
    public const string ConcentrationTargetString = "_CONCENTRATION_TARGET_";
    public const string ConcentrationSpellIdString = "_CONCENTRATION_SPELL";
    private static StrRef tlkEntry = StrRef.FromCustomTlk(190116);

    public static async void ApplyConcentrationEffect(NwCreature caster, int spellId, List<NwGameObject> targetList, int duration = 0)
    {
      await NwTask.NextFrame();

      if(caster.IsLoginPlayerCharacter) 
        tlkEntry.SetPlayerOverride(caster.ControllingPlayer, $"Concentration : {NwSpell.FromSpellId(spellId).Name}");

      if (duration > 0)
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, concentration, NwTimeSpan.FromRounds(duration)));
      else
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, concentration));

      int targetNumber = 1;

      foreach(var target in targetList)
      {
        caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{targetNumber}").Value = target;
        targetNumber++;
      }

      caster.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value = spellId;
      caster.OnDamaged += CreatureUtils.OnDamageConcentration;
    }
    public static void OnRemoveConcentration(OnEffectRemove onEffect)
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
          /*ModuleSystem.Log.Info($"effect: {eff.Tag}");

          if (eff.Creator is null)
            ModuleSystem.Log.Info($"effect creator: ");
          else
            ModuleSystem.Log.Info($"effect creator: {eff.Creator.Name}");

          if (eff.Spell is null)
            ModuleSystem.Log.Info($"effect spell: ");
          else
            ModuleSystem.Log.Info($"effect spell: {NwSpell.FromSpellId(eff.Spell.Id).Name.ToString()}");*/

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
    }
  }
}
