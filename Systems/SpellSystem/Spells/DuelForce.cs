using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DuelForce(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfPwstun));
      EffectSystem.ApplyDuelForce(target, caster, SpellUtils.GetSpellDuration(oCaster, spellEntry), spellEntry.savingThrowAbility, casterClass.SpellCastingAbility);
    }
  }
}
