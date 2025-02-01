using Anvil.API;
using System;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    /*public static async void Petrify(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster) || onSpellCast.TargetObject is not NwCreature target)
        return;

      CreatureUtils.IsImmuneToPetrification(target);

      int nCasterLevel = oCaster.CasterLevel;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);


      if (oCaster.CheckResistSpell(onSpellCast.TargetObject) == ResistSpellResult.Failed
        && target.RollSavingThrow(SavingThrow.Fortitude, onSpellCast.SaveDC, SavingThrowType.Spell) == SavingThrowResult.Failure)
      {
        Effect ePetrify = Effect.LinkEffects(Effect.Petrify(), Effect.VisualEffect(VfxType.DurCessateNegative));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Slashing, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Bludgeoning, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Piercing, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Acid, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Sonic, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Cold, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Divine, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Electrical, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Fire, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Magical, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Negative, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Positive, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom1, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom2, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom3, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom4, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom5, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom6, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom7, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom8, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom9, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom10, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom11, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom12, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom13, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom14, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom15, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom16, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom17, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom18, 50));
        ePetrify = Effect.LinkEffects(ePetrify, Effect.DamageImmunityIncrease(DamageType.Custom19, 50));

        if (target.IsLoginPlayerCharacter)
        {
          await NwTask.Delay(TimeSpan.FromSeconds(2.75));
          target.LoginPlayer.PopUpDeathPanel(true, true, 40579);
        }

        target.ApplyEffect(EffectDuration.Permanent, ePetrify);
        target.ClearActionQueue();
      }
    }*/
  }
}
