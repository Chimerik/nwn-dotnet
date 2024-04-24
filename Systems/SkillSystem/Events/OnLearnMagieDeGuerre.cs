using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMagieDeGuerre(PlayerSystem.Player player, int customSkillId)
    {
      player.oid.LoginCreature.OnSpellAction -= FighterUtils.OnSpellCastMagieDeGuerre;
      player.oid.LoginCreature.OnSpellAction += FighterUtils.OnSpellCastMagieDeGuerre;

      return true;
    }
  }
}
