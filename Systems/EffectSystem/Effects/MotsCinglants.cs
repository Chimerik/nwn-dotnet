using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MotsCinglantsEffectTag = "_MOTS_CINGLANTS_EFFECT";
    public static readonly Native.API.CExoString motsCinglantsEffectExoTag = MotsCinglantsEffectTag.ToExoString();
    public static Effect GetMotsCinglantsEffect(int bardLevel)
    {
      Effect eff = Effect.Icon((EffectIcon)170);
      eff.Tag = MotsCinglantsEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = NwRandom.Roll(Utils.random, bardLevel > 14 ? 12 : bardLevel > 9 ? 10 : bardLevel > 4 ? 8 : 6);
      return eff;
    }
  }
}
