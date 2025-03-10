using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> AuraDeVitalite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      oCaster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAuraHoly));

      DelayEffect(oCaster, EffectSystem.AuraDeVitalite(castingClass.Id, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));
      DelayEffect(oCaster, EffectSystem.AuraDeVitaliteHeal, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      
      return new List<NwGameObject>() { oCaster };
    }

    public static void AuraDeVitaliteHeal(NwGameObject oCaster)
    {
      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.EnterTargetMode(SelectVitaliteHealTarget, Config.CreatureTargetMode(10, new Vector2() { X = 1, Y = 1 }));
    }
    private static void SelectVitaliteHealTarget(Anvil.API.Events.ModuleEvents.OnPlayerTarget selection)
    {
      if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
        return;

      NwCreature caster = selection.Player.ControlledCreature;
      Effect eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.AuraDeVitaliteEffectTag);

      if (eff is null)
        return;

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.AuraDeVitalite];
      SpellUtils.SignalEventSpellCast(target, caster, (Spell)CustomSpell.AuraDeVitalite, false);

      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, 
        Effect.Heal(SpellUtils.GetHealAmount(caster, target, eff.Spell, spellEntry, NwClass.FromClassId(eff.CasterLevel), spellEntry.numDice))));

      EffectUtils.RemoveTaggedEffect(caster, EffectSystem.AuraDeVitaliteHealEffectTag);
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 6, CustomSpell.AuraDeVitalite));
    }
  }
}
