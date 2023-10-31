using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DodgeEffectTag = "_EFFECT_DODGE";
    public static readonly Native.API.CExoString DodgeEffectExoTag = "_EFFECT_DODGE".ToExoString();
    public static Effect dodgeEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(140));
        eff.Tag = DodgeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    
  }
}
