 using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackSearingSmite(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
          {
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(Utils.Roll(6, onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1), DamageType.Fire), Effect.VisualEffect(VfxType.ImpFlameS))));
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.searingSmiteBurn, NwTimeSpan.FromRounds(Spells2da.spellTable[CustomSpell.SearingSmite].duration)));

            target.OnHeartbeat -= EffectSystem.OnSearingSmiteBurn;
            target.OnHeartbeat += EffectSystem.OnSearingSmiteBurn;

            await NwTask.NextFrame();
            onAttack.Attacker.OnCreatureAttack -= OnAttackSearingSmite;
          }

          break;
      }
    }
  }
}
