using Anvil.API;
using System.Security.Cryptography;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Fighter
  {
    public static void HandleArcherMageLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.FighterArcaneArcher;
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value = (int)SkillConfig.SkillOptionType.Proficiency;
          player.InitializeBonusSkillChoice();

          break;

        case 7:

          player.learnableSkills.TryAdd(CustomSkill.ArcaneArcherTirIncurve, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ArcaneArcherTirIncurve]));
          player.learnableSkills[CustomSkill.ArcaneArcherTirIncurve].LevelUp(player);
          player.learnableSkills[CustomSkill.ArcaneArcherTirIncurve].source.Add(Category.Class);

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.FighterArcaneArcher;
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value = (int)SkillConfig.SkillOptionType.ArcaneShot;
          player.InitializeBonusSkillChoice();

          break;

        case 10:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.FighterArcaneArcher;
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value = (int)SkillConfig.SkillOptionType.ArcaneShot;
          player.InitializeBonusSkillChoice();

          break;

        case 15:

          player.oid.OnCombatStatusChange -= FighterUtils.OnCombatArcaneArcherRecoverTirArcanique;
          player.oid.OnCombatStatusChange += FighterUtils.OnCombatArcaneArcherRecoverTirArcanique;

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.FighterArcaneArcher;
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value = (int)SkillConfig.SkillOptionType.ArcaneShot;
          player.InitializeBonusSkillChoice();

          break;

        case 18:

          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomSkill.FighterArcaneArcher;
          player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_OPTION_CHOICE_FEAT").Value = (int)SkillConfig.SkillOptionType.ArcaneShot;
          player.InitializeBonusSkillChoice();

          break;
      }
    }
  }
}
