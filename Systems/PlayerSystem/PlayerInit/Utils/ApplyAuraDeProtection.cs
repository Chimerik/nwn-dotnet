using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeProtection()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeProtection)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeProtectionEffectTag && e.Creator == oid.LoginCreature))
            NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeProtectionEffect(oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level)));
      }
    }
  }
}
