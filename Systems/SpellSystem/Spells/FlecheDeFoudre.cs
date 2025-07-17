using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FlecheDeFoudre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadElectricity));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.FlecheDeFoudre));

      caster.OnCreatureAttack -= OnAttackFlecheDeFoudre;
      caster.OnCreatureAttack += OnAttackFlecheDeFoudre;
    }

    public static async void OnAttackFlecheDeFoudre(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
        return;

      onAttack.AttackResult = AttackResult.Miss;

      var nbDices = onAttack.AttackResult switch
      {
        AttackResult.Hit or AttackResult.CriticalHit or AttackResult.AutomaticHit => 4,
        _ => 2,
      };

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.FlecheDeFoudre];

      SpellUtils.DealSpellDamage(target, 3, spellEntry, nbDices, onAttack.Attacker, 3);

      int spellDC = SpellUtils.GetCasterSpellDC(onAttack.Attacker, NwSpell.FromSpellId(CustomSpell.FlecheDeFoudre), Ability.Wisdom);

      foreach (var targetCreature in target.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 3, false))
      {
        SpellUtils.DealSpellDamage(targetCreature, 3, spellEntry, 2, onAttack.Attacker, 3, 
          CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, onAttack.Attacker, spellDC, spellEntry));
      }

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfElectricExplosion));
      EffectUtils.RemoveTaggedEffect(onAttack.Attacker, EffectSystem.FlecheDeFoudreEffectTag);

      await NwTask.NextFrame();
      onAttack.Attacker.OnCreatureAttack -= OnAttackFlecheDeFoudre;
    }
  }
}
