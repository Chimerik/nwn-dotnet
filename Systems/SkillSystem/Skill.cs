using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public class Skill
    {
      public readonly int oid;
      private readonly Player player;
      public float acquiredPoints { get; set; }
      public string name { get; set; }
      public string description { get; set; }
      public Boolean currentJob { get; set; }
      public int currentLevel { get; set; }
      public int successorId { get; set; }
      public Boolean trained { get; set; }
      public Boolean databaseSaved { get; set; }
      private int multiplier;
      public readonly int pointsToNextLevel;
      public readonly int primaryAbility;
      public readonly int secondaryAbility;

      public Skill(int Id, float SP, Player player, Boolean loadedFromDB = false)
      {
        this.oid = Id;
        this.player = player;
        this.acquiredPoints = SP;
        this.trained = false;
        this.databaseSaved = loadedFromDB;

        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", Id), out value))
          this.name = NWScript.GetStringByStrRef(value);
        else
        {
          this.name = "Nom indisponible";
          Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : no available name");
        }

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", Id), out value))
          this.description = NWScript.GetStringByStrRef(value);
        else
        {
          this.description = "Description indisponible";
          Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : no available description");
        }

        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", Id), out value))
        {
          this.currentLevel = value;
          if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", Id), out value))
            this.successorId = value;
          else
            this.successorId = 0;
        }
        else
          this.currentLevel = 1;
 
        if (int.TryParse(NWScript.Get2DAString("feat", "CRValue", Id), out value))
          this.multiplier = value;
        else
          this.multiplier = 1;

        Dictionary<int, int> iSkillAbilities = new Dictionary<int, int>();
        
        if (int.TryParse(NWScript.Get2DAString("feat", "MINSTR", Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_STRENGTH, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINDEX", Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_DEXTERITY, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINCON", Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_CONSTITUTION, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MININT", Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_INTELLIGENCE, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINWIS", Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_WISDOM, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINCHA", Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_CHARISMA, value);

        iSkillAbilities.OrderBy(key => key.Value);

        if (iSkillAbilities.Count > 0)
          this.primaryAbility = iSkillAbilities.ElementAt(0).Key;
        else
        {
          this.primaryAbility = NWScript.ABILITY_INTELLIGENCE;
          Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : Primary ability not set");
        }

        if (iSkillAbilities.Count > 1)
          this.secondaryAbility = iSkillAbilities.ElementAt(1).Key;
        else
        {
          this.secondaryAbility = NWScript.ABILITY_WISDOM;
          Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : Secondary ability not set");
        }

        this.pointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.currentLevel);

        if (this.player.currentSkillJob == this.oid)
        {
          this.currentJob = true;
          this.CreateSkillJournalEntry();
        }
      }
      public double GetTimeToNextLevel(float pointPerSecond)
      {
        float RemainingPoints = this.pointsToNextLevel - this.acquiredPoints;
        return RemainingPoints / pointPerSecond;
      }
      public void CreateSkillJournalEntry()
      {
        player.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(this.GetTimeToNextLevel(player.CalculateSkillPointsPerSecond(this)));
        JournalEntry journalEntry = new JournalEntry();
        journalEntry.sName = $"Entrainement - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}";
        journalEntry.sText = $"Entrainement en cours :\n\n " +
          $"{this.name}\n\n" +
          $"{this.description}";
        journalEntry.sTag = "skill_job";
        journalEntry.nPriority = 1;
        journalEntry.nQuestDisplayed = 1;
        PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
      }
      public void CancelSkillJournalEntry()
      {
        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "skill_job");
        journalEntry.sName = $"Entrainement annulé - {this.name}";
        journalEntry.sTag = "skill_job";
        journalEntry.nQuestDisplayed = 0;
        PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public void CloseSkillJournalEntry()
      {
        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "skill_job");
        journalEntry.sName = $"Entrainement terminé - {this.name}";
        journalEntry.sTag = "skill_job";
        journalEntry.nQuestCompleted = 1;
        journalEntry.nQuestDisplayed = 0;
        PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
    }
  }
}
