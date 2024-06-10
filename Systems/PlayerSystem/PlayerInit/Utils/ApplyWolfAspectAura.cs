using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyWolfAspectAura()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.TotemAspectLoup) && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.WolfTotemAuraEffectTag))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.wolfAspectAura));
      }
    }
  }
}
