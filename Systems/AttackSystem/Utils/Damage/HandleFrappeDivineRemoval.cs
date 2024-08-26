using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static async void HandleFrappeDivineRemoval(CNWSCreature attacker)
    {
      await NwTask.NextFrame();

      EffectUtils.RemoveTaggedEffect(attacker, EffectSystem.FrappeDivineDuperieEffectExoTag, EffectSystem.FrappeDivineGuerreEffectExoTag,
        EffectSystem.FurieElementaireEffectExoTag, EffectSystem.FrappeDivineTempeteEffectExoTag, EffectSystem.FrappeDivineVieEffectExoTag,
        EffectSystem.FrappePrimordialeEffectExoTag);

    }
  }
}
