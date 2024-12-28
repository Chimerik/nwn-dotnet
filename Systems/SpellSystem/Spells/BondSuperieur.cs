using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BondSuperieur(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, NwGameObject target)
    {
      SpellUtils.SignalEventSpellCast(oCaster, target, spell.SpellType);
      NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.BondSuperieur, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
    }
  }
}
