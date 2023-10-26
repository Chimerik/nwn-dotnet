using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowProficiencyBonus(NwCreature target, SpellEntry spellEntry)
    {
      if (PlayerSystem.Players.TryGetValue(target, out PlayerSystem.Player player))
      {
        if (player.learnableSkills.TryGetValue(SkillSystem.GetSavingThrowIdFromAbility(spellEntry.savingThrowAbility), out LearnableSkill proficiency)
        && proficiency.currentLevel > 0)
          return NativeUtils.GetCreatureProficiencyBonus(target);
      }
      else
        return NativeUtils.GetCreatureProficiencyBonus(target);

      return 0;
    }
  }
}
