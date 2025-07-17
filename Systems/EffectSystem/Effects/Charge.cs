using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChargeEffectTag = "_CHARGE_EFFECT";
    public const string ChargeDebuffEffectTag = "_CHARGE_DEBUFF_EFFECT";
    private static ScriptCallbackHandle onRemoveChargeCallback;
    public static Effect Charge(NwCreature caster)
    {
      caster.OnCreatureAttack -= OnAttackCharge;
      caster.OnCreatureAttack += OnAttackCharge;

      Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(50), Effect.RunAction(onRemovedHandle: onRemoveChargeCallback));
      eff.Tag = ChargeEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    public static Effect ChargeDebuff
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = ChargeDebuffEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onRemoveCharge(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.OnCreatureAttack -= OnAttackCharge;
      }

      return ScriptHandleResult.Handled;
    }
    public static async void OnAttackCharge(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is NwCreature target)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.CriticalHit:
          case AttackResult.AutomaticHit:

            NwItem weapon;

            switch (onAttack.WeaponAttackType)
            {
              case WeaponAttackType.MainHand:
              case WeaponAttackType.HastedAttack: weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand); break;
              default: return;
            }

            if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(onAttack.Attacker, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Halberd, BaseItemType.ShortSpear))
            {
              int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NativeUtils.GetAttackAbility(onAttack.Attacker, onAttack.IsRangedAttack, weapon));

              if (CreatureUtils.GetSavingThrowResult(target, Ability.Strength, onAttack.Attacker, spellDC) == SavingThrowResult.Failure)
              {
                NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, ChargeDebuff, NwTimeSpan.FromRounds(2)));
              }

              await NwTask.NextFrame();
              onAttack.Attacker.OnCreatureAttack -= OnAttackCharge;
            }

            break;
        }
      }
    }
  }
}

