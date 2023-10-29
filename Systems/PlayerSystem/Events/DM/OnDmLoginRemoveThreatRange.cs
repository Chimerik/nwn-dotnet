using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public static void OnDmLoginRemoveThreatRange(OnDMPlayerDMLogin onLogin)
    {
      foreach(var eff in onLogin.DungeonMaster.LoginCreature.ActiveEffects)
        if(eff.Tag == EffectSystem.ThreatenedAoETag)
          onLogin.DungeonMaster.LoginCreature.RemoveEffect(eff); 
    }
  }
}
