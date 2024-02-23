using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Systems;
using static NWN.Systems.SpellConfig;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureSkillAdvantage(NwCreature creature, int skill)
    {
      int advantage = 0;

      switch (skill)
      {
        case CustomSkill.PerceptionProficiency:

          if (creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectAigle)))
            advantage += 1;

          advantage += GetCreatureAbilityAdvantage(creature, Ability.Wisdom);

          break;

        case CustomSkill.AcrobaticsProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Dexterity); break;
        case CustomSkill.AnimalHandlingProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Wisdom); break;
        case CustomSkill.ArcanaProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Intelligence); break;
        case CustomSkill.AthleticsProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Strength); break;
        case CustomSkill.MedicineProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Wisdom); break;
        case CustomSkill.NatureProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Wisdom); break;
        case CustomSkill.PerformanceProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Charisma); break;
        case CustomSkill.StealthProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Dexterity); break;
        case CustomSkill.DeceptionProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Charisma); break;
        case CustomSkill.HistoryProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Intelligence); break;
        case CustomSkill.InsightProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Wisdom); break;
        case CustomSkill.IntimidationProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Charisma); break;
        case CustomSkill.InvestigationProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Intelligence); break;
        case CustomSkill.PersuasionProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Charisma); break;
        case CustomSkill.ReligionProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Intelligence); break;
        case CustomSkill.SleightOfHandProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Dexterity); break;
        case CustomSkill.SurvivalProficiency: advantage += GetCreatureAbilityAdvantage(creature, Ability.Wisdom); break;
      }

      return advantage;
    }
  }
}
