using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API;
using System.Linq;
using NWN.API.Constants;
using Discord;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    [ScriptHandler("a_spellbroadcast")]
    private void HandleAfterSpellBroadcast(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;

      if (oPC.IsDM || oPC.IsDMPossessed || oPC.IsPlayerDM)
        return;

      if (!oPC.ActiveEffects.Where(e => e.EffectType == API.Constants.EffectType.Invisibility || e.EffectType == API.Constants.EffectType.ImprovedInvisibility).Any())
        return;

      if (int.Parse(EventsPlugin.GetEventData("META_TYPE")) == NWScript.METAMAGIC_SILENT)
        return;

      foreach (NwPlayer spotter in oPC.Area.FindObjectsOfTypeInArea<NwPlayer>().Where(p => p.Distance(oPC) < 20.0f))
      {
        if (NWScript.GetObjectSeen(oPC, spotter) != 1)
        {
          spotter.SendServerMessage("Quelqu'un d'invisible est en train de lancer un sort à proximité !");
          PlayerPlugin.ShowVisualEffect(spotter, 191, NWScript.GetPosition(oPC));
        }
      }
    }
    [ScriptHandler("spellhook")]
    private void HandleSpellHook(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer player = (NwPlayer)callInfo.ObjectSelf;

      player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_REFLEX").Value = player.GetBaseSavingThrow(SavingThrow.Reflex);
      player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_WILL").Value = player.GetBaseSavingThrow(SavingThrow.Will);
      player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_FORT").Value = player.GetBaseSavingThrow(SavingThrow.Fortitude);

      if (int.TryParse(NWScript.Get2DAString("feat", "GAINMULTIPLE", CreaturePlugin.GetHighestLevelOfFeat(player, (int)Feat.ImprovedCasterLevel)), out int casterLevel))
        CreaturePlugin.SetLevelByPosition(player, 0, casterLevel + 1);

      int spellId = NWScript.GetSpellId();
      int classe = 43; // aventurier

      if (player.GetAbilityScore(Ability.Charisma) > player.GetAbilityScore(Ability.Intelligence))
        classe = (int)ClassType.Sorcerer;

      if (int.TryParse(NWScript.Get2DAString("spells", "Cleric", spellId), out int value))
        classe = (int)ClassType.Cleric;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Druid", spellId), out value))
        classe = (int)ClassType.Druid;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Bard", spellId), out value))
        classe = (int)ClassType.Bard;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Paladin", spellId), out value))
        classe = (int)ClassType.Paladin;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Ranger", spellId), out value))
        classe = (int)ClassType.Ranger;

      CreaturePlugin.SetClassByPosition(player, 0, classe);
      NWScript.DelayCommand(0.0f, () => DelayedSpellHook(player));
    }
    private void DelayedSpellHook(NwPlayer player)
    {
      CreaturePlugin.SetLevelByPosition(player, 0, 1);
      CreaturePlugin.SetClassByPosition(player, 0, 43);
      CreaturePlugin.SetBaseSavingThrow(player, NWScript.SAVING_THROW_REFLEX, player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_REFLEX").Value);
      CreaturePlugin.SetBaseSavingThrow(player, NWScript.SAVING_THROW_WILL, player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_WILL").Value);
      CreaturePlugin.SetBaseSavingThrow(player, NWScript.SAVING_THROW_FORT, player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_FORT").Value);
    }
    private void RestoreSpell(uint caster, int spellId)
    {
      for (int i = 0; i < CreaturePlugin.GetMemorisedSpellCountByLevel(caster, 43, 0); i++)
      {
        MemorisedSpell spell = CreaturePlugin.GetMemorisedSpell(caster, 43, 0, i);
        if (spell.id == spellId)
        {
          spell.ready = 1;
          CreaturePlugin.SetMemorisedSpell(caster, 43, 0, i, spell);
          break;
        }
      }
    }
    [ScriptHandler("b_spellcast")]
    private void HandleBeforeSpellCast(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;

      var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));

      if (int.TryParse(NWScript.Get2DAString("spells", "Bard", spellId), out int classe))
        classe = NWScript.CLASS_TYPE_BARD;

      if (int.TryParse(NWScript.Get2DAString("spells", "Ranger", spellId), out classe))
        classe = NWScript.CLASS_TYPE_RANGER;

      if (int.TryParse(NWScript.Get2DAString("spells", "Paladin", spellId), out classe))
        classe = NWScript.CLASS_TYPE_PALADIN;

      if (int.TryParse(NWScript.Get2DAString("spells", "Druid", spellId), out classe))
        classe = NWScript.CLASS_TYPE_DRUID;

      if (int.TryParse(NWScript.Get2DAString("spells", "Cleric", spellId), out classe))
        classe = NWScript.CLASS_TYPE_CLERIC;

      CreaturePlugin.SetClassByPosition(oPC, 0, classe);
    }
    [ScriptHandler("a_spellcast")]
    private void HandleAfterSpellCast(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;

      var spellId = int.Parse(EventsPlugin.GetEventData("SPELL_ID"));
      CreaturePlugin.SetClassByPosition(oPC, 0, 43); // 43 = aventurier

      if (NWScript.Get2DAString("spells", "School", spellId) == "D" && oPC.GetItemInSlot(InventorySlot.Neck).Tag != "amulettorillink")
      {
        (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync(
          $"{oPC.Name} " +
          $"vient de lancer un sort de divination ({NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "Name", spellId)))})" +
          $" en portant l'amulette de traçage. L'Amiral s'apprête à punir l'impudent !");
      }
    }
  }
}
