using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Services;

namespace NWN.Systems
{
  public class SpellsTable : ITwoDimArray
  {
    private readonly Dictionary<Spell, Entry> entries = new Dictionary<Spell, Entry>();

    public Entry GetSpellDataEntry(Spell row)
    {
      return entries[row];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      ClassType castClass;

      int clericCastLevel = int.TryParse(twoDimEntry("Cleric"), out clericCastLevel) ? clericCastLevel : 0;
      int druidCastLevel = int.TryParse(twoDimEntry("Druid"), out druidCastLevel) ? druidCastLevel : 0;
      int paladinCastLevel = int.TryParse(twoDimEntry("Paladin"), out paladinCastLevel) ? paladinCastLevel : 0;
      int rangerCastLevel = int.TryParse(twoDimEntry("Ranger"), out rangerCastLevel) ? rangerCastLevel : 0;
      int bardCastLevel = int.TryParse(twoDimEntry("Bard"), out bardCastLevel) ? bardCastLevel : 0;

      Dictionary<ClassType, int> classSorter = new Dictionary<ClassType, int>() 
      {
        { ClassType.Cleric, clericCastLevel },
        { ClassType.Druid, druidCastLevel },
        { ClassType.Paladin, paladinCastLevel },
        { ClassType.Ranger, rangerCastLevel },
        { ClassType.Bard, bardCastLevel },
      };

      classSorter.OrderByDescending(c => c.Value);

      if (classSorter.ElementAt(0).Value > 0)
        castClass = classSorter.ElementAt(0).Key;
      else
        castClass = (ClassType)43;

      string school = twoDimEntry("School");

      uint strRef = uint.TryParse(twoDimEntry("Name"), out strRef) ? strRef : 0;
      string name = strRef == 0 ? name = "Nom manquant" : name = Spells2da.tlkTable.GetSimpleString(strRef);

      strRef = uint.TryParse(twoDimEntry("SpellDesc"), out strRef) ? strRef : 0;
      string description = strRef == 0 ? description = "Description manquante" : description = Spells2da.tlkTable.GetSimpleString(strRef);

      float level = float.TryParse(twoDimEntry("Wiz_Sorc"), out level) ? level : 0.5f;
      if (level < 1)
        level = 0.5f;

      entries.Add((Spell)rowIndex, new Entry(name, description, level, castClass, (SpellSchool)"GACDEVINT".IndexOf(school)));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string description;
      public readonly float level;
      public readonly ClassType castingClass;
      public readonly SpellSchool school;

      public Entry(string name, string description, float level, ClassType castingClass, SpellSchool school)
      {
        this.name = name;
        this.description = description;
        this.level = level;
        this.castingClass = castingClass;
        this.school = school;
      }
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
