using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.API.Constants;
using NWN.API;

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
      public readonly int primaryAbility;
      public readonly int secondaryAbility;

      public Skill(Feat Id, float SP, PlayerSystem.Player player)
      {
        this.oid = Id;
        this.player = player;
        this.acquiredPoints = SP;
        this.trained = false;

        int value;
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
          if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)Id), out value))
            this.name = NWScript.GetStringByStrRef(value);
          else
          {
            this.name = "Nom indisponible";
            Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : no available name");
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)Id), out value))
            this.description = NWScript.GetStringByStrRef(value);
          else
          {
            this.description = "Description indisponible";
            Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : no available description");
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", (int)Id), out value))
          {
            this.currentLevel = value;
            if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", (int)Id), out value))
              this.successorId = value;
            else
              this.successorId = 0;
          }
          else
            this.currentLevel = 1;
        }

        if (int.TryParse(NWScript.Get2DAString("feat", "CRValue", (int)Id), out value))
          this.multiplier = value;
        else
          this.multiplier = 1;

        Dictionary<int, int> iSkillAbilities = new Dictionary<int, int>();

        if (int.TryParse(NWScript.Get2DAString("feat", "MINSTR", (int)Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_STRENGTH, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINDEX", (int)Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_DEXTERITY, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINCON", (int)Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_CONSTITUTION, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MININT", (int)Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_INTELLIGENCE, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINWIS", (int)Id), out value))
          iSkillAbilities.Add(NWScript.ABILITY_WISDOM, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINCHA", (int)Id), out value))
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
        Core.NWNX.JournalEntry journalEntry = new Core.NWNX.JournalEntry();
        journalEntry.sName = $"Entrainement - {Utils.StripTimeSpanMilliseconds((TimeSpan)(player.playerJournal.skillJobCountDown - DateTime.Now))}";
        journalEntry.sText = $"Entrainement en cours :\n\n " +
          $"{this.name}\n\n" +
          $"{this.description}";
        journalEntry.sTag = "skill_job";
        journalEntry.nPriority = 1;
        journalEntry.nQuestDisplayed = 1;
        PlayerPlugin.AddCustomJournalEntry(player.oid.LoginCreature, journalEntry);

        PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1516);  
      }
      public void CancelSkillJournalEntry()
      {
        Core.NWNX.JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid.LoginCreature, "skill_job");
        journalEntry.sName = $"Entrainement annulé - {this.name}";
        journalEntry.sTag = "skill_job";
        journalEntry.nQuestDisplayed = 0;
        PlayerPlugin.AddCustomJournalEntry(player.oid.LoginCreature, journalEntry);
        player.playerJournal.skillJobCountDown = null;
      }
      public void CloseSkillJournalEntry()
      {
        Core.NWNX.JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(player.oid.LoginCreature, "skill_job");

        if (journalEntry.nUpdated == -1)
        {
          CreateSkillJournalEntry();
          journalEntry = PlayerPlugin.GetJournalEntry(player.oid.LoginCreature, "skill_job");
        }

        journalEntry.sName = $"Entrainement terminé - {this.name}";
        journalEntry.sTag = "skill_job";
        journalEntry.nQuestCompleted = 1;
        journalEntry.nQuestDisplayed = 0;
        PlayerPlugin.AddCustomJournalEntry(player.oid.LoginCreature, journalEntry);
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

          if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)oid), out int nameValue))
            PlayerPlugin.SetTlkOverride(player.oid.LoginCreature, nameValue, $"{customFeatName} - {currentLevel}");
            //player.oid.SetTlkOverride(nameValue, $"{customFeatName} - {currentLevel}");
          else
            Utils.LogMessageToDMs($"CUSTOM SKILL SYSTEM ERROR - Skill {customFeatName} - {(int)oid} : no available custom name StrRef");
          
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

        //if (!Convert.ToBoolean(CreaturePlugin.GetKnowsFeat(player.oid, oid)))
        // {
        player.oid.LoginCreature.AddFeat(oid);
        PlayNewSkillAcquiredEffects();
        //}
       /* else
        {
          int value;
          int skillCurrentLevel = CreaturePlugin.GetHighestLevelOfFeat(player.oid, oid);
          if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", skillCurrentLevel), out value))
          {
            CreaturePlugin.AddFeat(player.oid, value);
            CreaturePlugin.RemoveFeat(player.oid, value);
          }
          else
          {
            NWN.Utils.LogMessageToDMs($"SKILL LEVEL UP ERROR - Player : {NWScript.GetName(player.oid)}, Skill : {name} ({oid}), Current level : {skillCurrentLevel}");
          }
        }*/

        if (RegisterAddCustomFeatEffect.TryGetValue(oid, out Func<PlayerSystem.Player, Feat, int> handler))
          handler.Invoke(player, oid);

        player.currentSkillJob = (int)CustomFeats.Invalid;
        player.currentSkillType = SkillType.Invalid;
        currentJob = false;

        player.oid.ExportCharacter();
      }
      public void PlayNewSkillAcquiredEffects()
      {
        PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid.ControlledCreature, player.oid.ControlledCreature, 1516);
        CloseSkillJournalEntry();
      }
    }
  }
}
