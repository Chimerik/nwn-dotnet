using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeNumAttackPerRound()
      {
        foreach(var playerClass in oid.LoginCreature.Classes)
        {
          switch(playerClass.Class.Id)
          {
            case CustomClass.Fighter:

              if (playerClass.Level > 4)
                oid.LoginCreature.BaseAttackCount += 1;

              if (playerClass.Level > 10)
                oid.LoginCreature.BaseAttackCount += 1;

              if (playerClass.Level > 19)
                oid.LoginCreature.BaseAttackCount += 1;

              break;

            case CustomClass.Barbarian:
            case CustomClass.Rogue:

              if (playerClass.Level > 4)
                oid.LoginCreature.BaseAttackCount += 1;

              break;
          }
        }
      }
    }
  }
}
