﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentDivinEffectTag = "_CHATIMENT_DIVIN_EFFECT";
    public static Effect GetChatimentDivinEffect(int spellLevel)
    {
      Effect eff =Effect.Icon(CustomEffectIcon.ChatimentDivin);

      eff.Tag = ChatimentDivinEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = spellLevel;
      return eff;
    }
  }
}
