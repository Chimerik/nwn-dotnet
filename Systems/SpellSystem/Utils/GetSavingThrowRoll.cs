using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int GetSavingThrowRoll(NwCreature target, Ability ability, int saveDC, int advantage, SpellConfig.SavingThrowFeedback feedback, bool fromSpell = false)
    {
      int proficiencyBonus = GetSavingThrowProficiencyBonus(target, ability);

      LogUtils.LogMessage($"JDS proficiency bonus {ability} : {proficiencyBonus}", LogUtils.LogType.Combat);
      LogUtils.LogMessage($"JDS modifier {ability} : {target.GetAbilityModifier(ability)}", LogUtils.LogType.Combat);

      proficiencyBonus += target.GetAbilityModifier(ability) + ItemUtils.GetShieldMasterBonusSave(target, ability);

      if (target.ActiveEffects.Any(e => e.Tag == EffectSystem.SensDeLaMagieEffectTag))
        foreach(var eff in target.ActiveEffects)
        {
          switch(eff.Tag) 
          {
            case EffectSystem.SensDeLaMagieEffectTag: if(fromSpell) proficiencyBonus += NativeUtils.GetCreatureProficiencyBonus(target); break;
            case EffectSystem.WildMagicBienfaitEffectTag: proficiencyBonus += NwRandom.Roll(Utils.random, 4); break;
          }
        }

      int saveRoll = NativeUtils.HandlePresage(target); // Si présage, alors on remplace totalement le jet de la cible

      if (saveRoll < 1)
      {
        saveRoll = Utils.RollAdvantage(advantage);
        saveRoll = NativeUtils.HandleChanceDebordante(target, saveRoll);
        saveRoll = NativeUtils.HandleHalflingLuck(target, saveRoll);

        if (ability == Ability.Strength)
          saveRoll = BarbarianUtils.HandleBarbarianPuissanceIndomptable(target, saveRoll);

        if (saveRoll + proficiencyBonus < saveDC)
          saveRoll = FighterUtils.HandleInflexible(target, saveRoll);

        if (saveRoll + proficiencyBonus < saveDC)
          saveRoll = MonkUtils.HandleDiamondSoul(target, saveRoll);

        if (saveRoll + proficiencyBonus < saveDC)
        {
          foreach(var eff in target.ActiveEffects)
            if(eff.Tag == EffectSystem.InspirationBardiqueEffectTag)
            {
              saveRoll += eff.CasterLevel;

              LogUtils.LogMessage($"Activation inspiration bardique : +{eff.CasterLevel}", LogUtils.LogType.Combat);
              StringUtils.DisplayStringToAllPlayersNearTarget(target, $"Inspiration Bardique (+{StringUtils.ToWhitecolor(eff.CasterLevel)})".ColorString(StringUtils.gold), StringUtils.gold, true, true);
              target.RemoveEffect(eff);
              break;
            }
        }
      }

      feedback.proficiencyBonus = proficiencyBonus;
      feedback.saveRoll = saveRoll;

      return saveRoll + proficiencyBonus;
    }
  }
}
