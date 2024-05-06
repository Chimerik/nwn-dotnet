using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyContreCharme()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ContreCharme)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ContreCharmeAuraEffectTag && e.Creator == oid.LoginCreature))
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ContreCharmeAura));
      }
    }
  }
}
