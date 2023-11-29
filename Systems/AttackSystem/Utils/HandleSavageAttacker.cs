using Anvil.API;
using NWN.Native.API;
using CreatureSize = Anvil.API.CreatureSize;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleSavageAttacker(CNWSCreature creature, NwBaseItem weapon, bool isRangedAttack)
    {
      int damageRoll = NwRandom.Roll(Utils.random, weapon.DieToRoll, weapon.NumDamageDice);
      int secondaryDamageRoll = -1000;

      if(!isRangedAttack && creature.m_pStats.HasFeat(CustomSkill.AgresseurSauvage).ToBool())
        secondaryDamageRoll = NwRandom.Roll(Utils.random, weapon.DieToRoll, weapon.NumDamageDice);

      return damageRoll > secondaryDamageRoll ? damageRoll : secondaryDamageRoll;
    }
  }
}
