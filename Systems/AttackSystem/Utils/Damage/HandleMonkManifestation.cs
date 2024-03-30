using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static void HandleMonkManifestation(CNWSCreature creature, CNWSItem weapon)
    {
      if(weapon is not null && GetClassLevel(creature, ClassType.Monk) > 5)
      {
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ManifestationAmeEffectExoTag);
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ManifestationCorpsEffectExoTag);
        EffectUtils.RemoveTaggedEffect(creature, EffectSystem.ManifestationEspritEffectExoTag);
      }
    }
  }
}
