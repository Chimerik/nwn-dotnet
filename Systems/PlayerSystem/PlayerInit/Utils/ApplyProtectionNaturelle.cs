using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyProtectionNaturelle()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideProtectionNaturelle)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionNaturelleEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionNaturelle(this)));
      }
    }
  }
}
