using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void ElecTrap(NwGameObject trap, TrapEntry entry)
    {
      int targetHit = 0;

      foreach (NwCreature creature in trap.GetNearestCreatures(CreatureTypeFilter.Alive(true)))
      {
        ModuleSystem.Log.Info($"target : {creature.Name}");

        if (trap.DistanceSquared(creature) > 25 || targetHit > entry.aoeSize)
          return;

        targetHit++;

        Dictionary<string, bool> disadvantageDictionary = new()
        {
          { EffectSystem.ShieldArmorDisadvantageEffectTag, false } ,
        };

        Dictionary<string, bool> advantageDictionary = new()
        {
          { EffectSystem.DodgeEffectTag, false },
        };

        foreach (var eff in creature.ActiveEffects)
        {
          if (EffectUtils.IsIncapacitatingEffect(eff))
          {
            creature.LoginPlayer?.SendServerMessage($"Jet de dextérité contre les pièges : {"ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red)}".ColorString(ColorConstants.Orange));

            creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
            NWScript.AssignCommand(trap, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice), entry.damageType)));
            return;
          }

          advantageDictionary[EffectSystem.DodgeEffectTag] = advantageDictionary[EffectSystem.DodgeEffectTag] || EffectSystem.DodgeEffectTag == eff.Tag;
          disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] = disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] || EffectSystem.ShieldArmorDisadvantageEffectTag == eff.Tag;
        }

        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));

        SpellConfig.SavingThrowFeedback feedback = new();
        int advantage = -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value) + creature.KnowsFeat(Feat.KeenSense).ToInt();

        if (creature.Classes.Any(c => c.Class.ClassType == ClassType.Barbarian && c.Level > 1) && !creature.ActiveEffects.Any(e => e.EffectType == EffectType.Blindness || e.EffectType == EffectType.Deaf))
          advantage += 1;

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
