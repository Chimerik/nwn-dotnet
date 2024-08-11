using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> FrappePiegeuse(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

        NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappePiegeuseAttack, SpellUtils.GetSpellDuration(oCaster, spellEntry)));

        caster.OnCreatureAttack -= OnAttackFrappePiegeuse;
        caster.OnCreatureAttack += OnAttackFrappePiegeuse;
        caster.OnEffectRemove -= OnEffectRemoveFrappePiegeuse;
        caster.OnEffectRemove += OnEffectRemoveFrappePiegeuse;

        concentrationTargets.Add(caster);
      }

      return concentrationTargets;
    }
    private static void OnAttackFrappePiegeuse(OnCreatureAttack onAttack)
    {
      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsWeapon(weapon.BaseItem) || onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.AutomaticHit:
        case AttackResult.CriticalHit:

          EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.FrappePiegeuseAttackTag);
          SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FrappePiegeuse];
          int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NwSpell.FromSpellId(CustomSpell.FrappePiegeuse), Ability.Wisdom);

          if (CreatureUtils.GetSavingThrow(onAttack.Attacker, target, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.FrappePiegeuse, NwTimeSpan.FromRounds(10)));

          break;
      }
    }

    private static void OnEffectRemoveFrappePiegeuse(OnEffectRemove onRemove)
    {
      if (onRemove.Effect.Tag != EffectSystem.FrappePiegeuseAttackTag || onRemove.Object is not NwCreature creature)
        return;

      creature.OnCreatureAttack -= OnAttackFrappePiegeuse;
      creature.OnEffectRemove -= OnEffectRemoveFrappePiegeuse;
    }
  }
}
