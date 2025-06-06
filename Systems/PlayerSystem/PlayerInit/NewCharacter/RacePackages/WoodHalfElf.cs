﻿using System.Diagnostics.Metrics;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyWoodHalfElfPackage()
      {
        if (learnableSkills.TryAdd(CustomSkill.Elfique, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.Elfique], this)))
          learnableSkills[CustomSkill.Elfique].LevelUp(this);
        learnableSkills[CustomSkill.Elfique].source.Add(Category.Race);

        oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique, 1);

        oid.LoginCreature.AddFeat(Feat.HardinessVersusEnchantments);
        ApplyWoodElfSpeed();

        oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_SKILL_BONUS_CHOICE_FEAT").Value = CustomRace.WoodHalfElf;
        InitializeBonusSkillChoice();
      }
    }
  }
}
