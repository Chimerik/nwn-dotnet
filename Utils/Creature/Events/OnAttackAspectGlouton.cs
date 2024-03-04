using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnAttackAspectGlouton(OnCreatureAttack onAttack)
    {
      if (onAttack.IsRangedAttack)
        return;

      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(weapon.BaseItem))
        return;

      switch(onAttack.AttackResult) 
      {
        case AttackResult.Hit:
        case AttackResult.AutomaticHit:
        case AttackResult.CriticalHit:

          if (onAttack.Target is not NwCreature target)
            return;

          if (!target.ActiveEffects.Any(e => e.Tag == EffectSystem.SaignementEffectTag || e.EffectType == EffectType.Poison))
            return;

          target.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.CutsceneImmobilize(), Effect.VisualEffect(VfxType.DurEntangle)), NwTimeSpan.FromRounds(1));

          StringUtils.DisplayStringToAllPlayersNearTarget(target, "Aspect du Glouton", StringUtils.gold, true);

          break;
      }
      
    }
  }
}
