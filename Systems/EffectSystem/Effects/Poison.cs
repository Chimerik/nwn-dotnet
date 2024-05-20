using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonEffectTag = "_POISON_EFFECT";
    public static readonly Native.API.CExoString poisonEffectExoTag = PoisonEffectTag.ToExoString();
    public static Effect Poison
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.Poison);
        eff.Tag = PoisonEffectTag;
        return eff;
      }
    }
  }
}
