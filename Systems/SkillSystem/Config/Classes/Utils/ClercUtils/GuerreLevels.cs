using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleGuerreLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Guerre");
          player.oid.SetTextureOverride("clerc", "guerre");

          foreach (Learnable mastery in Fighter.startingPackage.learnables)
          {
            player.learnableSkills.TryAdd(mastery.id, new LearnableSkill((LearnableSkill)mastery, player));
            player.learnableSkills[mastery.id].source.Add(Category.Class);

            mastery.acquiredPoints += (mastery.pointsToNextLevel - mastery.acquiredPoints) / 4;
          }

          player.learnableSkills.TryAdd(CustomSkill.ClercMartial, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercMartial], player));
          player.learnableSkills[CustomSkill.ClercMartial].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercMartial].source.Add(Category.Class);

          ClercUtils.LearnDomaineSpell(player, (int)Spell.ShieldOfFaith);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.DivineFavor);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercFrappeGuidee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFrappeGuidee], player));
          player.learnableSkills[CustomSkill.ClercFrappeGuidee].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFrappeGuidee].source.Add(Category.Class);

          break;

        case 3:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.MagicWeapon);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.ShelgarnsPersistentBlade);

          break;

        case 5:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.EspritsGardiens);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.CapeDuCroise);

          break;

        case 7:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.Stoneskin);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.FreedomOfMovement);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerreFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerreFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercGuerreFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerreFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.FlameStrike);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.HoldMonster);

          break;

        case 17:

          player.learnableSkills.TryAdd(CustomSkill.ClercGuerreAvatarDeBataille, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercGuerreAvatarDeBataille], player));
          player.learnableSkills[CustomSkill.ClercGuerreAvatarDeBataille].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercGuerreAvatarDeBataille].source.Add(Category.Class);

          break;
      }
    }
  }
}
