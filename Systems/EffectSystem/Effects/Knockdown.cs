using System.Linq;
using Anvil.API;

using Ability = Anvil.API.Ability;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string KnockdownEffectTag = "_KNOCKDOWN_EFFECT";
    public static readonly Native.API.CExoString KnockdownEffectTagExo = KnockdownEffectTag.ToExoString();
    public static Effect Destabilisation
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingDisabled), Effect.MovementSpeedDecrease(50), Effect.Icon(CustomEffectIcon.Destabilise));
        eff.Tag = KnockdownEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect Knockdown
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Knockdown(), Effect.Icon(CustomEffectIcon.Destabilise));
        eff.Tag = KnockdownEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    /*public static void ApplyKnockdown(NwCreature caster, NwCreature target)
    {
      CreatureSize targetSize = target.ActiveEffects.Any(e => e.Tag == EnlargeEffectTag) ? target.Size + 1 : target.Size;

      if (targetSize > CreatureSize.Large)
      {
        caster.LoginPlayer?.SendServerMessage("La cible est de trop grande taille pour être renversée", ColorConstants.Red);
        return;
      }

      bool saveFailed = CreatureUtils.GetSkillDuelResult(caster, target, new List<Ability>() { Ability.Strength },
      new List<Ability>() { Ability.Strength, Ability.Dexterity }, new List<int>() { CustomSkill.AthleticsProficiency },
      new List<int>() { CustomSkill.AthleticsProficiency, CustomSkill.AcrobaticsProficiency }, SpellConfig.SpellEffectType.Knockdown);

      if (saveFailed)
      {
        target.ApplyEffect(EffectDuration.Temporary, knockdown,
        target.KnowsFeat((Feat)CustomSkill.Sportif) ? NwTimeSpan.FromRounds(1) : NwTimeSpan.FromRounds(2));
      }
    }*/
    public static void ApplyKnockdown(NwCreature target, NwCreature attacker, Ability DCAbility, Ability SaveAbility, Effect type, bool saveFailed = false)
    {
      if (IsKnockdownImmune(target, attacker))
        return;

      int spellDC = SpellUtils.GetCasterSpellDC(attacker, DCAbility);

      if (saveFailed || CreatureUtils.GetSavingThrow(attacker, target, SaveAbility, spellDC, effectType: SpellConfig.SpellEffectType.Knockdown) == SavingThrowResult.Failure)
      {
        target.ApplyEffect(EffectDuration.Temporary, type,
        target.KnowsFeat((Feat)CustomSkill.Sportif) ? NwTimeSpan.FromRounds(1) : NwTimeSpan.FromRounds(2));
        SpellUtils.DispelConcentrationEffects(target);
      }
    }
    public static void ApplyKnockdown(NwCreature target, NwCreature attacker, int skillCheck, Ability saveAbility, int saveDC)
    {
      if (IsKnockdownImmune(target, attacker))
        return;

      if (CreatureUtils.HandleSkillCheck(target, skillCheck, saveAbility, saveDC))
      {
        target.ApplyEffect(EffectDuration.Temporary, Knockdown,
        target.KnowsFeat((Feat)CustomSkill.Sportif) ? NwTimeSpan.FromRounds(1) : NwTimeSpan.FromRounds(2));
        SpellUtils.DispelConcentrationEffects(target);
      }
    }
    public static void ApplyKnockdown(NwCreature target, NwCreature attacker)
    {
      if (IsKnockdownImmune(target, attacker))
        return;

      target.ApplyEffect(EffectDuration.Temporary, Destabilisation, target.KnowsFeat((Feat)CustomSkill.Sportif) ? NwTimeSpan.FromRounds(1) : NwTimeSpan.FromRounds(2));
      SpellUtils.DispelConcentrationEffects(target);
    }

    public static bool IsKnockdownImmune(NwCreature target, NwCreature attacker)
    {
      CreatureSize targetSize = target.ActiveEffects.Any(e => e.Tag == EnlargeEffectTag) ? target.Size + 1 : target.Size;
      CreatureSize attackerSize = attacker.ActiveEffects.Any(e => e.Tag == EnlargeEffectTag) ? attacker.Size + 1 : attacker.Size;

      if (targetSize > attackerSize + 1 || target.ActiveEffects.Any(e => e.EffectType == EffectType.Immunity && e.IntParams[1] == 28))
      {
        attacker.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} ne peut pas être déstabilisé");
        return true;
      }

      return false;
    }
  }
}
