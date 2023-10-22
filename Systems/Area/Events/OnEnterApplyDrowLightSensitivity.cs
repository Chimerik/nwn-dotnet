using Anvil.API;
using Anvil.API.Events;
using System.Linq;

namespace NWN.Systems
{
  public partial class AreaSystem
  {
    private static void OnEnterApplyDrowLightSensitivity(AreaEvents.OnEnter onEnter)
    {
      if (onEnter.EnteringObject is not NwCreature creature)
        return;

      switch (creature.Race.Id)
      {
        case CustomRace.Drow:
        case CustomRace.Duergar:
          if (NwModule.Instance.IsNight || onEnter.Area.IsInterior || onEnter.Area.IsUnderGround
          || creature.ActiveEffects.Any(e => e.Tag == EffectSystem.lightSensitivity.Tag))
            return;

          creature.ApplyEffect(EffectDuration.Permanent, EffectSystem.lightSensitivity);

          break;

        default: return;
      }
    }
  }
}
