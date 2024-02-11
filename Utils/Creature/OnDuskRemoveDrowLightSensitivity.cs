using System.Diagnostics.CodeAnalysis;
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
      {
        if (player.LoginCreature is not null)
        {
          switch (player.LoginCreature.Race.Id)
          {
            case CustomRace.Drow:
            case CustomRace.Duergar:

              foreach (var eff in player.LoginCreature.ActiveEffects)
                if (eff.Tag == EffectSystem.lightSensitivityEffectTag)
                  player.LoginCreature.RemoveEffect(eff);

              break;
          }
        }
      }
    }
  }
}
