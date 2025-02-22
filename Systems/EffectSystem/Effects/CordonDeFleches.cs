using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CordonDeFlechesEffectTag = "_CORDON_DE_FLCHES_EFFECT";
    private static ScriptCallbackHandle onEnterCordonDeFlechesCallback;
    public static Effect CordonDeFleches(NwGameObject caster)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterCordonDeFlechesCallback);
      eff.Tag = CordonDeFlechesEffectTag;
      eff.Creator = caster;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterCordonDeFleches(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector || !protector.IsReactionTypeHostile(entering))
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.CordonDeFleches);
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);
      var saveResult = CreatureUtils.GetSavingThrow(protector, entering, spellEntry.savingThrowAbility, spellDC, spellEntry);
      eventData.Effect.GetObjectVariable<LocalVariableInt>("_MUNITIONS").Value -= 1;

      if (saveResult == SavingThrowResult.Failure)
      {
        SpellUtils.DealSpellDamage(entering, protector.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(protector, spell), protector, spell.InnateSpellLevel, saveResult);
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitSonic));
      }

      if (eventData.Effect.GetObjectVariable<LocalVariableInt>("_MUNITIONS").Value < 1)
        eventData.Effect.Destroy();

      return ScriptHandleResult.Handled;
    }
  }
}
