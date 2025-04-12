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
      private void ApplyTraqueur3()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Traqueur3)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AntidetectionEffectTag && e.DurationType == EffectDuration.Permanent))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Antidetection(false)));
        }
      }
    }
  }
}
