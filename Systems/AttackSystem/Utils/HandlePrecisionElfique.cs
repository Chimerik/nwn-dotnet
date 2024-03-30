using Anvil.API;
using NWN.Native.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandlePrecisionElfique(CNWSCreature creature, int attackRoll, int advantage, Ability attackStat)
    {
      if (advantage > 0 && creature.m_pStats.HasFeat(CustomSkill.PrecisionElfique).ToBool())
      {
        switch (attackStat)
        {
          case Ability.Dexterity:
          case Ability.Intelligence:
          case Ability.Wisdom:
          case Ability.Charisma:
            
            int reroll = NwRandom.Roll(Utils.random, 20);
            SendNativeServerMessage("Précision elfique".ColorString(StringUtils.gold), creature);
            LogUtils.LogMessage($"Précision elfique : {attackRoll} et {reroll}", LogUtils.LogType.Combat);

            return reroll > attackRoll ? reroll : attackRoll;
        }
      }

      return attackRoll;
    }
  }
}
