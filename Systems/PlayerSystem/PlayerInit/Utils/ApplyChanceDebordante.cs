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
      private void ApplyChanceDebordante()
      {
        if (learnableSkills.TryGetValue(CustomSkill.ChanceDebordante, out var protection) && protection.currentLevel > 0)
        {
          if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChanceDebordanteAuraEffectTag))
            NWScript.AssignCommand(oid.LoginCreature, () => oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.chanceDebordanteAura));
        }
      }
    }
  }
}
