using NWN.Native.API;
using ClassType = NWN.Native.API.ClassType;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetClassLevel(CNWSCreature creature, ClassType classType)
    {
      for (byte i = 0; i < creature.m_pStats.m_nNumMultiClasses; i++)
      {
        CNWSCreatureStats_ClassInfo classInfo = creature.m_pStats.GetClassInfo(i);
        if (classInfo.m_nClass == (int)ClassType.Invalid)
          continue;

        if (classInfo.m_nClass == (byte)classType)
          return classInfo.m_nLevel;
      }

      return 0;
    }
  }
}
