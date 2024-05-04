using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyNecroticResistance()
      {
        if (learnableSkills.TryGetValue(CustomSkill.WizardNecromancie, out LearnableSkill necromancie) && necromancie.currentLevel > 9)
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.NecroticResistance));
      }
    }
  }
}
