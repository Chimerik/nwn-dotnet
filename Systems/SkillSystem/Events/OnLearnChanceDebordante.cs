using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnChanceDebordante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChanceDebordanteAuraEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.chanceDebordanteAura));

      return true;
    }
  }
}
