using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetDazedDuration(PlayerSystem.Player player, NwGameObject targetObject, WeaponAttackType attackType, double duration)
    {
      if (targetObject is not NwCreature targetCreature)
        return 0;

      bool applyDazed = true;
      double durationIncrease = 1;

      durationIncrease += 0.04 * GetDazedModifierFromItem(attackType == WeaponAttackType.Offhand ? player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand) : player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand), CustomInscription.Mutisme);

      if (targetCreature.IsLoginPlayerCharacter)
      {
        durationIncrease -= 0.05 * GetDazedModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Récupération);
        durationIncrease -= 0.05 * GetDazedModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Récupération);
        durationIncrease -= 0.05 * GetDazedModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Récupération);
      }

      duration *= durationIncrease;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_DAZED")
        {
          if (eff.DurationRemaining > duration)
            applyDazed = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyDazed ? (int)Math.Round(duration, MidpointRounding.ToEven) : 0;
    }
    private static double GetDazedModifierFromItem(NwItem item, int inscription)
    {
      double durationModifier = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            durationModifier += 1;

      return durationModifier;
    }
  }
}
