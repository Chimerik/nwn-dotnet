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
      private void ApplyPuretePhysique()
      {
        if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 9)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.PuretePhysiqueEffectTag))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PuretePhysique));
        }
      }
    }
  }
}
