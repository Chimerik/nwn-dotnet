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

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Deguisement, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.CharmPerson, CustomClass.Clerc);

          break;

        case 2:

          player.learnableSkills.TryAdd(CustomSkill.ClercRepliqueInvoquee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercRepliqueInvoquee], player));
          player.learnableSkills[CustomSkill.ClercRepliqueInvoquee].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercRepliqueInvoquee].source.Add(Category.Class);

          break;

        case 3:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.PassageSansTrace, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ImageMiroir, CustomClass.Clerc);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.Fear, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.BestowCurse, CustomClass.Clerc);

          break;


        case 6:

          player.learnableSkills.TryAdd(CustomSkill.ClercLinceulDombre, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercLinceulDombre], player));
          player.learnableSkills[CustomSkill.ClercLinceulDombre].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercLinceulDombre].source.Add(Category.Class);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.PolymorphSelf, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.PorteDimensionnelle, CustomClass.Clerc);

          break;

        case 8:

          player.learnableSkills.TryAdd(CustomSkill.ClercDuperieFrappeDivine, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercDuperieFrappeDivine], player));
          player.learnableSkills[CustomSkill.ClercDuperieFrappeDivine].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercDuperieFrappeDivine].source.Add(Category.Class);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.DominatePerson, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ApparencesTrompeuses, CustomClass.Clerc);

          break;
      }
    }
  }
}
