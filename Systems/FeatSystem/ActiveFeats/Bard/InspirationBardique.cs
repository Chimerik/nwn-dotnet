using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void InspirationBardique(NwCreature caster, NwGameObject targetObject)
    {
      if(targetObject is not NwCreature target)
      {
        caster?.LoginPlayer.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      if (caster.KnowsFeat((Feat)CustomSkill.MotsCinglants) && caster.IsReactionTypeHostile(target))
      {
        if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.MotsCinglantsEffectTag))
        {
          caster?.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est déjà affectée par {StringUtils.ToWhitecolor("Mots Cinglants")}", ColorConstants.Orange);
          return;
        }

        if (!CreatureUtils.HandleBonusActionUse(caster))
          return;

        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMotsCinglantsEffect(target.GetClassInfo(ClassType.Bard).Level)));
        StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Mots Cinglants")} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red, true, true);
      }
      else
      {
        if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.InspirationBardiqueEffectTag))
        {
          caster?.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} bénéficie déjà d'{StringUtils.ToWhitecolor("Inspiration Bardique")}", ColorConstants.Orange);
          return;
        }

        if (!CreatureUtils.HandleBonusActionUse(caster))
          return;

        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetInspirationBardiqueEffect(target.GetClassInfo(ClassType.Bard).Level)));
        StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Inspiration Bardique")} sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      }
      
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
    }
  }
}
