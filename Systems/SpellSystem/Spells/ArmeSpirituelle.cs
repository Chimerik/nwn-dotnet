using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void ArmeSpirituelle(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster)
        return;

      var bonusActionCooldown = oCaster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == EffectSystem.BonusActionId);

      if (bonusActionCooldown is not null)
        oCaster.RemoveEffect(bonusActionCooldown);

      await caster.WaitForObjectContext();
      var duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);
      targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.SummonCreature("X2_S_FAERIE001", VfxType.FnfSummonMonster1), duration);   

      NwCreature summon = UtilPlugin.GetLastCreatedObject(NWNXObjectType.Creature).ToNwObject<NwCreature>();
      //NwItem weapon = await NwItem.Create("NW_WSWDG001", summon); 
      NwItem weapon = await NwItem.Create(BaseItems2da.baseItemTable[(int)BaseItemType.Warhammer].craftedItem, summon);
      weapon.Droppable = false;

      summon.RunEquip(weapon, InventorySlot.RightHand);
      summon.Tag = CreatureUtils.ArmeSpirituelleTag;
      summon.GetObjectVariable<LocalVariableInt>(CreatureUtils.CastAbilityVariable).Value = (int)castingClass.SpellCastingAbility;
      summon.SetsRawAbilityScore(Ability.Dexterity, 10);
      summon.SetsRawAbilityScore(Ability.Strength, 10);
      summon.ApplyEffect(EffectDuration.Permanent, Effect.VisualEffect(VfxType.DurGhostlyVisage));

      summon.OnCreatureAttack += OnAttackArmeSpirituelle;
      summon.OnCreatureAttack += OnAttackArmeSpirituelle;

      EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { summon }, duration);
    }

    public static void OnAttackArmeSpirituelle(OnCreatureAttack onAttack)
    {
      if (onAttack.Attacker.Master is null)
      {
        onAttack.Attacker.Unsummon();
        return;
      }

      var bonusAction = onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.BonusActionEffectTag);

      if (bonusAction is not null)
      {
        onAttack.Attacker.RemoveEffect(bonusAction);

        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.CriticalHit:
          case AttackResult.AutomaticHit:

            int bonusDamage = onAttack.Attacker.Master.GetAbilityModifier((Ability)onAttack.Attacker.GetObjectVariable<LocalVariableInt>(CreatureUtils.CastAbilityVariable).Value);
            NWScript.AssignCommand(onAttack.Attacker, 
              () => onAttack.Target.ApplyEffect(EffectDuration.Instant, 
                Effect.Damage(Utils.Roll(8, onAttack.AttackResult == AttackResult.CriticalHit ? 2 : 1) + bonusDamage)));

            onAttack.Target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpStarburstRed));

            break;
        }
      }
      else
        onAttack.AttackResult = AttackResult.Miss;
    }
  }
}
