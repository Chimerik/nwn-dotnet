using System.Linq;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class ModuleSystem
  {
    [ScriptHandler("apply_drow_sensi")]
    private void OnDawnApplyDrowLightSensitivity()
    {
      foreach (NwPlayer player in NwModule.Instance.Players)
      {
        if (!player.LoginCreature.IsValid)
          continue;

        switch(player.LoginCreature.Race.Id)
        {
          case CustomRace.Drow:
          case CustomRace.Duergar:

            if(player.LoginCreature.Area is not null
              && !player.LoginCreature.Area.IsInterior && !player.LoginCreature.Area.IsUnderGround
              && !player.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.lightSensitivity.Tag))
                player.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.lightSensitivity);

            break;
        }
      }
    }
  }
}
