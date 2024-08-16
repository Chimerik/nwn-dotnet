using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyColdAffinity()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFroid)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ColdAffinityEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ColdAffinity));
      }
    }
  }
}
