using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurOuraganFoudreEffectTag = "_FUREUR_OURAGAN_FOUDRE_EFFECT";
    public static readonly Native.API.CExoString FureurOuraganFoudreExoTag = FureurOuraganFoudreEffectTag.ToExoString();
    public static Effect FureurOuraganFoudre
    {
      get
      {
        Effect eff = Effect.DamageShield(0, DamageBonus.Plus2d8, DamageType.Electrical);
        eff.Tag = FureurOuraganFoudreEffectTag;
        return eff;
      }
    }
  }
}
