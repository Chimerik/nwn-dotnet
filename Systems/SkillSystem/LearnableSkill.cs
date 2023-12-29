﻿using System;
using System.Collections.Generic;

using Anvil.API;

namespace NWN.Systems
{
  public class LearnableSkill : Learnable
  {
    public SkillSystem.Category category { get; }
    public List<SkillSystem.Category> source { get; }
    private Func<PlayerSystem.Player, int, bool> skillEffect { get; }
    public int totalPoints { get { return currentLevel /*+ bonusPoints*/; } }
    public double bonusMultiplier { get { return 1 + (totalPoints / 100); } }
    public double bonusReduction { get { return 1 - (totalPoints / 100); } }
    public int levelTaken { get; }
    public Dictionary<int, int[]> featOptions { get; }
    public List<int> racePrerequiste { get; }
    public List<int> learnablePrerequiste { get; }


    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility,
      Ability secondaryAbility, Func<PlayerSystem.Player, int, bool> skillEffect = null, string descriptionLink = "", List<int> racePrerequiste = null, List<int> learnablePrerequiste = null) 
      : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility, descriptionLink)
    {
      this.category = category;
      this.skillEffect = skillEffect;
      this.racePrerequiste = racePrerequiste;
      this.learnablePrerequiste = learnablePrerequiste;
    }
    public LearnableSkill(LearnableSkill learnableBase, int skillSource = -1, bool active = false, double acquiredSP = 0, int currentLevel = 0, int levelTaken = 0, Dictionary<int, int[]> featOptions = null) : base(learnableBase)
    {
      this.category = learnableBase.category;
      this.skillEffect = learnableBase.skillEffect;
      this.racePrerequiste = learnableBase.racePrerequiste;
      this.learnablePrerequiste = learnableBase.learnablePrerequiste;
      this.active = active;
      this.acquiredPoints = acquiredSP;
      this.currentLevel = currentLevel;
      this.pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
      this.levelTaken = levelTaken;
      this.featOptions = featOptions;
      this.source = new();

      if (skillSource > -1)
        source.Add((SkillSystem.Category)skillSource);
    }
    public LearnableSkill(LearnableSkill learnableBase, SerializableLearnableSkill serializableBase) 
      : base(learnableBase)
    {
      category = learnableBase.category;
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
      spLastCalculation = serializableBase.spLastCalculation;
      skillEffect = learnableBase.skillEffect;
      levelTaken = serializableBase.levelTaken;
      featOptions = serializableBase.featOptions;
      source = new();

      if(serializableBase.source is not null)
        foreach(int sourceId in serializableBase.source)
          source.Add((SkillSystem.Category)sourceId);
    }

    public class SerializableLearnableSkill
    {
      public bool active { get; set; }
      public double acquiredPoints { get; set; }
      public int currentLevel { get; set; }
      public int levelTaken { get; set; }
      public Dictionary<int, int[]> featOptions { get; set; }
      public List<int> source { get; set; }
      public DateTime? spLastCalculation { get; set; }

      public SerializableLearnableSkill()
      {

      }
      public SerializableLearnableSkill(LearnableSkill learnableBase)
      {
        active = learnableBase.active;
        acquiredPoints = learnableBase.acquiredPoints;
        currentLevel = learnableBase.currentLevel;
        levelTaken = learnableBase.levelTaken;
        featOptions = learnableBase.featOptions;
        spLastCalculation = learnableBase.spLastCalculation;

        if (learnableBase.source is not null)
        {
          List<int> newSource = new();

          foreach (var skill in learnableBase.source)
            newSource.Add((int)skill);

          source = newSource;
        }
      }
    }

    public void LevelUp(PlayerSystem.Player player)
    {
      acquiredPoints = pointsToNextLevel;
      currentLevel += 1;
      pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
      active = false;

      skillEffect?.Invoke(player, id);

      if (player.TryGetOpenedWindow("activeLearnable", out PlayerSystem.Player.PlayerWindow activeLearnableWindow))
      {
        PlayerSystem.Player.ActiveLearnableWindow window = (PlayerSystem.Player.ActiveLearnableWindow)activeLearnableWindow;
        window.timeLeft.SetBindValue(player.oid, window.nuiToken.Token, "Apprentissage terminé");
        window.level.SetBindValue(player.oid, window.nuiToken.Token, $"{currentLevel}/{maxLevel}");
      }

      if (player.TryGetOpenedWindow("learnables", out PlayerSystem.Player.PlayerWindow learnableWindow))
      {
        PlayerSystem.Player.LearnableWindow window = (PlayerSystem.Player.LearnableWindow)learnableWindow;
        window.LoadLearnableList(window.currentList);
      }

      player.oid.ExportCharacter();
      LogUtils.LogMessage($"{player.oid.LoginCreature.Name} maîtrise {name} niveau {currentLevel}", LogUtils.LogType.Learnables);
    }
  }
}
