using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool HasTwoWeaponStyle(CNWSCreature creature)
    {
      if (PlayerSystem.Players.TryGetValue(creature.m_idSelf, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.TwoWeaponFighting, out LearnableSkill twoWeapon)
        && twoWeapon.currentLevel > 0)
        return true;
      
      return false;
    }
  }
}
