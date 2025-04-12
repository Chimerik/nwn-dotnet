using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TerrainDifficileEffectTag = "_TERRAIN_DIFFICILE_EFFECT";
    public static Effect TerrainDifficile(NwSpell spell, NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.TerrainDifficile), Effect.MovementSpeedDecrease(50));

      if (spell.Id == CustomSpell.CroissanceVegetale)
        eff = Effect.LinkEffects(eff, Effect.VisualEffect(VfxType.DurEntangle));

      eff.Tag = TerrainDifficileEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;
      return eff;
    }
    public static void ApplyTerrainDifficileEffect(NwCreature creature, NwCreature caster, NwSpell spell)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.Marcheur3))
        return;

      foreach(var eff in creature.ActiveEffects)
        if (EffectUtils.In(eff.Tag, TerrainDifficileEffectTag, SprintMobileEffectTag, "_LIBERTE_DE_MOUVEMENT_EFFECT") && eff.Spell?.Id == spell.Id)
          return;

      creature.ApplyEffect(EffectDuration.Permanent, TerrainDifficile(spell, caster));
    }
  }
}
