using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeActionSurge()
      {
        if (oid.LoginCreature.GetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.FighterSurge)) > 1
          && oid.LoginCreature.GetClassInfo(NwClass.FromClassType(ClassType.Fighter))?.Level < 17)
          oid.LoginCreature.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.FighterSurge), 1);
      }
    }
  }
}
