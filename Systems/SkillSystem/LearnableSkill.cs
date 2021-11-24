﻿using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public class LearnableSkill : Learnable
  {
    private bool activable { get; }
    public SkillSystem.Category category { get; }
    private Func<PlayerSystem.Player, bool> skillEffect { get; }
    public Dictionary<Ability, int> abilityPrerequisites { get; }
    public Dictionary<int, int> skillPrerequisites { get; }
    public int attackBonusPrerequisite { get; }

    public LearnableSkill(int id, string name, string description, SkillSystem.Category category, string icon, int maxLevel, int multiplier, Ability primaryAbility, 
      Ability secondaryAbility, bool activable = false, Func<PlayerSystem.Player, bool> skillEffect = null, Dictionary<Ability, int> abilityPrerequisites = null,
      Dictionary<int, int> skillPrerequisites = null, int attackBonusPrerequisite = 0) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
      this.activable = activable;
      this.category = category;
      this.skillEffect = skillEffect;
      this.attackBonusPrerequisite = attackBonusPrerequisite;

      if (abilityPrerequisites == null)
        this.abilityPrerequisites = new Dictionary<Ability, int>();
      else
        this.abilityPrerequisites = abilityPrerequisites;

      if (skillPrerequisites == null)
        this.skillPrerequisites = new Dictionary<int, int>();
      else
        this.skillPrerequisites = skillPrerequisites;
    }
    public LearnableSkill(LearnableSkill learnableBase, bool active = false, double acquiredSP = 0, int currentLevel = 0) : base(learnableBase)
    {
      this.activable = learnableBase.activable;
      this.category = learnableBase.category;
      this.active = active;
      this.acquiredPoints = acquiredSP;
      this.currentLevel = currentLevel;
      this.abilityPrerequisites = learnableBase.abilityPrerequisites;
      this.skillPrerequisites = learnableBase.skillPrerequisites;
      this.attackBonusPrerequisite = learnableBase.attackBonusPrerequisite;
    }
    public LearnableSkill(LearnableSkill learnableBase, SerializableLearnableSkill serializableBase) : base(learnableBase)
    {
      activable = learnableBase.activable;
      category = learnableBase.category;
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      spLastCalculation = serializableBase.spLastCalculation;
      skillEffect = learnableBase.skillEffect;
      abilityPrerequisites = learnableBase.abilityPrerequisites;
      skillPrerequisites = learnableBase.skillPrerequisites;
      attackBonusPrerequisite = learnableBase.attackBonusPrerequisite;
    }

    public class SerializableLearnableSkill
    {
      public bool active { get; set; }
      public double acquiredPoints { get; set; }
      public int currentLevel { get; set; }
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
      }
    }

    public void LevelUp(PlayerSystem.Player player)
    {
      acquiredPoints = GetPointsToNextLevel();
      currentLevel += 1;
      active = false;

      if (activable)
        player.oid.LoginCreature.AddFeat((Feat)player.learnableSkills.FirstOrDefault(l => l.Value == this).Key - 10000);

      if (skillEffect != null)
        skillEffect.Invoke(player);

      if (player.openedWindows.ContainsKey("activeLearnable"))
        player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);

      if (player.openedWindows.ContainsKey("learnables") && player.windows.ContainsKey("learnables"))
        ((PlayerSystem.Player.LearnableWindow)player.windows["learnables"]).RefreshWindow();

      player.oid.ExportCharacter();
    }
    private bool HandleHealthPoints(PlayerSystem.Player player)
    {
      int improvedHealth = 0;
      if (player.learnableSkills.ContainsKey(CustomSkill.ImprovedHealth))
        improvedHealth = player.learnableSkills[CustomSkill.ImprovedHealth].currentLevel;

      int toughness = 0;
      if (player.learnableSkills.ContainsKey(CustomSkill.Toughness))
        toughness = player.learnableSkills[CustomSkill.Toughness].currentLevel;

      player.oid.LoginCreature.LevelInfo[0].HitDie = (byte)(10
        + (1 + 3 * ((player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true) - 10) / 2)
        + toughness) * improvedHealth);

      return true;
    }
  }
}
