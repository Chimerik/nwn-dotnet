﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string GardienDeLaFoiAuraEffectTag = "_GARDIEN_DE_LA_FOI_AURA_EFFECT";
    public const string GardienDeLaFoiEffectTag = "_GARDIEN_DE_LA_FOI_EFFECT";
    private static ScriptCallbackHandle onEnterGardienDeLaFoiCallback;
    private static ScriptCallbackHandle onExitGardienDeLaFoiCallback;
    public static Effect GardienDeLaFoiAura
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.CutsceneGhost(), 
          Effect.AreaOfEffect((PersistentVfxType)185, onEnterHandle: onEnterGardienDeLaFoiCallback, onExitHandle: onExitGardienDeLaFoiCallback));
        eff.Tag = GardienDeLaFoiAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi);
        return eff;
      }
    }
    public static Effect GardienDeLaFoi
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = GardienDeLaFoiEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterGardienDeLaFoiAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature guardian || !entering.IsReactionTypeHostile(guardian)
        || entering.ActiveEffects.Any(e => e.Tag == GardienDeLaFoiEffectTag))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(guardian, () => entering.ApplyEffect(EffectDuration.Temporary, GardienDeLaFoi, NwTimeSpan.FromRounds(1)));

      NwCreature caster = guardian.GetObjectVariable<LocalVariableObject<NwCreature>>("_GUARDIAN_CASTER").Value;

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.GardienDeLaFoi];
      int spellDC = caster is not null ? SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi), Ability.Wisdom) : 10;
      int damageDealt = SpellUtils.DealSpellDamage(entering, 0, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi)), caster, 4, 
        CreatureUtils.GetSavingThrowResult(entering, spellEntry.savingThrowAbility, caster, spellDC, spellEntry));

      entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHarm));

      if (guardian.HP <= damageDealt)
      {
        guardian.PlotFlag = false;
        guardian.Destroy();
      }
      else
        guardian.HP -= damageDealt;

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitGardienDeLaFoiAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature guardian
        || exiting.ActiveEffects.Any(e => e.Tag == GardienDeLaFoiEffectTag))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(guardian, () => exiting.ApplyEffect(EffectDuration.Temporary, GardienDeLaFoi, NwTimeSpan.FromRounds(1)));

      NwCreature caster = guardian.GetObjectVariable<LocalVariableObject<NwCreature>>("_GUARDIAN_CASTER").Value;
      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.GardienDeLaFoi];
      int spellDC = caster is not null ? SpellUtils.GetCasterSpellDC(caster, NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi), Ability.Wisdom) : 10;
      int damageDealt = SpellUtils.DealSpellDamage(exiting, 0, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, NwSpell.FromSpellId(CustomSpell.GardienDeLaFoi)), caster, 4, 
        CreatureUtils.GetSavingThrowResult(exiting, spellEntry.savingThrowAbility, caster, spellDC, spellEntry));

      exiting.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHarm));

      if (guardian.HP <= damageDealt)
      {
        guardian.PlotFlag = false;
        guardian.Destroy();
      }
      else
        guardian.HP -= damageDealt;

      return ScriptHandleResult.Handled;
    }
  }
}
