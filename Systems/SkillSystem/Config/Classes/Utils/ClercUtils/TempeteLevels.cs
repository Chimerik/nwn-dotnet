using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleTempeteLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Tempête");
          player.oid.SetTextureOverride("clerc", "domaine_tempete");

          foreach (Learnable mastery in Fighter.startingPackage.learnables)
          {
            player.learnableSkills.TryAdd(mastery.id, new LearnableSkill((LearnableSkill)mastery, player));
            player.learnableSkills[mastery.id].source.Add(Category.Class);

            mastery.acquiredPoints += (mastery.pointsToNextLevel - mastery.acquiredPoints) / 4;
          }

          player.learnableSkills[CustomSkill.HeavyArmorProficiency].acquiredPoints = 0;

          ClercUtils.LearnDomaineSpell(player, CustomSpell.Injonction);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.Identify);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercSavoirAncestral, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercSavoirAncestral], player));
          player.learnableSkills[CustomSkill.ClercSavoirAncestral].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercSavoirAncestral].source.Add(Category.Class);

          break;

        case 3:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.Augure);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.HoldPerson);

          break;

        case 5:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.CommunicationAvecLesMorts);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.Antidetection);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercDetectionDesPensees, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercDetectionDesPensees], player));
          player.learnableSkills[CustomSkill.ClercDetectionDesPensees].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercDetectionDesPensees].source.Add(Category.Class);

          break;

        case 7:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.OeilMagique);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.Confusion);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercIncantationPuissante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercIncantationPuissante], player));
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercIncantationPuissante].source.Add(Category.Class);

          break;

        case 9:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.Scrutation);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.LegendLore);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.ClercHaloDeLumiere, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercHaloDeLumiere], player));
          player.learnableSkills[CustomSkill.ClercHaloDeLumiere].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercHaloDeLumiere].source.Add(Category.Class);

          break;
      }
    }
  }
}
