using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FrappeRuseeEffectTag = "_FRAPPE_RUSEE_EFFECT";
    public static readonly Native.API.CExoString FrappeRuseeEffectExoTag = FrappeRuseeEffectTag.ToExoString();
    public const string FrappeRuseeVariable = "_FRAPPE_RUSEE_VARIABLE";
    public static readonly Native.API.CExoString FrappeRuseeVariableExo = FrappeRuseeVariable.ToExoString();
    public static void ApplyFrappeRusee(NwCreature caster, NwSpell spell)
    {
      if(caster.ActiveEffects.Any(e => e.Tag == FrappeRuseeEffectTag && e.Spell == spell))
      {
        caster.LoginPlayer?.SendServerMessage($"{spell.Name.ToString().ColorString(ColorConstants.White)} - Désactivé", ColorConstants.Orange);
        EffectUtils.RemoveTaggedSpellEffect(caster, FrappeRuseeEffectTag, spell);
        return;
      }

      if(caster.KnowsFeat((Feat)CustomSkill.RoublardFrappeRuseeAmelioree))
      {
        if(caster.ActiveEffects.Count(e => e.Tag == FrappeRuseeEffectTag) > 1)
        {
          var previousEff = caster.ActiveEffects.First(e => e.Tag == FrappeRuseeEffectTag);
          NwSpell previousSpell = previousEff.Spell;
          EffectUtils.RemoveTaggedSpellEffect(caster, FrappeRuseeEffectTag, previousSpell);
          caster.LoginPlayer?.SendServerMessage($"{previousSpell.Name.ToString().ColorString(ColorConstants.White)} - Désactivé", ColorConstants.Orange);
        }
      }
      else
        EffectUtils.RemoveTaggedEffect(caster, FrappeRuseeEffectTag);

      var icon = spell.Id switch
      {
        CustomSpell.FrappeRuseeBousculade or CustomSpell.FrappePerfideBousculade => CustomEffectIcon.FrappeRuseeBousculade,
        CustomSpell.FrappeRuseeRetraite or CustomSpell.FrappePerfideRetraite => CustomEffectIcon.FrappeRuseeRetraite,
        CustomSpell.FrappePerfideHebeter => CustomEffectIcon.FrappePerfideHebeter,
        CustomSpell.FrappePerfideObscurcir => CustomEffectIcon.FrappePerfideObscurcir,
        CustomSpell.FrappePerfideAssommer => CustomEffectIcon.FrappePerfideAssommer,
        _ => CustomEffectIcon.FrappeRuseePoison,
      };

      Effect eff = Effect.Icon(icon);
      eff.Tag = FrappeRuseeEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;
      eff.Spell = spell;

      caster.ApplyEffect(EffectDuration.Permanent, eff);
      caster.OnCreatureDamage -= RogueUtils.OnDamageFrappeRusee;
      caster.OnCreatureDamage += RogueUtils.OnDamageFrappeRusee;

      caster.LoginPlayer?.SendServerMessage($"{spell.Name.ToString().ColorString(ColorConstants.White)} - Activé", ColorConstants.Orange);
    }
  }
}

