using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public LearnableSkill ApplyLearningDiscout(LearnableSkill learnable)
      {
        switch(learnable.id)
        {
          case CustomSkill.HeavyArmorProficiency:
            if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Fighter) || oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerChevalier))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;
            break;
        }

        switch(learnable.category) 
        {
          case SkillSystem.Category.Language:

            if (learnableSkills.ContainsKey(CustomSkill.HumanVersatility))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 10;

            if (learnableSkills.ContainsKey(CustomSkill.HighElfLanguage))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 10;

            if (learnableSkills.ContainsKey(CustomSkill.Prodige))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

            if (learnableSkills.ContainsKey(CustomSkill.Linguiste))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;

            if(learnable.id == CustomSkill.Infernal)
            {
              switch(oid.LoginCreature.Race.Id)
              {
                case CustomRace.AsmodeusThiefling:
                case CustomRace.MephistoThiefling:
                case CustomRace.ZarielThiefling:
                  learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;
                  return learnable;
              }
            }

            return learnable;

          case SkillSystem.Category.WeaponProficiency:

            if (learnableSkills.ContainsKey(CustomSkill.MaitreDarme))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;

            if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Fighter))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

            if (oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Rogue || c.Class.Id == CustomClass.Bard))
            {
              switch(learnable.id)
              {
                case CustomSkill.ShurikenProficiency:
                case CustomSkill.LongSwordProficiency:
                case CustomSkill.RapierProficiency:
                case CustomSkill.ShortSwordProficiency: learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4; break;
              }
            }

            if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDeLaVaillance) || oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Ranger))
            {
              switch (learnable.id)
              {
                case CustomSkill.LightFlailProficiency:
                case CustomSkill.BattleaxeProficiency:
                case CustomSkill.GreatswordProficiency:
                case CustomSkill.HalberdProficiency:
                case CustomSkill.GreataxeProficiency:
                case CustomSkill.ScimitarProficiency:
                case CustomSkill.ThrowingAxeProficiency:
                case CustomSkill.HeavyFlailProficiency:
                case CustomSkill.TridentProficiency:
                case CustomSkill.WarHammerProficiency:
                case CustomSkill.HeavyCrossbowProficiency:
                case CustomSkill.LongBowProficiency:
                case CustomSkill.WhipProficiency:
                case CustomSkill.MorningstarProficiency: learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4; break;
              }
            }

            switch (learnable.id)
            {
              case CustomSkill.DwarvenAxeProficiency:

                switch(oid.LoginCreature.Race.Id)
                {
                  case CustomRace.Dwarf:
                  case CustomRace.GoldDwarf:
                  case CustomRace.ShieldDwarf:
                  case CustomRace.Duergar:
                    learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;
                    return learnable;
                }

                return learnable;

              case CustomSkill.DoubleBladeProficiency:

                if(learnableSkills.ContainsKey(CustomSkill.LameDoutretombe))
                  learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

                return learnable;
            }

            return learnable;

          default: return learnable;
        }
      }
    }
  }
}
