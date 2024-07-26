using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ProtectionContreLaMort(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oTarget is not NwCreature target)
        return;

      SpellUtils.SignalEventSpellCast(target, oCaster, spell.SpellType, false);

      EffectUtils.RemoveTaggedEffect(target, EffectSystem.ProtectionContreLaMortEffectTag);
      NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.ProtectionContreLaMort, NwTimeSpan.FromRounds(spellEntry.duration)));

      target.OnDamaged -= SpellEvent.OnDamagedProtectionContreLaMort;
      target.OnDamaged += SpellEvent.OnDamagedProtectionContreLaMort;
    }
  }
}
