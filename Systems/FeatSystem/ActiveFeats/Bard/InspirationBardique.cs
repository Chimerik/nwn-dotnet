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

      var previousInspi = target.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.InspirationBardiqueEffectTag);
      int inspiBonus = EffectSystem.GetInspirationBardiqueBonus(caster.GetClassInfo(ClassType.Bard).Level);

      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      if (caster.KnowsFeat((Feat)CustomSkill.MotsCinglants) && caster.IsReactionTypeHostile(target))
      {
        if (previousInspi is not null)
        {
          if (previousInspi.CasterLevel < 0)
          {
            caster?.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} est déjà affectée par {StringUtils.ToWhitecolor("Mots Cinglants")}", ColorConstants.Orange);
            return;
          }
          else
          {
            EffectUtils.RemoveTaggedEffect(target, EffectSystem.InspirationBardiqueEffectTag);
            StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Mots Cinglants")} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red, true, true);

            inspiBonus -= previousInspi.CasterLevel;
          }
        }
        else
        {
          inspiBonus = -inspiBonus;
          StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Mots Cinglants")} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red, true, true);
        } 
      }
      else
      {
        if (previousInspi is not null)
        {
          if (previousInspi.CasterLevel > 0)
          {
            caster?.LoginPlayer.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} bénéficie déjà d'{StringUtils.ToWhitecolor("Inspiration Bardique")}", ColorConstants.Orange);
            return;
          }
          else
          {
            EffectUtils.RemoveTaggedEffect(target, EffectSystem.InspirationBardiqueEffectTag);
            StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Inspiration Bardique")} sur {target.Name.ColorString(ColorConstants.Cyan)}", ColorConstants.Red, true, true);

           inspiBonus += previousInspi.CasterLevel;
          }
        }
        else
          StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{caster.Name.ColorString(ColorConstants.Cyan)} lance {StringUtils.ToWhitecolor("Inspiration Bardique")} sur {target.Name.ColorString(ColorConstants.Cyan)}", StringUtils.gold, true, true);
      }

      if (inspiBonus != 0)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetInspirationBardiqueEffect(inspiBonus)));      

      caster.DecrementRemainingFeatUses((Feat)CustomSkill.BardInspiration);
    }
  }
}
