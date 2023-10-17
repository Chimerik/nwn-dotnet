using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnDmLogoutRemoveThreatRange(OnDMPlayerDMLogout onLogin)
    {
      CreatureUtils.InitThreatRange(onLogin.DungeonMaster.LoginCreature); 
    }
  }
}
