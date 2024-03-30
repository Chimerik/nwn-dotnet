using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyElkAspect()
      {
        if(oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TotemAspectElan)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ElkAspectAuraEffectTag))
          oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.elkAspectAura);
      }
    }
  }
}
