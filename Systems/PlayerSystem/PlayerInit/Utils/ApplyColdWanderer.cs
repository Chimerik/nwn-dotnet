using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyColdWanderer()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerColdWanderer)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ColdWandererEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ColdWanderer));
      }
    }
  }
}
