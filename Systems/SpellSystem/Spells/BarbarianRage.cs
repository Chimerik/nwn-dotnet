using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BarbarianRage(NwCreature caster, NwSpell spell, SpellEntry spellEntry)
    {
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

      ItemProperty physicalResistance = ItemProperty.DamageImmunity(IPDamageType.Physical, IPDamageImmunityType.Immunity50Pct);
      physicalResistance.Creator = caster;
      physicalResistance.Tag = "BARBARIAN_RAGE";

      caster.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(physicalResistance, EffectDuration.Temporary, NwTimeSpan.FromRounds(10));

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.barbarianRageEffect, NwTimeSpan.FromRounds(spellEntry.duration));

      byte barbarianLevel = caster.GetClassInfo(NwClass.FromClassType(ClassType.Barbarian)).Level;

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

      if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike)))
        caster.IncrementRemainingFeatUses(NwFeat.FromFeatId(CustomSkill.BersekerFrenziedStrike));

      if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.BersekerRageAveugle)))
        foreach(var eff in caster.ActiveEffects)
          switch(eff.Tag)
          {
            case EffectSystem.CharmEffectTag:
            case EffectSystem.FrightenedEffectTag: caster.RemoveEffect(eff); break;
          }

      ModuleSystem.Log.Info($"------------{caster.GetFeatRemainingUses(Feat.BarbarianRage)}---------------");

      if (caster.GetClassInfo(NwClass.FromClassType(ClassType.Barbarian))?.Level < 20)
        FeatUtils.DecrementFeatUses(caster, (int)Feat.BarbarianRage);

      CreatureUtils.HandleBonusActionCooldown(caster);
    }
  }
}
