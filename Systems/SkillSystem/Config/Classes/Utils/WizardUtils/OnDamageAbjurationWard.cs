﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void OnDamageAbjurationWard(CreatureEvents.OnDamaged onDamage)
    {
      var ward = onDamage.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.AbjurationWardEffectTag);

      if(ward is null)
      {
        onDamage.Creature.OnDamaged -= OnDamageAbjurationWard;
        return;
      }

      if(ward.Creator != onDamage.Creature)
      {
        if (ward.Creator is not NwCreature creator || !creator.IsValid)
        {
          EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.AbjurationWardEffectTag);
          return;
        }
        else if (creator.Area != onDamage.Creature.Area || onDamage.Creature.DistanceSquared(creator) > 80)
        {
          EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.AbjurationWardEffectTag, creator);
          NWScript.AssignCommand(creator, () => creator.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(ward.CasterLevel)));
        }

        onDamage.Creature.OnDamaged -= OnDamageAbjurationWard;
      }

      int negatedDamage = ward.CasterLevel > onDamage.DamageAmount ? onDamage.DamageAmount : ward.CasterLevel;

      EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.AbjurationWardEffectTag, ward.Creator);

      if(ward.CasterLevel > 1)
        NWScript.AssignCommand(ward.Creator, () => onDamage.Creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(ward.CasterLevel - 1)));

      onDamage.Creature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGlobeUse));

      LogUtils.LogMessage($"{onDamage.Creature.Name} - protection arcanique - dégâts réduits de {negatedDamage}", LogUtils.LogType.Combat);
    }
  }
}
