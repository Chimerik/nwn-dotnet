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
            if (oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.Id, CustomClass.Fighter, CustomClass.Paladin)) 
              || oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ClercProtecteur))
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

            if (learnableSkills.ContainsKey(CustomSkill.ClercSavoir))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

            if (learnableSkills.ContainsKey(CustomSkill.RangerExplorationHabile))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

            if (learnableSkills.ContainsKey(CustomSkill.Linguiste))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;

            if(learnable.id == CustomSkill.Infernal && oid.LoginCreature.Race.Id == CustomRace.InfernalThiefling)
                learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;
            else if (learnable.id == CustomSkill.Abyssal && oid.LoginCreature.Race.Id == CustomRace.AbyssalThiefling)
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;
            else if (learnable.id == CustomSkill.Primordiale && oid.LoginCreature.Race.Id == CustomRace.ChtonicThiefling)
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;

            return learnable;

          case SkillSystem.Category.WeaponProficiency:

            if (learnableSkills.ContainsKey(CustomSkill.MaitreDarme))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 2;

            if (oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.Id, CustomClass.Fighter)))
              learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4;

            if (oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.Id, CustomClass.Monk)))
            {
              switch (learnable.id)
              {
                case CustomSkill.ShurikenProficiency:
                case CustomSkill.ScimitarProficiency:
                case CustomSkill.ShortSwordProficiency: learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4; break;
              }
            }

            if (oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.Id, CustomClass.Rogue)))
            {
              switch(learnable.id)
              {
                case CustomSkill.ShurikenProficiency:
                case CustomSkill.LongSwordProficiency:
                case CustomSkill.RapierProficiency:
                case CustomSkill.ShortSwordProficiency: learnable.acquiredPoints += (learnable.pointsToNextLevel - learnable.acquiredPoints) / 4; break;
              }
            }

            if (learnableSkills.ContainsKey(CustomSkill.BardCollegeDeLaVaillance) 
              || learnableSkills.ContainsKey(CustomSkill.DruideGardien)
              || learnableSkills.ContainsKey(CustomSkill.ClercProtecteur)
              || oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.Id, CustomClass.Ranger, CustomClass.Barbarian)))
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
