using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SphereDeFeuEffectTag = "_SPHERE_DE_FEU_EFFECT";
    private static ScriptCallbackHandle onEnterSphereDeFeuCallback;
    private static ScriptCallbackHandle onHeartbeatSphereDeFeuCallback;
    public static Effect SphereDeFeu(NwGameObject caster, Ability castingAbility)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.SphereDeFeu, onEnterSphereDeFeuCallback, onHeartbeatSphereDeFeuCallback);
      eff.Tag = SphereDeFeuEffectTag;
      eff.Creator = caster;
      eff.IntParams[5] = (int)castingAbility;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterSphereDeFeu(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.SphereDeFeu).MasterSpell;
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      int spellDC = SpellUtils.GetCasterSpellDC(protector, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);
      HandleSphereDeFeuEffect(protector, entering, spellEntry, spell, spellDC);

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onHeartbeatSphereDeFeu(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData)
        && eventData.Effect.Creator is NwCreature caster)
      {
        NwSpell spell = NwSpell.FromSpellId(CustomSpell.SphereDeFeu).MasterSpell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

        int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, (Ability)eventData.Effect.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value);

        foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
          HandleSphereDeFeuEffect(caster, target, spellEntry, spell, spellDC);
      }

      return ScriptHandleResult.Handled;
    }

    private static void HandleSphereDeFeuEffect(NwCreature caster, NwCreature target, SpellEntry spellEntry, NwSpell spell, int spellDC)
    {
      SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spell.InnateSpellLevel,
      CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));

      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFlameM));
    }
  }
}
