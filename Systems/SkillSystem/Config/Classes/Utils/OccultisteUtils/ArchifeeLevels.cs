using Anvil.API;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public static partial class Occultiste
  {
    public static void HandleArchifeeLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3:

          NwClass.FromClassId(CustomClass.Occultiste).Name.SetPlayerOverride(player.oid, "Mécène Archifée");
          player.oid.SetTextureOverride("occultiste", "warlock_archfey");

          player.LearnAlwaysPreparedSpell(CustomSpell.Apaisement, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.FaerieFire, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.FouleeBrumeuse, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.ForceFantasmagorique, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.Sleep, CustomClass.Occultiste);

          player.learnableSkills.TryAdd(CustomSkill.FouleeRafraichissante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeRafraichissante], player));
          player.learnableSkills[CustomSkill.FouleeRafraichissante].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeRafraichissante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FouleeProvocatrice, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeProvocatrice], player));
          player.learnableSkills[CustomSkill.FouleeProvocatrice].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeProvocatrice].source.Add(Category.Class);

          break;

        case 5:

          player.LearnAlwaysPreparedSpell(CustomSpell.Clignotement, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.CroissanceVegetale, CustomClass.Occultiste);

          break;

        case 6:

          player.learnableSkills.TryAdd(CustomSkill.FouleeEvanescente, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeEvanescente], player));
          player.learnableSkills[CustomSkill.FouleeEvanescente].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeEvanescente].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FouleeRedoutable, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeRedoutable], player));
          player.learnableSkills[CustomSkill.FouleeRedoutable].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeRedoutable].source.Add(Category.Class);

          break;

        case 7:

          player.LearnAlwaysPreparedSpell((int)Spell.ImprovedInvisibility, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell((int)Spell.DominateAnimal, CustomClass.Occultiste);

          break;

        case 9:

          player.LearnAlwaysPreparedSpell((int)Spell.DominatePerson, CustomClass.Occultiste);
          player.LearnAlwaysPreparedSpell(CustomSpell.ApparencesTrompeuses, CustomClass.Occultiste);

          break;

        case 10:

          player.learnableSkills.TryAdd(CustomSkill.DefensesEnjoleuses, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.DefensesEnjoleuses], player));
          player.learnableSkills[CustomSkill.DefensesEnjoleuses].LevelUp(player);
          player.learnableSkills[CustomSkill.DefensesEnjoleuses].source.Add(Category.Class);

          break;

        case 14:

          player.learnableSkills.TryAdd(CustomSkill.FouleeEnjoleuse, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeEnjoleuse], player));
          player.learnableSkills[CustomSkill.FouleeEnjoleuse].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeEnjoleuse].source.Add(Category.Class);

          break;
      }
    }
  }
}
