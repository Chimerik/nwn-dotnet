using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SprintEffectTag = "_EFFECT_SPRINT";
    public static Effect sprintEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(99), Effect.Icon(NwGameTables.EffectIconTable.GetRow(142)));
        eff.Tag = SprintEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    
  }
}
