using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void InspirationBardique(NwCreature caster, NwGameObject targetObject)
    {
      if (targetObject is not NwCreature target)
      {
        caster?.LoginPlayer.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if(target.ActiveEffects.Any(e => e.Tag == EffectSystem.InspirationBardiqueEffectTag))
      {
        caster?.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est déjà affectée par {StringUtils.ToWhitecolor("Inspiration Bardique")}", StringUtils.gold);
        return;
      }

      

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      int inspiBonus = EffectSystem.GetInspirationBardiqueBonus(caster.GetClassInfo(ClassType.Bard).Level);

      if (caster.KnowsFeat((Feat)CustomSkill.MotsCinglants) && caster.IsReactionTypeHostile(target))
      {
        inspiBonus = -inspiBonus;
        StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Mots Cinglants")} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red, true, true);
      }
      else
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Inspiration Bardique")} sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      }

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetInspirationBardiqueEffect(inspiBonus), TimeSpan.FromHours(1)));

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DefenseVaillante);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.DegatsVaillants);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteDefensive);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BotteTranchante);
    }
  }
}
