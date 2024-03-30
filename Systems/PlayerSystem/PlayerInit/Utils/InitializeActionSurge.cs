using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeActionSurge()
      {
        if (oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.FighterSurge) > 1
          && oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Fighter && c.Level < 17))
          oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSurge, 1);
      }
    }
  }
}
