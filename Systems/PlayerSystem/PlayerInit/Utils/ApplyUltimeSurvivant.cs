using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyUltimeSurvivant()
      {
        if(learnableSkills.ContainsKey(CustomSkill.FighterChampionUltimeSurvivant))
        {
          oid.LoginCreature.OnHeartbeat -= FighterUtils.OnHeartbeatUltimeSurvivant;
          oid.LoginCreature.OnHeartbeat += FighterUtils.OnHeartbeatUltimeSurvivant;
        }
      }
    }
  }
}
