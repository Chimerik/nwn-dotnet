using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.API.Constants;
using NWN.Services;

namespace NWN.Systems
{
  public class FeatTable : ITwoDimArray
  {
    private readonly Dictionary<Feat, Entry> entries = new Dictionary<Feat, Entry>();

    public Entry GetFeatDataEntry(Feat feat)
    {
      return entries[feat];
    }
    void ITwoDimArray.DeserializeRow(int rowIndex, TwoDimEntry twoDimEntry)
    {
      uint tlkName = uint.TryParse(twoDimEntry("FEAT"), out tlkName) ? tlkName : 0;

      string name;
      if (tlkName == 0)
        name = "Nom manquant";
      else
        name = Feat2da.tlkTable.GetSimpleString(tlkName);

      uint tlkDescription = uint.TryParse(twoDimEntry("DESCRIPTION"), out tlkDescription) ? tlkDescription : 0;

      string description;
      if (tlkDescription == 0)
        description = "Description manquante";
      else
        description = Feat2da.tlkTable.GetSimpleString(tlkDescription);

      int CRValue = int.TryParse(twoDimEntry("CRValue"), out CRValue) ? CRValue : 1;
      int currentLevel = int.TryParse(twoDimEntry("GAINMULTIPLE"), out currentLevel) ? currentLevel : 1;
      int successor = int.TryParse(twoDimEntry("SUCCESSOR"), out successor) ? successor : 0;

      int minStr = int.TryParse(twoDimEntry("MINSTR"), out minStr) ? minStr : 0;
      int minDex = int.TryParse(twoDimEntry("MINDEX"), out minDex) ? minDex : 0;
      int minCon = int.TryParse(twoDimEntry("MINCON"), out minCon) ? minCon : 0;
      int minInt = int.TryParse(twoDimEntry("MININT"), out minInt) ? minInt : 0;
      int minWis = int.TryParse(twoDimEntry("MINWIS"), out minWis) ? minWis : 0;
      int minCha = int.TryParse(twoDimEntry("MINCHA"), out minCha) ? minCha : 0;

      Dictionary<Ability, int> statSorter = new Dictionary<Ability, int>()
      {
        { Ability.Intelligence, minInt },
        { Ability.Constitution, minCon },
        { Ability.Wisdom, minWis },
        { Ability.Charisma, minCha },
        { Ability.Strength, minStr },
        { Ability.Dexterity, minDex } 
      };

      statSorter.OrderByDescending(key => key.Value);

      Ability primaryAbility = statSorter.ElementAt(0).Key;
      Ability secondaryAbility = statSorter.ElementAt(1).Key;

      entries.Add((Feat)rowIndex, new Entry(name, description, tlkName, tlkDescription, CRValue, currentLevel, successor, primaryAbility, secondaryAbility));
    }
    public readonly struct Entry
    {
      public readonly string name;
      public readonly string description;
      public readonly uint tlkName;
      public readonly uint tlkDescription;
      public readonly int CRValue;
      public readonly int currentLevel;
      public readonly int successor;
      public readonly Ability primaryAbility;
      public readonly Ability secondaryAbility;

      public Entry(string name, string description, uint tlkName, uint tlkDescription, int CRValue, int currentLevel, int successor, Ability primaryAbility, Ability secondaryAbility)
      {
        this.name = name;
        this.description = description;
        this.tlkName = tlkName;
        this.tlkDescription = tlkDescription;
        this.CRValue = CRValue;
        this.currentLevel = currentLevel;
        this.successor = successor;
        this.primaryAbility = primaryAbility;
        this.secondaryAbility = secondaryAbility;
      }
    }
  }

  [ServiceBinding(typeof(Feat2da))]
  public class Feat2da
  {
    public static TlkTable tlkTable;
    public static FeatTable featTable;
    public Feat2da(TwoDimArrayFactory twoDimArrayFactory, TlkTable tlkService)
    {
      tlkTable = tlkService;
      featTable = twoDimArrayFactory.Get2DA<FeatTable>("feat");
    }
  }
}
