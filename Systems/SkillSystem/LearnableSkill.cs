using System;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public class LearnableSkill : Learnable
  {
    private bool activable { get; }
    private Func<PlayerSystem.Player, bool> skillEffect { get; }


    public LearnableSkill(int id, string name, string description, string icon, int maxLevel, int multiplier, Ability primaryAbility, Ability secondaryAbility, bool activable = false, Func<PlayerSystem.Player, bool> skillEffect = null) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
      this.activable = activable;
      this.skillEffect = skillEffect;
    }
    public LearnableSkill(LearnableSkill learnableBase, bool active = false, double acquiredSP = 0, int currentLevel = 0) : base(learnableBase)
    {
      this.activable = learnableBase.activable;
      this.active = active;
      this.acquiredPoints = acquiredSP;
      this.currentLevel = currentLevel;
    }
    public LearnableSkill(LearnableSkill learnableBase, SerializableLearnableSkill serializableBase) : base(learnableBase)
    {
      activable = learnableBase.activable;
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      spLastCalculation = serializableBase.spLastCalculation;
      skillEffect = learnableBase.skillEffect;
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
