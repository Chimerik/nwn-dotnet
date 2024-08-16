using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyFireAffinity()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFeu)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.FireAffinityEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.FireAffinity));
      }
    }
  }
}
