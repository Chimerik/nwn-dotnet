using System.Security.Cryptography;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ModuleSystem
  {
    [ScriptHandler("remov_drowsensi")]
    private void OnDuskRemoveDrowLightSensitivity()
    {
      foreach (NwPlayer player in NwModule.Instance.Players)
        if (player.LoginCreature.Race.Id == CustomRace.Drow)
          player.LoginCreature.RemoveEffect(EffectSystem.lightSensitivity);
    }
  }
}
