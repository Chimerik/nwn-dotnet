using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyVagabondage()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerVagabondage))
        {
          EffectSystem.ApplyVagabondage(oid.LoginCreature);
        }
      }
    }
  }
}
