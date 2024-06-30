using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onRemoveBarbarianRageCallback;
    private static ScriptCallbackHandle onIntervalBarbarianRageCallback;
    public const string BarbarianRageEffectTag = "_EFFECT_BARBARIAN_RAGE";
    public static readonly Native.API.CExoString barbarianRageEffectExoTag = "_EFFECT_BARBARIAN_RAGE".ToExoString();
    public const string BarbarianRageAveugleEffectTag = "_EFFECT_BARBARIAN_RAGE_AVEUGLE";
    public static Effect BarbarianRage
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)168), 
          Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50), Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Slashing, 50),
          Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = BarbarianRageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect BearBarbarianRage
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)168),
          Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50), Effect.DamageImmunityIncrease(DamageType.Piercing, 50), Effect.DamageImmunityIncrease(DamageType.Slashing, 50), Effect.DamageImmunityIncrease(DamageType.Acid, 50), Effect.DamageImmunityIncrease(DamageType.Sonic, 50), Effect.DamageImmunityIncrease(DamageType.Custom18, 50), Effect.DamageImmunityIncrease(DamageType.Custom9, 50), Effect.DamageImmunityIncrease(DamageType.Cold, 50), Effect.DamageImmunityIncrease(DamageType.Electrical, 50), Effect.DamageImmunityIncrease(DamageType.Custom1, 50), Effect.DamageImmunityIncrease(DamageType.Custom10, 50), Effect.DamageImmunityIncrease(DamageType.Custom11, 50), Effect.DamageImmunityIncrease(DamageType.Custom12, 50), Effect.DamageImmunityIncrease(DamageType.Custom13, 50), Effect.DamageImmunityIncrease(DamageType.Custom14, 50), Effect.DamageImmunityIncrease(DamageType.Custom15, 50), Effect.DamageImmunityIncrease(DamageType.Custom16, 50), Effect.DamageImmunityIncrease(DamageType.Custom17, 50), Effect.DamageImmunityIncrease(DamageType.Custom19, 50), Effect.DamageImmunityIncrease(DamageType.Custom2, 50), Effect.DamageImmunityIncrease(DamageType.Custom3, 50), Effect.DamageImmunityIncrease(DamageType.Custom4, 50), Effect.DamageImmunityIncrease(DamageType.Custom5, 50), Effect.DamageImmunityIncrease(DamageType.Custom6, 50), Effect.DamageImmunityIncrease(DamageType.Custom7, 50), Effect.DamageImmunityIncrease(DamageType.Custom8, 50), Effect.DamageImmunityIncrease(DamageType.Divine, 50), Effect.DamageImmunityIncrease(DamageType.Fire, 50), Effect.DamageImmunityIncrease(DamageType.Magical, 50), Effect.DamageImmunityIncrease(DamageType.Negative, 50), Effect.DamageImmunityIncrease(DamageType.Positive, 50),
          Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
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

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.DurCessatePositive));
      target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

      target.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
      target.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;
      target.OnItemEquip -= ItemSystem.OnEquipBarbarianRage;
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

      if (target.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle))
        EffectUtils.RemoveTaggedEffect(target, BarbarianRageAveugleEffectTag);

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
        EffectUtils.RemoveTaggedEffect(target, WildMagicEspritIntangibleEffectTag, WildMagicRayonDeLumiereEffectTag, WildMagicRepresaillesEffectTag,
          LumieresProtectricesAuraEffectTag, WildMagicCroissanceVegetaleAuraEffectTag);

        target.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").Delete();
        target.SetFeatRemainingUses((Feat)CustomSkill.WildMagicTeleportation, 0);

        NwItem mainWeapon = target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_1").Value;
        NwItem secondaryWeapon = target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_2").Value;

        if(mainWeapon is not null)
        {
          foreach(var ip in mainWeapon.ItemProperties)
            if(ip.Tag == "_WILDMAGIC_ARME_INFUSEE_ITEM_PROPERTY")
              mainWeapon.RemoveItemProperty(ip);

          target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_1").Delete();
        }

        if (secondaryWeapon is not null)
        {
          foreach (var ip in secondaryWeapon.ItemProperties)
            if (ip.Tag == "_WILDMAGIC_ARME_INFUSEE_ITEM_PROPERTY")
              secondaryWeapon.RemoveItemProperty(ip);

          target.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_2").Delete();
        }
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult OnIntervalBarbarianRage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();
      
      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").HasNothing 
        && !target.KnowsFeat((Feat)CustomSkill.BarbarianRagePersistante))
        EffectUtils.RemoveTaggedEffect(target, BarbarianRageEffectTag);
      else
      {
        target.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

        if (target.KnowsFeat((Feat)CustomSkill.TotemFerociteIndomptable))
          target.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 100);

        if (target.KnowsFeat((Feat)CustomSkill.TotemHurlementGalvanisant))
          target.SetFeatRemainingUses((Feat)CustomSkill.TotemHurlementGalvanisant, 100);

        if(target.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").HasValue)
          target.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 1);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
