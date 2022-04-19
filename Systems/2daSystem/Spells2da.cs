using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.Services;

namespace NWN.Systems
{
  public class SpellsTable : ITwoDimArray
  {
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      ClassType castClass;

      int clericCastLevel = int.TryParse(twoDimEntry("Cleric"), out clericCastLevel) ? clericCastLevel : -1;
      int druidCastLevel = int.TryParse(twoDimEntry("Druid"), out druidCastLevel) ? druidCastLevel : -1;
      int paladinCastLevel = int.TryParse(twoDimEntry("Paladin"), out paladinCastLevel) ? paladinCastLevel : -1;
      int rangerCastLevel = int.TryParse(twoDimEntry("Ranger"), out rangerCastLevel) ? rangerCastLevel : -1;
      int bardCastLevel = int.TryParse(twoDimEntry("Bard"), out bardCastLevel) ? bardCastLevel : -1;

      Dictionary<ClassType, int> classSorter = new Dictionary<ClassType, int>() 
      {
        { ClassType.Cleric, clericCastLevel },
        { ClassType.Druid, druidCastLevel },
        { ClassType.Paladin, paladinCastLevel },
        { ClassType.Ranger, rangerCastLevel },
        { ClassType.Bard, bardCastLevel },
      };

      classSorter.OrderByDescending(c => c.Value);
      castClass = classSorter.ElementAt(0).Value > -1 ? classSorter.ElementAt(0).Key : (ClassType)43;

      float level = float.TryParse(twoDimEntry("Wiz_Sorc"), out level) ? level : 0.5f;
      level = level < 1 ? 0.5f : level;

      NwSpell nwSpell = NwSpell.FromSpellId(rowIndex);
      SkillSystem.learnableDictionary.Add(rowIndex, new LearnableSpell(rowIndex, nwSpell.Name.ToString(), nwSpell.Description.ToString(), nwSpell.IconResRef, level < 1 ? 1 : (int)level, level < 1 ? 0 : (int)level, castClass == ClassType.Druid || castClass == ClassType.Cleric || castClass == ClassType.Ranger ? Ability.Wisdom : Ability.Intelligence, Ability.Charisma));
    }
  }

  [ServiceBinding(typeof(Spells2da))]
  public class Spells2da
  {
    public static TlkTable tlkTable;
    public static SpellsTable spellsTable;
    public Spells2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      spellsTable = twoDimArrayFactory.Get2DA<SpellsTable>("spells");
    }
  }
}
