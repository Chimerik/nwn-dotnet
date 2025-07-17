using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaledictionEffroiEffectTag = "_MALEDICTION_EFFROI_EFFECT";
    private static ScriptCallbackHandle onIntervalMaledictionEffroiCallback;
    public static Effect GetMaledictionEffroi(Ability spellCastingAbility)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Curse), Effect.VisualEffect(VfxType.DurMindAffectingFear), Effect.Pacified(),
        Effect.Silence(),
        Effect.RunAction(onIntervalHandle: onIntervalMaledictionEffroiCallback, interval:NwTimeSpan.FromRounds(1), data:((int)spellCastingAbility).ToString()));
      eff.Tag = MaledictionAttaqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onIntervalMaledictionEffroi(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      if (eventData.Effect.Creator is not NwCreature creator)
      {
        creature.RemoveEffect(eventData.Effect);
        return ScriptHandleResult.Handled;
      }

      SpellEntry spellEntry = Spells2da.spellTable[CustomSpell.MaledictionEffroi];
      int spellDC = SpellUtils.GetCasterSpellDC(creator, NwSpell.FromSpellId(CustomSpell.MaledictionEffroi), (Ability)int.Parse(eventData.Effect.StringParams[0]));

      if (CreatureUtils.GetSavingThrowResult(creature, spellEntry.savingThrowAbility, creator, spellDC, spellEntry, SpellConfig.SpellEffectType.Fear) != SavingThrowResult.Failure)
        EffectUtils.RemoveTaggedEffect(creature, creator, MaledictionEffroiEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
