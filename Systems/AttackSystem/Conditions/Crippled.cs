using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetCrippledModifiedDuration(PlayerSystem.Player player, NwGameObject targetObject, WeaponAttackType attackType, double duration)
    {
      if (targetObject is not NwCreature targetCreature)
        return 0;

      bool applyCrippled = true;
      double durationIncrease = 1;

      durationIncrease += 0.04 * GetCrippledDurationModifierFromItem(attackType == WeaponAttackType.Offhand ? player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand) : player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand), CustomInscription.Handicapant);

      if (targetCreature.IsLoginPlayerCharacter)
      {
        durationIncrease -= 0.05 * GetCrippledDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Hardiesse);
        durationIncrease -= 0.05 * GetCrippledDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Hardiesse);
        durationIncrease -= 0.05 * GetCrippledDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Hardiesse);
      }

      duration *= durationIncrease;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_CRIPPLED")
        {
          if (eff.DurationRemaining > duration)
            applyCrippled = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyCrippled ? (int)Math.Round(duration, MidpointRounding.ToEven) : 0;
    }
    private static double GetCrippledDurationModifierFromItem(NwItem item, int inscription)
    {
      double durationModifier = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            durationModifier += 1;

      return durationModifier;
    }
    public static ScriptHandleResult ApplyCrippled(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      //oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)115)); // TODO : choisir un effet visuel pour l'infirmité

      if(oTarget.MovementRateFactor > 0.5)
        oTarget.MovementRateFactor = 0.5f;

      return ScriptHandleResult.Handled;
    }

    public static ScriptHandleResult RemoveCrippled(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.MovementRateFactor = 1;

      return ScriptHandleResult.Handled;
    }
  }
}
