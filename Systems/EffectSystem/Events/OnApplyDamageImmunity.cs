using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static void OnApplyDamageImmunity(OnEffectApply onEffect)
    {
      if (onEffect.Effect.EffectType != EffectType.DamageImmunityIncrease || onEffect.Object is not NwCreature creature)
        return;

      var newEffect = onEffect.Effect;
      var durationType = newEffect.DurationType;

      if(durationType == EffectDuration.Permanent)
      {
        foreach(var eff in creature.ActiveEffects)
        {
          if(eff.EffectType == newEffect.EffectType && eff.IntParams[0] == newEffect.IntParams[0]) // IntParams[0] == DamageType
          {
            if(eff.DurationType == EffectDuration.Permanent)
            {
              if (eff.IntParams[1] < newEffect.IntParams[1]) // IntParams[1] == Percent of immunity 
                creature.RemoveEffect(eff);
              else
              {
                onEffect.PreventApply = true;
                return;
              }
            }
            else
            {
              if(newEffect.IntParams[1] < eff.IntParams[1])
              {
                EffectUtils.DelayedApplyEffect(creature, newEffect.DurationType, newEffect, TimeSpan.FromSeconds(eff.DurationRemaining));
                onEffect.PreventApply = true;
                return;
              }
              else
                creature.RemoveEffect(eff);
            }
          }
        }
      }
      else // Duration Temporary
      {
        foreach (var eff in creature.ActiveEffects)
        {
          if (eff.EffectType == newEffect.EffectType && eff.IntParams[0] == newEffect.IntParams[0]) // IntParams[0] == DamageType
          {
            if (eff.DurationType == EffectDuration.Permanent)
            {
              if (eff.IntParams[1] < newEffect.IntParams[1]) // IntParams[1] == Percent of immunity 
              {
                creature.RemoveEffect(eff);
                EffectUtils.DelayedApplyEffect(creature, eff.DurationType, eff, TimeSpan.FromSeconds(newEffect.TotalDuration));
              }
              else
              {
                onEffect.PreventApply = true;
                return;
              }
            }
            else
            {
              if (newEffect.TotalDuration > eff.DurationRemaining) 
              {
                if(eff.IntParams[1] < newEffect.IntParams[1]) // IntParams[1] == Percent of immunity 
                  creature.RemoveEffect(eff);
                else
                {
                  EffectUtils.DelayedApplyEffect(creature, newEffect.DurationType, newEffect, TimeSpan.FromSeconds(eff.DurationRemaining), TimeSpan.FromSeconds(newEffect.TotalDuration - eff.DurationRemaining));
                  onEffect.PreventApply = true;
                  return;
                }
              }
              else
              {
                if (newEffect.IntParams[1] > eff.IntParams[1])
                {
                  creature.RemoveEffect(eff);
                  EffectUtils.DelayedApplyEffect(creature, eff.DurationType, eff, TimeSpan.FromSeconds(newEffect.TotalDuration), TimeSpan.FromSeconds(eff.DurationRemaining - newEffect.TotalDuration));
                }
                else
                {
                  onEffect.PreventApply = true;
                  return;
                }
              }
            }
          }
        }
      }
    }
  }
}
