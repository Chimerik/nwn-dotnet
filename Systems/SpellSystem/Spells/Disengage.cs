using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Disengage(NwGameObject oCaster, NwSpell spell)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
    }
  }
}
