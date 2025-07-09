using System;
using System.Collections.Generic;
using System.Linq;

using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public class LearnableSpell : Learnable
  {
    public bool canLearn { get; set; }
    //public List<ClassType> availableToClasses { get; set; }
    public List<int> learntFromClasses { get; set; }
    public bool mastery { get; set; }
    public bool alwaysPrepared { get; set; }
    // Dans le cas des Spell, multiplier = spell Level - 1

    public LearnableSpell(int id, string name, string description, string icon, int multiplier, Ability primaryAbility, Ability secondaryAbility/*, List<ClassType> classes*/, int maxLevel = 1) : base(id, name, description, icon, maxLevel, multiplier, primaryAbility, secondaryAbility)
    {
      //this.availableToClasses = classes;
    }
    public LearnableSpell(LearnableSpell learnableBase, int fromClass) : base(learnableBase)
    {
      canLearn = true;
      active = false;
      acquiredPoints = 0;
      currentLevel = 0;
      pointsToNextLevel = 5000 * multiplier;
      learntFromClasses = new() { fromClass };
      mastery = false;
      alwaysPrepared = false;
    }
    public LearnableSpell(LearnableSpell learnableBase, List<int> fromClass) : base(learnableBase)
    {
      canLearn = true;
      active = false;
      acquiredPoints = 0;
      currentLevel = 0;
      pointsToNextLevel = 5000 * multiplier;
      learntFromClasses = fromClass;
      mastery = false;
      alwaysPrepared = false;
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
      learntFromClasses = serializableBase.learntFromClasses;
      mastery = serializableBase.mastery;
      alwaysPrepared = serializableBase.alwaysPrepared;
    }

    public class SerializableLearnableSpell
    {
      public bool active { get; set; }
      public double acquiredPoints { get; set; }
      public int currentLevel { get; set; }
      public bool canLearn { get; set; }
      public bool mastery { get; set; }
      public bool alwaysPrepared { get; set; }
      public List<int> learntFromClasses { get; set; }
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
        mastery = learnableBase.mastery;
        learntFromClasses = learnableBase.learntFromClasses;
        alwaysPrepared = learnableBase.alwaysPrepared;
      }
    }
    public void LevelUp(Player player)
    {
      acquiredPoints = pointsToNextLevel;
      currentLevel += 1;
      pointsToNextLevel = 5000 * (currentLevel) * multiplier + currentLevel * 250 + multiplier * 250;

      active = false;
      canLearn = false;

      foreach (int casterClass in learntFromClasses)
      {
        CreatureClassInfo classInfo = player.oid.LoginCreature.GetClassInfo((ClassType)casterClass);

        if (classInfo is null)
          continue;

        if (NwSpell.FromSpellId(id).GetSpellLevelForClass((ClassType)casterClass) > 0)
        {
          switch (casterClass)
          {
            case CustomClass.Clerc:
            case CustomClass.Druid:
            case CustomClass.Paladin: continue;
          }
        }

        var knownSpells = classInfo.KnownSpells[multiplier - 1];

        if (!knownSpells.Any(s => s.Id == id))
          knownSpells.Add(NwSpell.FromSpellId(id));
      }

      if (player.TryGetOpenedWindow("learnables", out Player.PlayerWindow learnableWindow))
      {
        Player.LearnableWindow window = (Player.LearnableWindow)learnableWindow;
        window.LoadLearnableList(window.currentList);
      }

      LogUtils.LogMessage($"{player.oid.LoginCreature.Name} apprend {name} (sort de niveau {NwSpell.FromSpellId(id).InnateSpellLevel})", LogUtils.LogType.Learnables);

      player.oid.ExportCharacter();
    }
  }
}
