using System.Collections.Generic;
using NWN.API;
using NWN.API.Constants;
using NWN.Services;

namespace NWN.Systems
{
  public class SkillsTable : ITwoDimArray
  {
    private readonly Dictionary<Skill, Entry> entries = new Dictionary<Skill, Entry>();

    public Entry GetDataEntry(Skill row)
    {
      return entries[row];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint tlkName = uint.TryParse(twoDimEntry("Name"), out tlkName) ? tlkName : 0;

      string name;
      if (tlkName == 0)
        name = "Nom manquant";
      else
        name = Feat2da.tlkTable.GetSimpleString(tlkName);

      entries.Add((Skill)rowIndex, new Entry(name));
    }
    public readonly struct Entry
    {
      public readonly string name;

      public Entry(string name)
      {
        this.name = name;
      }
    }
  }

  [ServiceBinding(typeof(Skills2da))]
  public class Skills2da
  {
    public static TlkTable tlkTable;
    public static SkillsTable skillsTable;
    public Skills2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      skillsTable = twoDimArrayFactory.Get2DA<SkillsTable>("skills");
    }
  }
}
