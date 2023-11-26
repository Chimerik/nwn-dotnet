using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CogneurLourdEffectTag = "_COGNEUR_LOURD_EFFECT";
    public static readonly Native.API.CExoString CogneurLourdExoTag = "_COGNEUR_LOURD_EFFECT".ToExoString();
    public const string CogneurLourdBonusAttackEffectTag = "_COGNEUR_LOURD_BONUS_ATTACK";
    public static readonly Native.API.CExoString CogneurLourdBonusAttackExoTag = "_COGNEUR_LOURD_BONUS_ATTACK".ToExoString();
    public static Effect cogneurLourdEffect
    {
      get
      {
        Effect eff = Effect.Icon(NwGameTables.EffectIconTable.GetRow(156));
        eff.Tag = CogneurLourdEffectTag;
        return eff;
      }
    }
    public static Effect cogneurLourdBonusAttackEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ModifyAttacks(1), Effect.Icon((EffectIcon)154));
        eff.Tag = CogneurLourdBonusAttackEffectTag;
        return eff;
      }
    }
  }
}
