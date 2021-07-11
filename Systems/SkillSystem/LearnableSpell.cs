using System;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public class LearnableSpell
    {
      public readonly int oid;
      private readonly PlayerSystem.Player player;
      public double acquiredPoints { get; set; }
      public string name { get; set; }
      public string description { get; set; }
      public Boolean currentJob { get; set; }
      public int level { get; set; }
      public int successorId { get; set; }
      public Boolean trained { get; set; }
      private float multiplier;
      public int nbScrollsUsed { get; set; }
      public readonly int pointsToNextLevel;
      public readonly Ability primaryAbility;
      public readonly Ability secondaryAbility;

      public LearnableSpell(int Id, float SP, PlayerSystem.Player player, int scrollsUsed = 0)
      {
        this.oid = Id;
        this.player = player;
        this.acquiredPoints = SP;
        this.trained = false;
        this.nbScrollsUsed = scrollsUsed;

        SpellsTable.Entry entry = Spells2da.spellsTable.GetSpellDataEntry((Spell)Id);

        this.name = entry.name;
        this.description = entry.description;
        this.multiplier = entry.level;

        if (entry.castingClass == ClassType.Druid || entry.castingClass == ClassType.Cleric || entry.castingClass == ClassType.Ranger)
          primaryAbility = Ability.Wisdom;
        else
          primaryAbility = Ability.Intelligence;

        secondaryAbility = Ability.Charisma;
        
        int knownSpells = player.oid.LoginCreature.GetClassInfo((ClassType)43).GetKnownSpellCountByLevel((byte)multiplier);
        if (knownSpells > 3)
          knownSpells = 3;

        if (knownSpells < 1)
          knownSpells = 1;

        this.pointsToNextLevel = (int)(250 * multiplier * Math.Pow(5, knownSpells - 1));

        if (this.player.currentSkillJob == this.oid)
        {
          this.currentJob = true;
          if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CONNECTING").HasNothing)
            this.CreateSkillJournalEntry();
        }
      }
      public double GetTimeToNextLevel(double pointPerSecond)
      {
        double RemainingPoints = this.pointsToNextLevel - this.acquiredPoints;
        return RemainingPoints / pointPerSecond;
      }
      public void CreateSkillJournalEntry()
      {
        player.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(this.GetTimeToNextLevel(this.CalculateSkillPointsPerSecond()));
        API.JournalEntry journalEntry = new API.JournalEntry();
        journalEntry.Name = $"Etude - {NWN.Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}";
        journalEntry.Text = $"Etude en cours :\n\n " +
          $"{this.name}\n\n" +
          $"{this.description}";
        journalEntry.QuestTag = "skill_job";
        journalEntry.Priority = 1;
        journalEntry.QuestDisplayed = true;
        player.oid.AddCustomJournalEntry(journalEntry);

        player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
      }
      public void CancelSkillJournalEntry()
      {
        API.JournalEntry journalEntry = player.oid.GetJournalEntry("skill_job");
        journalEntry.Name = $"Etude annulée - {this.name}";
        journalEntry.QuestTag = "skill_job";
        journalEntry.QuestDisplayed = false;
        player.oid.AddCustomJournalEntry(journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public void CloseSkillJournalEntry()
      {
        API.JournalEntry journalEntry = player.oid.GetJournalEntry("skill_job");
        journalEntry.Name = $"Etude terminée - {this.name}";
        journalEntry.QuestTag = "skill_job";
        journalEntry.QuestCompleted = true;
        journalEntry.QuestDisplayed = false;
        player.oid.AddCustomJournalEntry(journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public double CalculateSkillPointsPerSecond()
      {
        double SP = ((double)player.oid.LoginCreature.GetAbilityScore((Ability)primaryAbility) + ((double)player.oid.LoginCreature.GetAbilityScore((Ability)secondaryAbility) / 2.0)) / 60.0;

        switch (player.bonusRolePlay)
        {
          case 0:
            SP = SP * 10 / 100;
            break;
          case 1:
            SP = SP * 90 / 100;
            break;
          case 3:
            SP = SP * 110 / 100;
            break;
          case 4:
            SP = SP * 120 / 100;
            break;
          case 100:
            SP = SP * 10;
            break;
        }

        if (player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_CONNECTING").HasValue)
          SP = SP * 60 / 100;
        else if (player.isAFK)
          SP = SP * 80 / 100;

        return SP;
      }
      public void RefreshAcquiredSkillPoints()
      {
        double skillPointRate = CalculateSkillPointsPerSecond();
        acquiredPoints += skillPointRate * (float)(DateTime.Now - player.dateLastSaved).TotalSeconds;
        double remainingTime = GetTimeToNextLevel(skillPointRate);
        player.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(remainingTime);

        if (remainingTime <= 0)
          LevelUpSkill();
      }
      public void LevelUpSkill()
      {
        if (player.menu.isOpen)
          player.menu.Close();

        player.oid.LoginCreature.GetClassInfo((ClassType)43).AddKnownSpell((Spell)oid, (byte)level);
        PlayNewSkillAcquiredEffects();
        trained = true;
        player.currentSkillJob = (int)CustomFeats.Invalid;
        player.currentSkillType = SkillType.Invalid;
        player.oid.ExportCharacter();
      }
      public void PlayNewSkillAcquiredEffects()
      {
        player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
        CloseSkillJournalEntry();
      }
    }
  }
}
