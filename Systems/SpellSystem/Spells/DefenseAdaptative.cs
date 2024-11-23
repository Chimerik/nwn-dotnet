using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DefenseAdaptative(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      EffectUtils.RemoveTaggedEffect(caster, EffectSystem.EchapperALaHordeEffectTag, EffectSystem.DefenseAdaptativeEffectTag);
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Permanent, EffectSystem.DefenseAdaptative));
      caster.OnDamaged -= RangerUtils.OnDamageDefenseAdaptative;
      caster.OnDamaged += RangerUtils.OnDamageDefenseAdaptative;
    }
  }
}
