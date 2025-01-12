using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EchapperALaHordeEffectTag = "_ECHAPPER_A_LA_HORDE_EFFECT";
    public static Effect EchapperALaHorde
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = EchapperALaHordeEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}

