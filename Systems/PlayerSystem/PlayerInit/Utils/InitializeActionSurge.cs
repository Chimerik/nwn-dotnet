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
          && oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Fighter, (ClassType)CustomClass.EldritchKnight) && c.Level < 17))
          oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSurge, 1);
      }
    }
  }
}
