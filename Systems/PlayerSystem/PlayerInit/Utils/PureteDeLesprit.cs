using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPureteDeLesprit()
      {
        if (learnableSkills.TryGetValue(CustomSkill.PaladinSermentDevotion, out var devotion) && devotion.currentLevel > 14
          && !oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionContreLeMalEtLeBienEffectTag && e.Creator == oid.LoginCreature))
        {
          NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionContreLeMalEtLeBien));
        }
      }
    }
  }
}
