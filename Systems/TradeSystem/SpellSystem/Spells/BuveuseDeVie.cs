
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BuveuseDeVie(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      caster.GetObjectVariable<PersistentVariableInt>(CreatureUtils.BuveuseDeVieVariable).Value = (int)spellEntry.damageType[0];
      EffectSystem.ApplyBuveuseDeVie(caster);
    }
  }
}
