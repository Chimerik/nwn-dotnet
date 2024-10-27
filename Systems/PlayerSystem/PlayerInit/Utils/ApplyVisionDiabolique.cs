using System.Linq;
using Anvil.API;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyVisionDiabolique()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.VisionDiabolique)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.VisionDiaboliqueEffectTag))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.VisionDiabolique));
        }
      }
    }
  }
}
