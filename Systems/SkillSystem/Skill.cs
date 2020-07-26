using System;
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
        // TODO : charger ces données à partir des 2da
        this.oid = Id;
        this.Nom = "Placeholder Title";
        this.Description = "Placeholder Description";
        this.AcquiredPoints = SP;
        this.MaxLevel = 5;
        this.CurrentLevel = 1;
        this.multiplier = 1;
        this.PrimaryAbility = Ability.Strength;
        this.SecondaryAbility = Ability.Dexterity;
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
    }
  }
}
