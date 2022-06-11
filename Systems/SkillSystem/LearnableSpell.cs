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
      this.spellLevel = learnableBase.spellLevel;
    }
    public LearnableSpell(LearnableSpell learnableBase, SerializableLearnableSpell serializableBase) : base(learnableBase)
    {
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      spLastCalculation = serializableBase.spLastCalculation;
      nbScrollUsed = serializableBase.nbScrollUsed;
      spellLevel = learnableBase.spellLevel;
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
      player.learnableSpells.Remove(id);

      if (player.TryGetOpenedWindow("activeLearnable", out PlayerSystem.Player.PlayerWindow activeLearnableWindow))
        activeLearnableWindow.CloseWindow();

      if (player.TryGetOpenedWindow("learnables", out PlayerSystem.Player.PlayerWindow learnableWindow))
      {
        PlayerSystem.Player.LearnableWindow window = (PlayerSystem.Player.LearnableWindow)learnableWindow;
        window.LoadLearnableList(window.currentList);
      }

      player.oid.ExportCharacter();
    }
  }
}
