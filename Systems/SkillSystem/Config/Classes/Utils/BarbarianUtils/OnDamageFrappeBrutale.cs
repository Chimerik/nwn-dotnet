using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static async void OnDamageFrappeBrutale(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature caster || onDamage.Target is not NwCreature target
        || onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon) < 0)
        return;

      Effect frappe = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.FrappeBrutaleEffectTag);

      if (frappe is not null)
      {
        switch (frappe.CasterLevel)
        {
          case CustomSkill.FrappeBrutale:
            caster.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedIncrease(25), NwTimeSpan.FromRounds(1));
            target.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(1));
            break;

          case CustomSkill.FrappeSiderante:
            NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappeSiderante, NwTimeSpan.FromRounds(1)));
            break;

          case CustomSkill.FrappeDechirante:
            NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappeDechirante, NwTimeSpan.FromRounds(1)));
            break;
        }

        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.FrappeBrutaleEffectTag);
      }

      await NwTask.NextFrame();
      caster.OnCreatureDamage -= OnDamageFrappeBrutale;
    }
  }
}
