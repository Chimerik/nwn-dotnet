using Anvil.API.Events;
using Anvil.API;
using System.Linq;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class PaladinUtils
  {
    public static async void OnAttackPuissanceInquisitrice(OnCreatureAttack onAttack)
    {
      if (onAttack.Target is not NwCreature target)
        return;

      var eff = onAttack.Attacker.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.PuissanceInquisitriceEffectTag);

      if (eff is not null && eff.Creator is NwCreature creator)
      {
        switch (onAttack.AttackResult)
        {
          case AttackResult.Hit:
          case AttackResult.CriticalHit:
          case AttackResult.AutomaticHit:

            int spellDC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(creator) + creator.GetAbilityModifier(Ability.Charisma);

            if (CreatureUtils.GetSavingThrow(creator, target, Ability.Constitution, spellDC) == SavingThrowResult.Failure)
              NWScript.AssignCommand(creator, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Dazed(), NwTimeSpan.FromRounds(1)));

            break;
        }
      }
      else
      {
        await NwTask.NextFrame();
        onAttack.Attacker.OnCreatureAttack -= OnAttackPuissanceInquisitrice;
      }
    }
  }
}
