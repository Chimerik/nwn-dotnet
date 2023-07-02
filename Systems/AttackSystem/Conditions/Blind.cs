using Anvil.API;
using Anvil.Services;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetBlindModifiedDuration(NwGameObject targetObject, double duration)
    {
      if (targetObject is not NwCreature targetCreature)
        return 0;

      bool applyBlind = true;
      double durationIncrease = 1;

      if (targetCreature.IsLoginPlayerCharacter)
      {
        durationIncrease -= 0.05 * GetBlindDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Clarté);
        durationIncrease -= 0.05 * GetBlindDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Clarté);
        durationIncrease -= 0.05 * GetBlindDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Clarté);
      }

      duration *= durationIncrease;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_BLIND")
        {
          if (eff.DurationRemaining > duration)
            applyBlind = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyBlind ? (int)Math.Round(duration, MidpointRounding.ToEven) : 0;
    }
    private static double GetBlindDurationModifierFromItem(NwItem item, int inscription)
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
