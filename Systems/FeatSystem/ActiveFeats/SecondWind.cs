using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void SecondWind(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value > 0)
      {
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

        int fighterLevel = caster.LevelInfo.Count(c => c.ClassInfo.Class.Id == CustomClass.Fighter
        || c.ClassInfo.Class.Id == CustomClass.Champion || c.ClassInfo.Class.Id == CustomClass.EldritchKnight
        || c.ClassInfo.Class.Id == CustomClass.ArcaneArcher || c.ClassInfo.Class.Id == CustomClass.Warmaster);
        caster.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Heal(NwRandom.Roll(Utils.random, 10, 1) + fighterLevel), Effect.VisualEffect(VfxType.ImpHealingM)));

        caster.DecrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.FighterSecondWind));

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} utilise {"Second Souffle".ColorString(ColorConstants.White)}", ColorConstants.Orange, true);
      }
      else
        caster?.LoginPlayer?.SendServerMessage("Vous avez déjà utilisé toutes vos actions bonus pour ce round !", ColorConstants.Orange);
    }
  }
}
