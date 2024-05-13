using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPoisonWanderer()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerPoisonWanderer)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.PoisonWandererEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoisonWanderer));
      }
    }
  }
}
