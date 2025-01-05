using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RepliqueInvoqueeAuraEffectTag = "_REPLIQUE_INVOQUEE_AURA_EFFECT";
    public const string RepliqueInvoqueeEffectTag = "_REPLIQUE_INVOQUEE_EFFECT";
    public static readonly Native.API.CExoString RepliqueInvoqueeEffectExoTag = RepliqueInvoqueeEffectTag.ToExoString();
    public const string repliqueTag = "_REPLIQUE_DUPLICITE";
    public static readonly Native.API.CExoString repliqueExoTag = repliqueTag.ToExoString();
    private static ScriptCallbackHandle onEnterRepliqueInvoqueeCallback;
    private static ScriptCallbackHandle onExitRepliqueInvoqueeCallback;
    private static ScriptCallbackHandle onRemoveRepliqueDupliciteCallback;
    private static ScriptCallbackHandle onRemoveRepliqueDupliciteSanctuaryCallback;

    public static void CreateRepliqueInvoquee(NwCreature caster, Location targetLocation)
    {
      caster.SetFeatRemainingUses((Feat)CustomSkill.MoveRepliqueDuplicite, 100);

      var replique = caster.Clone(targetLocation, repliqueTag, false);
      replique.PlotFlag = true;
      replique.MovementRate = MovementRate.Immobile;
      replique.RemoveFeat((Feat)CustomSkill.ClercRepliqueInvoquee);
      replique.RemoveFeat((Feat)CustomSkill.ClercBenedictionDuFilou);
      replique.RemoveFeat((Feat)CustomSkill.MoveRepliqueDuplicite);

      foreach(var classInfo in replique.Classes.Where(c => c.Class.IsSpellCaster))
      {
        for (byte i = 1; i < 10; i++)
          classInfo.SetRemainingSpellSlots(i, caster.GetClassInfo(classInfo.Class).GetRemainingSpellSlots(i));
      }

      CreaturePlugin.AddAssociate(caster, replique, (int)AssociateType.Familiar);

      foreach (var item in replique.Inventory.Items)
        item.Destroy();

      foreach (var slot in (InventorySlot[])Enum.GetValues(typeof(InventorySlot)))
      {
        var item = replique.GetItemInSlot(slot);

        if (item is not null)
          item.Droppable = false;
      }

      var eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurGhostTransparent), Effect.CutsceneGhost(),
        Effect.RunAction(onRemovedHandle: onRemoveRepliqueDupliciteCallback),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterRepliqueInvoqueeCallback, onExitHandle: onExitRepliqueInvoqueeCallback));

      eff.Tag = RepliqueInvoqueeAuraEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = replique;

      replique.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(10));
      replique.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfSummonMonster1));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(4);

      eff = Effect.LinkEffects(Effect.Sanctuary(100), Effect.RunAction(onRemovedHandle: onRemoveRepliqueDupliciteSanctuaryCallback));
      replique.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(10));
    }

    public static Effect RepliqueInvoquee
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACDecrease);
        eff.Tag = RepliqueInvoqueeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    private static ScriptHandleResult onEnterRepliqueInvoquee(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering == protector || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, RepliqueInvoquee));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitRepliqueInvoquee(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, RepliqueInvoqueeEffectTag);
      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult OnRemoveRepliqueDuplicite(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        if (creature.Master is not null)
          NWScript.AssignCommand(creature, () => creature.Master.ApplyEffect(EffectDuration.Instant, Effect.Heal(creature.Master.GetClassInfo(ClassType.Cleric).Level)));

        creature.UnpossessFamiliar();
        creature.Unsummon();
      }

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onRemoveRepliqueDupliciteSanctuary(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        var eff = Effect.LinkEffects(Effect.Sanctuary(100), Effect.RunAction(onRemovedHandle: onRemoveRepliqueDupliciteSanctuaryCallback));
        creature.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(10));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
