using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFightingStyleDuel(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnCreatureDamage -= DamageUtils.HandleDuellingDamage;
      player.oid.LoginCreature.OnCreatureDamage += DamageUtils.HandleDuellingDamage;

      return true;
    }
  }
}
