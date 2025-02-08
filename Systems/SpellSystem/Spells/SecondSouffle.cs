
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SecondSouffle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;



      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      int fighterLevel = FighterUtils.GetFighterLevel(caster);
      caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Heal(NwRandom.Roll(Utils.random, spellEntry.damageDice) + fighterLevel), Effect.VisualEffect(VfxType.ImpHealingM)));

      if (caster.KnowsFeat((Feat)CustomSkill.FighterAvantageTactique))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.AvantageTactique, NwTimeSpan.FromRounds(1));
    }
  }
}
