using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeCourage()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeCourage)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeCourageEffectTag && e.Creator == oid.LoginCreature))
            NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeCourage));
      }
    }
  }
}
