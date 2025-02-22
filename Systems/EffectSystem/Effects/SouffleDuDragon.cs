using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SouffleDuDragonEffectTag = "_SOUFFLE_DU_DRAGON_EFFECT";
    private static ScriptCallbackHandle onRemoveSouffleDuDragonCallback;
    public static void ApplySouffleDuDragon(NwCreature caster, NwCreature target, NwSpell spell, SpellEntry spellEntry, Ability castAbility)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.SouffleDuDragon), Effect.RunAction(onRemovedHandle: onRemoveSouffleDuDragonCallback));
      eff.Tag = SouffleDuDragonEffectTag;
      eff.Creator = caster;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      eff.CasterLevel = SpellUtils.GetCasterSpellDC(caster, spell, castAbility);

      var breathFeat = spell.Id switch
      {
        CustomSkill.SouffleDuDragonAcide => (Feat)CustomSkill.SouffleDuDragonAcide,
        CustomSkill.SouffleDuDragonFroid => (Feat)CustomSkill.SouffleDuDragonFroid,
        CustomSkill.SouffleDuDragonElec => (Feat)CustomSkill.SouffleDuDragonElec,
        CustomSkill.SouffleDuDragonPoison => (Feat)CustomSkill.SouffleDuDragonPoison,
        _ => (Feat)CustomSkill.SouffleDuDragonFeu,
      };

      target.AddFeat(breathFeat);
      target.SetFeatRemainingUses(breathFeat, 100);
    }
    private static ScriptHandleResult OnRemoveSouffleDuDragon(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.RemoveFeat((Feat)CustomSkill.SouffleDuDragonAcide);
        creature.RemoveFeat((Feat)CustomSkill.SouffleDuDragonFroid);
        creature.RemoveFeat((Feat)CustomSkill.SouffleDuDragonFeu);
        creature.RemoveFeat((Feat)CustomSkill.SouffleDuDragonElec);
        creature.RemoveFeat((Feat)CustomSkill.SouffleDuDragonPoison);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
