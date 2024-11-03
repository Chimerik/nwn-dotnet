using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyBouclierPsychique()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BouclierPsychique)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.BouclierPsychiqueEffectTag))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BouclierPsychique));
        }
      }
    }
  }
}
