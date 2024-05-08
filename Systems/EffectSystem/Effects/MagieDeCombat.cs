using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MagieDeCombatEffectTag = "_MAGIE_DE_COMBAT_EFFECT";
    public static Effect MagieDeCombat
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ModifyAttacks(1), Effect.Icon(EffectIcon.Haste));
        eff.Tag = MagieDeCombatEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
