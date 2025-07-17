using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnDamageLoupMorsureInfectieuse(OnCreatureDamage onDamage)
    {
      if (onDamage.Target is not NwCreature target || onDamage.DamagedBy is not NwCreature damager || damager.Master is null
        || target.ActiveEffects.Any(e => e.Tag == EffectSystem.MorsureInfectieuseEffectTag))
        return;

      int spellDC = SpellUtils.GetCasterSpellDC(damager.Master, Ability.Wisdom);
      
      if(CreatureUtils.GetSavingThrowResult(target, Ability.Constitution, damager.Master, spellDC) == SavingThrowResult.Failure)
        NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MorsureInfectieuse, NwTimeSpan.FromRounds(3)));
    }
  }
}
