using System;
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

        int advantage = -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value) + creature.KnowsFeat(Feat.KeenSense).ToInt();
        int proficiencyBonus = SpellUtils.GetSavingThrowProficiencyBonus(creature, Ability.Dexterity);
        int saveRoll = NativeUtils.HandleHalflingLuck(creature, Utils.RollAdvantage(advantage));
        int totalSave = saveRoll + proficiencyBonus;
        bool saveFailed = totalSave < entry.baseDC; // TODO : Variabiliser le DD selon la compétence de celui qui a posé le piège
        int damage = NwRandom.Roll(Utils.random, entry.damageDice, entry.numDice); // TODO : Variabiliser les dégâts selon la compétence de l'artisan

        creature.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(entry.damageVFX));
        if (!saveFailed)
          damage /= 2;

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
