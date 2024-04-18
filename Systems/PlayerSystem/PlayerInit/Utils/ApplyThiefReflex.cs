using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyThiefReflex()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ThiefReflex))
        {
          oid.OnCombatStatusChange -= RogueUtils.OnCombatThiefReflex;
          oid.OnCombatStatusChange += RogueUtils.OnCombatThiefReflex;
        }
      }
    }
  }
}
