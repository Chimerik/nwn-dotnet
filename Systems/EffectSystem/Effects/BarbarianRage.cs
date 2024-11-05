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
    public static Effect BarbarianRage(NwCreature caster, bool bearRage = false)
    {
      int level = caster.GetClassInfo((ClassType)CustomClass.Barbarian).Level;

      Effect resBludg = Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50);
      resBludg.ShowIcon = false;
      Effect resPierc = Effect.DamageImmunityIncrease(DamageType.Piercing, 50);
      resPierc.ShowIcon = false;
      Effect resSlash = Effect.DamageImmunityIncrease(DamageType.Slashing, 50);
      resSlash.ShowIcon = false;

      Effect eff;

      if(bearRage)
      {
        Effect resAcid = Effect.DamageImmunityIncrease(DamageType.Acid, 50);
        resAcid.ShowIcon = false;
        Effect resCold = Effect.DamageImmunityIncrease(DamageType.Cold, 50);
        resCold.ShowIcon = false;
        Effect resElec = Effect.DamageImmunityIncrease(DamageType.Electrical, 50);
        resElec.ShowIcon = false;
        Effect resFire = Effect.DamageImmunityIncrease(DamageType.Fire, 50);
        resFire.ShowIcon = false;
        Effect resSonic = Effect.DamageImmunityIncrease(DamageType.Sonic, 50);
        resSonic.ShowIcon = false;
        Effect resPoison = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        resPoison.ShowIcon = false;

        eff = Effect.LinkEffects(resBludg, resPierc, resSlash, resAcid, resCold, resElec, resFire, resSonic, resPoison,
        Effect.Icon((EffectIcon)168), Effect.Icon((EffectIcon)211), Effect.Icon((EffectIcon)212), Effect.Icon((EffectIcon)213),
        Effect.Icon((EffectIcon)205), Effect.Icon((EffectIcon)206), Effect.Icon((EffectIcon)208), Effect.Icon((EffectIcon)209),
        Effect.Icon((EffectIcon)210), Effect.Icon((EffectIcon)214),
        Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
      }
      else
      {
        eff = Effect.LinkEffects(resBludg, resPierc, resSlash,
        Effect.Icon((EffectIcon)168), Effect.Icon((EffectIcon)211), Effect.Icon((EffectIcon)212), Effect.Icon((EffectIcon)213),
        Effect.RunAction(onRemovedHandle: onRemoveBarbarianRageCallback, onIntervalHandle: onIntervalBarbarianRageCallback, interval: NwTimeSpan.FromRounds(1)));
      }

      eff.Tag = BarbarianRageEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.IntParams[5] = level > 15 ? 4 : level > 8 ? 3 : 2;

      return eff;
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
      {
        if (target.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
          EffectUtils.RemoveTaggedEffect(target, BarbarianRageEffectTag);
        else
        {
          target.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;
          RenewBarbarianRage(target);
        }
      }
      else
        RenewBarbarianRage(target);

      return ScriptHandleResult.Handled;
    }
    private static void RenewBarbarianRage(NwCreature caster)
    {
      caster.GetObjectVariable<LocalVariableInt>("_BARBARIAN_RAGE_RENEW").Delete();

      if (caster.KnowsFeat((Feat)CustomSkill.TotemFerociteIndomptable))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemHurlementGalvanisant))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemHurlementGalvanisant, 100);

      if (caster.GetObjectVariable<LocalVariableInt>("_WILDMAGIC_TELEPORTATION").HasValue)
        caster.SetFeatRemainingUses(NwFeat.FromFeatId(CustomSkill.WildMagicTeleportation), 1);
    }
  }
}
