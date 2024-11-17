using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMonkParade()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MonkParade)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkParadeEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.MonkParade));
      }
    }
  }
}
