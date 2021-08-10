using System;

using Anvil.API;

namespace NWN.Systems
{
  public enum LearnableType
  {
    Feat,
    Spell
  }
  public class Learnable
  {
    public readonly int id;
    public readonly Feat featId;
    public readonly Spell spellId;
    public readonly LearnableType type;
    public double acquiredPoints { get; set; }
    public string name;
    public string description;
    public Boolean active { get; set; }
    public int currentLevel { get; set; }
    public int successorId { get; set; } 
    public Boolean trained { get; set; }
    public float multiplier { get; set; }
    public int pointsToNextLevel { get; set; }
    public Ability primaryAbility { get; set; }
    public Ability secondaryAbility { get; set; }
    public int nbScrollsUsed { get; set; }
    public DateTime levelUpDate { get; set; }

    public Learnable(LearnableType type, int id, float SP, Boolean active = false, int scrollsUsed = 0)
    {
      this.id = id;
      this.type = type;
      this.acquiredPoints = SP;
      this.trained = false;
      this.nbScrollsUsed = scrollsUsed;
      this.active = active;

      switch (type)
      {
        case LearnableType.Feat:
          featId = (Feat)id;
          InitializeLearnableFeat(featId, (int)SP);
          break;
        case LearnableType.Spell:
          spellId = (Spell)id;
          InitializeLearnableSpell(spellId, (int)SP);
          break;
      }
    }
    private void InitializeLearnableFeat(Feat id, int SP)
    {
      FeatTable.Entry entry = Feat2da.featTable.GetFeatDataEntry(id);

      if (SkillSystem.customFeatsDictionnary.ContainsKey(id))
      {
        name = SkillSystem.customFeatsDictionnary[id].name;
        description = SkillSystem.customFeatsDictionnary[id].description;
      }
      else
      {
        name = entry.name;
        description = entry.description;
        currentLevel = entry.currentLevel;
        successorId = entry.successor;
      }

      multiplier = entry.CRValue;
      primaryAbility = entry.primaryAbility;
      secondaryAbility = entry.secondaryAbility;
    }
    public Learnable InitializeLearnableLevel(PlayerSystem.Player player)
    {
      switch(type)
      {
        case LearnableType.Feat:

          if (SkillSystem.customFeatsDictionnary.ContainsKey(featId))
          {
            player.learntCustomFeats.Add(featId, (int)acquiredPoints);
            currentLevel = SkillSystem.GetCustomFeatLevelFromSkillPoints(featId, (int)acquiredPoints);
          }
          else
            currentLevel = 0;

          if (currentLevel > 4)
          {
            int skillLevelCap = 4;
            pointsToNextLevel = (int)(250 * multiplier * Math.Pow(5, skillLevelCap)) * (1 + currentLevel - skillLevelCap);
          }
          else
            pointsToNextLevel = (int)(250 * multiplier * Math.Pow(5, currentLevel));

          //if (Config.env == Config.Env.Chim)
          //pointsToNextLevel = 10;

          break;
        case LearnableType.Spell:

          int knownSpells = player.oid.LoginCreature.GetClassInfo((ClassType)43).GetKnownSpellCountByLevel((byte)multiplier);
          if (knownSpells > 3)
            knownSpells = 3;

          if (knownSpells < 1)
            knownSpells = 1;

          pointsToNextLevel = (int)(250 * multiplier * Math.Pow(5, knownSpells - 1));

          break;
      }

      return this;
    }
    private void InitializeLearnableSpell(Spell id, int SP)
    {
      SpellsTable.Entry entry = Spells2da.spellsTable.GetSpellDataEntry(id);

      name = entry.name;
      description = entry.description;
      multiplier = entry.level;

      if (entry.castingClass == ClassType.Druid || entry.castingClass == ClassType.Cleric || entry.castingClass == ClassType.Ranger)
        primaryAbility = Ability.Wisdom;
      else
        primaryAbility = Ability.Intelligence;

      secondaryAbility = Ability.Charisma;
    }
  }
}
