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
    public const string PuitsDeLuneEffectTag = "_PUITS_DE_LUNE_EFFECT";
    public const string PuitsDeLuneDamageEffectTag = "_PUITS_DE_LUNE_DAMAGE_EFFECT";
    private static ScriptCallbackHandle onIntervalPuitsDeLuneCallback;
    private static ScriptCallbackHandle onRemovePuitsDeLuneCallback;
    public static Effect PuitsDeLune(NwCreature caster, NwSpell spell, Ability ability, TimeSpan spellDuration)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Divine, 50),
        Effect.RunAction(onRemovedHandle: onRemovePuitsDeLuneCallback, onIntervalHandle: onIntervalPuitsDeLuneCallback, interval: TimeSpan.FromSeconds(2)));
      
      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);
      
      if(weapon is null || !weapon.IsRangedWeapon)
      {
        var damageEff = Effect.DamageIncrease((int)DamageBonus.Plus2d6, DamageType.Divine);
        damageEff.Tag = PuitsDeLuneDamageEffectTag;
        damageEff.SubType = EffectSubType.Supernatural;

        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, damageEff, spellDuration));
      }
      
      eff.Tag = PuitsDeLuneEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = (int)ability;
      eff.Spell = spell;

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, eff, spellDuration));

      caster.OnDamaged -= OnDamagedPuitsDeLune;
      caster.OnDamaged += OnDamagedPuitsDeLune;

      return eff;
    }
    private static ScriptHandleResult OnRemovePuitsDeLune(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature caster)
        return ScriptHandleResult.Handled;

      caster.OnDamaged -= OnDamagedPuitsDeLune;
      EffectUtils.RemoveTaggedEffect(caster, PuitsDeLuneDamageEffectTag);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalPuitsDeLune(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature caster)
        return ScriptHandleResult.Handled;

      var weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      if ((weapon is null || !weapon.IsRangedWeapon))
      {
        if (!caster.ActiveEffects.Any(e => e.Tag == PuitsDeLuneDamageEffectTag))
        {
          var damageEff = Effect.DamageIncrease((int)DamageBonus.Plus2d6, DamageType.Divine);
          damageEff.Tag = PuitsDeLuneDamageEffectTag;
          damageEff.SubType = EffectSubType.Supernatural;

          NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Temporary, damageEff, TimeSpan.FromSeconds(eventData.Effect.DurationRemaining)));
        } 
      }
      else
        EffectUtils.RemoveTaggedEffect(caster, PuitsDeLuneDamageEffectTag);

      return ScriptHandleResult.Handled;
    }
    public static void OnDamagedPuitsDeLune(CreatureEvents.OnDamaged onDamaged)
    {
      var oDamager = NWScript.GetLastDamager(onDamaged.Creature).ToNwObject<NwObject>();

      if (oDamager is not NwCreature damager)
        return;

      var caster = onDamaged.Creature;
      var eff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == PuitsDeLuneEffectTag);

      if(eff is null)
      {
        caster.OnDamaged -= OnDamagedPuitsDeLune;
        return;
      }

      if (onDamaged.DamageAmount > 0 && caster.DistanceSquared(damager) < 324)
      {
        var reaction = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {

          var spellEntry = Spells2da.spellTable.GetRow(CustomSpell.PuitsDeLune);
          int DC = SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.PuitsDeLune), (Ability)eff.CasterLevel);

          if (CreatureUtils.GetSavingThrow(caster, damager, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
          {
            NWScript.AssignCommand(caster, () => damager.ApplyEffect(EffectDuration.Temporary, Effect.Blindness(), NwTimeSpan.FromRounds(1)));
            damager.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpBlindDeafM));
          }

          caster.RemoveEffect(reaction);
        }
      }
    }
  }
}
