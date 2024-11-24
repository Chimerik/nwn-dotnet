using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Bourrasque(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, NwFeat feat = null)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        if (feat is not null && feat.Id == CustomSkill.MonkRueeDesEspritsDuVent)
        {
          caster.IncrementRemainingFeatUses(feat.FeatType);
          FeatUtils.DecrementKi(caster, 2);
          castingClass = NwClass.FromClassId(CustomClass.Monk);
        }

        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        oCaster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{spell.Id}").Value = (int)castingClass.SpellCastingAbility;
        
        oCaster.Location.ApplyEffect(EffectDuration.Temporary, EffectSystem.Bourrasque(caster, castingClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry));  
        concentrationList.Add(UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>());

        oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosNormal20));
      }

      return concentrationList;
    }
  }
}
