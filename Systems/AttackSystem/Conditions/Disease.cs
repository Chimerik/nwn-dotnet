using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(AttackSystem))]
  public partial class AttackSystem
  {
    public static int GetDiseaseModifiedDuration(PlayerSystem.Player player, NwGameObject targetObject, WeaponAttackType attackType, double duration)
    {
      if (targetObject is not NwCreature targetCreature || targetCreature.Race.RacialType == RacialType.Construct || targetCreature.Race.RacialType == RacialType.Undead)
      {
        player.oid.SendServerMessage($"La cible {StringUtils.ToWhitecolor(targetObject.Name)} ne peut pas être affectée par la maladie", ColorConstants.Orange);
        return 0;
      }

      bool applyDisease = true;
      double durationIncrease = 1;

      if (targetCreature.IsLoginPlayerCharacter)
      {
        durationIncrease -= 0.05 * GetDiseaseDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.Neck), CustomInscription.Pureté);
        durationIncrease -= 0.05 * GetDiseaseDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.RightRing), CustomInscription.Pureté);
        durationIncrease -= 0.05 * GetDiseaseDurationModifierFromItem(targetCreature.GetItemInSlot(InventorySlot.LeftRing), CustomInscription.Pureté);
      }

      duration *= durationIncrease;

      foreach (var eff in targetCreature.ActiveEffects)
      {
        if (eff.Tag == "CUSTOM_CONDITION_DISEASE")
        {
          if (eff.DurationRemaining > duration)
            applyDisease = false;
          else
            targetCreature.RemoveEffect(eff);
        }
      }

      return applyDisease ? (int)Math.Round(duration, MidpointRounding.ToEven) : 0;
    }
    private static double GetDiseaseDurationModifierFromItem(NwItem item, int inscription)
    {
      double durationModifier = 0;

      if (item is not null)
        for (int i = 0; i < item.GetObjectVariable<LocalVariableInt>("TOTAL_SLOTS").Value; i++)
          if (item.GetObjectVariable<LocalVariableInt>($"SLOT{i}").Value == inscription)
            durationModifier += 1;

      return durationModifier;
    }
    public static ScriptHandleResult IntervalDisease(CallInfo _)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature oTarget)
        return ScriptHandleResult.Handled;

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect((VfxType)491)); // TODO : trouver un effet visuel pour la maladie
      
      foreach(NwCreature creature in oTarget.Area.FindObjectsOfTypeInArea<NwCreature>())
      {
        if(oTarget.IsPlayableRace)
        {
          if(creature.IsPlayableRace)
          {
            foreach (var eff in creature.ActiveEffects)
              if (eff.Tag == "CUSTOM_CONDITION_DISEASE")
                return ScriptHandleResult.Handled;

            if (creature.DistanceSquared(oTarget) < 1)
              creature.ApplyEffect(EffectDuration.Temporary, disease, TimeSpan.FromSeconds(eventData.Effect.DurationRemaining));
          }
        }
        else 
        {
          if(oTarget.Race.RacialType == creature.Race.RacialType)
          {
            foreach (var eff in creature.ActiveEffects)
              if (eff.Tag == "CUSTOM_CONDITION_DISEASE")
                return ScriptHandleResult.Handled;

            if (creature.DistanceSquared(oTarget) < 1)
              creature.ApplyEffect(EffectDuration.Temporary, disease, TimeSpan.FromSeconds(eventData.Effect.DurationRemaining));
          }
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
