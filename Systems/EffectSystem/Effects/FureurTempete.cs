using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurTempeteEffectTag = "_FUREUR_TEMPETE_EFFECT";
    public static Effect FureurTempete(int ensoLevel)
    {
        Effect eff = Effect.DamageShield(ensoLevel, 0, DamageType.Electrical);
        eff.Tag = FureurTempeteEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
    }
  }
}
