using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static async void OnAttackPatteMielleuse(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      StringUtils.DisplayStringToAllPlayersNearTarget(onAttack.Attacker, "Patte Mielleuse", StringUtils.gold);

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwItem weapon = target.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is not null)
          {
            if (weapon.GetObjectVariable<LocalVariableInt>("_ARME_LIEE").HasNothing || EffectUtils.IsIncapacitated(onAttack.Attacker))
            {
              target.RunUnequip(weapon);

              target.OnItemEquip -= ItemSystem.OnEquipDesarmement;
              target.OnItemEquip += ItemSystem.OnEquipDesarmement;

              target.ApplyEffect(EffectDuration.Temporary, EffectSystem.warMasterDesarmement, NwTimeSpan.FromRounds(1));
            }
            else
              onAttack.Attacker?.Master?.LoginPlayer.SendServerMessage($"L'arme de {target.Name.ColorString(ColorConstants.Cyan)} est liée et ne peut être désarmée");
          }
          else if (target.Size <= onAttack.Attacker.Size + 1)
          {
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(1));
          }
          else
            onAttack.Attacker?.Master?.LoginPlayer.SendServerMessage("Impossible de renverser une créature de cette taille !", ColorConstants.Red);

          break;
      }

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackPatteMielleuse;
    }
  }
}
