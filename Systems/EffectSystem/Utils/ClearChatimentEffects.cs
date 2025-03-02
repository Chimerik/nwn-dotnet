using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static void ClearChatimentEffects(NwCreature caster)
    {
      RemoveTaggedEffect(caster, EffectSystem.ChatimentDivinEffectTag, EffectSystem.ChatimentDuCourrouxEffectTag, EffectSystem.ChatimentTonitruantEffectTag, EffectSystem.ChatimentOcculteEffectTag, EffectSystem.BrandingSmiteAttackEffectTag, EffectSystem.searingSmiteAttackEffectTag);
    }
  }
}
