using System;
using System.Collections.Generic;
using System.Linq;
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
    public int minLevel { get; }
    public int levelTaken { get; }
    public bool restoreOnShortRest { get; }
    public Dictionary<int, int[]> featOptions { get; set; }
    public List<int> racePrerequiste { get; }
    public List<int> learnablePrerequiste { get; }


    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility,
      Ability secondaryAbility, Func<Player, int, bool> skillEffect = null, string descriptionLink = "", List<int> racePrerequiste = null, List<int> learnablePrerequiste = null, bool restoreOnShortRest = false, int minLevel = 0) 
      : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility, descriptionLink)
    {
      this.category = category;
      this.skillEffect = skillEffect;
      this.racePrerequiste = racePrerequiste;
      this.learnablePrerequiste = learnablePrerequiste;
      this.restoreOnShortRest = restoreOnShortRest;
      this.minLevel = minLevel;
    }
    public LearnableSkill(LearnableSkill learnableBase, Player player, int skillSource = -1, bool active = false, double acquiredSP = 0, int currentLevel = 0, int levelTaken = 0, Dictionary<int, int[]> featOptions = null) : base(learnableBase)
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
      this.minLevel = learnableBase.minLevel;
      this.levelTaken = levelTaken;
      this.featOptions = featOptions;
      this.source = new();

      if (skillSource > -1)
        source.Add((SkillSystem.Category)skillSource);
    }
    public LearnableSkill(LearnableSkill learnableBase, SerializableLearnableSkill serializableBase, Player player, int playerLevel = 0) 
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

      if(Utils.In(category, SkillSystem.Category.Class, SkillSystem.Category.BarbarianSubClass, SkillSystem.Category.BardSubClass,
        SkillSystem.Category.ClercSubClass, SkillSystem.Category.DruidSubclass, SkillSystem.Category.EnsorceleurSubClass,
        SkillSystem.Category.FighterSubClass, SkillSystem.Category.MonkSubClass, SkillSystem.Category.OccultisteSubClass, 
        SkillSystem.Category.PaladinSubClass, SkillSystem.Category.RangerSubClass, SkillSystem.Category.RogueSubClass, SkillSystem.Category.WizardSubClass))
      {
        foreach(var learnable in player.learnableSkills.Values.Where(l => Utils.In(l.category, SkillSystem.Category.Class, SkillSystem.Category.BarbarianSubClass, SkillSystem.Category.BardSubClass,
        SkillSystem.Category.ClercSubClass, SkillSystem.Category.DruidSubclass, SkillSystem.Category.EnsorceleurSubClass,
        SkillSystem.Category.FighterSubClass, SkillSystem.Category.MonkSubClass, SkillSystem.Category.OccultisteSubClass,
        SkillSystem.Category.PaladinSubClass, SkillSystem.Category.RangerSubClass, SkillSystem.Category.RogueSubClass, SkillSystem.Category.WizardSubClass)))
          learnable.pointsToNextLevel = GetPointsToLevelUp(player);
      }
      else
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
        case SkillSystem.Category.EnsorceleurSubClass:
        case SkillSystem.Category.DruidSubclass:
        case SkillSystem.Category.OccultisteSubClass:

          if (playerLevel < 1)
            playerLevel = player.oid.LoginCreature.Level;

          //if (multiClassCount < 1)
            //multiClassCount = player.oid.LoginCreature.Classes.Distinct().Count();

          //double multiClassMultiplier = playerLevel > 12 ? multiClassCount * 1.5 : 1;

          return playerLevel switch
          {
            1 => 8640,// * multiClassMultiplier,
            2 => 34640,// * multiClassMultiplier,
            3 => 114640,// * multiClassMultiplier,
            4 => 304640,// * multiClassMultiplier,
            5 => 709640,// * multiClassMultiplier,
            6 => 1369640,// * multiClassMultiplier,
            7 => 2349640,// * multiClassMultiplier,
            8 => 3729640,// * multiClassMultiplier,
            9 => 5569640,// * multiClassMultiplier,
            10 => 8019640,// * multiClassMultiplier,
            11 => 10899640,// * multiClassMultiplier,
            12 => 14355640,// * multiClassMultiplier,
            13 => 18387640,//* multiClassMultiplier,
            14 => 23139640,// * multiClassMultiplier,
            15 => 28755640,//* multiClassMultiplier,
            16 => 35235640,// * multiClassMultiplier,
            17 => 42867640,// * multiClassMultiplier,
            18 => 51651640,// * multiClassMultiplier,
            19 => 61875640,// * multiClassMultiplier,
            20 => 73635640,// * multiClassMultiplier,
            _ => 73635640 * 1.15 * (playerLevel - 19),// * multiClassMultiplier * 1.15 * (playerLevel - 20),
          };

        default:
          return 250 * multiplier * Math.Pow(5, currentLevel);
      }
    }
  }
}
