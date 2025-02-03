using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FeintTrepas(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oTarget is not NwCreature target || oCaster is not NwCreature caster || caster.Faction.GetMembers().Contains(target))
        return;

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSleep));
      target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FeintTrepas,SpellUtils.GetSpellDuration(oCaster, spellEntry));
    }
  }
}
