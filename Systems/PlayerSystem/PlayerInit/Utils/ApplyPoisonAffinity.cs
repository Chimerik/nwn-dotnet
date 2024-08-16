using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPoisonAffinity()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffinitePoison)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.PoisonAffinityEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoisonAffinity));
      }
    }
  }
}
