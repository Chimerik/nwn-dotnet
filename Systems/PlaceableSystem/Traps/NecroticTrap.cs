using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void NecroticTrap(NwCreature creature, NwGameObject trap, TrapEntry entry)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(creature, Ability.Constitution) + creature.KnowsFeat(Feat.KeenSense).ToInt();
      int totalSave = SpellUtils.GetSavingThrowRoll(creature, Ability.Constitution, entry.baseDC, advantage, feedback);
      int damage = NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice); // TODO : Variabiliser les dégâts selon la compétence de l'artisan
      bool saveFailed = totalSave < entry.baseDC; // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège

      if (saveFailed)
        NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Permanent, Effect.LinkEffects(Effect.AbilityDecrease(Ability.Strength, entry.aoeSize), Effect.VisualEffect(VfxType.ImpReduceAbilityScore))));

      damage = SpellUtils.GetEsquiveTotaleDamageReduction(creature, damage, saveFailed);

      TrapUtils.SendSavingThrowFeedbackMessage(creature, feedback.saveRoll, feedback.proficiencyBonus, advantage, entry.baseDC, totalSave, saveFailed, Ability.Dexterity);

      creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
      NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, entry.damageType)));

      LogUtils.LogMessage($"Dégâts finaux : {damage}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"------------------------------------------", LogUtils.LogType.Combat);
    }
  }
}
