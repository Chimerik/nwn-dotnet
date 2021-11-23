using System;

using Anvil.API;

namespace NWN.Systems
{
  public class LearnableSpell : Learnable
  {
    public int nbScrollUsed { get; set; }
    public int spellLevel { get; }

    public LearnableSpell(int id, string name, string description, string icon, int multiplier, int spellLevel, Ability primaryAbility, Ability secondaryAbility, int maxLevel = 1) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
      this.spellLevel = spellLevel;
      currentLevel = multiplier / 2;
    }
    public LearnableSpell(LearnableSpell learnableBase, bool active = false, int nbScrollUsed = 0, double acquiredSP = 0) : base(learnableBase)
    {
      this.nbScrollUsed = nbScrollUsed;
      this.active = active;
      this.acquiredPoints = acquiredSP;
    }
    public LearnableSpell(LearnableSpell learnableBase, SerializableLearnableSpell serializableBase) : base(learnableBase)
    {
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      spLastCalculation = serializableBase.spLastCalculation;
      nbScrollUsed = serializableBase.nbScrollUsed;
    }

    public class SerializableLearnableSpell
    {
      public bool active { get; }
      public double acquiredPoints { get; }
      public int currentLevel { get; }
      public int nbScrollUsed { get; }
      public DateTime? spLastCalculation { get; }

      public SerializableLearnableSpell()
      {

      }
      public SerializableLearnableSpell(LearnableSpell learnableBase)
      {
        active = learnableBase.active;
        acquiredPoints = learnableBase.acquiredPoints;
        currentLevel = learnableBase.currentLevel;
        spLastCalculation = learnableBase.spLastCalculation;
        nbScrollUsed = learnableBase.nbScrollUsed;
      }
    }
    public void LevelUp(PlayerSystem.Player player)
    {
      acquiredPoints = GetPointsToNextLevel();
      active = false;

      player.oid.LoginCreature.GetClassInfo((ClassType)43).AddKnownSpell((Spell)id, (byte)spellLevel);

      if (player.openedWindows.ContainsKey("activeLearnable"))
        player.oid.NuiDestroy(player.openedWindows["activeLearnable"]);

      if (player.openedWindows.ContainsKey("learnables") && player.windows.ContainsKey("learnables"))
        ((PlayerSystem.Player.LearnableWindow)player.windows["learnables"]).RefreshWindow();

      player.oid.ExportCharacter();
    }
  }
}
