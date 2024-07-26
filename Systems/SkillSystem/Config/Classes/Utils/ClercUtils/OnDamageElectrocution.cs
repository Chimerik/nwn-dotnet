﻿using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static void OnDamageElectrocution(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature caster || onDamage.Target is not NwCreature target || target.Size > CreatureSize.Large 
        || (onDamage.DamageData.GetDamageByType(DamageType.Sonic) < 1 && onDamage.DamageData.GetDamageByType(DamageType.Electrical) < 1))
        return;

      int DC = SpellUtils.GetCasterSpellDC(caster, Ability.Wisdom);

      if (CreatureUtils.GetSavingThrow(caster, target, Ability.Constitution, DC))
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(1)));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSonic));
      }
    }
  }
}
