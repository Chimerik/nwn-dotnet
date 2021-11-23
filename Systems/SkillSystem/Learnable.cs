using System;
using System.Threading;

using Anvil.API;

namespace NWN.Systems
{
  public abstract class Learnable
  {
    public int id { get; }
    public string name { get; }
    public string description { get; }
    public string icon { get; }
    public int maxLevel { get; }
    public int multiplier { get; }
    public Ability primaryAbility { get; }
    public Ability secondaryAbility { get; }
    public bool active { get; set; }
    public double acquiredPoints { get; set; }  
    public int currentLevel { get; set; }
    public DateTime? spLastCalculation { get; set; }

    public Learnable(int id, string name, string description, string icon, int maxLevel, int multiplier, Ability primaryAbility, Ability secondaryAbility)
    {
      this.id = id;
      this.name = name;
      this.description = description;
      this.icon = icon;
      this.maxLevel = maxLevel;
      this.multiplier = multiplier;
      this.primaryAbility = primaryAbility;
      this.secondaryAbility = secondaryAbility;
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
    }
    public Learnable()
    {

    }

    public double GetPointsToNextLevel()
    {
      return 250 * multiplier * Math.Pow(5, currentLevel);
    }

    public TimeSpan GetTimeSpanToNextLevel(PlayerSystem.Player player)
    {
      if (player.oid.LoginCreature == null)
        return TimeSpan.FromDays(300);

      return TimeSpan.FromSeconds((GetPointsToNextLevel() - acquiredPoints) / player.GetSkillPointsPerSecond(this));
    }

    public string GetReadableTimeSpanToNextLevel(PlayerSystem.Player player)
    {
      TimeSpan timespan = GetTimeSpanToNextLevel(player);
      return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, timespan.Seconds).ToString();
    }

    public async void AwaitPlayerStateChangeToCalculateSPGain(PlayerSystem.Player player)
    {
      TimeSpan timeToNextLevel = GetTimeSpanToNextLevel(player);

      if (timeToNextLevel.TotalSeconds <= 0)
      {
        LevelUpWrapper(player);
        return;
      }

      var scheduler = ModuleSystem.scheduler.Schedule(() => { LevelUpWrapper(player);}, GetTimeSpanToNextLevel(player));

      CancellationTokenSource tokenSource = new CancellationTokenSource();

      int primary = player.oid.LoginCreature.GetAbilityScore(primaryAbility);
      int secondary = player.oid.LoginCreature.GetAbilityScore(secondaryAbility);
      PlayerSystem.Player.PcState pcState = player.pcState;
      
      double spPerSecond = player.GetSkillPointsPerSecond(this);

      await NwTask.WaitUntil(() => player.oid.LoginCreature == null || pcState != player.pcState || primary != player.oid.LoginCreature.GetAbilityScore(primaryAbility) || secondary != player.oid.LoginCreature.GetAbilityScore(secondaryAbility) || !active, tokenSource.Token);
      tokenSource.Cancel();
      scheduler.Dispose();

      if (spLastCalculation.HasValue)
        acquiredPoints += (DateTime.Now - spLastCalculation).Value.TotalSeconds * spPerSecond;

      spLastCalculation = DateTime.Now;

      if (pcState == PlayerSystem.Player.PcState.Offline || player.oid.LoginCreature == null)
        return;

      if (!active)
      {
        spLastCalculation = null;
        return;
      }

      AwaitPlayerStateChangeToCalculateSPGain(player);
    }

    private async void LevelUpWrapper(PlayerSystem.Player player)
    {
      if (this is LearnableSkill)
      {
        ((LearnableSkill)this).LevelUp(player);
      }
      else if (this is LearnableSpell)
      {
        ((LearnableSpell)this).LevelUp(player);
      }

      spLastCalculation = null;

      player.oid.SendServerMessage($"Vous venez de terminer l'apprentissage de {name}, niveau {currentLevel} !");
      await NwTask.WaitUntil(() => player.oid.LoginCreature.Area != null);
      await NwTask.Delay(TimeSpan.FromSeconds(2));
      player.oid.ApplyInstantVisualEffectToObject((VfxType)1516, player.oid.ControlledCreature);
      player.oid.PlaySound("gui_level_up");
    }
  }
}
