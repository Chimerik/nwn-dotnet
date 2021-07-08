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
    public bool HasFeatPrerequisites(Feat feat, NwCreature oCreature)
    {
      Entry entry = entries[feat];

      foreach (Feat reqFeat in entry.preRequisites)
        if(!oCreature.KnowsFeat(reqFeat))
        {
          oCreature.ControllingPlayer.SendServerMessage($"Avant d'apprendre les bases de {entry.name.ColorString(ColorConstants.White)}, il faut maîtriser {entries[reqFeat].name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
          return false;
        }

      if (entry.orRequisites.Count > 0)
      {
        if (entry.orRequisites.Any(f => oCreature.KnowsFeat(f)))
        {
          return true;
        }
        else
        {
          string unknownFeats = $"Avant d'apprendre les bases de {entry.name.ColorString(ColorConstants.White)}, il vous faut maîtriser au moins l'une de ces connaissances [";
          foreach (Feat orFeat in entry.orRequisites.Where(f => !oCreature.KnowsFeat(f)))
            unknownFeats += $"{Feat2da.featTable.entries[orFeat].name.ColorString(ColorConstants.White)} ";

          unknownFeats += "]";

          oCreature.ControllingPlayer.SendServerMessage(unknownFeats, ColorConstants.Red);

          return false;
        }
      }

      return true;
    }
    public bool HasSkillPrerequisites(Feat feat, NwCreature oCreature)
    {
      Entry entry = entries[feat];

      if (oCreature.GetSkillRank(entry.reqSkill, true) < entry.reqSkillMinRank)
      {
        oCreature.ControllingPlayer.SendServerMessage($"Un minimum de {entry.reqSkillMinRank.ToString().ColorString(ColorConstants.White)} points en {Skills2da.skillsTable.GetDataEntry(entry.reqSkill).name.ColorString(ColorConstants.White)} est nécessaire avant de pouvoir apprendre les bases de {entry.name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
        return false;
      }

      if (oCreature.GetSkillRank(entry.reqSkill2, true) < entry.reqSkillMinRank2)
      {
        oCreature.ControllingPlayer.SendServerMessage($"Un minimum de {entry.reqSkillMinRank2.ToString().ColorString(ColorConstants.White)} points en {Skills2da.skillsTable.GetDataEntry(entry.reqSkill2).name.ColorString(ColorConstants.White)} est nécessaire avant de pouvoir apprendre les bases de {entry.name.ColorString(ColorConstants.White)}.", ColorConstants.Red);
        return false;
      }

      return true;
    }
    public bool HasAbilityPrerequisites(Feat feat, NwCreature oCreature)
    {
      Entry entry = entries[feat];

      if(entry.minAttack > oCreature.BaseAttackBonus)
      {
        oCreature.ControllingPlayer.SendServerMessage($"Avant d'apprendre les bases de {entry.name.ColorString(ColorConstants.White)}, il faut avoir un bonus d'attaque de base supérieur à {entry.minAttack.ToString().ColorString(ColorConstants.White)}.", ColorConstants.Red);
        return false;
      }

      foreach(var minAbility in entry.minAbility)
      {
        if(minAbility.Value > oCreature.GetAbilityScore(minAbility.Key, true))
        {
          oCreature.ControllingPlayer.SendServerMessage($"Avant d'apprendre les bases de {entry.name.ColorString(ColorConstants.White)}, il faut avoir un score de {minAbility.Key.ToString().ColorString(ColorConstants.White)} supérieur à {entry.minAttack.ToString().ColorString(ColorConstants.White)}.", ColorConstants.Red);
          return false;
        }
      }

      return true;
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

      List<Feat> preRequisites = new List<Feat>();

      int preReq = int.TryParse(twoDimEntry("PREREQFEAT1"), out preReq) ? preReq : -1;
      if (preReq > -1)
        preRequisites.Add((Feat)preReq);

      preReq = int.TryParse(twoDimEntry("PREREQFEAT2"), out preReq) ? preReq : -1;
      if (preReq > -1)
        preRequisites.Add((Feat)preReq);

      List<Feat> orRequisites = new List<Feat>();

      preReq = int.TryParse(twoDimEntry("OrReqFeat0"), out preReq) ? preReq : -1;
      if (preReq > -1)
        orRequisites.Add((Feat)preReq);

      preReq = int.TryParse(twoDimEntry("OrReqFeat1"), out preReq) ? preReq : -1;
      if (preReq > -1)
        orRequisites.Add((Feat)preReq);

      preReq = int.TryParse(twoDimEntry("OrReqFeat2"), out preReq) ? preReq : -1;
      if (preReq > -1)
        orRequisites.Add((Feat)preReq);

      preReq = int.TryParse(twoDimEntry("OrReqFeat3"), out preReq) ? preReq : -1;
      if (preReq > -1)
        orRequisites.Add((Feat)preReq);

      preReq = int.TryParse(twoDimEntry("OrReqFeat4"), out preReq) ? preReq : -1;
      if (preReq > -1)
        orRequisites.Add((Feat)preReq);

      int reqSkill = int.TryParse(twoDimEntry("REQSKILL"), out reqSkill) ? reqSkill : 0;
      int reqSkillMinRank = -1;
      if(reqSkill > -1)
        reqSkillMinRank = int.TryParse(twoDimEntry("ReqSkillMinRanks"), out reqSkill) ? reqSkill : -1;

      int reqSkill2 = int.TryParse(twoDimEntry("REQSKILL"), out reqSkill2) ? reqSkill2 : 0;
      int reqSkillMinRank2 = -1;
      if (reqSkill2 > -1)
        reqSkillMinRank2 = int.TryParse(twoDimEntry("ReqSkillMinRanks"), out reqSkillMinRank2) ? reqSkillMinRank2 : -1;

      int minAttack = int.TryParse(twoDimEntry("MINATTACKBONUS"), out minAttack) ? minAttack : -1;

      Dictionary<Ability, int> minAbility = new Dictionary<Ability, int>();

      preReq = int.TryParse(twoDimEntry("MINSTR"), out preReq) ? preReq : 0;
      if (preReq > 0)
        minAbility.Add(Ability.Strength, preReq);

      preReq = int.TryParse(twoDimEntry("MINCON"), out preReq) ? preReq : 0;
      if (preReq > 0)
        minAbility.Add(Ability.Constitution, preReq);

      preReq = int.TryParse(twoDimEntry("MINDEX"), out preReq) ? preReq : 0;
      if (preReq > 0)
        minAbility.Add(Ability.Dexterity, preReq);

      preReq = int.TryParse(twoDimEntry("MININT"), out preReq) ? preReq : 0;
      if (preReq > 0)
        minAbility.Add(Ability.Intelligence, preReq);

      preReq = int.TryParse(twoDimEntry("MINSAG"), out preReq) ? preReq : 0;
      if (preReq > 0)
        minAbility.Add(Ability.Wisdom, preReq);

      preReq = int.TryParse(twoDimEntry("MINCHA"), out preReq) ? preReq : 0;
      if (preReq > 0)
        minAbility.Add(Ability.Charisma, preReq);

      entries.Add((Feat)rowIndex, new Entry(name, description, tlkName, tlkDescription, CRValue, currentLevel, successor, primaryAbility, secondaryAbility, preRequisites, orRequisites, (Skill)reqSkill, reqSkillMinRank, (Skill)reqSkill2, reqSkillMinRank2, minAttack, minAbility));
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
      public readonly List<Feat> preRequisites;
      public readonly List<Feat> orRequisites;
      public readonly Skill reqSkill;
      public readonly Skill reqSkill2;
      public readonly int reqSkillMinRank;
      public readonly int reqSkillMinRank2;
      public readonly int minAttack;
      public readonly Dictionary<Ability, int> minAbility;
      public Entry(string name, string description, uint tlkName, uint tlkDescription, int CRValue, int currentLevel, int successor, Ability primaryAbility, Ability secondaryAbility, List<Feat> preRequisites, List<Feat> orRequisites, Skill reqSkill, int reqSkillMinRank, Skill reqSkill2, int reqSkillMinRank2, int minAttack, Dictionary<Ability, int> minAbility)
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
        this.preRequisites = preRequisites;
        this.orRequisites = orRequisites;
        this.reqSkill = reqSkill;
        this.reqSkill2 = reqSkill2;
        this.reqSkillMinRank = reqSkillMinRank;
        this.reqSkillMinRank2 = reqSkillMinRank2;
        this.minAttack = minAttack;
        this.minAbility = minAbility;
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
