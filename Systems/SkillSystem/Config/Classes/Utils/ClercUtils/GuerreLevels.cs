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
        case 3: 
          
          new StrRef(12).SetPlayerOverride(player.oid, "Domaine de la Guerre");
          player.oid.SetTextureOverride("clerc", "guerre");

          player.learnableSkills.TryAdd(CustomSkill.ClercMartial, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercMartial], player));
          player.learnableSkills[CustomSkill.ClercMartial].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercMartial].source.Add(Category.Class);

          player.learnableSkills.TryAdd(CustomSkill.ClercFrappeGuidee, new LearnableSkill((LearnableSkill)learnableDictionary[CustomSkill.ClercFrappeGuidee], player));
          player.learnableSkills[CustomSkill.ClercFrappeGuidee].LevelUp(player);
          player.learnableSkills[CustomSkill.ClercFrappeGuidee].source.Add(Category.Class);

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ShieldOfFaith, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.DivineFavor, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.MagicWeapon, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ShelgarnsPersistentBlade, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.EclairTracant, CustomClass.Clerc);

          break;

        case 5:

          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.EspritsGardiens, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, CustomSpell.CapeDuCroise, CustomClass.Clerc);

          break;

        case 7:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.ElementalShield, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FreedomOfMovement, CustomClass.Clerc);

          break;

        case 9:

          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.FlameStrike, CustomClass.Clerc);
          SpellUtils.LearnAlwaysPreparedSpell(player, (int)Spell.HoldMonster, CustomClass.Clerc);

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
