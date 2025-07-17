using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ApparitionAnimaleTag = "espritanimal";
    public const string ApparitionAnimaleAuraEffectTag = "_APPARITION_ANIMALE_AURA_EFFECT";
    public const string ApparitionAnimaleEffectTag = "_APPARITION_ANIMALE_EFFECT";
    private static ScriptCallbackHandle onEnterApparitionAnimaleCallback;
    private static ScriptCallbackHandle onExitApparitionAnimaleCallback;
    private static ScriptCallbackHandle onHeartbeatApparitionAnimaleCallback;
    private static ScriptCallbackHandle onRemoveApparitionAnimaleCallback;

    public static void CreateApparitionAnimale(NwCreature caster, Location targetLocation, NwClass casterClass, TimeSpan duration)
    {
      NwCreature summon = CreatureUtils.SummonAssociate(caster, AssociateType.Summoned, ApparitionAnimaleTag);

      summon.PlotFlag = true;
      summon.MovementRate = MovementRate.Immobile;
      summon.AiLevel = AiLevel.VeryLow;

      var eff = Effect.LinkEffects(Effect.CutsceneGhost(), Effect.Sanctuary(100), Effect.RunAction(onRemovedHandle: onRemoveApparitionAnimaleCallback),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterApparitionAnimaleCallback, onHeartbeatApparitionAnimaleCallback, onExitApparitionAnimaleCallback));

      eff.Tag = ApparitionAnimaleAuraEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = summon;
      eff.CasterLevel = SpellUtils.GetCasterSpellDC(caster, casterClass.SpellCastingAbility);

      summon.ApplyEffect(EffectDuration.Temporary, eff, duration);
      summon.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster2));
    }

    public static Effect ApparitionAnimale
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SavingThrowIncrease);
        eff.Tag = ApparitionAnimaleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    private static ScriptHandleResult onEnterApparitionAnimale(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature summon)
        return ScriptHandleResult.Handled;

      if (entering == summon.Master)
      {
        NWScript.AssignCommand(summon, () => summon.Master.ApplyEffect(EffectDuration.Permanent, ApparitionAnimale));
      }
      else if (summon.Master.IsReactionTypeHostile(entering))
      {
        var spellEntry = Spells2da.spellTable[CustomSpell.ApparitionAnimale];

        if (CreatureUtils.GetSavingThrowResult(entering, Ability.Dexterity, summon.Master, eventData.Effect.CasterLevel, spellEntry) == SavingThrowResult.Failure)
        {
          SpellUtils.DealSpellDamage(entering, 3, spellEntry, SpellUtils.GetSpellDamageDiceNumber(summon.Master, NwSpell.FromSpellId(CustomSpell.ApparitionAnimale)), summon.Master, 3,
            SavingThrowResult.Failure);
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitApparitionAnimale(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature summon || exiting != summon.Master)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(summon.Master, ApparitionAnimaleEffectTag);
      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onHeartbeatApparitionAnimale(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature summon)
      {
        foreach (NwCreature target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
        {
          if (summon.Master.IsReactionTypeHostile(target))
          {
            var spellEntry = Spells2da.spellTable[CustomSpell.ApparitionAnimale];

            if (CreatureUtils.GetSavingThrowResult(target, Ability.Dexterity, summon.Master, eventData.Effect.CasterLevel, spellEntry) == SavingThrowResult.Failure)
            {
              SpellUtils.DealSpellDamage(target, 3, spellEntry, SpellUtils.GetSpellDamageDiceNumber(summon.Master, NwSpell.FromSpellId(CustomSpell.ApparitionAnimale)), summon.Master, 3,
                SavingThrowResult.Failure);
            }
          }
        }
      }

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult OnRemoveApparitionAnimale(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.Unsummon();
      }

      return ScriptHandleResult.Handled;
    }
  }
}
