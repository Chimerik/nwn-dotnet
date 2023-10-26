
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static int GetSavingThrowIdFromAbility(Ability ability)
    {
      return ability switch
      {
        Ability.Strength => CustomSkill.StrengthSavesProficiency,
        Ability.Constitution => CustomSkill.ConstitutionSavesProficiency,
        Ability.Intelligence => CustomSkill.IntelligenceSavesProficiency,
        Ability.Wisdom => CustomSkill.WisdomSavesProficiency,
        Ability.Charisma => CustomSkill.CharismaSavesProficiency,
        _ => CustomSkill.DexteritySavesProficiency,
      };
    }
  }
}
