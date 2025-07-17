using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetSkillRoll(NwCreature creature, int skill, int advantage, int score, int DC = -1)
    {
      int roll = NativeUtils.HandlePresage(creature);

      if (roll > 0)
        return roll;

      roll = Utils.RollAdvantage(advantage);
      roll = skill > -1 ? RogueUtils.HandleSavoirFaire(creature, skill, roll) : roll;

      if(DC > 0 && roll + score < DC)
      {
        foreach (var eff in creature.ActiveEffects)
          if (eff.Tag == EffectSystem.InspirationBardiqueEffectTag)
          {
            roll += eff.CasterLevel;

            LogUtils.LogMessage($"Activation inspiration bardique : +{eff.CasterLevel}", LogUtils.LogType.Combat);
            StringUtils.DisplayStringToAllPlayersNearTarget(creature, $"Inspiration Bardique (+{StringUtils.ToWhitecolor(eff.CasterLevel)})".ColorString(StringUtils.gold), StringUtils.gold, true, true);
            creature.RemoveEffect(eff);
            break;
          }
      }

      return roll;
    }
  }
}
