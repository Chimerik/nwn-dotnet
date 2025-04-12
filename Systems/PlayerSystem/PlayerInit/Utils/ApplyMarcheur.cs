using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMarcheur()
      {
        if(oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Marcheur3)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.Marcheur3EffectTag))
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Marcheur3);

        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Marcheur4)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.Marcheur4EffectTag))
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Marcheur4);
      }
    }
  }
}
