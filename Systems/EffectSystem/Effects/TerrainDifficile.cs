using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TerrainDifficileEffectTag = "_TERRAIN_DIFFICILE_EFFECT";
    public static Effect TerrainDifficile(int spellId, NwCreature caster)
    {
      Effect eff = Effect.MovementSpeedDecrease(50);
      eff.Tag = TerrainDifficileEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.IntParams[5] = spellId;
      return eff;
    }
    public static void ApplyTerrainDifficileEffect(NwCreature creature, NwCreature caster, int spellId)
    {
      foreach(var eff in creature.ActiveEffects)
        if (EffectUtils.In(eff.Tag, TerrainDifficileEffectTag, SprintMobileEffectTag, "_LIBERTE_DE_MOUVEMENT_EFFECT") && eff.Spell?.Id == spellId)
          return;

      creature.ApplyEffect(EffectDuration.Permanent, TerrainDifficile(spellId, caster));
    }
  }
}
