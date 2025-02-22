using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AmeliorationCaracteristiqueEffectTag = "_AMELIORATION_CARACTERISTIQUE_EFFECT";
    public static Effect AmeliorationCaracteristique(NwGameObject caster, NwSpell spell)
    {
      var icon = spell.Id switch
      {
        CustomSpell.AmeliorationForce => CustomEffectIcon.ForceduTaureau,
        CustomSpell.AmeliorationDexterite => CustomEffectIcon.GraceFeline,
        CustomSpell.AmeliorationIntelligence => CustomEffectIcon.RuseduRenard,
        CustomSpell.AmeliorationCharisme => CustomEffectIcon.SplendeurdelAigle,
        CustomSpell.AmeliorationSagesse => CustomEffectIcon.SagesseduHibou,
        _ => CustomEffectIcon.EndurancedelOurs,
      };

      Effect eff = Effect.Icon(icon);   
      eff.Tag = AmeliorationCaracteristiqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Spell = spell;

      return eff;
    }
  }
}
