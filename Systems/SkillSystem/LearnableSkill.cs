using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

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
    public bool restoreOnShortRest { get; }
    public Dictionary<int, int[]> featOptions { get; }
    public List<int> racePrerequiste { get; }
    public List<int> learnablePrerequiste { get; }


    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility,
      Ability secondaryAbility, Func<PlayerSystem.Player, int, bool> skillEffect = null, string descriptionLink = "", List<int> racePrerequiste = null, List<int> learnablePrerequiste = null, bool restoreOnShortRest = false) 
      : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility, descriptionLink)
    {
      this.category = category;
      this.skillEffect = skillEffect;
      this.racePrerequiste = racePrerequiste;
      this.learnablePrerequiste = learnablePrerequiste;
      this.restoreOnShortRest = restoreOnShortRest;
    }
    public LearnableSkill(LearnableSkill learnableBase, int skillSource = -1, bool active = false, double acquiredSP = 0, int currentLevel = 0, int levelTaken = 0, Dictionary<int, int[]> featOptions = null, bool restoreOnShortRest = false) : base(learnableBase)
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
      this.restoreOnShortRest = restoreOnShortRest;
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
      restoreOnShortRest = serializableBase.restoreOnShortRest;
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
      public bool restoreOnShortRest { get; set; }
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
        restoreOnShortRest = learnableBase.restoreOnShortRest;

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
      active = false;

      skillEffect?.Invoke(player, id);
      pointsToNextLevel = GetPointsToLevelUp(player);

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
    private double GetPointsToLevelUp(PlayerSystem.Player player)
    {
      switch(category)
      {
        case SkillSystem.Category.Class:
        case SkillSystem.Category.FighterSubClass:

          double multiClassMultiplier = player.oid.LoginCreature.Level > 11 
            ? player.oid.LoginCreature.Classes.Distinct().Count() * 1.5  : 1;

          return player.oid.LoginCreature.Level switch
          {
            1 => 300 * multiClassMultiplier,
            2 => 900 * multiClassMultiplier,
            3 => 2700 * multiClassMultiplier,
            4 => 6500 * multiClassMultiplier,
            5 => 14000 * multiClassMultiplier,
            6 => 23000 * multiClassMultiplier,
            7 => 34000 * multiClassMultiplier,
            8 => 48000 * multiClassMultiplier,
            9 => 64000 * multiClassMultiplier,
            10 => 85000 * multiClassMultiplier,
            11 => 100000 * multiClassMultiplier,
            12 => 120000 * multiClassMultiplier,
            13 => 140000 * multiClassMultiplier,
            14 => 165000 * multiClassMultiplier,
            15 => 195000 * multiClassMultiplier,
            16 => 225000 * multiClassMultiplier,
            17 => 265000 * multiClassMultiplier,
            18 => 305000 * multiClassMultiplier,
            19 => 355000 * multiClassMultiplier,
            20 => 408250 * multiClassMultiplier,
            _ => 408250 * multiClassMultiplier * 1.15 * (player.oid.LoginCreature.Level - 20),
          };
        default:
          return 250 * multiplier * Math.Pow(5, currentLevel);
      }
    }
  }
}
