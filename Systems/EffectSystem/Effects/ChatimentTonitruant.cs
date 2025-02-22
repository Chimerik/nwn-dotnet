using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentTonitruantEffectTag = "_CHATIMENT_TONITRUANT_EFFECT";
    public static Effect ChatimentTonitruant(NwCreature caster, NwSpell spell)
    {
      EffectUtils.ClearChatimentEffects(caster);
      caster.OnCreatureAttack -= OnAttackChatimentTonitruant;
      caster.OnCreatureAttack += OnAttackChatimentTonitruant;

      Effect eff = Effect.RunAction();
      eff.Tag = ChatimentTonitruantEffectTag;
      eff.Spell = spell;
      return eff;
    }
    private static async void OnAttackChatimentTonitruant(OnCreatureAttack onAttack)
    {   
      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
          {
            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(Utils.Roll(6, onAttack.AttackResult == AttackResult.CriticalHit ? 4 : 2), DamageType.Sonic), Effect.VisualEffect(VfxType.FnfSoundBurst))));

            if (onAttack.Target is NwCreature target)
              ApplyKnockdown(target, onAttack.Attacker, Ability.Charisma, Ability.Strength, Destabilisation);
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackChatimentTonitruant;

          break;
      }
    }
  }
}
