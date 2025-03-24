using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleWildMagicLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(5213).SetPlayerOverride(player.oid, "Magie Sauvage");
          player.oid.SetTextureOverride("barbarian", "wildmagic");

          player.LearnClassSkill(CustomSkill.WildMagicSense);
          player.LearnClassSkill(CustomSkill.WildMagicTeleportation);

          break;

        case 6:

          player.LearnClassSkill(CustomSkill.WildMagicMagieGalvanisanteBienfait);
          player.LearnClassSkill(CustomSkill.WildMagicMagieGalvanisanteRecuperation);

          break;

        case 14:

          // TODO : table de priorité des effets de magie sauvage

          break;
      }
    }
  }
}
