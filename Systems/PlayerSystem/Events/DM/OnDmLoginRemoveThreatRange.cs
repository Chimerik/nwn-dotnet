using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnDmLoginRemoveThreatRange(OnDMPlayerDMLogin onLogin)
    {
      onLogin.DungeonMaster.LoginCreature.RemoveEffect(EffectSystem.threatAoE); 
    }
  }
}
