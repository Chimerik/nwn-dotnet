using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API;
using System.Linq;
using NWN.API.Constants;
using Discord;
using NLog;
using NWN.API.Events;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace NWN.Systems
{
  [ServiceBinding(typeof(SpellSystem))]
  public partial class SpellSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static int[] lowEnchantements = new int[] { 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554 };
    public static int[] mediumEnchantements = new int[] { 555, 556, 557, 558, 559, 560, 561, 562 };
    public static int[] highEnchantements = new int[] { 563, 564, 565, 566, 567, 568 };

    public static void RegisterMetaMagicOnSpellInput(OnSpellAction onSpellAction)
    {
      if (onSpellAction.MetaMagic == MetaMagic.Silent)
        onSpellAction.Caster.GetLocalVariable<int>("_IS_SILENT_SPELL").Value = 1;
    }
    public static void OnSpellBroadcast(OnSpellBroadcast onSpellBroadcast)
    {
      NwPlayer oPC = (NwPlayer)onSpellBroadcast.Caster;

      int classe = 43; // aventurier

      if (oPC.GetAbilityScore(Ability.Charisma) > oPC.GetAbilityScore(Ability.Intelligence))
        classe = (int)ClassType.Sorcerer;
      if (int.TryParse(NWScript.Get2DAString("spells", "Cleric", (int)onSpellBroadcast.Spell), out int value))
        classe = (int)ClassType.Cleric;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Druid", (int)onSpellBroadcast.Spell), out value))
        classe = (int)ClassType.Druid;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Bard", (int)onSpellBroadcast.Spell), out value))
        classe = (int)ClassType.Bard;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Paladin", (int)onSpellBroadcast.Spell), out value))
        classe = (int)ClassType.Paladin;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Ranger", (int)onSpellBroadcast.Spell), out value))
        classe = (int)ClassType.Ranger;

      if (classe != 43)
      {
        Task resetClassOnNextFrame = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.7));
          
          CreaturePlugin.SetClassByPosition(oPC, 0, classe);
          CancellationTokenSource tokenSource = new CancellationTokenSource();

          Task spellCast = NwTask.WaitUntil(() => oPC.GetLocalVariable<int>("_SPELLCAST").HasValue, tokenSource.Token);
          Task timeOut = NwTask.Delay(TimeSpan.FromSeconds(0.1), tokenSource.Token);

          await NwTask.WhenAny(spellCast, timeOut);
          tokenSource.Cancel();

          CreaturePlugin.SetClassByPosition(oPC, 0, 43);
          oPC.GetLocalVariable<int>("_SPELLCAST").Delete();
        });
      }

      if (oPC.IsDM || oPC.IsDMPossessed || oPC.IsPlayerDM)
        return;
      
      if (!oPC.ActiveEffects.Any(e => e.EffectType == EffectType.Invisibility || e.EffectType == EffectType.ImprovedInvisibility))
        return;

      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();

      if (onSpellBroadcast.Caster.GetLocalVariable<int>("_IS_SILENT_SPELL").HasValue)
      {
        onSpellBroadcast.Caster.GetLocalVariable<int>("_IS_SILENT_SPELL").Delete();
        return;
      }

      foreach (NwPlayer spotter in oPC.Area.FindObjectsOfTypeInArea<NwPlayer>().Where(p => p.Distance(oPC) < 20.0f))
      {
        if (NWScript.GetObjectSeen(oPC, spotter) != 1)
        {
          spotter.SendServerMessage("Quelqu'un d'invisible est en train de lancer un sort à proximité !", API.Color.CYAN);
          PlayerPlugin.ShowVisualEffect(spotter, 191, oPC.Position);
        }
      }
    }
    [ScriptHandler("spellhook")]
    private void HandleSpellHook(CallInfo callInfo)
    {
      if (!(callInfo.ObjectSelf is NwPlayer))
        return;

      NwPlayer oPC = (NwPlayer)callInfo.ObjectSelf;

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      SpellEvents.OnSpellCast onSpellCast = new SpellEvents.OnSpellCast();

      CreaturePlugin.SetClassByPosition(oPC, 0, 43);

      oPC.GetLocalVariable<int>("_DELAYED_SPELLHOOK_REFLEX").Value = CreaturePlugin.GetBaseSavingThrow(oPC, NWScript.SAVING_THROW_REFLEX);
      oPC.GetLocalVariable<int>("_DELAYED_SPELLHOOK_WILL").Value = CreaturePlugin.GetBaseSavingThrow(oPC, NWScript.SAVING_THROW_WILL);
      oPC.GetLocalVariable<int>("_DELAYED_SPELLHOOK_FORT").Value = CreaturePlugin.GetBaseSavingThrow(oPC, NWScript.SAVING_THROW_FORT);

      if (player.learntCustomFeats.ContainsKey(CustomFeats.ImprovedCasterLevel))
        CreaturePlugin.SetLevelByPosition(oPC, 0, SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ImprovedCasterLevel, player.learntCustomFeats[CustomFeats.ImprovedCasterLevel]) + 1);

      int classe = 43; // aventurier

      if (oPC.GetAbilityScore(Ability.Charisma) > oPC.GetAbilityScore(Ability.Intelligence))
        classe = (int)ClassType.Sorcerer;
      if (int.TryParse(NWScript.Get2DAString("spells", "Cleric", (int)onSpellCast.Spell), out int value))
        classe = (int)ClassType.Cleric;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Druid", (int)onSpellCast.Spell), out value))
        classe = (int)ClassType.Druid;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Bard", (int)onSpellCast.Spell), out value))
        classe = (int)ClassType.Bard;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Paladin", (int)onSpellCast.Spell), out value))
        classe = (int)ClassType.Paladin;
      else if (int.TryParse(NWScript.Get2DAString("spells", "Ranger", (int)onSpellCast.Spell), out value))
        classe = (int)ClassType.Ranger;

      CreaturePlugin.SetClassByPosition(oPC, 0, classe);

      switch (onSpellCast.Spell)
      {
        case Spell.AcidSplash:
          new AcidSplash(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Daze:
          new Daze(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.ElectricJolt:
          new EletricJolt(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Flare:
          new Flare(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Light:
          new Light(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RayOfFrost:
          new RayOfFrost(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Resistance:
          new Resistance(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.Virtue:
          new Virtue(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;

        case Spell.RaiseDead:
        case Spell.Resurrection:
          new RaiseDead(onSpellCast);
          oPC.GetLocalVariable<int>("X2_L_BLOCK_LAST_SPELL").Value = 1;
          break;
      }

      NWScript.DelayCommand(0.0f, () => DelayedSpellHook(oPC));
    }
    private void DelayedSpellHook(NwPlayer player)
    {
      CreaturePlugin.SetLevelByPosition(player, 0, 1);
      CreaturePlugin.SetClassByPosition(player, 0, 43);
      CreaturePlugin.SetBaseSavingThrow(player, NWScript.SAVING_THROW_REFLEX, player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_REFLEX").Value);
      CreaturePlugin.SetBaseSavingThrow(player, NWScript.SAVING_THROW_WILL, player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_WILL").Value);
      CreaturePlugin.SetBaseSavingThrow(player, NWScript.SAVING_THROW_FORT, player.GetLocalVariable<int>("_DELAYED_SPELLHOOK_FORT").Value);
    }
    public static void RestoreSpell(NwCreature caster, Spell spell)
    {
      if (caster == null)
        Log.Info("caster is null");

      foreach (MemorizedSpellSlot spellSlot in caster.GetClassInfo((ClassType)43).GetMemorizedSpellSlots(0).Where(s => s.Spell == spell && !s.IsReady))
        spellSlot.IsReady = true;
    }
    public static void HandleBeforeSpellCast(OnSpellCast onSpellCast)
    {
      NwPlayer oPC = (NwPlayer)onSpellCast.Caster;

      if(oPC.Classes.Any(c => (int)c != 43))
        oPC.GetLocalVariable<int>("_SPELLCAST").Value = 1;

      if (onSpellCast.Caster.GetLocalVariable<int>("_AUTO_SPELL").HasValue && onSpellCast.Caster.GetLocalVariable<int>("_AUTO_SPELL").Value != (int)onSpellCast.Spell)
      {
        onSpellCast.Caster.GetLocalVariable<int>("_AUTO_SPELL").Delete();
        onSpellCast.Caster.GetLocalVariable<NwObject>("_AUTO_SPELL_TARGET").Delete();
        ((NwCreature)onSpellCast.Caster).OnCombatRoundEnd -= PlayerSystem.HandleCombatRoundEndForAutoSpells;
      }

      if (NWScript.Get2DAString("spells", "School", (int)onSpellCast.Spell) == "D" && ((NwCreature)onSpellCast.Caster).GetItemInSlot(InventorySlot.Neck).Tag != "amulettorillink")
      {
        (Bot._client.GetChannel(680072044364562532) as IMessageChannel).SendMessageAsync(
          $"{onSpellCast.Caster.Name} " +
          $"vient de lancer un sort de divination ({NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("spells", "Name", (int)onSpellCast.Spell)))})" +
          $" en portant l'amulette de traçage. L'Amiral s'apprête à punir l'impudent !");
      }
    }
    [ScriptHandler("invi_hb")]
    private void HandleInvisibiltyHeartBeat(CallInfo callInfo)
    {
      NwAreaOfEffect inviAoE = (NwAreaOfEffect)callInfo.ObjectSelf;
      NwPlayer oInvi = (NwPlayer)inviAoE.Creator;

      int iMoveSilentlyCheck = oInvi.GetSkillRank(Skill.MoveSilently) + NwRandom.Roll(Utils.random, 20);
      NwPlaceable invisMarker = NwObject.FindObjectsWithTag<NwPlaceable>($"invis_marker_{oInvi.PlayerName}").FirstOrDefault();
      bool listenTriggered = false;

      foreach (NwCreature oSpotter in inviAoE.GetObjectsInEffectArea<NwPlayer>().Where(p => NWScript.GetObjectSeen(oInvi, p) == 0 && (p.DetectModeActive || p.HasFeatEffect(Feat.KeenSense))))
      {
        int iListencheck = oSpotter.GetSkillRank(Skill.Listen) + NwRandom.Roll(Utils.random, 20) - (int)oInvi.Distance(oSpotter);

        if (NWScript.GetDetectMode(oSpotter) == 2)
          iListencheck -= 10;

        if (iMoveSilentlyCheck >= iListencheck)
        {
          if (invisMarker != null)
            VisibilityPlugin.SetVisibilityOverride(oSpotter, invisMarker, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
          continue;
        }

        if (invisMarker == null)
        {
          invisMarker = NwPlaceable.Create("silhouette", oInvi.Location, false, $"invis_marker_{oInvi.PlayerName}");
          VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, invisMarker, VisibilityPlugin.NWNX_VISIBILITY_HIDDEN);
          OnInvisMarkerPositionChanged(oInvi, invisMarker);
        }

        VisibilityPlugin.SetVisibilityOverride(oSpotter, invisMarker, VisibilityPlugin.NWNX_VISIBILITY_VISIBLE);
        listenTriggered = true;
      }

      if (!listenTriggered && invisMarker != null)
        invisMarker.Destroy();
    }
    private static async void OnInvisMarkerPositionChanged(NwPlayer oPC, NwPlaceable silhouette)
    {
      CancellationTokenSource tokenSource = new CancellationTokenSource();

      Task markerDestroyed = NwTask.WaitUntil(() => silhouette == null, tokenSource.Token);
      Task playerDisconnecting = NwTask.WaitUntil(() => !oPC.IsValid, tokenSource.Token);
      Task positionChanged = NwTask.WaitUntilValueChanged(() => oPC.Location.Position, tokenSource.Token);
      
      await NwTask.WhenAny(positionChanged, markerDestroyed, playerDisconnecting);
      tokenSource.Cancel();

      if (markerDestroyed.IsCompletedSuccessfully)
        return;

      if(playerDisconnecting.IsCompletedSuccessfully)
      {
        if(silhouette != null)
          silhouette.Destroy();
        return;
      }

      silhouette.Location = oPC.Location;
      OnInvisMarkerPositionChanged(oPC, silhouette);
    }
    [ScriptHandler("effect_applied")]
    private void HandleEffectApplied(CallInfo callInfo)
    {
      string customTag = EventsPlugin.GetEventData("CUSTOM_TAG");

      if (customTag == "")
      {
        if (!int.TryParse(EventsPlugin.GetEventData("TYPE"), out int effectType))
          return;

        switch (effectType)
        {
          case 47: // 47 = Invisibility

            API.Effect inviAoE = API.Effect.AreaOfEffect(193, null, "invi_hb"); // 193 = AoE 20 m
            inviAoE.Creator = callInfo.ObjectSelf;
            inviAoE.Tag = "invi_aoe";
            inviAoE.SubType = EffectSubType.Supernatural;
            ((NwGameObject)callInfo.ObjectSelf).ApplyEffect(EffectDuration.Permanent, inviAoE);

            break;
        }
        return;
      }
        
      switch (customTag)
      {
        case "CUSTOM_EFFECT_FROG":
          new Frog((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_POISON":
          new Poison((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_NOMAGIC":
          new NoMagic((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_NOHEALMAGIC":
          new NoHealMagic((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_NOSUMMON":
          new NoSummon((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_NOOFFENSIVEMAGIC":
          new NoOffensiveMagic((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_NOBUFF":
          new NoBuff((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_NOUSEABLEITEM":
          new NoUseableItem((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_SLOW":
          new Slow((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_MINI":
          new Mini((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_HALF_HEALTH":
          new HalfHealth((NwCreature)callInfo.ObjectSelf);
          break;
        case "CUSTOM_EFFECT_SPELL_FAILURE":
          new SpellFailure((NwCreature)callInfo.ObjectSelf);
          break;
      }
    }
    [ScriptHandler("effect_removed")]
    private void HandleEffectRemoved(CallInfo callInfo)
    {
      string customTag = EventsPlugin.GetEventData("CUSTOM_TAG");

      if (customTag == "")
      {
        if (!int.TryParse(EventsPlugin.GetEventData("TYPE"), out int effectType))
          return;

        switch (effectType)
        {
          case 47: // 47 = Invisibility

            foreach (API.Effect inviAoE in ((NwCreature)callInfo.ObjectSelf).ActiveEffects.Where(f => f.Tag == "invi_aoe"))
              ((NwGameObject)callInfo.ObjectSelf).RemoveEffect(inviAoE);

            break;
        }
        return;
      }

      switch (customTag)
      {
        case "CUSTOM_EFFECT_FROG":
          new Frog((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_POISON":
          new Poison((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_NOMAGIC":
          new NoMagic((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_NOHEALMAGIC":
          new NoHealMagic((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_NOSUMMON":
          new NoSummon((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_NOOFFENSIVEMAGIC":
          new NoOffensiveMagic((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_NOBUFF":
          new NoBuff((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_NOUSEABLEITEM":
          new NoUseableItem((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_SLOW":
          new Slow((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_MINI":
          new Mini((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_HALF_HEALTH":
          new HalfHealth((NwCreature)callInfo.ObjectSelf, false);
          break;
        case "CUSTOM_EFFECT_SPELL_FAILURE":
          new SpellFailure((NwCreature)callInfo.ObjectSelf, false);
          break;
      }
    }
  }
}
