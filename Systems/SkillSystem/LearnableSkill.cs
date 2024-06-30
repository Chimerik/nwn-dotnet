using System;
using System.Collections.Generic;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public class LearnableSkill : Learnable
  {
    public SkillSystem.Category category { get; }
    public List<SkillSystem.Category> source { get; }
    private Func<Player, int, bool> skillEffect { get; }
    public int totalPoints { get { return currentLevel /*+ bonusPoints*/; } }
    public double bonusMultiplier { get { return 1 + (totalPoints / 100); } }
    public double bonusReduction { get { return 1 - (totalPoints / 100); } }
    public int levelTaken { get; }
    public bool restoreOnShortRest { get; }
    public Dictionary<int, int[]> featOptions { get; set; }
    public List<int> racePrerequiste { get; }
    public List<int> learnablePrerequiste { get; }


    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility,
      Ability secondaryAbility, Func<Player, int, bool> skillEffect = null, string descriptionLink = "", List<int> racePrerequiste = null, List<int> learnablePrerequiste = null, bool restoreOnShortRest = false) 
      : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility, descriptionLink)
    {
      this.category = category;
      this.skillEffect = skillEffect;
      this.racePrerequiste = racePrerequiste;
      this.learnablePrerequiste = learnablePrerequiste;
      this.restoreOnShortRest = restoreOnShortRest;
    }
    public LearnableSkill(LearnableSkill learnableBase, Player player, int skillSource = -1, bool active = false, double acquiredSP = 0, int currentLevel = 0, int levelTaken = 0, Dictionary<int, int[]> featOptions = null, bool restoreOnShortRest = false) : base(learnableBase)
    {
      this.category = learnableBase.category;
      this.skillEffect = learnableBase.skillEffect;
      this.racePrerequiste = learnableBase.racePrerequiste;
      this.learnablePrerequiste = learnableBase.learnablePrerequiste;
      this.restoreOnShortRest = learnableBase.restoreOnShortRest;
      this.active = active;
      this.acquiredPoints = acquiredSP;
      this.currentLevel = currentLevel;
      this.pointsToNextLevel = GetPointsToLevelUp(player);
      this.levelTaken = levelTaken;
      this.featOptions = featOptions;
      this.source = new();

      if (skillSource > -1)
        source.Add((SkillSystem.Category)skillSource);
    }
    public LearnableSkill(LearnableSkill learnableBase, SerializableLearnableSkill serializableBase, Player player, int playerLevel = 0, int multiClassMultiplier = 0) 
      : base(learnableBase)
    {
      category = learnableBase.category;
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      pointsToNextLevel = GetPointsToLevelUp(player, playerLevel/*, multiClassMultiplier*/);
      spLastCalculation = serializableBase.spLastCalculation;
      skillEffect = learnableBase.skillEffect;
      levelTaken = serializableBase.levelTaken;
      featOptions = serializableBase.featOptions;
      restoreOnShortRest = learnableBase.restoreOnShortRest;
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

    public void LevelUp(Player player)
    {
      acquiredPoints = pointsToNextLevel;
      currentLevel += 1;
      active = false;

      skillEffect?.Invoke(player, id);
      pointsToNextLevel = GetPointsToLevelUp(player);

      if (player.TryGetOpenedWindow("activeLearnable", out Player.PlayerWindow activeLearnableWindow))
      {
        Player.ActiveLearnableWindow window = (Player.ActiveLearnableWindow)activeLearnableWindow;
        window.timeLeft.SetBindValue(player.oid, window.nuiToken.Token, "Apprentissage terminé");
        window.level.SetBindValue(player.oid, window.nuiToken.Token, $"{currentLevel}/{maxLevel}");
      }

      if (player.TryGetOpenedWindow("learnables", out Player.PlayerWindow learnableWindow))
      {
        Player.LearnableWindow window = (Player.LearnableWindow)learnableWindow;
        window.LoadLearnableList(window.currentList);
      }
      
      player.oid.ExportCharacter();
      LogUtils.LogMessage($"{player.oid.LoginCreature.Name} maîtrise {name} niveau {currentLevel}", LogUtils.LogType.Learnables);
    }
    private double GetPointsToLevelUp(Player player, int playerLevel = 0/*, int multiClassCount = 0*/)
    {
      switch(category)
      {
        case SkillSystem.Category.Class:
        case SkillSystem.Category.FighterSubClass:
        case SkillSystem.Category.BarbarianSubClass:
        case SkillSystem.Category.RogueSubClass:
        case SkillSystem.Category.MonkSubClass:
        case SkillSystem.Category.WizardSubClass:
        case SkillSystem.Category.BardSubClass:
        case SkillSystem.Category.RangerSubClass:
        case SkillSystem.Category.PaladinSubClass:
        case SkillSystem.Category.ClercSubClass:

          if (playerLevel < 1)
            playerLevel = player.oid.LoginCreature.Level;

          //if (multiClassCount < 1)
            //multiClassCount = player.oid.LoginCreature.Classes.Distinct().Count();

          //double multiClassMultiplier = playerLevel > 12 ? multiClassCount * 1.5 : 1;

          return playerLevel switch
          {
            1 => 8640,// * multiClassMultiplier,
            2 => 8640,// * multiClassMultiplier,
            3 => 26000,// * multiClassMultiplier,
            4 => 80000,// * multiClassMultiplier,
            5 => 190000,// * multiClassMultiplier,
            6 => 405000,// * multiClassMultiplier,
            7 => 660000,// * multiClassMultiplier,
            8 => 980000,// * multiClassMultiplier,
            9 => 1380000,// * multiClassMultiplier,
            10 => 1840000,// * multiClassMultiplier,
            11 => 2450000,// * multiClassMultiplier,
            12 => 2880000,// * multiClassMultiplier,
            13 => 3456000 ,//* multiClassMultiplier,
            14 => 4032000,// * multiClassMultiplier,
            15 => 4752000 ,//* multiClassMultiplier,
            16 => 5616000,// * multiClassMultiplier,
            17 => 6480000,// * multiClassMultiplier,
            18 => 7632000,// * multiClassMultiplier,
            19 => 8784000,// * multiClassMultiplier,
            20 => 10224000,// * multiClassMultiplier,
            21 => 11760000,// * multiClassMultiplier,
            _ => 11760000 * 1.15 * (playerLevel - 20),// * multiClassMultiplier * 1.15 * (playerLevel - 20),
          };
        default:
          return 250 * multiplier * Math.Pow(5, currentLevel);
      }
    }
  }
}
