using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentAveuglantEffectTag = "_CHATIMENT_AVEUGLANT_EFFECT";
    private static ScriptCallbackHandle onIntervalChatimentAveuglantCallback;
    public static Effect ChatimentAveuglant(NwCreature caster)
    {
      EffectUtils.ClearChatimentEffects(caster);
      caster.OnCreatureAttack -= OnAttackChatimentAveuglant;
      caster.OnCreatureAttack += OnAttackChatimentAveuglant;

      Effect eff = Effect.Icon(CustomEffectIcon.ChatimentAveuglant);
      eff.Tag = ChatimentAveuglantEffectTag;
      eff.Spell = NwSpell.FromSpellId(CustomSpell.ChatimentAveuglant);
      return eff;
    }

    private static async void OnAttackChatimentAveuglant(OnCreatureAttack onAttack)
    {   
      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
          {
            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(Utils.Roll(8, onAttack.AttackResult == AttackResult.CriticalHit ? 6 : 3), DamageType.Divine), Effect.VisualEffect(VfxType.FnfStrikeHoly))));

            if (onAttack.Target is NwCreature target)
              NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Blindness(),
                Effect.RunAction(onIntervalHandle:onIntervalChatimentAveuglantCallback, interval:NwTimeSpan.FromRounds(1))), NwTimeSpan.FromRounds(10)));
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackChatimentAveuglant;

          break;
      }
    }

    private static ScriptHandleResult OnIntervalChatimentAveuglant(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature target && eventData.Effect.Creator is NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Charisma);

        if (CreatureUtils.GetSavingThrow(caster, target, Ability.Constitution, spellDC) != SavingThrowResult.Failure)
          target.RemoveEffect(eventData.Effect);
      }
      else if (eventData.EffectTarget is NwGameObject oTarget)
        oTarget.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}
