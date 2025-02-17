
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AmeliorationCaracteristiqueEffectTag = "_AMELIORATION_CARACTERISTIQUE_EFFECT";
    public static Effect AmeliorationCaracteristique(NwGameObject caster, SpellEntry spell)
    {
      var icon = spell.savingThrowAbility switch
      {
        Ability.Strength => CustomEffectIcon.ForceduTaureau,
        Ability.Dexterity => CustomEffectIcon.GraceFeline,
        Ability.Intelligence => CustomEffectIcon.RuseduRenard,
        Ability.Wisdom => CustomEffectIcon.SplendeurdelAigle,
        Ability.Charisma => CustomEffectIcon.ForceduTaureau,
        _ => CustomEffectIcon.EndurancedelOurs,
      };

      Effect eff = Effect.Icon(icon);   
      eff.Tag = AmeliorationCaracteristiqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.IntParams[5] = (int)spell.savingThrowAbility;
      eff.Spell = NwSpell.FromSpellId(spell.RowIndex);

      return eff;
    }
  }
}
