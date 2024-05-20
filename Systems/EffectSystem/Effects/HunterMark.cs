using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HunterMarkEffectTag = "_HUNTER_MARK_EFFECT";
    public static readonly Native.API.CExoString hunterMarkEffectExoTag = HunterMarkEffectTag.ToExoString();
    public static Effect HunteMark
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.EnemyAttackBonus);
        eff.Tag = HunterMarkEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
