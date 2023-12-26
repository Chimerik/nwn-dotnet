using System;
using System.Linq;

using Anvil.API;

namespace NWN.Systems
{
  public abstract partial class Learnable
  {
    public int id { get; }
    public string name { get; }
    public string description { get; set; }
    public string descriptionLink { get; }
    public string icon { get; }
    public int maxLevel { get; }
    public int multiplier { get; }
    public Ability primaryAbility { get; }
    public Ability secondaryAbility { get; }
    public bool active { get; set; }
    public double acquiredPoints { get; set; }
    public int currentLevel { get; set; }
    public DateTime? spLastCalculation { get; set; }
    public double pointsToNextLevel { get; set; }

    public Learnable(int id, string name, string description, string icon, int maxLevel, int multiplier, Ability primaryAbility, Ability secondaryAbility, string descriptionLink = "")
    {
      this.id = id;
      this.name = name;
      this.description = description;
      this.icon = icon;
      this.maxLevel = maxLevel;
      this.multiplier = multiplier;
      this.primaryAbility = primaryAbility;
      this.secondaryAbility = secondaryAbility;
      this.descriptionLink = descriptionLink;

    }
    public Learnable(Learnable learnableBase)
    {
      this.id = learnableBase.id;
      this.name = learnableBase.name;
      this.description = learnableBase.description;
      this.icon = learnableBase.icon;
      this.maxLevel = learnableBase.maxLevel;
      this.multiplier = learnableBase.multiplier;
      this.primaryAbility = learnableBase.primaryAbility;
      this.secondaryAbility = learnableBase.secondaryAbility;
      this.descriptionLink = learnableBase.descriptionLink;
    }
    public Learnable()
    {

    }
    public TimeSpan GetTimeSpanToNextLevel(PlayerSystem.Player player)
    {
      if (player.oid.LoginCreature == null)
        return TimeSpan.FromDays(300);

      return TimeSpan.FromSeconds((pointsToNextLevel - acquiredPoints) / player.GetSkillPointsPerSecond(this));
    }
    public string GetReadableTimeSpanToNextLevel(PlayerSystem.Player player)
    {
      TimeSpan timespan = GetTimeSpanToNextLevel(player);
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
    }
    public async void LevelUpWrapper(PlayerSystem.Player player)
    {
      if (currentLevel >= maxLevel)
      {
        player.oid.SendServerMessage($"Vous maîtrisez déjà {name}, impossible d'en apprendre davantage.");
        return;
      }

      if (this is LearnableSkill skill)
      {
        skill.LevelUp(player);
        player.oid.SendServerMessage($"Vous venez de terminer l'apprentissage de {name}, niveau {currentLevel} !");
      }
      else if (this is LearnableSpell spell)
      {
        spell.LevelUp(player);
        player.oid.SendServerMessage($"Vous venez de terminer l'apprentissage de {name} !");
      }

      spLastCalculation = null;

      await NwTask.WaitUntil(() => player.oid.LoginCreature.Area != null);
      await NwTask.Delay(TimeSpan.FromSeconds(2));
      player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
      player.oid.PlaySound("gui_level_up");
    }
    public void StartLearning(PlayerSystem.Player player) // on met en pause le learnable précédent et on active le nouveau
    {
      if (player.learnableSpells.Any(l => l.Value.active))
      {
        LearnableSpell spell = player.learnableSpells.FirstOrDefault(l => l.Value.active).Value;
        spell.active = false;
        spell.spLastCalculation = null;
      }
      else if (player.learnableSkills.Any(l => l.Value.active))
      {
        LearnableSkill skill = player.learnableSkills.FirstOrDefault(l => l.Value.active).Value;
        skill.active = false;
        skill.spLastCalculation = null;
      }

      active = true;
      player.activeLearnable = this;
      spLastCalculation = DateTime.Now;
    }
  }
}
