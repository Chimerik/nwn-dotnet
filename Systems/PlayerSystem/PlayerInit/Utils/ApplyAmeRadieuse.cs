using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAmeRadieuse()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AmeRadieuse) 
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmeRadieuseEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AmeRadieuse));
      }
    }
  }
}
