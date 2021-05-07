﻿using NWN.API;
using NWN.API.Constants;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Events;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static void HandleDamageEvent(OnCreatureDamage onDamage)
    {
      if (onDamage.Target.GetLocalVariable<int>("_IS_GNOME_MECH").HasValue && onDamage.DamageData.Electrical > 0)
      {
        onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.Heal(onDamage.DamageData.Electrical));
        onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));
        onDamage.Target.GetLocalVariable<int>("_SPARK_LEVEL").Value += 5;

        foreach (NwPlayer oPC in onDamage.Target.Area.FindObjectsOfTypeInArea<NwPlayer>())
          oPC.DisplayFloatingTextStringOnCreature((NwCreature)onDamage.Target, onDamage.DamageData.Electrical.ToString().ColorString(new Color(32, 255, 32)));

        if (onDamage.Target.Tag == "dog_meca_defect" && CreaturePlugin.GetAttacksPerRound(onDamage.Target) < 6
          && NwRandom.Roll(Utils.random, 100) <= onDamage.Target.GetLocalVariable<int>("_SPARK_LEVEL").Value)
        {
          foreach (NwPlayer oPC in onDamage.Target.Area.FindObjectsOfTypeInArea<NwPlayer>())
            oPC.SendServerMessage("Attention, la décharge électrique surcharge le chien mécanisé défectueux, le rendant plus dangereux !");

          onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
          onDamage.Target.ApplyEffect(EffectDuration.Instant, Effect.Damage(onDamage.Target.MaxHP / 10, DamageType.Magical));
          ((NwCreature)onDamage.Target).BaseAttackCount = CreaturePlugin.GetAttacksPerRound(onDamage.Target) + 1;
        }

        onDamage.DamageData.Electrical = 0;
      }
    }
  }
}
