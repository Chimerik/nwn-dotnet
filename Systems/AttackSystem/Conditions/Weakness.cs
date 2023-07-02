using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetWeaknesModifiedDuration(PlayerSystem.Player player, NwGameObject targetObject, WeaponAttackType attackType, double duration)
    {
      if (targetObject is not NwCreature targetCreature)
        return 0;

      bool applyWeakness = true;
      double durationIncrease = 1;

      durationIncrease += 0.04 * GetWeaknessDurationModifierFromItem(attackType == WeaponAttackType.Offhand ? player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand) : player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand), CustomInscription.Pesanteur);

      if (targetCreature.IsLoginPlayerCharacter)
      {
        durationIncrease -= 0.05 * GetWeaknessDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Ardeur);
        durationIncrease -= 0.05 * GetWeaknessDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Ardeur);
        durationIncrease -= 0.05 * GetWeaknessDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Ardeur);
      }

      duration *= durationIncrease;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_WEAKNESS")
        {
          if (eff.DurationRemaining > duration)
            applyWeakness = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyWeakness ? (int)Math.Round(duration, MidpointRounding.ToEven) : 0;
    }
    private static double GetWeaknessDurationModifierFromItem(NwItem item, int inscription)
    {
      double durationModifier = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            durationModifier += 1;

      return durationModifier;
    }
    public static ScriptHandleResult ApplyWeakness(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      if(PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player player))
      {
        // TODO : réduire tous les attributs de la cible de 1
      }

      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult RemoveWeakness(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      if (PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player player))
      {
        // TODO : rétablir tous les attributs de la cible à leur valeur d'origine
      }

      return ScriptHandleResult.Handled;
    }
  }
}
