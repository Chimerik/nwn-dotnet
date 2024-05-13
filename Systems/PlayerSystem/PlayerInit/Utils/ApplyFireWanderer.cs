using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFireWanderer()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerFireWanderer)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.FireWandererEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.FireWanderer));
      }
    }
  }
}
