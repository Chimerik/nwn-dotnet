using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurOuraganTonnerreEffectTag = "_FUREUR_OURAGAN_TONNERRE_EFFECT";
    public static readonly Native.API.CExoString FureurOuraganTonnerreExoTag = FureurOuraganTonnerreEffectTag.ToExoString();
    public static Effect FureurOuraganTonnerre
    {
      get
      {
        Effect eff = Effect.DamageShield(0, DamageBonus.Plus2d8, DamageType.Sonic);
        eff.Tag = FureurOuraganTonnerreEffectTag;
        return eff;
      }
    }
  }
}
