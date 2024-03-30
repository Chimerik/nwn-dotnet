using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveBarbarianRageCallback;
    private static ScriptCallbackHandle onIntervalBarbarianRageCallback;
    public const string BarbarianRageItemPropertyTag = "_ITEMPROPERTY_BARBARIAN_RAGE";
    public const string BarbarianRageEffectTag = "_EFFECT_BARBARIAN_RAGE";
    public static readonly Native.API.CExoString barbarianRageEffectExoTag = "_EFFECT_BARBARIAN_RAGE".ToExoString();
    public static Effect barbarianRageEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurCessatePositive), Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = BarbarianRageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveBarbarianRage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      target.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;
      target.OnItemEquip -= ItemSystem.OnEquipBarbarianRage;
      target.OnSpellAction -= SpellSystem.CancelSpellBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedRageImplacable;
      target.OnDamaged -= BarbarianUtils.OnDamagedWildMagic;

      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.AspectTigreVariable).Delete();

      if (target.KnowsFeat((Feat)CustomSkill.TotemFerociteIndomptable))
        target.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 0);

      if (target.KnowsFeat((Feat)CustomSkill.TotemHurlementGalvanisant))
        target.SetFeatRemainingUses((Feat)CustomSkill.TotemHurlementGalvanisant, 0);

      if (target.KnowsFeat((Feat)CustomSkill.TotemAspectTigre))
        target.SetFeatRemainingUses((Feat)CustomSkill.TotemAspectTigre, 0);

      if (target.KnowsFeat((Feat)CustomSkill.TotemLienElan))
      {
        target.SetFeatRemainingUses((Feat)CustomSkill.TotemLienElan, 0);
        EffectUtils.RemoveTaggedEffect(target, LienTotemElanAuraEffectTag);
      }

      if (target.KnowsFeat((Feat)CustomSkill.TotemLienLoup))
      {
        target.SetFeatRemainingUses((Feat)CustomSkill.TotemLienLoup, 0);
        target.OnCreatureAttack -= CreatureUtils.OnAttackLoupKnockdown;
      }

      if (target.KnowsFeat((Feat)CustomSkill.TotemLienOurs))
        EffectUtils.RemoveTaggedEffect(target, LienTotemOursAuraEffectTag);

      foreach (var eff in target.ActiveEffects)
      {
        switch(eff.Tag)
        {
          case ElkTotemSpeedEffectTag:
          case WolfTotemAuraEffectTag: target.RemoveEffect(eff); break;
        }
      }
      
      if (target.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
      {
        target.SetFeatRemainingUses((Feat)CustomSkill.BersekerFrenziedStrike, 0);
        target.GetObjectVariable<LocalVariableInt>(CreatureUtils.FrappeFrenetiqueMalusVariable).Delete();
      }

      if (target.KnowsFeat((Feat)CustomSkill.WildMagicSense))
      {
        foreach (var eff in target.ActiveEffects)
          if (eff.Tag == WildMagicEspritIntangibleEffectTag || eff.Tag == WildMagicRayonDeLumiereEffectTag || eff.Tag == wildMagicRepresaillesEffectTag
            || eff.Tag == LumieresProtectricesAuraEffectTag)
            target.RemoveEffect(eff);

        target.SetFeatRemainingUses((Feat)CustomSkill.WildMagicTeleportation, 0);
      }

      NwItem skin = target.GetItemInSlot(InventorySlot.CreatureSkin);

      if (skin is not null)
        foreach (var ip in skin.ItemProperties)
          if (ip.Tag == BarbarianRageItemPropertyTag)
            skin.RemoveItemProperty(ip);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalBarbarianRage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").HasNothing)
        EffectUtils.RemoveTaggedEffect(target, BarbarianRageEffectTag);
      else
        target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

      return ScriptHandleResult.Handled;
    }
  }
}
