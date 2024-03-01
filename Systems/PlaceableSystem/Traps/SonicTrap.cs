using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void SonicTrap(NwGameObject trap, TrapEntry entry)
    {
      foreach (NwCreature creature in trap.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, entry.aoeSize, false))
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int advantage = creature.KnowsFeat(Feat.KeenSense).ToInt() + (creature.Race.Id == CustomRace.RockGnome || creature.Race.Id == CustomRace.ForestGnome|| creature.Race.Id == CustomRace.DeepGnome).ToInt();
        int totalSave = SpellUtils.GetSavingThrowRoll(creature, Ability.Wisdom, entry.baseDC, advantage, feedback);
        bool saveFailed = totalSave < entry.baseDC; // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège
        int damage = NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice); // TODO : Variabiliser les dégâts selon la compétence de l'artisan

        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));

        if (saveFailed)
          NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Temporary, Effect.LinkEffects(Effect.Stunned(), Effect.VisualEffect(VfxType.DurMindAffectingDisabled)), TimeSpan.FromSeconds(entry.duration)));

        if (creature.KnowsFeat(Feat.KeenSense))
        {
          damage /= 2;
          creature?.LoginPlayer.DisplayFloatingTextStringOnCreature(creature, "Expert en donjons".ColorString(StringUtils.gold));
          LogUtils.LogMessage($"Expert en donjons : dégâts {damage}", LogUtils.LogType.Combat);
        }

        if (creature.IsLoginPlayerCharacter)
          TrapUtils.SendSavingThrowFeedbackMessage(creature, feedback.saveRoll, feedback.proficiencyBonus, advantage, entry.baseDC, totalSave, saveFailed, Ability.Dexterity);

        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
        NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, entry.damageType)));

        LogUtils.LogMessage($"Dégâts finaux : {damage}", LogUtils.LogType.Combat);
        LogUtils.LogMessage($"------------------------------------------", LogUtils.LogType.Combat);
      }
    }
  }
}
