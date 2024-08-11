using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageFrappeMeurtriere(OnCreatureDamage onDmg)
    {
      if (onDmg.DamagedBy is not NwCreature attacker || onDmg.Target is not NwCreature target
        || onDmg.DamageData.GetDamageByType(DamageType.BaseWeapon) < 1)
        return;

      if (!attacker.KnowsFeat((Feat)CustomSkill.AssassinAssassinate) || !attacker.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 16)
          || !attacker.ActiveEffects.Any(e => e.Tag == EffectSystem.AssassinateEffectTag))
        return;

      int DC = SpellConfig.BaseSpellDC + attacker.GetAbilityModifier(Ability.Dexterity) + NativeUtils.GetCreatureProficiencyBonus(attacker);

      StringUtils.DisplayStringToAllPlayersNearTarget(target, "Frappe Meurtrière", StringUtils.gold, true, true);
      LogUtils.LogMessage($"{attacker.Name} - Frappe Meurtrière sur {target.Name}", LogUtils.LogType.Combat);

      if (GetSavingThrow(attacker, target, Ability.Constitution, DC) == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodSparkMedium));
        var damage = onDmg.DamageData.GetDamageByType(DamageType.BaseWeapon) * 2;
        onDmg.DamageData.SetDamageByType(DamageType.BaseWeapon, damage);
        LogUtils.LogMessage($"Frappe meutrière : Dégâts {damage}", LogUtils.LogType.Combat);
      }
    }
  }
}
