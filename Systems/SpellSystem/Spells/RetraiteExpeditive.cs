using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> RetraiteExpeditive(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      List<NwGameObject> targets = new() { oCaster };

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.RetraiteExpeditive, SpellUtils.GetSpellDuration(oCaster, spellEntry));

      EffectUtils.RemoveTaggedEffect(oCaster, EffectSystem.SprintEffectTag);

      if (oCaster is NwCreature caster)
      {
        oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Sprint(caster), NwTimeSpan.FromRounds(1));

        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Chargeur)))
          caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

        if (caster.KnowsFeat((Feat)CustomSkill.BelluaireEntrainementExceptionnel))
        {
          var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

          if (companion is not null)
          {
            companion.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
            companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.Sprint(companion), NwTimeSpan.FromRounds(1));
          }
        }
      }

      return targets;
    }  
  }
}
