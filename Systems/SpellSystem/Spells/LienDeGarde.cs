using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void LienDeGarde(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      EffectSystem.ApplyLienDeGarde(caster, target, spell, spellEntry);
    }
  }
}
