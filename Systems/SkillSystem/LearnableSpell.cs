using System;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public class LearnableSpell : Learnable
  {
    public bool canLearn { get; set; }
    // Dans le cas des Spell, multiplier = spell Level - 1

    public LearnableSpell(int id, string name, string description, string icon, int multiplier, Ability primaryAbility, Ability secondaryAbility, int maxLevel = 15, SkillSystem.Attribut attribut = SkillSystem.Attribut.Invalid, SkillSystem.Type type = SkillSystem.Type.Invalid) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility, attribut, type)
    {
     

    }
    public LearnableSpell(LearnableSpell learnableBase) : base(learnableBase)
    {
      canLearn = true;
      active = false;
      acquiredPoints = 0;
      currentLevel = 0;
      pointsToNextLevel = 5000 * multiplier;
    }
    public LearnableSpell(LearnableSpell learnableBase, SerializableLearnableSpell serializableBase) : base(learnableBase)
    {
      active = serializableBase.active;
      acquiredPoints = serializableBase.acquiredPoints;
      currentLevel = serializableBase.currentLevel;
      pointsToNextLevel = serializableBase.currentLevel > 0 ? 5000 * serializableBase.currentLevel * multiplier : 5000 * multiplier;
      pointsToNextLevel += currentLevel * 250 + multiplier * 250;
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
      pointsToNextLevel = 5000 * (currentLevel) * multiplier + currentLevel * 250 + multiplier * 250;

      active = false;
      canLearn = false;
      
      if (!player.oid.LoginCreature.GetClassInfo((ClassType)43).KnownSpells[multiplier - 1].Any(s => s.Id == id))
        player.oid.LoginCreature.GetClassInfo((ClassType)43).KnownSpells[multiplier - 1].Add(NwSpell.FromSpellId(id));

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
    }
  }
}
