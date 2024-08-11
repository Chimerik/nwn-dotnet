using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyRetablissementSorcier()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RetablissementSorcier))
        {
          oid.OnCombatStatusChange -= EnsoUtils.OnCombatEnsoRecoverSource;
          oid.OnCombatStatusChange += EnsoUtils.OnCombatEnsoRecoverSource;
        }
      }
    }
  }
}
