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

      public float AcquiredPoints { get; set; }
      public string Nom { get; set; }
      public string Description { get; set; }
      public Boolean CurrentJob { get; set; }
      private int MaxLevel;
      private int CurrentLevel;
      private int multiplier;
      private int PointsToNextLevel;
      public readonly Ability PrimaryAbility;
      public readonly Ability SecondaryAbility;

      public Skill(int Id, float SP)
      {
        this.oid = Id;
        this.AcquiredPoints = SP;
        this.Nom = NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", Id)));
        this.Description = NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "DESCRIPTION", Id)));
        this.MaxLevel = int.Parse(NWScript.Get2DAString("feat", "GAINMULTIPLE", Id));
        this.CurrentLevel = this.GetCurrentLevel();
        this.multiplier = int.Parse(NWScript.Get2DAString("feat", "CRValue", Id)); ;

        Dictionary<int, int> iSkillAbilities = new Dictionary<int, int>();
        int value;
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
        this.PrimaryAbility = (Ability)iSkillAbilities.ElementAt(0).Key;
        this.SecondaryAbility = (Ability)iSkillAbilities.ElementAt(1).Key;

        this.PointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.CurrentLevel - 1);
      }
      public double GetTimeToNextLevel(Player oPC)
      {
        float RemainingPoints = this.PointsToNextLevel - this.AcquiredPoints;
        float PointsGenerationPerSecond = (float)(NWScript.GetAbilityScore(oPC, PrimaryAbility) + (NWScript.GetAbilityScore(oPC, SecondaryAbility) / 2)) / 60;
        return RemainingPoints / PointsGenerationPerSecond;
      }
      public string GetTimeToNextLevelAsString(Player oPC)
      {
        TimeSpan EndTime = DateTime.Now.AddSeconds(this.GetTimeToNextLevel(oPC)).Subtract(DateTime.Now);
        string Countdown = "";
        if (EndTime.Days > 0)
          Countdown += EndTime.Days + ":";
        if (EndTime.Hours > 0)
          Countdown += EndTime.Hours + ":";
        if (EndTime.Minutes > 0)
          Countdown += EndTime.Minutes + ":";
        if (EndTime.Seconds > 0)
          Countdown += EndTime.Seconds;

        return Countdown;
      }
      public void DisplayTimeToNextLevel(Player oPC)
      {
        string Countdown = this.GetTimeToNextLevelAsString(oPC);

        NWScript.PostString(oPC, $"Apprentissage terminé dans {Countdown}", 80, 10, ScreenAnchor.TopLeft, 1.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        NWScript.DelayCommand(1.0f, () => DisplayTimeToNextLevel(oPC));
      }
      public int GetCurrentLevel()
      {
        if (this.AcquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 1 - 1))
          return 1;
        if (this.AcquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 2 - 1))
          return 2;
        if (this.AcquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 1 - 1))
          return 3;
        if (this.AcquiredPoints < 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), 4 - 1))
          return 4;

        return 5;
      }
    }
  }
}
