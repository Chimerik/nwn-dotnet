using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAttackRoll(CNWSCreature creature, int advantage, Ability attackStat)
    {
      int attackRoll = HandlePresage(creature);

      if (attackRoll > 0)
        return attackRoll;

      attackRoll = Utils.RollAdvantage(advantage);
      attackRoll = HandlePrecisionElfique(creature, attackRoll, advantage, attackStat);
      attackRoll = HandleChanceDebordante(creature, attackRoll);
      return HandleHalflingLuck(creature, attackRoll);
    }
  }
}
