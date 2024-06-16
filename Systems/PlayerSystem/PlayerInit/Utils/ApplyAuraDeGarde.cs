using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAuraDeGarde()
      {
        if (learnableSkills.ContainsKey(CustomSkill.PaladinAuraDeGarde)
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeGardeEffectTag))
            NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeGarde(oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level)));
      }
    }
  }
}
