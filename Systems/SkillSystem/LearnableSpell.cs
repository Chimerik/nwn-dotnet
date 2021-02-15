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
      private int multiplier;
      public readonly int pointsToNextLevel;
      public readonly int primaryAbility;
      public readonly int secondaryAbility;

      public LearnableSpell(int Id, float SP, PlayerSystem.Player player)
      {
        this.oid = Id;
        this.player = player;
        this.acquiredPoints = SP;
        this.trained = false;

        int value;
        if (int.TryParse(NWScript.Get2DAString("spells", "Name", Id), out value))
          this.name = NWScript.GetStringByStrRef(value);
        else
        {
          this.name = "Nom indisponible";
          NWN.Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Spell {this.oid} : no available name");
        }

        if (int.TryParse(NWScript.Get2DAString("spells", "SpellDesc", Id), out value))
          this.description = NWScript.GetStringByStrRef(value);
        else
        {
          this.description = "Description indisponible";
          NWN.Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Spell {this.oid} : no available description");
        }

        if (int.TryParse(NWScript.Get2DAString("spells", "Wiz_Sorc", Id), out value))
          this.multiplier = value;
        else
        {
          this.multiplier = 1;
          NWN.Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Spell {this.oid} : no available level");
        }

        if (multiplier <= 0)
          multiplier = 1;

        if (int.TryParse(NWScript.Get2DAString("spells", "Druid", Id), out value) || int.TryParse(NWScript.Get2DAString("spells", "Cleric", Id), out value) || int.TryParse(NWScript.Get2DAString("spells", "Ranger", Id), out value))
          primaryAbility = NWScript.ABILITY_WISDOM;
        else
          primaryAbility = NWScript.ABILITY_INTELLIGENCE;

        secondaryAbility = NWScript.ABILITY_CHARISMA;

        int knownSpells = CreaturePlugin.GetKnownSpellCount(player.oid, 43, multiplier);
        if (knownSpells > 5)
          knownSpells = 5;

        if (knownSpells < 1)
          knownSpells = 1;

        this.pointsToNextLevel = 250 * (this.multiplier) * (int)Math.Pow(Math.Sqrt(32), knownSpells - 1);

        if (this.player.currentSkillJob == this.oid)
        {
          this.currentJob = true;
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
        JournalEntry journalEntry = new JournalEntry();
        journalEntry.sName = $"Etude - {NWN.Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}";
        journalEntry.sText = $"Etude en cours :\n\n " +
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
        journalEntry.sName = $"Etude annulée - {this.name}";
        journalEntry.sTag = "skill_job";
        journalEntry.nQuestDisplayed = 0;
        PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public void CloseSkillJournalEntry()
      {
        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid, "skill_job");
        journalEntry.sName = $"Etude terminée - {this.name}";
        journalEntry.sTag = "skill_job";
        journalEntry.nQuestCompleted = 1;
        journalEntry.nQuestDisplayed = 0;
        PlayerPlugin.AddCustomJournalEntry(player.oid, journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public double CalculateSkillPointsPerSecond()
      {
        double SP = ((double)player.oid.GetAbilityScore((Ability)primaryAbility) + ((double)player.oid.GetAbilityScore((Ability)secondaryAbility) / 2.0)) / 60.0;

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

        if (!player.isConnected)
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

        CreaturePlugin.AddKnownSpell(player.oid, 43, level, oid);
        PlayNewSkillAcquiredEffects();
        trained = true;
        player.currentSkillJob = (int)Feat.Invalid;
        player.currentSkillType = SkillType.Invalid;
        NWScript.ExportSingleCharacter(player.oid);
      }
      public void PlayNewSkillAcquiredEffects()
      {
        PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid, player.oid, NWScript.VFX_IMP_SPELL_MANTLE_USE);
        CloseSkillJournalEntry();
      }
    }
  }
}
