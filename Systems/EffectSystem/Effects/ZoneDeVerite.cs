using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ZoneDeVeriteEffectTag = "_ZONE_DE_VERITE_EFFECT";
    private static ScriptCallbackHandle onEnterZoneDeVeriteCallback;
    public static Effect ZoneDeVerite(NwGameObject caster)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.ZoneDeVerite, onEnterZoneDeVeriteCallback);
      eff.Tag = ZoneDeVeriteEffectTag;
      eff.Creator = caster;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterZoneDeVerite(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.ZoneDeVerite);
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);
      CreatureUtils.GetSavingThrow(protector, entering, spellEntry.savingThrowAbility, spellDC, spellEntry);

      return ScriptHandleResult.Handled;
    }
  }
}
