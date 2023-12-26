using System;
using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void FrostTrap(List<NwCreature> targets, NwGameObject trap, TrapEntry entry)
    {
      foreach (var creature in targets)
      {
        int advantage = creature.KnowsFeat(Feat.KeenSense).ToInt() + (creature.Race.Id == CustomRace.Duergar).ToInt();
        int proficiencyBonus = SpellUtils.GetSavingThrowProficiencyBonus(creature, Ability.Constitution);
        int saveRoll = NativeUtils.HandleHalflingLuck(creature, NativeUtils.HandleChanceDebordante(creature, Utils.RollAdvantage(advantage)));
        int totalSave = saveRoll + proficiencyBonus;
        int damage = NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice); // TODO : Variabiliser les dégâts selon la compétence de l'artisan
        bool saveFailed = totalSave < entry.baseDC; // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège

        if (saveFailed)
          NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Paralyze(), Effect.VisualEffect(VfxType.DurBlur)), TimeSpan.FromSeconds(entry.duration)));

        if (creature.KnowsFeat(Feat.KeenSense))
        {
          damage /= 2;
          creature?.LoginPlayer.DisplayFloatingTextStringOnCreature(creature, "Expert en donjons".ColorString(StringUtils.gold));
        }

        if (creature.IsLoginPlayerCharacter)
          TrapUtils.SendSavingThrowFeedbackMessage(creature, saveRoll, proficiencyBonus, advantage, entry.baseDC, totalSave, saveFailed, Ability.Dexterity);

        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
        NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, entry.damageType)));
      }
    }
  }
}
