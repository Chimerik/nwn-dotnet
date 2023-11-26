using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyCrossbowMaster()
      {
        if (oid.LoginCreature.KnowsFeat(Feat.RapidReload))
        {
          oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackCrossbowMaster;
          oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackCrossbowMaster;
        }
      }
    }
  }
}
