using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAssassinate()
      {
        if (oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.AssassinAssassinate)))
        {
          oid.OnCombatStatusChange -= RogueUtils.OnCombatAssassinate;
          oid.OnCombatStatusChange += RogueUtils.OnCombatAssassinate;
        }
      }
    }
  }
}
