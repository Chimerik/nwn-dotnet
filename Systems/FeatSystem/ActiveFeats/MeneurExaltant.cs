using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MeneurExaltant(NwCreature caster)
    {
      caster.LoginPlayer?.EnterTargetMode(SelectMeneurExaltantTargets, Config.selectCreatureMagicTargetMode);
      caster.LoginPlayer?.SendServerMessage("Sélectionnez jusqu'à 6 cibles", ColorConstants.Orange);
    }

    private static void SelectMeneurExaltantTargets(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      NwCreature caster = selection.Player.LoginCreature;
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_MENEUR_TARGETS").Value;

      if (selection.IsCancelled)
      {
        if (nbTargets > 0)
          HandleMeneurExaltant(caster);

        return;
      }

      if (selection.TargetObject is not NwCreature target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectMeneurExaltantTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_MENEUR_TARGET_{nbTargets}").Value = target;

      nbTargets += 1;
      caster.GetObjectVariable<LocalVariableInt>("_MENEUR_TARGETS").Value = nbTargets;

      if (nbTargets > 5)
      {
        HandleMeneurExaltant(caster);
      }
      else
      {
        caster.LoginPlayer.EnterTargetMode(SelectMeneurExaltantTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {6 - nbTargets} cible(s)", ColorConstants.Orange);
      }
    }

    private static void HandleMeneurExaltant(NwCreature caster)
    {
      int abilityModifier = caster.GetAbilityModifier(Ability.Wisdom) > caster.GetAbilityModifier(Ability.Charisma) ? caster.GetAbilityModifier(Ability.Wisdom) : caster.GetAbilityModifier(Ability.Charisma);

      for (int i = 0; i < 6; i++)
      {
        NwCreature target = caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_MENEUR_TARGET_{i}").Value;

        if (target is not null && caster.DistanceSquared(target) < 82)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(caster.Level + abilityModifier)));
          caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_MENEUR_TARGET_{i}").Delete();
        }
      }

      caster.GetObjectVariable<LocalVariableInt>("_MENEUR_TARGETS").Delete();
      FeatUtils.DecrementFeatUses(caster, CustomSkill.MeneurExaltant);
    }
  }
}
