using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Clerc
  {
    public static void HandleDuperieLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 1: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Duperie");
          player.oid.SetTextureOverride("clerc", "duperie");

          player.learnableSkills.TryAdd(CustomSkill.ClercBenedictionEscroc, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercBenedictionEscroc], player));
          player.learnableSkills[CustomSkill.ClercBenedictionEscroc].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercBenedictionEscroc].source.Add(Category.Class);

          ClercUtils.LearnDomaineSpell(player, CustomSpell.Deguisement);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.CharmPerson);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercRepliqueInvoquee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRepliqueInvoquee], player));
          player.learnableSkills[CustomSkill.ClercRepliqueInvoquee].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRepliqueInvoquee].source.Add(Category.Class);

          break;

        case 3:

          ClercUtils.LearnDomaineSpell(player, CustomSpell.PassageSansTrace);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.ImageMiroir);

          break;

        case 5:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.Fear);
          ClercUtils.LearnDomaineSpell(player, (int)Spell.BestowCurse);

          break;


        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercLinceulDombre], player));
          player.learnableSkills[CustomSkill.ClercLinceulDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercLinceulDombre].source.Add(Category.Class);

          break;

        case 7:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.PolymorphSelf);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.PorteDimensionnelle);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercDuperieFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercDuperieFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercDuperieFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercDuperieFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          ClercUtils.LearnDomaineSpell(player, (int)Spell.DominatePerson);
          ClercUtils.LearnDomaineSpell(player, CustomSpell.ApparencesTrompeuses);

          break;
      }
    }
  }
}
