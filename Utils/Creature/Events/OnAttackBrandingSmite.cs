using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void OnAttackBrandingSmite(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      switch (onAttack.AttackResult)
      {
        case AttackResult.Hit:
        case AttackResult.CriticalHit:
        case AttackResult.AutomaticHit:

          NwItem weapon = onAttack.Attacker.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is null || ItemUtils.IsMeleeWeapon(weapon.BaseItem.ItemType))
          {
            var spellEntry = Spells2da.spellTable[CustomSpell.BrandingSmite];
            var duration = SpellUtils.GetSpellDuration(onAttack.Attacker, spellEntry);

            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.Damage(NwRandom.Roll(Utils.random, 6, onAttack.AttackResult == AttackResult.CriticalHit ? 4 : 2), DamageType.Divine), Effect.VisualEffect(VfxType.ImpDivineStrikeHoly))));
            NWScript.AssignCommand(onAttack.Attacker, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.brandingSmiteReveal, duration));
           
            target.SetActionMode(ActionMode.Stealth, false);
            target.OnStealthModeUpdate -= EffectSystem.OnBrandingSmiteReveal;
            target.OnStealthModeUpdate += EffectSystem.OnBrandingSmiteReveal;

            target.OnEffectApply -= EffectSystem.OnBrandingSmiteReveal;
            target.OnEffectApply += EffectSystem.OnBrandingSmiteReveal;

            foreach (var eff in target.ActiveEffects)
            {
              switch (eff.EffectType)
              {
                case EffectType.Invisibility:
                case EffectType.ImprovedInvisibility: target.RemoveEffect(eff); break;
              }
            }

            EffectSystem.ApplyConcentrationEffect(onAttack.Attacker, spellEntry.RowIndex, new List<NwGameObject>() { target }, duration);

            await NwTask.NextFrame();
            onAttack.Attacker.OnCreatureAttack -= OnAttackBrandingSmite;
          }

          break;
      }
    }
  }
}
