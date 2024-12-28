using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CoeurDeLaTempeteEffectTag = "_COEUR_DE_LA_TEMPETE_EFFECT";
    public static Effect CoeurDeLaTempete
    {
      get
      {
        Effect elec = Effect.DamageImmunityIncrease(DamageType.Electrical, 50);
        elec.ShowIcon = false;

        Effect thunder = Effect.DamageImmunityIncrease(DamageType.Sonic, 50);
        thunder.ShowIcon = false;

        Effect eff = Effect.LinkEffects(elec, thunder, Effect.Icon(CustomEffectIcon.ElectricalResistance), Effect.Icon(CustomEffectIcon.TonnerreResistance));
        eff.Tag = CoeurDeLaTempeteEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
