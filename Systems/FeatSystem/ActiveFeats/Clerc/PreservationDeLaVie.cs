using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void PreservationDeLaVie(NwCreature caster)
    {
      caster.ControllingPlayer?.EnterTargetMode(SelectAdditionnalTargets, Config.selectCreatureMagicTargetMode);
      caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS_TO_SELECT").Value = 5;
      caster.ControllingPlayer?.SendServerMessage($"Sélectionnez jusqu'à 5 cibles", ColorConstants.Orange);
    }
    private static void SelectAdditionnalTargets(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      NwCreature caster = selection.Player.LoginCreature;
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS").Value;

      if (selection.IsCancelled)
      {
        if (nbTargets > 0)
        {
          CastPreservationDeLaVie(caster);
        }

        return;
      }

      if (selection.TargetObject is not NwCreature target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_MULTI_TARGET_{nbTargets}").Value = target;

      nbTargets += 1;
      caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS").Value = nbTargets;
      int remaningTargets = caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS_TO_SELECT").Value - nbTargets;

      if (remaningTargets < 1)
      {
        CastPreservationDeLaVie(caster);
      }
      else
      {
        caster.LoginPlayer.EnterTargetMode(SelectAdditionnalTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {remaningTargets} cible(s)", ColorConstants.Orange);
      }
    }
    private static void CastPreservationDeLaVie(NwCreature caster)
    {
      List<NwGameObject> targets = GetMultiTargets(caster);
      int clercLevel = caster.GetClassInfo((ClassType)CustomClass.Clerc) is null ? 0 : caster.GetClassInfo((ClassType)CustomClass.Clerc).Level;
      int healAmount = 5 * clercLevel / targets.Count;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadHoly));

      foreach(var target in targets)
     {
       ModuleSystem.Log.Info($"target : {target.Name} - {healAmount}");
       NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(healAmount)));
       target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
     }

      ClercUtils.ConsumeConduitDivin(caster);
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Préservation de la Vie", StringUtils.gold, true, true);
    }
    private static List<NwGameObject> GetMultiTargets(NwCreature caster)
    {
      List<NwGameObject> targets = new();

      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS").Value;

      for (int i = 0; i < nbTargets; i++)
      {
        NwGameObject multiTarget = caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_MULTI_TARGET_{i}").Value;

        float distance = caster.DistanceSquared(multiTarget);

        if (-1 < distance && distance < 82)
          targets.Add(multiTarget);
        else 
          caster.LoginPlayer?.SendServerMessage($"{multiTarget.Name} n'est plus à portée", ColorConstants.Orange);
        

        caster.GetObjectVariable<LocalVariableObject<NwGameObject>>($"_MULTI_TARGET_{i}").Delete();
      }
     
      caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS").Delete();
      caster.GetObjectVariable<LocalVariableInt>("_MULTI_TARGETS_TO_SELECT").Delete();
      return targets;
    }
  }
}
