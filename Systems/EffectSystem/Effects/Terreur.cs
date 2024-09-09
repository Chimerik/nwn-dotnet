using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalTerreurCallback;
    public const string TerreurEffectTag = "_TERREUR_EFFECT";
    public static Effect GetTerreurEffect(Ability ability)
    {
      Effect eff = Effect.LinkEffects(Effect.Frightened(), 
        Effect.RunAction(onIntervalHandle: onIntervalTerreurCallback, interval: NwTimeSpan.FromRounds(1), data:((int)ability).ToString()));
      eff.Tag = TerreurEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnIntervalTerreur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if(eventData.EffectTarget is not NwCreature target || eventData.Effect.Creator is not NwCreature caster || target.HasLineOfSight(caster))
        return ScriptHandleResult.Handled;

      SpellEntry spellEntry = Spells2da.spellTable[(int)Spell.Fear];
      Ability castingAbility = (Ability)int.Parse(eventData.Effect.StringParams[0]);
      int spellDC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellType(Spell.Fear), castingAbility);

      if (CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry, SpellConfig.SpellEffectType.Fear) == SavingThrowResult.Failure)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, GetTerreurEffect(castingAbility), NwTimeSpan.FromRounds(spellEntry.duration)));

      return ScriptHandleResult.Handled;
    }
  }
}
