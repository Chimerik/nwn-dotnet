using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void DegatsVaillants(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature target)
      {
        caster?.LoginPlayer.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if(target.ActiveEffects.Any(e => e.Tag == EffectSystem.DegatsVaillanteEffectTag))
      {
        caster?.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est déjà affectée par {StringUtils.ToWhitecolor("Dégâts Vaillants")}", ColorConstants.Orange);
        return;
      }

      int inspiBonus = EffectSystem.GetInspirationBardiqueBonus(caster.GetClassInfo(ClassType.Bard).Level);
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetDegatsVaillanteEffect(target,  inspiBonus)));

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Dégâts Vaillants")} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red, true, true);

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DefenseVaillante);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DegatsVaillants);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteDefensive);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteTranchante);
    }
  }
}
