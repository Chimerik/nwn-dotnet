using Anvil.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentDivinEffectTag = "_CHATIMENT_DIVIN_EFFECT";
    public static Effect GetChatimentDivinEffect(int spellLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)181), Effect.RunAction());

      eff.Tag = ChatimentDivinEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = spellLevel;
      return eff;
    }
  }
}
