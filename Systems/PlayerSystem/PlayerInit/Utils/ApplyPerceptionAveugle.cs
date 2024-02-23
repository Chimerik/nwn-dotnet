using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPerceptionAveugle()
      {
        if(oid.LoginCreature.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 13)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.PerceptionAveugleAuraEffectTag))
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.perceptionAveugleAura);
      }
    }
  }
}
