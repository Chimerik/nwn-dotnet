using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDungeonExpert(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.AddFeat(Feat.KeenSense);
      
      return true;
    }
  }
}
