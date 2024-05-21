using Anvil.API;

namespace NWN.Systems
{
  public static partial class RogueUtils
  {
    public static int HandleSavoirFaire(NwCreature creature, int skill, int roll)
    {
      int rogueLevel = GetRogueLevel(creature);

      if (roll > 9 || rogueLevel < 11)
        return roll;

      if(PlayerSystem.Players.TryGetValue(creature, out PlayerSystem.Player player))
      {
        if(player.learnableSkills.TryGetValue(skill, out LearnableSkill learnable) && learnable.currentLevel > 0)
          return 10;
      }
      else
        return 10;

      return roll;
    }
  }
}
