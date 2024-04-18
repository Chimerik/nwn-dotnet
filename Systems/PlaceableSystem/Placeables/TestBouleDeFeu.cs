using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlaceableSystem
  {
    public static void TestBouleDeFeu(PlaceableEvents.OnLeftClick onUsed)
    {
      NwSpell burningHands = NwSpell.FromSpellType(Spell.BurningHands);
      var spellEntry = Spells2da.spellTable[burningHands.Id];

      SpellUtils.SignalEventSpellCast(onUsed.ClickedBy.ControlledCreature, onUsed.Placeable, burningHands.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = 12;

      foreach (NwCreature target in onUsed.ClickedBy.ControlledCreature.Location.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, onUsed.ClickedBy.ControlledCreature.Location.Position))
      {
        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Dexterity, spellEntry, SpellConfig.SpellEffectType.Invalid);

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Dexterity, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
        string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
        Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

        string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(Ability.Dexterity)}{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(spellDC, hitColor)}";

        target.LoginPlayer?.SendServerMessage($"{onUsed.Placeable.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));

        LogUtils.LogMessage($"{target.Name} - {advantageString}{rollString} {hitString}".StripColors(), LogUtils.LogType.Combat);

        int damage = 0;

        for (int i = 0; i < spellEntry.numDice; i++)
          damage += NwRandom.Roll(Utils.random, spellEntry.damageDice);

        if (target is NwCreature targetCreature)
        {
          damage = SpellUtils.HandleSpellEvasion(targetCreature, damage, spellEntry.savingThrowAbility, saveFailed);
          damage = ItemUtils.GetShieldMasterReducedDamage(targetCreature, damage, saveFailed, spellEntry.savingThrowAbility);
          damage = WizardUtils.GetAbjurationReducedDamage(targetCreature, damage);
        }

        NWScript.AssignCommand(onUsed.Placeable, () => target.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(spellEntry.damageVFX), Effect.Damage(damage, spellEntry.damageType))));
        LogUtils.LogMessage($"Dégâts sur {target.Name} : {spellEntry.numDice}d{spellEntry.damageDice} (caster lvl {6}) = {damage} {spellEntry.damageType}", LogUtils.LogType.Combat);

      }
    }
  }
}
