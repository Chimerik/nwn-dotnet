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
      private void ApplyAthleteAccompli()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FighterChampionAthleteAccompli))
        {
          oid.LoginCreature.OnCreatureAttack -= FighterUtils.OnAttackAthleteAccompli;
          oid.LoginCreature.OnCreatureAttack += FighterUtils.OnAttackAthleteAccompli;
        }
      }
    }
  }
}
