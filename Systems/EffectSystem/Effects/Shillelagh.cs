using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ShillelaghEffectTag = "_SHILLELAGH_EFFECT";
    public static readonly Native.API.CExoString ShillelaghEffectExoTag = ShillelaghEffectTag.ToExoString();
    private static ScriptCallbackHandle onRemoveShillelaghCallback;
    public static void ApplyShillelagh(NwCreature caster, NwSpell spell, Ability casterAbility)
    {
      EffectUtils.RemoveTaggedEffect(caster, ShillelaghEffectTag);
      caster.OnCreatureDamage -= OnDamageShillelagh;

      Effect attack = Effect.LinkEffects(Effect.Icon(EffectIcon.AttackIncrease), Effect.RunAction(onRemovedHandle: onRemoveShillelaghCallback));
      attack.Tag = ShillelaghEffectTag;
      attack.SubType = EffectSubType.Supernatural;
      attack.IntParams[5] = (int)casterAbility;
      caster.ApplyEffect(EffectDuration.Permanent, attack);

      if (spell.Id == CustomSpell.ShillelaghForce) 
        caster.OnCreatureDamage += OnDamageShillelagh;
    }
    public static void OnDamageShillelagh(OnCreatureDamage onDamage)
    {
      int baseDamage = onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon);

      if (baseDamage > 0 && onDamage.DamagedBy is NwCreature caster)
      {
        NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

        if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Club, BaseItemType.Quarterstaff, BaseItemType.MagicStaff))
        {
          int forceDamage = onDamage.DamageData.GetDamageByType(DamageType.Magical);

          if (forceDamage < 0)
            forceDamage += 1;

          onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, -1);
          onDamage.DamageData.SetDamageByType(DamageType.Magical, forceDamage + baseDamage);
        }
      }
    }
    private static ScriptHandleResult onRemoveShillelagh(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target)
      {
        target.OnCreatureDamage -= OnDamageShillelagh;
      }

      return ScriptHandleResult.Handled;
    }
  }
}

