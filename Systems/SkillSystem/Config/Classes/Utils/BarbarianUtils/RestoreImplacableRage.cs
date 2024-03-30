using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void RestoreImplacableRage(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Barbarian)?.Level;

      if (level.HasValue && level > 10)
        creature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value = 10;
    }
  }
}
