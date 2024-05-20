using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

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

          if (!player.windows.TryGetValue("belluaireCompanionSelection", out var value)) player.windows.Add("belluaireCompanionSelection", new BelluaireCompanionSelectionWindow(player));
          else ((BelluaireCompanionSelectionWindow)value).CreateWindow();

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.BelluaireAttaqueCoordonnee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BelluaireAttaqueCoordonnee], player));
          player.learnableSkills[CustomSkill.BelluaireAttaqueCoordonnee].LevelUp(player);
          player.learnableSkills[CustomSkill.BelluaireAttaqueCoordonnee].source.Add(Category.Class);

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.BelluaireDefenseDeLaBete, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BelluaireDefenseDeLaBete], player));
          player.learnableSkills[CustomSkill.BelluaireDefenseDeLaBete].LevelUp(player);
          player.learnableSkills[CustomSkill.BelluaireDefenseDeLaBete].source.Add(Category.Class);

          break;

        case 11:

          player.learnableSkills.TryAdd(CustomSkill.BelluaireFurieBestiale, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BelluaireFurieBestiale], player));
          player.learnableSkills[CustomSkill.BelluaireFurieBestiale].LevelUp(player);
          player.learnableSkills[CustomSkill.BelluaireFurieBestiale].source.Add(Category.Class);

          break;

        case 15:

          player.learnableSkills.TryAdd(CustomSkill.BelluaireDefenseDeLaBeteSuperieure, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BelluaireDefenseDeLaBeteSuperieure], player));
          player.learnableSkills[CustomSkill.BelluaireDefenseDeLaBeteSuperieure].LevelUp(player);
          player.learnableSkills[CustomSkill.BelluaireDefenseDeLaBeteSuperieure].source.Add(Category.Class);

          break;
      }
    }
  }
}
