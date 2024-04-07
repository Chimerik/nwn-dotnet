using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAbjurationWard()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AbjurationWard)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AbjurationWardEffectTag && e.Creator == oid.LoginCreature))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(oid.LoginCreature.GetClassInfo(ClassType.Wizard).Level)));

          oid.LoginCreature.OnDamaged -= WizardUtils.OnDamageAbjurationWard;
          oid.LoginCreature.OnDamaged += WizardUtils.OnDamageAbjurationWard;
        }
      }
    }
  }
}
