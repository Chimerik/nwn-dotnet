using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LienDeGardeAuraEffectTag = "_LIEN_DE_GARDE_AURA_EFFECT";
    public const string LienDeGardeBonusEffectTag = "_LIEN_DE_GARDE_BONUS_EFFECT";
    private static ScriptCallbackHandle onExitLienDeGardeCallback;

    public static void ApplyLienDeGarde(NwCreature caster, NwCreature target, NwSpell spell, SpellEntry spellEntry)
    {
      var previousLien = target.ActiveEffects.FirstOrDefault(e => e.Tag == LienDeGardeBonusEffectTag);

      if(previousLien is not null)
      {
        if(previousLien.Creator is NwCreature previousCreator)
          EffectUtils.RemoveTaggedEffect(previousCreator, target, LienDeGardeAuraEffectTag);

        EffectUtils.RemoveTaggedEffect(target, caster, LienDeGardeBonusEffectTag);
      }

      Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobDragonFear, onExitHandle: onExitLienDeGardeCallback);
      eff.Tag = LienDeGardeAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = target;
      eff.Spell = spell;

      caster.ApplyEffect(EffectDuration.Temporary, eff, SpellUtils.GetSpellDuration(caster, spellEntry)); 

      eff = Effect.LinkEffects(Effect.ACIncrease(1), Effect.Beam(VfxType.BeamSilentHoly, caster, BodyNode.Chest), Effect.VisualEffect(CustomVfx.BouclierDeLaFoi),
        ResistanceElec, ResistanceFeu, ResistanceFroid, ResistanceAcide, ResistanceContondant, ResistanceForce, ResistanceNecrotique, ResistancePercant, ResistancePoison, ResistancePsychique, ResistanceRadiant, ResistanceTonnerre, ResistanceTranchant);
      eff.Tag = LienDeGardeAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;

      target.ApplyEffect(EffectDuration.Temporary, eff, SpellUtils.GetSpellDuration(caster, spellEntry));

      target.OnDamaged -= OnDamagedLienDeGarde; 
      target.OnDamaged += OnDamagedLienDeGarde; 
    }
    private static ScriptHandleResult onExitLienDeGarde(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, LienDeGardeBonusEffectTag);
      EffectUtils.RemoveTaggedEffect(protector, exiting, LienDeGardeAuraEffectTag);

      return ScriptHandleResult.Handled;
    }
    public static void OnDamagedLienDeGarde(CreatureEvents.OnDamaged onDamaged)
    {
      var previousLien = onDamaged.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == LienDeGardeBonusEffectTag);

      if (previousLien is not null || previousLien.Creator is not NwCreature caster)
      {
        onDamaged.Creature.OnDamaged -= OnDamagedLienDeGarde;
        return;
      }

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
      {
        int damage = onDamaged.GetDamageDealtByType(damageType);

        if (damage > 0)
          NWScript.AssignCommand(onDamaged.Creature, () => caster.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, damageType)));
      }
    }
  }
}
