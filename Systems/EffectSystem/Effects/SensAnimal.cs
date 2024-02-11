using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect sensAnimalEffect
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.TrueSeeing);
        eff.Tag = SensAnimalEffectTag;
        return eff;
      }
    }
    public const string SensAnimalEffectTag = "_SENS_ANIMAL_EFFECT";
  }
}
