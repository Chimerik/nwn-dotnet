using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using NWN.Enums;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public class Skill
    {
      public readonly uint oid;

      private float AcquiredPoints;
      private int MaxLevel;
      private int CurrentLevel;
      private int multiplier;
      private Ability PrimaryAbility;
      private Ability SecondaryAbility;

      public Skill()
      {
        this.AcquiredPoints = 0;
        this.MaxLevel = 5;
        this.CurrentLevel = 1;
        this.multiplier = 1;
        this.PrimaryAbility = Ability.Strength;
        this.SecondaryAbility = Ability.Dexterity;
      }

      public void CalculateTimeToNextLevel(Player oPC)
      {
        int PointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.CurrentLevel - 1);
        float RemainingPoints = PointsToNextLevel - this.AcquiredPoints;
        float PointsGenerationPerSecond = (float)(NWScript.GetAbilityScore(oPC, PrimaryAbility) + (NWScript.GetAbilityScore(oPC, SecondaryAbility) / 2)) / 60;
        double RemainingSeconds = RemainingPoints / PointsGenerationPerSecond;
        TimeSpan EndTime = DateTime.Now.AddSeconds(RemainingSeconds).Subtract(DateTime.Now);
        string Countdown = "";
        if (EndTime.Days > 0)
          Countdown += EndTime.Days + ":";
        if (EndTime.Hours > 0)
          Countdown += EndTime.Hours + ":";
        if (EndTime.Minutes > 0)
          Countdown += EndTime.Minutes + ":";
        if (EndTime.Seconds > 0)
          Countdown += EndTime.Seconds;
        NWScript.PostString(oPC, $"Apprentissage terminé dans {Countdown}", 80, 10, ScreenAnchor.TopLeft, 30.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");

        this.AcquiredPoints += PointsGenerationPerSecond;
        NWScript.DelayCommand(1.0f, () => CalculateTimeToNextLevel(oPC));
      }
    }
  }
}
