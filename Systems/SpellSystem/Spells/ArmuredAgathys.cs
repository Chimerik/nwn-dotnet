
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ArmuredAgathys(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(caster, caster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.ArmuredAgathys(caster), SpellUtils.GetSpellDuration(caster, spellEntry));
    }  
  }
}
