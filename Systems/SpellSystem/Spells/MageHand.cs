using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MageHand(NwGameObject oCaster, NwSpell spell, NwGameObject target)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(target, caster, spell.SpellType);
      
      if (target is not null && caster.KnowsFeat((Feat)CustomSkill.ArcaneTricksterPolyvalent))
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.ArcaneTricksterPolyvalent, NwTimeSpan.FromRounds(1)));
        target.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      }
      else
        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
    }
  }
}
