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
      private int multiplier;
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
            currentLevel = GetCustomFeatLevelFromSkillPoints(Id, player.learntCustomFeats[Id]);
          else
            currentLevel = 1;
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

        if (currentLevel < 1)
          currentLevel = 1;

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

        this.pointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.currentLevel - 1);

        //if (Config.env == Config.Env.Chim)
          //pointsToNextLevel = 10;

        if (this.player.currentSkillJob == (int)oid)
        {
          this.currentJob = true;
          if(player.oid.GetLocalVariable<int>("_CONNECTING").HasNothing)
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
        Log.Info("Calculating Skill Points from Create journal entry");
        player.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(this.GetTimeToNextLevel(CalculateSkillPointsPerSecond()));
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

        if (player.oid.GetLocalVariable<int>("_CONNECTING").HasValue)
        {
          SP = SP * 60 / 100;
          Log.Info($"{player.oid.Name} was not connected. Applying 40 % malus.");
        }
        else if (player.isAFK)
        {
          SP = SP * 80 / 100;
          Log.Info($"{player.oid.Name} was afk. Applying 20 % malus.");
        }

        //Log.Info($"SP CALCULATION - {player.oid.Name} - {SP} SP.");

        return SP;
      }
      public void RefreshAcquiredSkillPoints()
      {
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
          currentLevel += 1;
          pointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.currentLevel - 1);

          if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)oid), out int nameValue))
            player.oid.SetTlkOverride(nameValue, $"{customFeatName} - {currentLevel}");
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
          CreaturePlugin.AddFeat(player.oid, (int)oid);
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

        player.oid.ExportCharacter();
      }
      public void PlayNewSkillAcquiredEffects()
      {
        PlayerPlugin.ApplyInstantVisualEffectToObject(player.oid, player.oid, NWScript.VFX_IMP_GLOBE_USE);
        CloseSkillJournalEntry();
      }
    }
  }
}
