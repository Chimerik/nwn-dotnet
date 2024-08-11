using Anvil.API;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void RetablissementSorcier(NwCreature creature)
    {
      byte? level =creature.GetClassInfo(ClassType.Sorcerer)?.Level;
      if (!level.HasValue)
        return;

      if (GetSorcerySource(creature) < 1)
        RestoreSorcerySource(creature, (byte)(level.Value / 5));
    }
  }
}
