using System;
using NWN.Core.NWNX;
using NWN.API.Constants;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public class Skill
    {
      public readonly Feat oid;
      private readonly PlayerSystem.Player player;
      public double acquiredPoints { get; set; }
      public string name { get; set; }
      public string description { get; set; }
      public Boolean currentJob { get; set; }
      public int currentLevel { get; set; }
      public int successorId { get; set; }
      public Boolean trained { get; set; }
      public int multiplier { get; }
      public int pointsToNextLevel;
      public Ability primaryAbility { get; }
      public Ability secondaryAbility { get; }

      public Skill(Feat Id, float SP, PlayerSystem.Player player)
      {
        this.oid = Id;
        this.player = player;
        this.acquiredPoints = SP;
        this.trained = false;

        FeatTable.Entry entry = Feat2da.featTable.GetFeatDataEntry(Id);

        if (customFeatsDictionnary.ContainsKey(Id))
        {
          name = customFeatsDictionnary[Id].name;
          description = customFeatsDictionnary[Id].description;
          if (player.learntCustomFeats.ContainsKey(Id))
            currentLevel = GetCustomFeatLevelFromSkillPoints(Id, (int)SP);
          else
            currentLevel = 0;
        }
        else
        {
          this.name = entry.name;
          this.description = entry.description;
          this.currentLevel = entry.currentLevel;
          this.successorId = entry.successor;
        }

        this.multiplier = entry.CRValue;
        this.primaryAbility = entry.primaryAbility;
        this.secondaryAbility = entry.secondaryAbility;
  
        if (this.currentLevel > 4)
        {
          int skillLevelCap = 4;
          this.pointsToNextLevel = (int)(250 * this.multiplier * Math.Pow(5, skillLevelCap)) * (1 + currentLevel - skillLevelCap);
        }
        else
          this.pointsToNextLevel = (int)(250 * this.multiplier * Math.Pow(5, currentLevel));

        //if (Config.env == Config.Env.Chim)
        //pointsToNextLevel = 10;

        if (this.player.currentSkillJob == (int)oid)
        {
          this.currentJob = true;
          if(player.oid.LoginCreature.GetLocalVariable<int>("_CONNECTING").HasNothing)
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
        player.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(this.GetTimeToNextLevel(CalculateSkillPointsPerSecond()));
        API.JournalEntry journalEntry = new API.JournalEntry();
        journalEntry.Name = $"Entrainement - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}";
        journalEntry.Text = $"Entrainement en cours :\n\n " +
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
        journalEntry.Name = $"Entrainement annulé - {this.name}";
        journalEntry.QuestTag = "skill_job";
        journalEntry.QuestDisplayed = false;
        player.oid.AddCustomJournalEntry(journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public void CloseSkillJournalEntry()
      {
        API.JournalEntry journalEntry = player.oid.GetJournalEntry("skill_job");

        if (journalEntry == null)
        {
          CreateSkillJournalEntry();
          journalEntry = player.oid.GetJournalEntry("skill_job");
        }

        journalEntry.Name = $"Entrainement terminé - {this.name}";
        journalEntry.QuestTag = "skill_job";
        journalEntry.QuestCompleted = true;
        journalEntry.QuestDisplayed = false;
        player.oid.AddCustomJournalEntry(journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public double CalculateSkillPointsPerSecond()
      {
        double SP = (player.oid.LoginCreature.GetAbilityScore(primaryAbility) + (player.oid.LoginCreature.GetAbilityScore(secondaryAbility) / 2.0)) / 60.0;

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

        if (player.oid.LoginCreature.GetLocalVariable<int>("_CONNECTING").HasValue)
        {
          SP = SP * 60 / 100;
          Log.Info($"{player.oid.LoginCreature.Name} was not connected. Applying 40 % malus.");
        }
        else if (player.isAFK)
        {
          SP = SP * 80 / 100;
          Log.Info($"{player.oid.LoginCreature.Name} was afk. Applying 20 % malus.");
        }

        //Log.Info($"SP CALCULATION - {player.oid.Name} - {SP} SP.");

        return SP;
      }
      public void RefreshAcquiredSkillPoints()
      {
        int pooledPoints = ObjectPlugin.GetInt(player.oid.LoginCreature, "_STARTING_SKILL_POINTS");
        
        if (pooledPoints > 0)
        {
          if (pooledPoints > pointsToNextLevel)
          {
            ObjectPlugin.SetInt(player.oid.LoginCreature, "_STARTING_SKILL_POINTS", pooledPoints - pointsToNextLevel, 1);
            acquiredPoints += pointsToNextLevel;
          }
          else
          {
            acquiredPoints += pooledPoints;
            ObjectPlugin.DeleteInt(player.oid.LoginCreature, "_STARTING_SKILL_POINTS");
          }
        }

        double skillPointRate = CalculateSkillPointsPerSecond();
        acquiredPoints += skillPointRate * (DateTime.Now - player.dateLastSaved).TotalSeconds;
        double remainingTime = GetTimeToNextLevel(skillPointRate);
        player.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(remainingTime);

        if (remainingTime <= 0)
          LevelUpSkill();
      }
      public void LevelUpSkill()
      {
        if (player.menu.isOpen)
          player.menu.Close();
       
        if(customFeatsDictionnary.ContainsKey(oid)) // Il s'agit d'un Custom Feat
        {
          if (player.learntCustomFeats.ContainsKey(oid))
            player.learntCustomFeats[oid] = (int)acquiredPoints;
          else
            player.learntCustomFeats.Add(oid, (int)acquiredPoints);

          string customFeatName = customFeatsDictionnary[oid].name;
          name = customFeatName;

          currentLevel = GetCustomFeatLevelFromSkillPoints(oid, (int)acquiredPoints);

          int skillLevelCap = currentLevel;

          if (this.currentLevel > 4)
          {
            skillLevelCap = 4;
            pointsToNextLevel += (int)(250 * this.multiplier * Math.Pow(5, skillLevelCap));
          }
          else
            pointsToNextLevel = (int)(250 * this.multiplier * Math.Pow(5, skillLevelCap));
          
          player.oid.SetTlkOverride((int)Feat2da.featTable.GetFeatDataEntry(oid).tlkName, $"{customFeatName} - {currentLevel}");
          
          if (currentLevel >= customFeatsDictionnary[oid].maxLevel)
            trained = true;
        }
        else
        {
          trained = true;

          if (successorId > 0)
          {
            player.learnableSkills.Add((Feat)successorId, new Skill((Feat)successorId, 0, player));
          }
        }

        player.oid.LoginCreature.AddFeat(oid);
        PlayNewSkillAcquiredEffects();

        if (RegisterAddCustomFeatEffect.TryGetValue(oid, out Func<PlayerSystem.Player, Feat, int> handler))
          handler.Invoke(player, oid);

        player.currentSkillJob = (int)CustomFeats.Invalid;
        player.currentSkillType = SkillType.Invalid;
        currentJob = false;

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
