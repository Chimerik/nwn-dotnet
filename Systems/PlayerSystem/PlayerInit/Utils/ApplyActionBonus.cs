using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyActionBonus()
      {
        int maxBonusAction = 1 + oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MainLeste).ToInt();
        
        for (int nbBonusAction = oid.LoginCreature.ActiveEffects.Where(e => e.Tag == EffectSystem.BonusActionEffectTag || (e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == EffectSystem.BonusActionId)).GroupBy(e => e.LinkId).Count(); nbBonusAction < maxBonusAction; nbBonusAction++)
          EffectSystem.ApplyActionBonus(oid.LoginCreature);
      }
    }
  }
}
