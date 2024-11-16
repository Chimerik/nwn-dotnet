
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FrappeDivine(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      caster.GetObjectVariable<PersistentVariableInt>(CreatureUtils.FrappeDivineVariable).Value = (int)spellEntry.damageType[0];
      EffectSystem.ApplyFrappeDivine(caster);
    }
  }
}
