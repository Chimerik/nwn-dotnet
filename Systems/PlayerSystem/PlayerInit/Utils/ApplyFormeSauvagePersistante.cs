using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFormeSauvagePersistante()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.FormeSauvagePersistante))
        {
          oid.OnCombatStatusChange -= DruideUtils.OnCombatDruideRecoverFormeSauvage;
          oid.OnCombatStatusChange += DruideUtils.OnCombatDruideRecoverFormeSauvage;
        }
      }
    }
  }
}
