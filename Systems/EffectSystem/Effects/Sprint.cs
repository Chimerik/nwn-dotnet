using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SprintEffectTag = "_EFFECT_SPRINT";
    public const string SprintMobileEffectTag = "_EFFECT_SPRINT_MOBILE";
    public static Effect Sprint(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(caster.KnowsFeat((Feat)CustomSkill.Chargeur) ? 65 : 50), Effect.Icon(NwGameTables.EffectIconTable.GetRow(142)));

      eff.Tag = SprintEffectTag;

      if (caster.KnowsFeat((Feat)CustomSkill.Mobile))
      {
        eff = Effect.LinkEffects(eff, Effect.Immunity(ImmunityType.Entangle), Effect.Immunity(ImmunityType.Slow));
        eff.Tag = SprintMobileEffectTag;
      }

      eff.SubType = EffectSubType.Supernatural;

      ApplyAttaqueMobile(caster);

      return eff;
    }
  }
}
