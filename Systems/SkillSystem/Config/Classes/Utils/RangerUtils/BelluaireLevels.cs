using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleBelluaireLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(14).SetPlayerOverride(player.oid, "Conclave des Belluaires");
          player.oid.SetTextureOverride("ranger", "conclave_betes");

          player.LearnClassSkill(CustomSkill.BelluaireCompagnonAnimal);

          break;

        case 5: player.LearnClassSkill(CustomSkill.BelluaireAttaqueCoordonnee); break;
        case 7: player.LearnClassSkill(CustomSkill.BelluaireEntrainementExceptionnel); break;
        case 11: player.LearnClassSkill(CustomSkill.BelluaireFurieBestiale); break;
        case 15: player.LearnClassSkill(CustomSkill.BelluaireDefenseDeLaBeteSuperieure); break;
      }
    }
  }
}
