using System.Linq;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAttaquesEtudiees()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FighterAttaquesEtudiees))
        {
          oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackAttaquesEtudiees;
          oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackAttaquesEtudiees;
        }
      }
    }
  }
}
