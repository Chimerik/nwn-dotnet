using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureSkillAdvantage(NwCreature creature, int skill)
    {
      foreach(var eff in creature.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case EffectSystem.InspirationHeroiqueEffectTag:
            creature.RemoveEffect(eff);
            LogUtils.LogMessage("Avantage - Inspiration Heroïque", LogUtils.LogType.Combat);
            return 1;

          case EffectSystem.ChanceuxAvantageEffectTag:
            creature.RemoveEffect(eff);
            LogUtils.LogMessage("Avantage - Chanceux", LogUtils.LogType.Combat);
            return 1;
        }
      }

      return skill switch
      {
        CustomSkill.PerceptionProficiency => GetCreatureAbilityAdvantage(creature, Ability.Wisdom),
        CustomSkill.AcrobaticsProficiency => GetCreatureAbilityAdvantage(creature, Ability.Dexterity),
        CustomSkill.AnimalHandlingProficiency => GetCreatureAbilityAdvantage(creature, Ability.Wisdom),
        CustomSkill.ArcanaProficiency => GetCreatureAbilityAdvantage(creature, Ability.Intelligence),
        CustomSkill.AthleticsProficiency => GetCreatureAbilityAdvantage(creature, Ability.Strength),
        CustomSkill.MedicineProficiency => GetCreatureAbilityAdvantage(creature, Ability.Wisdom),
        CustomSkill.NatureProficiency => GetCreatureAbilityAdvantage(creature, Ability.Wisdom),
        CustomSkill.PerformanceProficiency => GetCreatureAbilityAdvantage(creature, Ability.Charisma),
        CustomSkill.StealthProficiency => GetCreatureAbilityAdvantage(creature, Ability.Dexterity),
        CustomSkill.DeceptionProficiency => GetCreatureAbilityAdvantage(creature, Ability.Charisma),
        CustomSkill.HistoryProficiency => GetCreatureAbilityAdvantage(creature, Ability.Intelligence),
        CustomSkill.InsightProficiency => GetCreatureAbilityAdvantage(creature, Ability.Wisdom),
        CustomSkill.IntimidationProficiency => GetCreatureAbilityAdvantage(creature, Ability.Charisma),
        CustomSkill.InvestigationProficiency => GetCreatureAbilityAdvantage(creature, Ability.Intelligence),
        CustomSkill.PersuasionProficiency => GetCreatureAbilityAdvantage(creature, Ability.Charisma),
        CustomSkill.ReligionProficiency => GetCreatureAbilityAdvantage(creature, Ability.Intelligence),
        CustomSkill.SleightOfHandProficiency => GetCreatureAbilityAdvantage(creature, Ability.Dexterity),
        CustomSkill.SurvivalProficiency => GetCreatureAbilityAdvantage(creature, Ability.Wisdom),
        _ => 0,
      };
    }
  }
}
