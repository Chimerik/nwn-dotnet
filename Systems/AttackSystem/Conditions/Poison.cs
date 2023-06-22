using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetPoisonModifiedDuration(PlayerSystem.Player player, NwGameObject targetObject, WeaponAttackType attackType, double duration)
    {
      if (targetObject is not NwCreature targetCreature || targetCreature.Race.RacialType == RacialType.Construct || targetCreature.Race.RacialType == RacialType.Undead)
      {
        player.oid.SendServerMessage($"La cible {StringUtils.ToWhitecolor(targetObject.Name)} ne peut pas être affectée par le poison", ColorConstants.Orange);
        return 0;
      }

      bool applyPoison = true;
      double durationIncrease = 1;

      durationIncrease += 0.04 * GetPoisonDurationModifierFromItem(attackType == WeaponAttackType.Offhand ? player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand) : player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand), CustomInscription.Venimeuse);

      if (targetCreature.IsLoginPlayerCharacter)
      {
        durationIncrease -= 0.05 * GetPoisonDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Mithridate);
        durationIncrease -= 0.05 * GetPoisonDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Mithridate);
        durationIncrease -= 0.05 * GetPoisonDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Mithridate);
      }

      duration *= durationIncrease;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_POISON")
        {
          if (eff.DurationRemaining > duration)
            applyPoison = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyPoison ? (int)Math.Round(duration, MidpointRounding.ToEven) : 0;
    }
    private static double GetPoisonDurationModifierFromItem(NwItem item, int inscription)
    {
      double durationModifier = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            durationModifier += 1;

      return durationModifier;
    }
    public static ScriptHandleResult ApplyPoison(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)115)); // TODO : choisir un effet visuel pour l'empoisonnement

      /*if(PlayerSystem.Players.TryGetValue(oTarget, out PlayerSystem.Player player))
      {

      }
      else 
      {

      }*/

      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult IntervalPoison(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)491)); // TODO : choisir un effet visuel pour l'empoisonnement

      return ScriptHandleResult.Handled;
    }
  }
}
