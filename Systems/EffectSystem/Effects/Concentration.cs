using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

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
        caster.ApplyEffect(EffectDuration.Temporary, concentration, NwTimeSpan.FromRounds(duration));
      else
        caster.ApplyEffect(EffectDuration.Permanent, concentration);

      int targetNumber = 1;

      foreach(var target in targetList)
      {
        caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}{targetNumber}").Value = target;
        targetNumber++;
      }

      caster.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value = spellId;
    }
    public static void OnRemoveConcentration(OnEffectRemove onEffect)
    {
      if (!onEffect.Effect.IsValid || onEffect.Effect.Tag != ConcentrationEffectTag || onEffect.Effect.EffectType != EffectType.RunScript)
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

        foreach (var eff in target.ActiveEffects)
        {
          if (eff.Creator == onEffect.Object
            && onEffect.Object.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Value == eff.Spell.Id)
          {
            target.RemoveEffect(eff);
            onEffect.Object.GetObjectVariable<LocalVariableObject<NwGameObject>>($"{ConcentrationTargetString}_{i}").Delete();
          }
        }

        i++;
      }

      onEffect.Object.GetObjectVariable<LocalVariableInt>(ConcentrationSpellIdString).Delete();
    }
  }
}
