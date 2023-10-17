using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void InitializeStartingLearnables()
      {
        if (!oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Sprint)))
          oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Sprint));

        if (!oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Disengage)))
          oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Disengage));

        int startingLanguage = -1;

        switch (oid.LoginCreature.Race.RacialType)
        {
          case RacialType.Dwarf:
            startingLanguage = CustomSkill.Nain;
            break;
          case RacialType.Elf:
          case RacialType.HalfElf:
            startingLanguage = CustomSkill.Elfique;
            break;
          case RacialType.Halfling:
            startingLanguage = CustomSkill.Halfelin;
            break;
          case RacialType.Gnome:
            startingLanguage = CustomSkill.Gnome;
            break;
          case RacialType.HalfOrc:
            startingLanguage = CustomSkill.Orc;
            break;
        }

        if (startingLanguage > -1)
        {
          LearnableSkill language = new LearnableSkill((LearnableSkill)SkillSystem.learnableDictionary[startingLanguage]);
          learnableSkills.Add(startingLanguage, language);
          language.LevelUp(this);
        }
      }
    }
  }
}
