using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyTraqueurRedoutable()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TraqueurRedoutable))
        {
          oid.OnCombatStatusChange -= RangerUtils.OnCombatTraqueurRedoutable;
          oid.OnCombatStatusChange += RangerUtils.OnCombatTraqueurRedoutable;
        }
      }
    }
  }
}
