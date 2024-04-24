using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BarbarianRage(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      NwItem armor = caster.GetItemInSlot(InventorySlot.Chest);

      if(armor.BaseACValue > 5)
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas entrer en rage tant que vous portez une armure lourde", ColorConstants.Red);
        return;
      }

      StringUtils.ForceBroadcastSpellCasting(caster, spell);

      switch(NwRandom.Roll(Utils.random, 3))
      {
        case 1: caster.PlayVoiceChat(VoiceChatType.BattleCry1); break;
        case 2: caster.PlayVoiceChat(VoiceChatType.BattleCry2); break;
        case 3: caster.PlayVoiceChat(VoiceChatType.BattleCry3); break;
      }

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpImproveAbilityScore));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.barbarianRageEffect, NwTimeSpan.FromRounds(spellEntry.duration));

      if (caster.KnowsFeat((Feat)CustomSkill.TotemHurlementGalvanisant))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemHurlementGalvanisant, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemAspectTigre))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemAspectTigre, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemLienElan))
        caster.SetFeatRemainingUses((Feat)CustomSkill.TotemLienElan, 100);

      if (caster.KnowsFeat((Feat)CustomSkill.TotemLienOurs))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.totemLienOursAura, NwTimeSpan.FromRounds(10));

      if (caster.KnowsFeat((Feat)CustomSkill.TotemLienLoup))
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackLoupKnockdown;
        caster.OnCreatureAttack += CreatureUtils.OnAttackLoupKnockdown;
      }

      NwItem skin = caster.GetItemInSlot(InventorySlot.CreatureSkin);

      if (skin is not null)
      {
        if (caster.KnowsFeat((Feat)CustomSkill.TotemEspritOurs))
        {
          caster.SetFeatRemainingUses((Feat)CustomSkill.TotemFerociteIndomptable, 1);

          for (int i = 0; i < 17; i++)
          {
            ItemProperty bearResistance = ItemProperty.DamageImmunity((IPDamageType)i, IPDamageImmunityType.Immunity50Pct);
            bearResistance.Creator = caster;
            bearResistance.Tag = EffectSystem.BarbarianRageItemPropertyTag;

            skin.AddItemProperty(bearResistance, EffectDuration.Temporary, NwTimeSpan.FromRounds(10));
          }
        }
        else
        {
          ItemProperty physicalResistance = ItemProperty.DamageImmunity(IPDamageType.Physical, IPDamageImmunityType.Immunity50Pct);
          physicalResistance.Creator = caster;
          physicalResistance.Tag = EffectSystem.BarbarianRageItemPropertyTag;

          skin.AddItemProperty(physicalResistance, EffectDuration.Temporary, NwTimeSpan.FromRounds(10));
        }
      }
      else
        LogUtils.LogMessage($"ERREUR - {caster.Name} ({caster.LoginPlayer?.PlayerName}) ne dispose pas de peau de créature", LogUtils.LogType.IllegalItems);

      byte barbarianLevel = caster.GetClassInfo(ClassType.Barbarian).Level;

      if(barbarianLevel > 10)
      {
        caster.OnDamaged -= CreatureUtils.OnDamagedRageImplacable;
        caster.OnDamaged += CreatureUtils.OnDamagedRageImplacable;
      }

      if (barbarianLevel < 15)
      {
        caster.OnCreatureAttack -= CreatureUtils.OnAttackBarbarianRage;
        caster.OnCreatureAttack += CreatureUtils.OnAttackBarbarianRage;

        caster.OnDamaged -= CreatureUtils.OnDamagedBarbarianRage;
        caster.OnDamaged += CreatureUtils.OnDamagedBarbarianRage;
      }

      caster.OnItemEquip -= ItemSystem.OnEquipBarbarianRage;
      caster.OnItemEquip += ItemSystem.OnEquipBarbarianRage;
      caster.OnSpellAction -= CancelSpellBarbarianRage;
      caster.OnSpellAction += CancelSpellBarbarianRage;

      SpellUtils.DispelConcentrationEffects(caster);

      if (caster.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
        caster.IncrementRemainingFeatUses((Feat)CustomSkill.BersekerFrenziedStrike);

      if (caster.KnowsFeat((Feat)CustomSkill.BersekerRageAveugle))
        foreach(var eff in caster.ActiveEffects)
          switch(eff.Tag)
          {
            case EffectSystem.CharmEffectTag:
            case EffectSystem.FrightenedEffectTag: caster.RemoveEffect(eff); break;
          }

      if (caster.KnowsFeat((Feat)CustomSkill.TotemEspritElan))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.elkTotemSpeed, NwTimeSpan.FromRounds(10));

      if (caster.KnowsFeat((Feat)CustomSkill.TotemEspritLoup))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.wolfTotemAura, NwTimeSpan.FromRounds(10));

      bool freeRageRoll = BarbarianUtils.IsRatelTriggered(caster) && Utils.random.Next(0, 2).ToBool();
        
      if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level < 20) || freeRageRoll)
        FeatUtils.DecrementFeatUses(caster, (int)Feat.BarbarianRage);

      if (caster.KnowsFeat((Feat)CustomSkill.WildMagicSense))
      {
        HandleWildMagicRage(caster);

        if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 9))
          caster.OnDamaged += BarbarianUtils.OnDamagedWildMagic;
      }

      CreatureUtils.HandleBonusActionCooldown(caster);
    }
  }
}
