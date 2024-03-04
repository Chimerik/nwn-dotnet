using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamagePourfendeur(OnCreatureDamage onDamage)
    {
      if ((onDamage.DamageData.GetDamageByType(DamageType.Slashing) > 0
        || onDamage.DamagedBy.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_SLASH").HasValue) 
        && onDamage.DamagedBy.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_COOLDOWN").HasNothing)
      {
        onDamage.Target.ApplyEffect(EffectDuration.Temporary, EffectSystem.PourfendeurSlowEffect, NwTimeSpan.FromRounds(1));
        onDamage.DamagedBy.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_COOLDOWN").Value = 1;

        if (onDamage.DamagedBy.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").HasValue)
          onDamage.Target.ApplyEffect(EffectDuration.Temporary, EffectSystem.PourfendeurDisadvantageEffect, NwTimeSpan.FromRounds(1));

        RemovePourfendeurCooldown(onDamage.DamagedBy);
      }

      onDamage.DamagedBy.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_SLASH").Delete();
      onDamage.DamagedBy.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_CRIT").Delete();
    }
    private static async void RemovePourfendeurCooldown(NwObject damager)
    {
      await NwTask.Delay(NwTimeSpan.FromRounds(1));
      damager.GetObjectVariable<LocalVariableInt>("_POURFENDEUR_COOLDOWN").Delete();
    }
  }
}
