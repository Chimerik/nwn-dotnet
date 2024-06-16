using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeDevotion()
      {
        if (learnableSkills.ContainsKey(CustomSkill.PaladinAuraDeDevotion)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeDevotionEffectTag))
            NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeDevotion(oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level)));
      }
    }
  }
}
