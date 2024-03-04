using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureSkillAdvantage(NwCreature creature, int skill)
    {
      switch (skill)
      {
        case CustomSkill.PerceptionProficiency:

          if (creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectAigle)))
            return 1;

          return GetCreatureAbilityAdvantage(creature, Ability.Wisdom);

        case CustomSkill.AcrobaticsProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Dexterity);
        case CustomSkill.AnimalHandlingProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Wisdom);
        case CustomSkill.ArcanaProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Intelligence);
        case CustomSkill.AthleticsProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Strength);
        case CustomSkill.MedicineProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Wisdom);
        case CustomSkill.NatureProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Wisdom);
        case CustomSkill.PerformanceProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Charisma);
        case CustomSkill.StealthProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Dexterity);
        case CustomSkill.DeceptionProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Charisma);
        case CustomSkill.HistoryProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Intelligence);
        case CustomSkill.InsightProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Wisdom);
        case CustomSkill.IntimidationProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Charisma);
        case CustomSkill.InvestigationProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Intelligence);
        case CustomSkill.PersuasionProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Charisma);
        case CustomSkill.ReligionProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Intelligence);
        case CustomSkill.SleightOfHandProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Dexterity);
        case CustomSkill.SurvivalProficiency: return GetCreatureAbilityAdvantage(creature, Ability.Wisdom);
      }

      return 0;
    }
  }
}
