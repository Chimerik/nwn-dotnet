using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Barbarian
  {
    public static void HandleTotemLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(5213).SetPlayerOverride(player.oid, "Totem");
          player.oid.SetTextureOverride("barbarian", "totem");

          player.LearnClassSkill(CustomSkill.TotemSpeakAnimal);
          player.LearnClassSkill(CustomSkill.TotemSensAnimal);
          player.LearnClassSkill(CustomSkill.TotemRage);

          break;

        case 6: player.LearnClassSkill(CustomSkill.TotemAspectSauvage); break;
        case 10: player.LearnClassSkill(CustomSkill.TotemCommunionAvecLaNature); break;
        case 14: player.LearnClassSkill(CustomSkill.TotemPuissanceSauvage); break;
      }
    }
  }
}
