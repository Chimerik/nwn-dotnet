using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ElectricJolt(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, caster, spell.SpellType);
      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpLightningS));

      int nbDice = SpellUtils.GetSpellDamageDiceNumber(caster, spell);

      switch (SpellUtils.GetSpellAttackRoll(oTarget, caster, spell, casterClass.SpellCastingAbility, 0))
      {
        case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(caster, spellEntry, nbDice); ; break;
        case TouchAttackResult.Hit: break;
        default: return;
      }

      oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.noReactions, NwTimeSpan.FromRounds(1)) ;
      oTarget.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value = 0;

      SpellUtils.DealSpellDamage(oTarget, caster.CasterLevel, spellEntry, nbDice, caster, spell.GetSpellLevelForClass(casterClass.ClassType));
    }
  }
}
