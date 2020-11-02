using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Systems;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public class Skill
    {
      public readonly int oid;

      public float acquiredPoints { get; set; }
      public string name { get; set; }
      public string description { get; set; }
      public Boolean currentJob { get; set; }
      public int currentLevel { get; set; }
      public int successorId { get; set; }
      public Boolean trained { get; set; }
      public Boolean databaseSaved { get; set; }
      private int multiplier;
      private int pointsToNextLevel;
      public readonly int primaryAbility;
      public readonly int secondaryAbility;

      public Skill(int Id, float SP)
      {
        this.oid = Id;
        this.acquiredPoints = SP;
        this.trained = false;
        int value;

        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", Id), out value))
          this.name = NWScript.GetStringByStrRef(value);
        else
        {
          this.name = "Nom non disponible";
          Utils.LogMessageToDMs($"SKILL SYSTEM ERROR - Skill {this.oid} : no available name");
        }

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", Id), out value))
          this.description = NWScript.GetStringByStrRef(value);
        else
        {
          this.description = "Description non disponible";
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
      }
      public double GetTimeToNextLevel(PlayerSystem.Player oPC)
      {
        float RemainingPoints = this.pointsToNextLevel - this.acquiredPoints;
        float PointsGenerationPerSecond = (float)(NWScript.GetAbilityScore(oPC.oid, primaryAbility) + (NWScript.GetAbilityScore(oPC.oid, secondaryAbility) / 2)) / 60;
        if(!oPC.isConnected)
          PointsGenerationPerSecond = PointsGenerationPerSecond * 60 / 100;
        else if (oPC.isAFK)
          PointsGenerationPerSecond = PointsGenerationPerSecond * 80 / 100;
        return RemainingPoints / PointsGenerationPerSecond;
      }
      public string GetTimeToNextLevelAsString(PlayerSystem.Player oPC)
      {
        TimeSpan EndTime = DateTime.Now.AddSeconds(this.GetTimeToNextLevel(oPC)).Subtract(DateTime.Now);
        string Countdown = "";
        if (EndTime.Days > 0)
        {
          if (EndTime.Days < 10)
            Countdown += "0" + EndTime.Days + ":";
          else
            Countdown += EndTime.Days + ":";
        }
        if (EndTime.Hours > 0)
        {
          if (EndTime.Hours < 10)
            Countdown += "0" + EndTime.Hours + ":";
          else
            Countdown += EndTime.Hours + ":";
        }
        if (EndTime.Minutes > 0)
        {
          if (EndTime.Minutes < 10)
            Countdown += "0" + EndTime.Minutes + ":";
          else
            Countdown += EndTime.Minutes + ":";
        }
        if (EndTime.Seconds > 0)
        {
          if (EndTime.Seconds < 10)
            Countdown += "0" + EndTime.Seconds;
          else
            Countdown += EndTime.Seconds;
        }

        return Countdown;
      }
 /*     public void DisplayTimeToNextLevel(Player oPC) // TODO : revoir méthode d'affichage du temps restant pour skill + craft jobs
      {
        string Countdown = this.GetTimeToNextLevelAsString(oPC);
        oPC.RefreshAcquiredSkillPoints();

        NWScript.PostString(oPC.oid, $"Apprentissage terminé dans {Countdown}", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 1.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        
        if(NWScript.GetLocalInt(oPC.oid, "_DISPLAY_JOBS") == 1)
          NWScript.DelayCommand(1.0f, () => DisplayTimeToNextLevel(oPC));
      }*/
    }
  }
}
