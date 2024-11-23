using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetCreatureAC(CNWSCreature creature, CNWSCreature attacker)
    {
      int AC = creature.m_pStats.GetArmorClassVersus(attacker)
        + GetFightingStyleDefenseBonus(creature)
        + GetDefenseVaillanteBonus(creature)
        + GetAnimalCompanionBonusAC(creature);

      AC = GetPeauDecorceAC(creature, AC);
      return AC;
    }
  }
}
