using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Ranger
  {
    public static void HandleBetesLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          new StrRef(14).SetPlayerOverride(player.oid, "Conclave des Bêtes");
          player.oid.SetTextureOverride("ranger", "conclave_betes");

          if (!player.windows.TryGetValue("fightingStyleSelection", out var style)) player.windows.Add("fightingStyleSelection", new FightingStyleSelectionWindow(player, CustomSkill.BardCollegeDeLescrime));
          else ((FightingStyleSelectionWindow)style).CreateWindow(CustomSkill.BardCollegeDeLescrime);

          player.oid.LoginCreature.OnCreatureAttack -= BardUtils.OnAttackEscrime;
          player.oid.LoginCreature.OnCreatureAttack += BardUtils.OnAttackEscrime;

          player.learnableSkills.TryAdd(CustomSkill.BotteDefensive, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BotteDefensive], player));
          player.learnableSkills[CustomSkill.BotteDefensive].LevelUp(player);
          player.learnableSkills[CustomSkill.BotteDefensive].source.Add(Category.Class);

          break;

        case 5:

          player.learnableSkills.TryAdd(CustomSkill.EscrimeBonusAttack, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EscrimeBonusAttack], player));
          player.learnableSkills[CustomSkill.EscrimeBonusAttack].LevelUp(player);
          player.learnableSkills[CustomSkill.EscrimeBonusAttack].source.Add(Category.Class);

          player.oid.LoginCreature.BaseAttackCount += 1;

          break;

        case 7:

          break;

        case 11:

          break;

        case 15:

          break;
      }
    }
  }
}
