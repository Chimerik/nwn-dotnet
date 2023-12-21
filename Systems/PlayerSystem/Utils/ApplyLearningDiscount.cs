using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public LearnableSkill ApplyLearningDiscout(LearnableSkill learnable)
      {
        switch(learnable.category) 
        {
          case SkillSystem.Category.Language:

            if (learnableSkills.ContainsKey(CustomSkill.HumanVersatility))
              learnable.acquiredPoints += learnable.pointsToNextLevel / 10;

            if (learnableSkills.ContainsKey(CustomSkill.HighElfLanguage))
              learnable.acquiredPoints += learnable.pointsToNextLevel / 10;

            if (learnableSkills.ContainsKey(CustomSkill.Prodige))
              learnable.acquiredPoints += learnable.pointsToNextLevel / 4;

            if (learnableSkills.ContainsKey(CustomSkill.Linguiste))
              learnable.acquiredPoints += learnable.pointsToNextLevel / 2;

            if(learnable.id == CustomSkill.Infernal)
            {
              switch(oid.LoginCreature.Race.Id)
              {
                case CustomRace.AsmodeusThiefling:
                case CustomRace.MephistoThiefling:
                case CustomRace.ZarielThiefling:
                  learnable.acquiredPoints += learnable.pointsToNextLevel / 2;
                  return learnable;
              }
            }

            return learnable;

          case SkillSystem.Category.WeaponProficiency:
            
            if(learnableSkills.ContainsKey(CustomSkill.MaitreDarme))
              learnable.acquiredPoints += learnable.pointsToNextLevel / 2;

            switch (learnable.id)
            {
              case CustomSkill.DwarvenAxeProficiency:

                switch(oid.LoginCreature.Race.Id)
                {
                  case CustomRace.Dwarf:
                  case CustomRace.GoldDwarf:
                  case CustomRace.ShieldDwarf:
                  case CustomRace.Duergar:
                    learnable.acquiredPoints += learnable.pointsToNextLevel / 2;
                    return learnable;
                }

                return learnable;

              case CustomSkill.DoubleBladeProficiency:

                if(learnableSkills.ContainsKey(CustomSkill.LameDoutretombe))
                  learnable.acquiredPoints += learnable.pointsToNextLevel / 4;

                return learnable;
            }

            return learnable;

          default: return learnable;
        }
      }
    }
  }
}
