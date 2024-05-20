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
        + GetDefenseAdaptativeBonus(creature, attacker)
        + GetAnimalCompanionBonusAC(creature);

      return AC;
    }
  }
}
