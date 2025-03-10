using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Musicien(NwCreature caster)
    {
      caster.LoginPlayer?.EnterTargetMode(SelectMusicienTargets, Config.selectCreatureMagicTargetMode);
      caster.LoginPlayer?.SendServerMessage($"Sélectionnez jusqu'à {NativeUtils.GetCreatureProficiencyBonus(caster)} cibles", ColorConstants.Orange);
    }

    private static void SelectMusicienTargets(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      NwCreature caster = selection.Player.LoginCreature;
      int nbTargets = caster.GetObjectVariable<LocalVariableInt>("_MENEUR_TARGETS").Value;

      if (selection.IsCancelled)
      {
        if (nbTargets > 0)
          HandleMusicien(caster, nbTargets);

        return;
      }

      if (selection.TargetObject is not NwCreature target)
      {
        caster.LoginPlayer.EnterTargetMode(SelectMusicienTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage("Veuillez sélectionner une cible valide", ColorConstants.Red);
        return;
      }

      caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_MENEUR_TARGET_{nbTargets}").Value = target;

      nbTargets += 1;
      caster.GetObjectVariable<LocalVariableInt>("_MENEUR_TARGETS").Value = nbTargets;

      int maxTargets = NativeUtils.GetCreatureProficiencyBonus(caster);

      if (nbTargets > maxTargets)
      {
        HandleMusicien(caster, maxTargets);
      }
      else
      {
        caster.LoginPlayer.EnterTargetMode(SelectMeneurExaltantTargets, Config.selectCreatureMagicTargetMode);
        caster.LoginPlayer.SendServerMessage($"Vous pouvez encore choisir {maxTargets - nbTargets} cible(s)", ColorConstants.Orange);
      }
    }

    private static void HandleMusicien(NwCreature caster, int maxTargets)
    {
      for (int i = 0; i < maxTargets; i++)
      {
        NwCreature target = caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_MENEUR_TARGET_{i}").Value;

        if (target is not null && caster.Area == target.Area && target.GetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique) < 3)
        {
          target.IncrementRemainingFeatUses((Feat)CustomSkill.InspirationHeroique);
          target.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.DurBardSong), NwTimeSpan.FromRounds(2));
        }

        caster.GetObjectVariable<LocalVariableObject<NwCreature>>($"_MENEUR_TARGET_{i}").Delete();
      }

      caster.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.DurBardSong), NwTimeSpan.FromRounds(2));
      caster.GetObjectVariable<LocalVariableInt>("_MENEUR_TARGETS").Delete();
      FeatUtils.DecrementFeatUses(caster, CustomSkill.Musicien);
    }
  }
}
