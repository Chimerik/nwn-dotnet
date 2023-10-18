using System;
using System.Collections.Generic;
using System.Linq;

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
    


    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility,
      Ability secondaryAbility, Func<PlayerSystem.Player, int, bool> skillEffect = null) 
      : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
      this.category = category;
      this.skillEffect = skillEffect;
    }
    public LearnableSkill(LearnableSkill learnableBase, int skillSource = -1, bool active = false, double acquiredSP = 0, int currentLevel = 0) : base(learnableBase)
    {
      this.category = learnableBase.category;
      this.skillEffect = learnableBase.skillEffect;
      this.active = active;
      this.acquiredPoints = acquiredSP;
      this.currentLevel = currentLevel;
      this.pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
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
      source = learnableBase.source;
    }

    public class SerializableLearnableSkill
    {
      public bool active { get; set; }
      public double acquiredPoints { get; set; }
      public int currentLevel { get; set; }
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
