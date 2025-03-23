using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleBersekerLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(5213).SetPlayerOverride(player.oid, "Berseker");
          player.oid.SetTextureOverride("barbarian", "berseker");

          player.LearnClassSkill(CustomSkill.BersekerFrenziedStrike);

          break;

        case 6: player.LearnClassSkill(CustomSkill.BersekerRageAveugle); break;

        case 10: player.LearnClassSkill(CustomSkill.BersekerRepresailles); break;

        case 14:

          player.LearnClassSkill(CustomSkill.BersekerPresenceIntimidante);
          player.LearnClassSkill(CustomSkill.BersekerRestorePresenceIntimidante);

          break;
      }
    }
  }
}
