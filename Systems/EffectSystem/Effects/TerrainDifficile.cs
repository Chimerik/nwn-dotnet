using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TerrainDifficileEffectTag = "_TERRAIN_DIFFICILE_EFFECT";
    public static Effect TerrainDifficile(NwSpell spell, NwCreature caster)
    {
      Effect eff = Effect.MovementSpeedDecrease(50);
      eff.Tag = TerrainDifficileEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;
      return eff;
    }
    public static void ApplyTerrainDifficileEffect(NwCreature creature, NwCreature caster, NwSpell spell)
    {
      foreach(var eff in creature.ActiveEffects)
        if (EffectUtils.In(eff.Tag, TerrainDifficileEffectTag, SprintMobileEffectTag, "_LIBERTE_DE_MOUVEMENT_EFFECT") && eff.Spell?.Id == spell.Id)
          return;

      creature.ApplyEffect(EffectDuration.Permanent, TerrainDifficile(spell, caster));
    }
  }
}
