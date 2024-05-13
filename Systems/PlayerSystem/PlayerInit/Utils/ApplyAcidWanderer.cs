using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAcidWanderer()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerAcidWanderer)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AcidWandererEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AcidWanderer));
      }
    }
  }
}
