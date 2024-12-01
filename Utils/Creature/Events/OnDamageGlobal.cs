using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageGlobal(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature damager)
        return;

      int baseWeaponDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

      if (baseWeaponDamage > -1 && onDamage.Target is NwCreature target)
      {
        if(target.ActiveEffects.Any(e => e.Tag == EffectSystem.DefensesEnjoleusesEffectTag))
        {
          EffectUtils.RemoveTaggedEffect(target, EffectSystem.DefensesEnjoleusesEffectTag);

          int spellDC = SpellUtils.GetCasterSpellDC(damager, Ability.Charisma);
          int reducedDamage = baseWeaponDamage / 2;
          onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, reducedDamage);
          
          LogUtils.LogMessage($"Defenses Enjoleuses - Dégâts initiaux : {baseWeaponDamage} / 2 = {reducedDamage}", LogUtils.LogType.Combat);
          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Défenses Enjôleuses", ColorConstants.Pink, true, true);
          
          if(GetSavingThrow(damager, target, Ability.Wisdom, spellDC) == SavingThrowResult.Failure)
          {
            NWScript.AssignCommand(target, () => damager.ApplyEffect(EffectDuration.Instant, Effect.Damage(reducedDamage, CustomDamageType.Psychic)));
          }
        }
      }
    }
  }
}
