﻿using System;
using System.Linq;

using Anvil.API;

using NLog;

namespace NWN.Systems
{
  public class LearnableSpell : Learnable
  {
    public bool canLearn { get; set; }
    // Dans le cas des Spell, multiplier = spell Level - 1

    public LearnableSpell(int id, string name, string description, string icon, int multiplier, Ability primaryAbility, Ability secondaryAbility, int maxLevel = 15) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
     

    }
    public LearnableSpell(LearnableSpell learnableBase) : base(learnableBase)
    {
      canLearn = true;
      active = false;
      acquiredPoints = 0;
      currentLevel = 0;
      pointsToNextLevel = 5000 * multiplier / 2;
    }
    public LearnableSpell(LearnableSpell learnableBase, SerializableLearnableSpell serializableBase) : base(learnableBase)
    {
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      pointsToNextLevel = serializableBase.currentLevel > 0 ? 5000 * serializableBase.currentLevel * multiplier : 5000 * multiplier / 2;
      spLastCalculation = serializableBase.spLastCalculation;
      canLearn = serializableBase.canLearn;
    }

    public class SerializableLearnableSpell
    {
      public bool active { get; set; }
      public double acquiredPoints { get; set; }
      public int currentLevel { get; set; }
      public bool canLearn { get; set; }
      public DateTime? spLastCalculation { get; set; }

      public SerializableLearnableSpell()
      {

      }
      public SerializableLearnableSpell(LearnableSpell learnableBase)
      {
        active = learnableBase.active;
        acquiredPoints = learnableBase.acquiredPoints;
        currentLevel = learnableBase.currentLevel;
        spLastCalculation = learnableBase.spLastCalculation;
        canLearn = learnableBase.canLearn;
      }
    }
    public void LevelUp(PlayerSystem.Player player)
    {
      acquiredPoints = pointsToNextLevel;
      currentLevel += 1;
      pointsToNextLevel = 5000 * (currentLevel) * multiplier;

      active = false;
      canLearn = false;

      if (!player.oid.LoginCreature.GetClassInfo((ClassType)43).GetKnownSpells((byte)(multiplier - 1)).Any(s => s.Id == id))
        player.oid.LoginCreature.GetClassInfo((ClassType)43).AddKnownSpell((Spell)id, (byte)(multiplier - 1));

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
