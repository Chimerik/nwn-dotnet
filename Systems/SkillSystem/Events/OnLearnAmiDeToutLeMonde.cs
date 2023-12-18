using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAmiDeToutLeMonde(PlayerSystem.Player player, int customSkillId)
    {
      byte rawCharisma = player.oid.LoginCreature.GetRawAbilityScore(Ability.Charisma);
      if (rawCharisma < 20)
        player.oid.LoginCreature.SetsRawAbilityScore(Ability.Charisma, (byte)(rawCharisma + 1));

      if(player.learnableSkills.TryGetValue(CustomSkill.DeceptionProficiency, out LearnableSkill deceptionProf))
      {
        if (deceptionProf.currentLevel < 1)
          deceptionProf.LevelUp(player);
        else if (player.learnableSkills.TryGetValue(CustomSkill.DeceptionExpertise, out LearnableSkill deceptionExpertise))
        {
          if (deceptionExpertise.currentLevel < 1)
            deceptionExpertise.LevelUp(player);
        }
        else
        {
          player.learnableSkills.Add(CustomSkill.DeceptionExpertise, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionExpertise]));
          player.learnableSkills[CustomSkill.DeceptionExpertise].LevelUp(player);
        }
      }
      else
      {
        player.learnableSkills.Add(CustomSkill.DeceptionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DeceptionProficiency]));
        player.learnableSkills[CustomSkill.DeceptionProficiency].LevelUp(player);
      }

      if (player.learnableSkills.TryGetValue(CustomSkill.PersuasionProficiency, out LearnableSkill persuasionProf))
      {
        if (persuasionProf.currentLevel < 1)
          persuasionProf.LevelUp(player);
        else if (player.learnableSkills.TryGetValue(CustomSkill.PersuasionExpertise, out LearnableSkill persuasionExpertise))
        {
          if (persuasionExpertise.currentLevel < 1)
            persuasionExpertise.LevelUp(player);
        }
        else
        {
          player.learnableSkills.Add(CustomSkill.PersuasionExpertise, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionExpertise]));
          player.learnableSkills[CustomSkill.PersuasionExpertise].LevelUp(player);
        }
      }
      else
      {
        player.learnableSkills.Add(CustomSkill.PersuasionProficiency, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.PersuasionProficiency]));
        player.learnableSkills[CustomSkill.PersuasionProficiency].LevelUp(player);
      }

      return true;
    }
  }
}
