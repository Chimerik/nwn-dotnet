
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FormeGazeuse(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwFeat feat = null)
    {
      if (oCaster is NwCreature castingCreature && feat is not null && feat.Id == CustomSkill.MonkPostureBrumeuse)
      {
        castingCreature.IncrementRemainingFeatUses(feat.FeatType);
        FeatUtils.DecrementKi(castingCreature, 4);
        //casterClass = NwClass.FromClassId(CustomClass.Monk);
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
    }  
  }
}
