using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Enums;
using static NWN.Systems.PlayerSystem;

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
      private int multiplier;
      private int pointsToNextLevel;
      public readonly Ability primaryAbility;
      public readonly Ability secondaryAbility;

      public Skill(int Id, float SP)
      {
        this.oid = Id;
        this.acquiredPoints = SP;
        int value;

        // TODO : logs + message sur chan dm + message sur discord en cas de valeur non configurée

        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", Id), out value))
          this.name = NWScript.GetStringByStrRef(value);
        else
          this.name = "Nom non disponible";

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", Id), out value))
          this.description = NWScript.GetStringByStrRef(value);
        else
          this.description = "Description non disponible";

        if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", Id), out value))
          this.currentLevel = value;
        else
          this.currentLevel = 1;
 
        if (int.TryParse(NWScript.Get2DAString("feat", "CRValue", Id), out value))
          this.multiplier = value;
        else
          this.multiplier = 1;

        Dictionary<int, int> iSkillAbilities = new Dictionary<int, int>();
        
        if (int.TryParse(NWScript.Get2DAString("feat", "MINSTR", Id), out value))
          iSkillAbilities.Add((int)Ability.Strength, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINDEX", Id), out value))
          iSkillAbilities.Add((int)Ability.Dexterity, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINCON", Id), out value))
          iSkillAbilities.Add((int)Ability.Constitution, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MININT", Id), out value))
          iSkillAbilities.Add((int)Ability.Intelligence, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINWIS", Id), out value))
          iSkillAbilities.Add((int)Ability.Wisdom, value);
        if (int.TryParse(NWScript.Get2DAString("feat", "MINCHA", Id), out value))
          iSkillAbilities.Add((int)Ability.Charisma, value);

        iSkillAbilities.OrderBy(key => key.Value);

        if(iSkillAbilities.Count > 0)
          this.primaryAbility = (Ability)iSkillAbilities.ElementAt(0).Key;
        else
          this.primaryAbility = Ability.Intelligence;

        if (iSkillAbilities.Count > 1)
          this.secondaryAbility = (Ability)iSkillAbilities.ElementAt(1).Key;
        else
          this.secondaryAbility = Ability.Wisdom;

        this.pointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.currentLevel);
      }
      public double GetTimeToNextLevel(Player oPC)
      {
        float RemainingPoints = this.pointsToNextLevel - this.acquiredPoints;
        float PointsGenerationPerSecond = (float)(NWScript.GetAbilityScore(oPC, primaryAbility) + (NWScript.GetAbilityScore(oPC, secondaryAbility) / 2)) / 60;
        if(!oPC.isConnected)
          PointsGenerationPerSecond = PointsGenerationPerSecond * 60 / 100;
        else if (oPC.isAFK)
          PointsGenerationPerSecond = PointsGenerationPerSecond * 80 / 100;
        return RemainingPoints / PointsGenerationPerSecond;
      }
      public string GetTimeToNextLevelAsString(Player oPC)
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
      public void DisplayTimeToNextLevel(Player oPC)
      {
        string Countdown = this.GetTimeToNextLevelAsString(oPC);
        oPC.RefreshAcquiredSkillPoints();

        NWScript.PostString(oPC, $"Apprentissage terminé dans {Countdown}", 80, 10, ScreenAnchor.TopLeft, 1.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        
        if(oPC.Locals.Int.Get("_DISPLAY_JOBS") == 1)
          NWScript.DelayCommand(1.0f, () => DisplayTimeToNextLevel(oPC));
      }
/*      public int GetcurrentLevel()
      {
        if (this.IsAtmaxLevel())
          return this.maxLevel;
        if (this.acquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 0))
          return 0;
        if (this.acquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 1))
          return 1;
        if (this.acquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 2))
          return 2;
        if (this.acquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 3))
          return 3;

        return 4;
      }
      public Boolean IsAtmaxLevel()
      {
        if (this.acquiredPoints >= 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.maxLevel))
          return true;
        else
          return false;
      }*/
    }
  }
}
