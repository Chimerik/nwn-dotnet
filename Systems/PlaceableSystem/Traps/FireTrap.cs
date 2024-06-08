using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void FireTrap(NwGameObject trap, TrapEntry entry)
    {
      foreach (NwCreature creature in trap.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, entry.aoeSize, false))
      {
        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));

        SpellConfig.SavingThrowFeedback feedback = new();
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(creature, Ability.Dexterity, null, SpellConfig.SpellEffectType.Trap, trap);

        if (advantage < 900)
        {
          creature.LoginPlayer?.SendServerMessage($"Jet de dextérité contre les pièges : {"ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red)}".ColorString(ColorConstants.Orange));

          creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
          NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice), entry.damageType)));
          return;
        }

        int totalSave = SpellUtils.GetSavingThrowRoll(creature, Ability.Dexterity, entry.baseDC, advantage, feedback);
        bool saveFailed = totalSave < entry.baseDC; // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège
        int damage = NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice); // TODO : Variabiliser les dégâts selon la compétence de l'artisan

        LogUtils.LogMessage($"Dégâts initiaux : {damage}", LogUtils.LogType.Combat);

        damage = SpellUtils.HandleSpellEvasion(creature, damage, Ability.Dexterity, saveFailed);
        damage = ItemUtils.GetShieldMasterReducedDamage(creature, damage, saveFailed);
        damage = TrapUtils.GetKeenSenseDamageReduction(creature, damage);

        if (!saveFailed)
          damage /= 2;

        TrapUtils.SendSavingThrowFeedbackMessage(creature, feedback.saveRoll, feedback.proficiencyBonus, advantage, entry.baseDC, totalSave, saveFailed, Ability.Dexterity);

        NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, entry.damageType)));

        LogUtils.LogMessage($"Dégâts finaux : {damage}", LogUtils.LogType.Combat);
        LogUtils.LogMessage($"------------------------------------------", LogUtils.LogType.Combat);
      }
    }
  }
}
