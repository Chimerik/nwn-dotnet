using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyOeilDeSorciere()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.OeilDeSorciere)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.VisionLucideEffectTag))
        {
          var eff = EffectSystem.VisionLucide;
          eff.SubType = EffectSubType.Unyielding;

          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, eff));
        }
      }
    }
  }
}
