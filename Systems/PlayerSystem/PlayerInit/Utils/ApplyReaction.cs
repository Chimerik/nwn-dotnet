using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyReaction()
      {
        if(!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ReactionEffectTag || (e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == EffectSystem.ReactionId)))
          EffectSystem.ApplyReaction(oid.LoginCreature);
        //int maxBonusAction = 1 + oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MainLeste).ToInt();

        //for (int nbBonusAction = oid.LoginCreature.ActiveEffects.Where(e => e.Tag == EffectSystem.BonusActionEffectTag || (e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == EffectSystem.BonusActionId)).GroupBy(e => e.LinkId).Count(); nbBonusAction < maxBonusAction; nbBonusAction++)

      }
    }
  }
}
