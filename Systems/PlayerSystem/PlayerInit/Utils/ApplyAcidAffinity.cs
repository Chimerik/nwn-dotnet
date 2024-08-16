using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAcidAffinity()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteAcide)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AcidAffinityEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AcidAffinity));
      }
    }
  }
}
