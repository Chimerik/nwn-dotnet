using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalTerreurCallback;
    public const string TerreurEffectTag = "_TERREUR_EFFECT";
    public static Effect GetTerreurEffect(NwCreature caster, NwCreature target, NwSpell spell, Ability ability)
    {
      _ = target.ActionMoveAwayFrom(caster, true);

      Effect eff = Effect.LinkEffects(Effect.Frightened(), Effect.VisualEffect(VfxType.DurMindAffectingFear),
        Effect.RunAction(onIntervalHandle: onIntervalTerreurCallback, interval: NwTimeSpan.FromRounds(1)));
      eff.Tag = TerreurEffectTag;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = (int)ability;
      return eff;
    }
    private static ScriptHandleResult OnIntervalTerreur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if(eventData.EffectTarget is not NwCreature target || eventData.Effect.Creator is not NwCreature caster || target.HasLineOfSight(caster))
        return ScriptHandleResult.Handled;

      if(target.HasLineOfSight(caster))
      {
        _ = target.ActionMoveAwayFrom(caster, true);
      }
      else
      {
        SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.Fear];
        Ability castingAbility = (Ability)eventData.Effect.CasterLevel;
        int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellType(Spell.Fear), castingAbility);

        if (CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC, spellEntry, SpellConfig.SpellEffectType.Fear) != SavingThrowResult.Failure)
          EffectUtils.RemoveTaggedEffect(target, caster, TerreurEffectTag);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
