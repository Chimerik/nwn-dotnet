using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetSkillRoll(NwCreature creature, int skill, int advantage)
    {
      int roll = NativeUtils.HandlePresage(creature);

      if (roll > 0)
        return roll;

      roll = Utils.RollAdvantage(advantage);
      roll = RogueUtils.HandleSavoirFaire(creature, skill, roll);

      return roll;
    }
  }
}
