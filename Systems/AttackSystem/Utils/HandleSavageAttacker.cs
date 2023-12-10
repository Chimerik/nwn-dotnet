using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleSavageAttacker(CNWSCreature creature, NwBaseItem weapon, bool isRangedAttack)
    {
      int numDamageDice = weapon.NumDamageDice;

      if (creature.m_ScriptVars.GetInt(CreatureUtils.FureurOrcBonusDamageVariableExo).ToBool())
      {
        numDamageDice += 1;
        creature.m_ScriptVars.DestroyInt(CreatureUtils.FureurOrcBonusDamageVariableExo);
      }


      int damageRoll = NwRandom.Roll(Utils.random, weapon.DieToRoll, numDamageDice);
      int secondaryDamageRoll = -1000;

      if(!isRangedAttack && creature.m_pStats.HasFeat(CustomSkill.AgresseurSauvage).ToBool())
        secondaryDamageRoll = NwRandom.Roll(Utils.random, weapon.DieToRoll, numDamageDice);

      return damageRoll > secondaryDamageRoll ? damageRoll : secondaryDamageRoll;
    }
  }
}
