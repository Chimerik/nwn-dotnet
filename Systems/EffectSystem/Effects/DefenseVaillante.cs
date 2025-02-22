using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DefenseVaillanteEffectTag = "_DEFENSE_VAILLANTE_EFFECT";
    public static readonly Native.API.CExoString defenseVaillanteEffectExoTag = DefenseVaillanteEffectTag.ToExoString();
    public static Effect GetDefenseVaillanteEffect(int bonus)
    {
      Effect eff = Effect.Icon(CustomEffectIcon.DefenseVaillante);
      eff.Tag = DefenseVaillanteEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = bonus;
      return eff;
    }
  }
}
