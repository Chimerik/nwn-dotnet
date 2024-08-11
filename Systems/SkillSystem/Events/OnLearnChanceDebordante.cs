using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnChanceDebordante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ChanceDebordanteAuraEffectTag))
      {
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.chanceDebordanteAura(player.oid.LoginCreature));
        UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(10);
      }

      return true;
    }
  }
}
