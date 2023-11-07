using Anvil.API;
using NWN.Native.API;
using CreatureSize = Anvil.API.CreatureSize;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleGreatWeaponStyle(CNWSCreature creature, NwBaseItem baseWeapon, int damageRoll)
    {
      if (damageRoll < 3 && ItemUtils.IsTwoHandedWeapon(baseWeapon, (CreatureSize)creature.m_nCreatureSize)
        && PlayerSystem.Players.TryGetValue(creature.m_idSelf, out PlayerSystem.Player player)
        && player.learnableSkills.TryGetValue(CustomSkill.FighterCombatStyleTwoHanded, out LearnableSkill defense)
        && defense.currentLevel > 0)
      {
        SendNativeServerMessage("Style de combat à deux mains : jet relancé !".ColorString(StringUtils.gold), creature);
        return NwRandom.Roll(Utils.random, baseWeapon.DieToRoll, baseWeapon.NumDamageDice);
      }
      else
        return damageRoll;
    }
  }
}
