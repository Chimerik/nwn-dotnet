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
        CustomSkill.PerceptionProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Wisdom),
        CustomSkill.AcrobaticsProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Dexterity),
        CustomSkill.AnimalHandlingProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Wisdom),
        CustomSkill.ArcanaProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Intelligence),
        CustomSkill.AthleticsProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Strength),
        CustomSkill.MedicineProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Wisdom),
        CustomSkill.NatureProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Wisdom),
        CustomSkill.PerformanceProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Charisma),
        CustomSkill.StealthProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Dexterity),
        CustomSkill.DeceptionProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Charisma),
        CustomSkill.HistoryProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Intelligence),
        CustomSkill.InsightProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Wisdom),
        CustomSkill.IntimidationProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Charisma),
        CustomSkill.InvestigationProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Intelligence),
        CustomSkill.PersuasionProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Charisma),
        CustomSkill.ReligionProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Intelligence),
        CustomSkill.SleightOfHandProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Dexterity),
        CustomSkill.SurvivalProficiency => GetCreatureSavingThrowAdvantage(creature, Ability.Wisdom),
        _ => 0,
      };
    }
  }
}
