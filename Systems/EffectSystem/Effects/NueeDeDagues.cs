using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NueeDeDaguesEffectTag = "_NUEE_DE_DAGUES_EFFECT";
    private static ScriptCallbackHandle onEnterNueeDeDaguesCallback;
    private static ScriptCallbackHandle onHeartbeatNueeDeDaguesCallback;
    public static Effect NueeDeDagues(NwGameObject caster, NwSpell spell)
    {
      Effect eff = Effect.AreaOfEffect(CustomAoE.NueeDeDagues, onEnterNueeDeDaguesCallback, onHeartbeatNueeDeDaguesCallback);
      eff.Tag = NueeDeDaguesEffectTag;
      eff.Creator = caster;
      eff.Spell = spell;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterNueeDeDagues(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      ModuleSystem.Log.Info($"entering : {entering.Name}");

      NwSpell spell = NwSpell.FromSpellId(CustomSpell.NueeDeDagues).MasterSpell;
      SpellEntry spellEntry = Spells2da.spellTable[spell.Id];

      HandleNueeDeDaguesEffect(protector, entering, spellEntry, spell);

      return ScriptHandleResult.Handled;
    }

    private static ScriptHandleResult onHeartbeatNueeDeDagues(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData)
        && eventData.Effect.Creator is NwCreature caster)
      {
        NwSpell spell = NwSpell.FromSpellId(CustomSpell.NueeDeDagues).MasterSpell;
        SpellEntry spellEntry = Spells2da.spellTable[spell.Id];
        
        foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
        {
          HandleNueeDeDaguesEffect(caster, target, spellEntry, spell);
        }
      }

      return ScriptHandleResult.Handled;
    }

    private static void HandleNueeDeDaguesEffect(NwCreature caster, NwCreature target, SpellEntry spellEntry, NwSpell spell)
    {
      SpellUtils.DealSpellDamage(target, caster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(caster, spell), caster, spell.InnateSpellLevel);
      target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComBloodCrtRed));
    }
  }
}
