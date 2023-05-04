using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public class LearnableSkill : Learnable
  {
    private bool activable { get; }
    public SkillSystem.Category category { get; }
    public SkillSystem.Type type { get; }
    private Func<PlayerSystem.Player, int, bool> skillEffect { get; }
    public Dictionary<Ability, int> abilityPrerequisites { get; }
    public Dictionary<int, int> skillPrerequisites { get; }
    public int attackBonusPrerequisite { get; }
    public int bonusPoints { get; set; }
    public int totalPoints { get { return currentLevel + bonusPoints; } }
    public double bonusMultiplier { get { return 1 + (totalPoints / 100); } }
    public double bonusReduction { get { return 1 - (totalPoints / 100); } }


    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility,
      Ability secondaryAbility, bool activable = false, Func<PlayerSystem.Player, int, bool> skillEffect = null, Dictionary<Ability, int> abilityPrerequisites = null,
      Dictionary<int, int> skillPrerequisites = null, int attackBonusPrerequisite = 0, int bonusPoints = 0) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
      this.activable = activable;
      this.category = category;
      this.skillEffect = skillEffect;
      this.attackBonusPrerequisite = attackBonusPrerequisite;
      this.bonusPoints = bonusPoints;

      if (abilityPrerequisites == null)
        this.abilityPrerequisites = new Dictionary<Ability, int>();
      else
        this.abilityPrerequisites = abilityPrerequisites;

      if (skillPrerequisites == null)
        this.skillPrerequisites = new Dictionary<int, int>();
      else
        this.skillPrerequisites = skillPrerequisites;
    }
    public LearnableSkill(LearnableSkill learnableBase, bool active = false, double acquiredSP = 0, int currentLevel = 0, int bonusPoints = 0) : base(learnableBase)
    {
      this.activable = learnableBase.activable;
      this.category = learnableBase.category;
      this.skillEffect = learnableBase.skillEffect;
      this.active = active;
      this.acquiredPoints = acquiredSP;
      this.currentLevel = currentLevel;
      this.pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
      this.abilityPrerequisites = learnableBase.abilityPrerequisites;
      this.skillPrerequisites = learnableBase.skillPrerequisites;
      this.attackBonusPrerequisite = learnableBase.attackBonusPrerequisite;
      this.bonusPoints = bonusPoints;
    }
    public LearnableSkill(LearnableSkill learnableBase, SerializableLearnableSkill serializableBase) : base(learnableBase)
    {
      activable = learnableBase.activable;
      category = learnableBase.category;
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
      spLastCalculation = serializableBase.spLastCalculation;
      skillEffect = learnableBase.skillEffect;
      abilityPrerequisites = learnableBase.abilityPrerequisites;
      skillPrerequisites = learnableBase.skillPrerequisites;
      attackBonusPrerequisite = learnableBase.attackBonusPrerequisite;
      bonusPoints = serializableBase.bonusPoints;
    }

    public class SerializableLearnableSkill
    {
      public bool active { get; set; }
      public double acquiredPoints { get; set; }
      public int currentLevel { get; set; }
      public int bonusPoints { get; set; }
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
        bonusPoints = learnableBase.bonusPoints;
      }
    }

    public void LevelUp(PlayerSystem.Player player)
    {
      acquiredPoints = pointsToNextLevel;
      currentLevel += 1;
      pointsToNextLevel = 250 * multiplier * Math.Pow(5, currentLevel);
      active = false;

      if (activable && !player.oid.LoginCreature.Feats.Any(f => f.Id == id - 10000))
        player.oid.LoginCreature.AddFeat((Feat)(id - 10000));

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
