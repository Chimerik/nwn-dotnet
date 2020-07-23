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

      private int AcquiredPoints;
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

      public int CalculateTimeToNextLevel(Player oPC)
      {
        int PointsToNextLevel = 250 * this.multiplier * (int)Math.Pow(Math.Sqrt(32), this.CurrentLevel - 1);
        int RemainingPoints = PointsToNextLevel - this.AcquiredPoints;
        int PointsGenerationPerSecond = (NWScript.GetAbilityScore(oPC, PrimaryAbility) + (NWScript.GetAbilityScore(oPC, SecondaryAbility) / 2)) / 60;
        int RemainingSeconds = RemainingPoints / PointsGenerationPerSecond;
        DateTime EndDate = DateTime.Now.AddSeconds(RemainingSeconds);
        return 0;
      }
    }
  }
}
