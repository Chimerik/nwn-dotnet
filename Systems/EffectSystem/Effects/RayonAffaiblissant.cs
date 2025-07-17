using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalRayonAffaiblissantCallback;
    public const string RayonAffaiblissantEffectTag = "_RAYON_AFFAIBLISSANT_EFFECT";
    public const string RayonAffaiblissantDesavantageEffectTag = "_RAYON_AFFAIBLISSANT_DESAVANTAGE_EFFECT";
    public static Effect RayonAffaiblissant(NwSpell spell, Ability spellCastingAbility)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.RayonAffaiblissant), Effect.VisualEffect(VfxType.DurCessateNegative));
      Effect action = Effect.RunAction(onIntervalHandle: onIntervalRayonAffaiblissantCallback, interval: NwTimeSpan.FromRounds(1));
      action.IntParams[5] = (int)spellCastingAbility;

      eff = Effect.LinkEffects(eff, action);
      eff.Tag = RayonAffaiblissantEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = (int)spellCastingAbility;
      eff.Spell = spell;
      return eff;
    }

    public static Effect RayonAffaiblissantDesavantage
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.AttackDecrease);
        eff.Tag = RayonAffaiblissantDesavantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    private static ScriptHandleResult OnIntervalRayonAffaiblissant(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      Effect eff = eventData.Effect;
      
      if (eventData.EffectTarget is NwCreature target)
      {
        if (eff.Creator is NwCreature caster)
        {
          SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.RayonAffaiblissant];
          int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.RayonAffaiblissant), (Ability)eventData.Effect.CasterLevel);

          if (CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry, SpellConfig.SpellEffectType.Paralysis) != SavingThrowResult.Failure)
          {
            target.RemoveEffect(eff);
            target.ApplyEffect(EffectDuration.Temporary, RayonAffaiblissantDesavantage, NwTimeSpan.FromRounds(1));
          }
        }
        else
          target.RemoveEffect(eff);

      }

      return ScriptHandleResult.Handled;
    }
  }
}
