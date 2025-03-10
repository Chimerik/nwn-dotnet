using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAgresseurSauvage()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AgresseurSauvage))
          EffectSystem.ApplyAgresseurSauvage(oid.LoginCreature);
      }
    }
  }
}
