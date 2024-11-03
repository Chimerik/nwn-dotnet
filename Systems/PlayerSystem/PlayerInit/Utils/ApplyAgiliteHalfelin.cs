using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAgiliteHalfelin()
      {
        if (oid.LoginCreature.Race.RacialType == RacialType.Halfling
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AgiliteHalfelinEffectTag))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AgiliteHalfelin));
        }
      }
    }
  }
}
