using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CoupAuButAttackEffectTag = "_COUP_AU_BUT_ATTACK_EFFECT";
    public const string CoupAuButDamageEffectTag = "_COUP_AU_BUT_DAMAGE_EFFECT";
    public static readonly Native.API.CExoString CoupAuButAttackEffectExoTag = CoupAuButAttackEffectTag.ToExoString();
    public static readonly Native.API.CExoString CoupAuButDamageEffectExoTag = CoupAuButDamageEffectTag.ToExoString();
    public static void ApplyCoupAuBut(NwCreature caster, NwSpell spell, Ability casterAbility)
    {
      EffectUtils.RemoveTaggedEffect(caster, CoupAuButAttackEffectTag, CoupAuButDamageEffectTag);
      caster.OnCreatureAttack -= OnAttackCoupAuBut;
      caster.OnCreatureDamage -= OnDamageCoupAuBut;

      Effect attack = Effect.Icon(EffectIcon.AttackIncrease);
      attack.Tag = CoupAuButAttackEffectTag;
      attack.SubType = EffectSubType.Supernatural;
      attack.IntParams[5] = (int)casterAbility;
      caster.ApplyEffect(EffectDuration.Permanent, attack);

      Effect damage = Effect.Icon(EffectIcon.DamageIncrease);
      damage.Tag = CoupAuButDamageEffectTag;
      damage.SubType = EffectSubType.Supernatural;
      damage.IntParams[5] = (int)casterAbility;
      caster.ApplyEffect(EffectDuration.Permanent, damage);

      if (spell.Id == CustomSpell.CoupAuButRadiant)
      { 
        caster.OnCreatureAttack += OnAttackCoupAuBut;
        caster.OnCreatureDamage += OnDamageCoupAuBut;
      }
    }
    public static async void OnAttackCoupAuBut(OnCreatureAttack onAttack)
    {
      switch (onAttack.AttackResult)
      {
        case AttackResult.Miss:
        case AttackResult.MissChance:
        case AttackResult.Concealed:
        case AttackResult.Parried:

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackCoupAuBut;
          onAttack.Attacker.OnCreatureDamage -= OnDamageCoupAuBut;

          break;

        case AttackResult.Hit:
        case AttackResult.AutomaticHit:
        case AttackResult.CriticalHit:

          int nbDice = onAttack.Attacker.Level > 16 ? 3 : onAttack.Attacker.Level > 10 ? 2 : onAttack.Attacker.Level > 4 ? 1 : 0;
          nbDice *= onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1;

          if(nbDice > 0)
          {
            string logString = "";
            int damage = 0;

            for (int i = 0; i < nbDice; i++)
            {
              int roll = NwRandom.Roll(Utils.random, 6);
              logString += $"{roll} + ";
              damage += roll;
            }

            LogUtils.LogMessage($"Coup au But - {nbDice}d6 : {logString.Remove(logString.Length - 2)} = {damage}", LogUtils.LogType.Combat);

            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Divine)));
          }

          break;
      }      
    }
    public static async void OnDamageCoupAuBut(OnCreatureDamage onDamage)
    {
      if (onDamage.DamagedBy is not NwCreature caster)
        return;

      int baseDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

      if (baseDamage > 0)
      {
        int radiantDamage = onDamage.DamageData.GetDamageByType(DamageType.Divine);

        if (radiantDamage < 0)
          radiantDamage += 1;

        onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, -1);
        onDamage.DamageData.SetDamageByType(DamageType.Divine, radiantDamage + baseDamage);
      }


      await NwTask.NextFrame();
      caster.OnCreatureAttack -= OnAttackCoupAuBut;
      caster.OnCreatureDamage -= OnDamageCoupAuBut;
    }
  }
}

