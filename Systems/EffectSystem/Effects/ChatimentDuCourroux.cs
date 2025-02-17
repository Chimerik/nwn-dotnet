using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentDuCourrouxEffectTag = "_CHATIMENT_DU_COURROUX_EFFECT";
    public static Effect ChatimentDuCourroux(NwCreature caster, NwSpell spell)
    {
      EffectUtils.ClearChatimentEffects(caster);
      caster.OnCreatureAttack -= OnAttackChatimentDuCourroux;
      caster.OnCreatureAttack += OnAttackChatimentDuCourroux;

      Effect eff = Effect.RunAction();
      eff.Tag = ChatimentDuCourrouxEffectTag;
      eff.Spell = spell;
      return eff;
    }
    private static async void OnAttackChatimentDuCourroux(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
          {
            NWScript.AssignCommand(onAttack.Attacker, () => onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(Utils.Roll(6, onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1), CustomDamageType.Necrotic), Effect.VisualEffect(VfxType.ImpDiseaseS))));
            ApplyEffroi(target, onAttack.Attacker, SpellUtils.GetSpellDuration(onAttack.Attacker, Spells2da.spellTable[CustomSpell.ChatimentDuCourroux]), true);
          }

          await NwTask.NextFrame();
          onAttack.Attacker.OnCreatureAttack -= OnAttackChatimentDuCourroux;

          break;
      }
    }
  }
}
