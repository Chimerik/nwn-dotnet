using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ResistanceFeuEffectTag = "_RESISTANCE_FEU_EFFECT";
    public static Effect ResistanceFeu
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.FireResistance);
        eff.Tag = ResistanceFeuEffectTag;
        return eff;
      }
    }
  }
}
