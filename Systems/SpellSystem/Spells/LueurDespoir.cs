using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> LueurDespoir(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

        foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
        {
          if (target.IsReactionTypeHostile(caster))
            continue;

          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.LueurDespoir, SpellUtils.GetSpellDuration(oCaster, spellEntry));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
          concentrationTargets.Add(target);
        }
      }

      return concentrationTargets;
    }
  }
}
