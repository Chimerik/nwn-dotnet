using System.Security.Cryptography;
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

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Apaisement, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FaerieFire, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.FouleeBrumeuse, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ForceFantasmagorique, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player,(int)Spell.Sleep, CustomClass.Occultiste);

          player.learnableSkills.TryAdd(CustomSkill.FouleeRafraichissante, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeRafraichissante], player));
          player.learnableSkills[CustomSkill.FouleeRafraichissante].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeRafraichissante].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.FouleeProvocatrice, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.FouleeProvocatrice], player));
          player.learnableSkills[CustomSkill.FouleeProvocatrice].LevelUp(player);
          player.learnableSkills[CustomSkill.FouleeProvocatrice].source.Add(Category.Class);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.Clignotement, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.CroissanceVegetale, CustomClass.Occultiste);

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

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ImprovedInvisibility, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.DominateAnimal, CustomClass.Occultiste);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.DominatePerson, CustomClass.Occultiste);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.ApparencesTrompeuses, CustomClass.Occultiste);

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
