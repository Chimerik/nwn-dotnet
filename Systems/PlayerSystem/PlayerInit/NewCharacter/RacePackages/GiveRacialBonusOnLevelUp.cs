using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public void GiveRacialBonusOnLevelUp()
      {
        switch (oid.LoginCreature.Race.Id)
        {
          case CustomRace.Drow:
          case CustomRace.DrowHalfElf:

            switch (oid.LoginCreature.Level)
            {
              case 3:

                if (learnableSkills.TryAdd(CustomSkill.FaerieFireDrow, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FaerieFireDrow])))
                  learnableSkills[CustomSkill.FaerieFireDrow].LevelUp(this);

                learnableSkills[CustomSkill.FaerieFireDrow].source.Add(Category.Race);

                break;

              case 5:

                if (learnableSkills.TryAdd(CustomSkill.DarknessDrow, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DarknessDrow])))
                  learnableSkills[CustomSkill.DarknessDrow].LevelUp(this);

                learnableSkills[CustomSkill.DarknessDrow].source.Add(Category.Race);

                break;
            }

            break;

          case CustomRace.AsmodeusThiefling:

            switch (oid.LoginCreature.Level)
            {
              case 3:

                if (learnableSkills.TryAdd(CustomSkill.HellishRebuke, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.HellishRebuke])))
                  learnableSkills[CustomSkill.HellishRebuke].LevelUp(this);

                learnableSkills[CustomSkill.HellishRebuke].source.Add(Category.Race);

                break;

              case 5:

                if (learnableSkills.TryAdd(CustomSkill.DarknessDrow, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DarknessDrow])))
                  learnableSkills[CustomSkill.DarknessDrow].LevelUp(this);

                learnableSkills[CustomSkill.DarknessDrow].source.Add(Category.Race);

                break;
            }

            break;

          case CustomRace.MephistoThiefling:

            switch (oid.LoginCreature.Level)
            {
              case 3:

                if (learnableSkills.TryAdd(CustomSkill.BurningHands, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BurningHands])))
                  learnableSkills[CustomSkill.BurningHands].LevelUp(this);

                learnableSkills[CustomSkill.BurningHands].source.Add(Category.Race);

                break;

              case 5:

                if (learnableSkills.TryAdd(CustomSkill.FlameBlade, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FlameBlade])))
                  learnableSkills[CustomSkill.FlameBlade].LevelUp(this);

                learnableSkills[CustomSkill.FlameBlade].source.Add(Category.Race);

                break;
            }

            break;

          case CustomRace.ZarielThiefling:

            switch (oid.LoginCreature.Level)
            {
              case 3:

                if (learnableSkills.TryAdd(CustomSkill.SearingSmite, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.SearingSmite])))
                  learnableSkills[CustomSkill.SearingSmite].LevelUp(this);

                learnableSkills[CustomSkill.SearingSmite].source.Add(Category.Race);

                break;

              case 5:

                if (learnableSkills.TryAdd(CustomSkill.BrandingSmite, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.BrandingSmite])))
                  learnableSkills[CustomSkill.BrandingSmite].LevelUp(this);

                learnableSkills[CustomSkill.BrandingSmite].source.Add(Category.Race);

                break;
            }

            break;

          case CustomRace.Duergar:

            switch (oid.LoginCreature.Level)
            {
              case 3:

                if (learnableSkills.TryAdd(CustomSkill.EnlargeDuergar, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.EnlargeDuergar])))
                  learnableSkills[CustomSkill.EnlargeDuergar].LevelUp(this);

                learnableSkills[CustomSkill.EnlargeDuergar].source.Add(Category.Race);

                break;

              case 5:

                if (learnableSkills.TryAdd(CustomSkill.InvisibilityDuergar, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.InvisibilityDuergar])))
                  learnableSkills[CustomSkill.InvisibilityDuergar].LevelUp(this);

                learnableSkills[CustomSkill.InvisibilityDuergar].source.Add(Category.Race);

                break;
            }

            break;
        }
      }
    }
  }
}
