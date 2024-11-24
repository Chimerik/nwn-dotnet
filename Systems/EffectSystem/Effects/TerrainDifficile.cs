using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TerrainDifficileEffectTag = "_TERRAIN_DIFFICILE_EFFECT";
    public static Effect TerrainDifficile
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(50);
        eff.Tag = TerrainDifficileEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static void ApplyTerrainDifficileEffect(NwCreature creature)
    {
      if (creature.ActiveEffects.Any(e => EffectUtils.In(e.Tag, TerrainDifficileEffectTag, SprintMobileEffectTag, "_LIBERTE_DE_MOUVEMENT_EFFECT")))
        return;

      creature.ApplyEffect(EffectDuration.Permanent, TerrainDifficile);
    }
  }
}
