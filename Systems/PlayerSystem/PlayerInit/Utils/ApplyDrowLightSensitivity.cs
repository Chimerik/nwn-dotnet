using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyDrowLightSensitivity()
      {
        switch(oid.LoginCreature.Race.Id)
        {
          case CustomRace.Drow:
          case CustomRace.Duergar:

            if (NwModule.Instance.IsNight || location.Area is null || location.Area.IsInterior 
              || location.Area.IsUnderGround || oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.lightSensitivityEffectTag))
              return;

            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.lightSensitivity);

            break;

          default: return;
        }
      }
    }
  }
}
