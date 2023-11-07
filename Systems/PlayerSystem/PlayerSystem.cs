using System;
using System.Collections.Generic;
using System.Linq;
using NWN.Core;
using NWN.Core.NWNX;
using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using System.Threading.Tasks;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlayerSystem))]
  public partial class PlayerSystem
  {
    public EventService eventService { get; set; }
    public FeedbackService feedbackService;
    public ScriptHandleFactory scriptHandleFactory;
    public SpellSystem spellSystem;
    public AreaSystem areaSystem;
    public static SchedulerService scheduler;
    public static readonly Dictionary<uint, Player> Players = new();
    public PlayerSystem(EventService eventServices, FeedbackService feedback, ScriptHandleFactory scriptFactory, AreaSystem areaSystem, SpellSystem spellSystem, SchedulerService schedulerService)
    {
      NwModule.Instance.OnClientEnter += HandlePlayerConnect;
      NwModule.Instance.OnClientDisconnect += HandlePlayerLeave;

      eventService = eventServices;
      feedbackService = feedback;
      scriptHandleFactory = scriptFactory;
      scheduler = schedulerService;
      this.spellSystem = spellSystem;
      this.areaSystem = areaSystem;
    }
    
    public static void HandleCombatModeOff(OnCombatModeToggle onCombatMode)
    {
      if (onCombatMode.NewMode == CombatMode.None && onCombatMode.Creature.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").HasValue) // Permet de conserver sa posture de combat après avoir utilisé taunt
      {
        onCombatMode.PreventToggle = true;
        onCombatMode.Creature.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").Delete();
      }
    }

    private static void HandleBeforeSkillUsed(OnUseSkill onUseSkill)
    {
      NwCreature oPC = onUseSkill.Creature;

      switch (onUseSkill.Skill.SkillType)
      {
        case Skill.Taunt:
          oPC.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").Value = 1;

          Task waitForCooldown = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(6));
            oPC.GetObjectVariable<LocalVariableInt>("_ACTIVATED_TAUNT").Delete();
          });

          break;
        case Skill.PickPocket:
          onUseSkill.PreventSkillUse = true;

          if (!(onUseSkill.Target is NwCreature { IsLoginPlayerCharacter: true } oTarget) || oTarget.ControllingPlayer.IsDM)
          {
            oPC.ControllingPlayer.FloatingTextString("Seuls d'autres joueurs peuvent être ciblés par cette compétence. Les tentatives de vol sur PNJ doivent être jouées en rp avec un dm.".ColorString(ColorConstants.Red), false);
            return;
          }

          if (!DateTime.TryParse(oTarget.GetObjectVariable<PersistentVariableString>($"_PICKPOCKET_TIMER_{oPC.Name}").Value, out DateTime previousDate)
              || (DateTime.Now - previousDate).TotalHours < 24)
          {
            oPC.ControllingPlayer.FloatingTextString($"Vous ne serez autorisé à faire une nouvelle tentative de vol que dans : {(DateTime.Now - previousDate).TotalHours + 1} heure(s)", false);
            return;
          }

          oTarget.GetObjectVariable<PersistentVariableString>($"_PICKPOCKET_TIMER_{oPC.Name}").Value = DateTime.Now.ToString();

          FeedbackPlugin.SetFeedbackMessageHidden(13, 1, oTarget); // 13 = COMBAT_TOUCH_ATTACK

          Task waitForMessage = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(2));
            FeedbackPlugin.SetFeedbackMessageHidden(13, 0, oTarget);
          });

          int iSpot = oTarget.GetSkillRank(Skill.Spot);
          if (oTarget.DetectModeActive || oTarget.HasFeatEffect(Feat.KeenSense))
            iSpot += NwRandom.Roll(Utils.random, 20, 1);

          if (!oPC.DoSkillCheck(Skill.PickPocket, iSpot))
          {
            oTarget.ControllingPlayer.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
            oPC.ControllingPlayer.FloatingTextString($"{oPC} est en train d'essayer de faire les poches de {oTarget} !", true);
          }

          Task waitForTouch = NwTask.Run(async () =>
          {
            TouchAttackResult touch = await oPC.TouchAttackMelee(oTarget);
            if (touch == TouchAttackResult.Miss)
            {
              oPC.ControllingPlayer.FloatingTextString($"Vous ne parvenez pas à atteindre les poches de {oTarget.Name} !", false);
              return;
            }

            int iStolenGold = (NwRandom.Roll(Utils.random, 20, 1) + oPC.GetSkillRank(Skill.PickPocket) - iSpot) * 10;

            if (oTarget.Gold >= iStolenGold)
            {
              oTarget.Gold = (uint)(oTarget.Gold - iStolenGold);
              oPC.GiveGold(iStolenGold);
              oPC.ControllingPlayer.FloatingTextString($"Vous venez de dérober {iStolenGold} pièces d'or des poches de {oTarget.Name} !", false);
            }
            else
            {
              oPC.ControllingPlayer.FloatingTextString($"Vous venez de vider les poches de {oTarget.Name} ! {oTarget.Gold} pièces d'or de plus pour vous.", false);
              oPC.GiveGold((int)oTarget.Gold);
              oTarget.Gold = 0;
            }
          });

          break;
        case Skill.AnimalEmpathy:
          if (oPC.Area.Tag == "Promenadetest")
          {
            oPC.ControllingPlayer.FloatingTextString("L'endroit est bien trop agité pour que vous puissiez vous permettre de nouer un lien avec l'animal.", false);
            EventsPlugin.SkipEvent();
          }
          break;
      }
    }

    [ScriptHandler("on_input_emote")]
    private async void HandleInputEmote(CallInfo callInfo)
    {
      if (callInfo.ObjectSelf is not NwCreature creature || !Players.TryGetValue(creature.ControllingPlayer.LoginCreature, out Player player))
        return;

      Animation animation = Utils.TranslateEngineAnimation(int.Parse(EventsPlugin.GetEventData("ANIMATION")));

      switch (animation)
      {
        case Animation.LoopingMeditate:
        case Animation.LoopingConjure1:
        case Animation.LoopingConjure2:
        case Animation.LoopingGetMid:
        case Animation.LoopingGetLow:
        case Animation.LoopingListen:
        case Animation.LoopingLookFar:
        case Animation.LoopingPause:
        case Animation.LoopingPauseDrunk:
        case Animation.LoopingPauseTired:
        case Animation.LoopingPause2:
        case Animation.LoopingSpasm:
        case Animation.LoopingTalkForceful:
        case Animation.LoopingTalkLaughing:
        case Animation.LoopingTalkNormal:
        case Animation.LoopingTalkPleading:
        case Animation.LoopingWorship:
          EventsPlugin.SkipEvent();
          await player.oid.ControlledCreature.PlayAnimation(animation, 1, false, TimeSpan.FromDays(1));
          break;
        case Animation.LoopingDeadBack:
        case Animation.LoopingDeadFront:
        case Animation.LoopingSitChair:
        case Animation.LoopingSitCross:
          EventsPlugin.SkipEvent();
          await player.oid.ControlledCreature.PlayAnimation(animation, 1, false, TimeSpan.FromDays(1));

          if (!player.windows.ContainsKey("sitAnywhere")) player.windows.Add("sitAnywhere", new Player.SitAnywhereWindow(player));
          else ((Player.SitAnywhereWindow)player.windows["sitAnywhere"]).CreateWindow();

          break;
      }
    }
    public static void HandleBeforeScrollLearn(OnItemScrollLearn onScrollLearn)
    {
      NwCreature oPC = onScrollLearn.Creature;
      onScrollLearn.PreventLearnScroll = true;

      if (!Players.TryGetValue(onScrollLearn.Creature, out Player player))
        return;


      NwItem oScroll = onScrollLearn.Scroll;
      int spellId = SpellUtils.GetSpellIDFromScroll(oScroll);
      byte spellLevel = NwSpell.FromSpellId(spellId).InnateSpellLevel;

      if (spellId < 0 || spellLevel > 10)
      {
        LogUtils.LogMessage($"LEARN SPELL FROM SCROLL - Player : {oPC.Name}, SpellId : {spellId}, SpellLevel : {spellLevel} - INVALID", LogUtils.LogType.Learnables);
        oPC.ControllingPlayer.SendServerMessage("HRP - Ce parchemin ne semble pas correctement configuré, impossible d'en apprendre quoique ce soit. Le staff a été informé du problème.", ColorConstants.Red);
        return;
      }

      if (player.learnableSpells.ContainsKey(spellId))
      {
        if (oScroll.GetObjectVariable<LocalVariableInt>("_ONE_USE_ONLY").HasValue && Config.env == Config.Env.Prod)
        {
          player.oid.SendServerMessage("Vous avez déjà retiré tout ce qui était possible de ce parchemin. Essayez d'en trouver une autre version pour en apprendre davantage", ColorConstants.Orange);
          return;
        }
        
        LearnableSpell learnable = player.learnableSpells[spellId];

        if (!learnable.canLearn) 
        {
          learnable.canLearn = true;
          oPC.ControllingPlayer.SendServerMessage($"L'étude des informations supplémentaires contenues dans ce parchemin vous permettra d'accéder à un niveau de maîtrise supérieur du sort {StringUtils.ToWhitecolor(learnable.name)}.", new Color(32, 255, 32));
        }
        else
        {
          learnable.acquiredPoints += learnable.currentLevel > 1 ? (learnable.pointsToNextLevel - (5000 * (learnable.currentLevel - 1) * learnable.multiplier)) / 5
            : 1000 * learnable.multiplier;
          oPC.ControllingPlayer.SendServerMessage($"Les informations supplémentaires contenues dans ce parchemin vous permettent d'affiner votre connaissance du sort {StringUtils.ToWhitecolor(learnable.name)}. Votre étude sera plus rapide.", new Color(32, 255, 32));
        }
      }
      else
      {
        player.learnableSpells.Add(spellId, new LearnableSpell((LearnableSpell)SkillSystem.learnableDictionary[spellId]));
        oPC.ControllingPlayer.SendServerMessage($"Le sort a été ajouté à votre liste d'apprentissage et est désormais disponible pour étude.");

        LogUtils.LogMessage($"SPELL SYSTEM - Player : {oPC.Name} vient d'ajouter {NwSpell.FromSpellId(spellId).Name.ToString()} ({spellId}) à sa liste d'apprentissage", LogUtils.LogType.Learnables);
      }

      if (player.TryGetOpenedWindow("learnables", out Player.PlayerWindow learnableWindow))
      {
        Player.LearnableWindow window = (Player.LearnableWindow)learnableWindow;
        window.LoadLearnableList(window.currentList);
      }

      if (oScroll.StackSize > 1)
        oScroll.StackSize -= 1;
      else
        oScroll.Destroy();
    }

    public static void HandleOnClientLevelUp(OnClientLevelUpBegin onLevelUp)
    {
      onLevelUp.PreventLevelUp = true;
      LogUtils.LogMessage($"{onLevelUp.Player.LoginCreature.Name} vient d'essayer de level up.", LogUtils.LogType.ModuleAdministration);
      onLevelUp.Player.LoginCreature.Xp = 1;
    }
    public static void HandleGuiEvents(ModuleEvents.OnPlayerGuiEvent guiEvent)
    {
      NwPlayer oPC = guiEvent.Player;
      oPC.LoginCreature.GetObjectVariable<DateTimeLocalVariable>("_LAST_ACTION_DATE").Value = DateTime.Now;

      if (!Players.TryGetValue(oPC.LoginCreature, out Player player))
        return;

      switch (guiEvent.EventType)
      {
        case GuiEventType.DisabledPanelAttemptOpen:

          switch (guiEvent.OpenedPanel)
          {
            case GUIPanel.ExamineItem:

              if (!player.windows.ContainsKey("itemExamine")) player.windows.Add("itemExamine", new Player.ItemExamineWindow(player, (NwItem)guiEvent.EventObject));
              else ((Player.ItemExamineWindow)player.windows["itemExamine"]).CreateWindow((NwItem)guiEvent.EventObject);

              return;

            case GUIPanel.ExaminePlaceable:

              if (!player.windows.ContainsKey("editorPlaceable")) player.windows.Add("editorPlaceable", new Player.EditorPlaceableWindow(player, (NwPlaceable)guiEvent.EventObject));
              else ((Player.EditorPlaceableWindow)player.windows["editorPlaceable"]).CreateWindow((NwPlaceable)guiEvent.EventObject);

              return;

            case GUIPanel.ExamineCreature:

              if (!player.windows.ContainsKey("editorPNJ")) player.windows.Add("editorPNJ", new Player.EditorPNJWindow(player, (NwCreature)guiEvent.EventObject));
              else ((Player.EditorPNJWindow)player.windows["editorPNJ"]).CreateWindow((NwCreature)guiEvent.EventObject);

              return;

            case GUIPanel.Journal:

              if (!player.windows.ContainsKey("mainMenu")) player.windows.Add("mainMenu", new Player.MainMenuWindow(player));
              else ((Player.MainMenuWindow)player.windows["mainMenu"]).CreateWindow();

              return;

            case GUIPanel.PlayerList:

              if (!player.windows.ContainsKey("playerList")) player.windows.Add("playerList", new Player.PlayerListWindow(player));
              else ((Player.PlayerListWindow)player.windows["playerList"]).CreateWindow();

              return;
          }

          break;

        case GuiEventType.ExamineObject:

          // TODO : Lorsque la créature examinée est une invocation du joueur et que le joueur possède le don spell focus conjuration, permettre de la renommer

          if (guiEvent.EventObject is NwCreature examineCreature && examineCreature == oPC.LoginCreature) // TODO : plutôt mettre ça dans le menu
          {
            if (player.craftJob != null)
            {
              if (!player.windows.ContainsKey("activeCraftJob")) player.windows.Add("activeCraftJob", new Player.ActiveCraftJobWindow(player));
              else ((Player.ActiveCraftJobWindow)player.windows["activeCraftJob"]).CreateWindow();
            }

            if (!player.windows.ContainsKey("learnables")) player.windows.Add("learnables", new Player.LearnableWindow(player));
            else ((Player.LearnableWindow)player.windows["learnables"]).CreateWindow();
          }

          break;

        case GuiEventType.PartyBarPortraitClick:
          //oPC.SendServerMessage("portrait click");
          break;

        case GuiEventType.PlayerListPlayerClick:
          //oPC.SendServerMessage("player list click");
          break;

        case GuiEventType.ChatBarFocus:

          if (guiEvent.ChatBarChannel != ChatBarChannel.Talk && guiEvent.ChatBarChannel != ChatBarChannel.Whisper)
            return;

          Effect visualMark = Effect.VisualEffect((VfxType)1248);
          visualMark.Tag = "VFX_SPEAKING_MARK";
          visualMark.SubType = EffectSubType.Supernatural;
          oPC.ControlledCreature.ApplyEffect(EffectDuration.Permanent, visualMark);

          break;

        case GuiEventType.ChatBarUnFocus:

          if (guiEvent.ChatBarChannel != ChatBarChannel.Talk && guiEvent.ChatBarChannel != ChatBarChannel.Whisper)
            return;

          foreach (Effect eff in oPC.ControlledCreature.ActiveEffects.Where(e => e.Tag == "VFX_SPEAKING_MARK"))
            oPC.ControlledCreature.RemoveEffect(eff);

          break;

        case GuiEventType.EffectIconClick:

          EffectIconTableEntry nEffectIcon = guiEvent.EffectIcon;
          EffectType nIconEffectType = EffectIconToEffectType(nEffectIcon.RowIndex);

          if (nEffectIcon.RowIndex == 148) // concentration
          {
            if (!player.windows.ContainsKey("removeConcentration")) player.windows.Add("removeConcentration", new Player.RemoveConcentrationWindow(player));
            else ((Player.RemoveConcentrationWindow)player.windows["removeConcentration"]).CreateWindow();
          }

          if (nIconEffectType == EffectType.InvalidEffect)
            return;

          bool bSkipDisplay = false, bIsSpellLevelAbsorptionPretendingToBeSpellImmunity = false;

          foreach (Effect eff in oPC.ControlledCreature.ActiveEffects.Where(e => e.EffectType == nIconEffectType))
          {
            string name = "Echec des sorts 50 %";
            int nAmount, nRemaining;

            if (eff.Spell.SpellType != Spell.AllSpells)
              name = eff.Spell.Name.ToString();

            float percentageRemaining = eff.DurationRemaining / eff.TotalDuration;
            Color color = ColorConstants.White;

            if (percentageRemaining > 0.5)
              color = new Color(32, 255, 32);
            else if (percentageRemaining < 0.25)
              color = ColorConstants.Maroon;
            else
              color = ColorConstants.Lime;

            string durationRemaining = eff.DurationType == EffectDuration.Permanent ? "Permanent".ColorString(new Color(32, 255, 32)) : $"Temps restant : {Utils.StripTimeSpanMilliseconds(DateTime.Now.AddSeconds(eff.DurationRemaining) - DateTime.Now)}".ColorString(color);
            string sStats = "";
            string sRacialTypeAlignment = "";

            string sModifier = "";

            switch (eff.EffectType)
            {
              case EffectType.AcIncrease:
              case EffectType.AcDecrease:

                sModifier = GetModifierType(eff.EffectType, EffectType.AcIncrease, EffectType.AcDecrease);
                sStats = $"{sModifier} {eff.IntParams.ElementAt(1)} {ACTypeToString(eff.IntParams.ElementAt(0))} CA";
                sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));

                break;

              case EffectType.AttackIncrease:
              case EffectType.AttackDecrease:

                sModifier = GetModifierType(eff.EffectType, EffectType.AttackIncrease, EffectType.AttackDecrease);
                sStats = sModifier + eff.IntParams.ElementAt(0) + " BA";
                sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));

                break;

              case EffectType.SavingThrowIncrease:
              case EffectType.SavingThrowDecrease:

                sModifier = GetModifierType(eff.EffectType, EffectType.SavingThrowIncrease, EffectType.SavingThrowDecrease);
                string sSavingThrow = SavingThrowToString(eff.IntParams.ElementAt(1));
                string sSavingThrowType = SavingThrowTypeToString(eff.IntParams.ElementAt(2));
                sStats = sModifier + eff.IntParams.ElementAt(0) + " " + sSavingThrow + (sSavingThrowType == "" ? "" : " (vs. " + sSavingThrowType + ")");
                sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4), eff.IntParams.ElementAt(5));

                break;

              case EffectType.AbilityIncrease:
              case EffectType.AbilityDecrease:
                {
                  Ability nAbility = AbilityTypeFromEffectIconAbility(nEffectIcon.RowIndex);

                  if ((int)nAbility != eff.IntParams.ElementAt(0))
                    bSkipDisplay = true;
                  else
                  {
                    sModifier = GetModifierType(eff.EffectType, EffectType.AbilityIncrease, EffectType.AbilityDecrease);
                    sStats = $"{sModifier}{eff.IntParams.ElementAt(1)}  {StringUtils.TranslateAttributeToFrench(nAbility)}";
                  }
                  break;
                }

              case EffectType.DamageIncrease:
              case EffectType.DamageDecrease:
                {
                  sModifier = GetModifierType(eff.EffectType, EffectType.DamageIncrease, EffectType.DamageDecrease);

                  sStats = sModifier + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("iprp_damagecost", "Name", eff.IntParams.ElementAt(0)))) + " (" + DamageTypeToString(eff.IntParams.ElementAt(1)) + ")";
                  sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));
                  break;
                }

              case EffectType.SkillIncrease:
              case EffectType.SkillDecrease:
                {
                  int nSkill = eff.IntParams.ElementAt(0);
                  string sSkill = nSkill == 255 ? "Toute compétence" : NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", nSkill)));
                  sModifier = GetModifierType(eff.EffectType, EffectType.SkillIncrease, EffectType.SkillDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(1) + " " + sSkill;
                  sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3), eff.IntParams.ElementAt(4));
                  break;
                }

              case EffectType.TemporaryHitpoints:
                {
                  sStats = "+" + eff.IntParams.ElementAt(0) + " Points de vie";
                  break;
                }

              case EffectType.DamageReduction:
                {
                  nAmount = eff.IntParams.ElementAt(0);
                  int nDamagePower = eff.IntParams.ElementAt(1);
                  nDamagePower = nDamagePower > 6 ? --nDamagePower : nDamagePower;
                  nRemaining = eff.IntParams.ElementAt(2);
                  sStats = nAmount + "/+" + nDamagePower + " (" + (nRemaining == 0 ? "Illimité" : nRemaining + " Dégâts Restants") + ")";
                  break;
                }

              case EffectType.DamageResistance:
                {
                  nAmount = eff.IntParams.ElementAt(1);
                  nRemaining = eff.IntParams.ElementAt(2);
                  sStats = nAmount + "/- " + DamageTypeToString(eff.IntParams.ElementAt(0)) + " Résistance (" + (nRemaining == 0 ? "Illimité" : nRemaining + " Dégâts Restants") + ")";
                  break;
                }

              case EffectType.Immunity:
                {
                  ImmunityType nImmunity = ImmunityTypeFromEffectIconImmunity(nEffectIcon.RowIndex);

                  if ((int)nImmunity != eff.IntParams.ElementAt(0))
                    bSkipDisplay = true;
                  else
                  {
                    sStats = NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("effecticons", "StrRef", nEffectIcon.RowIndex)));
                    sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(1), eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3));
                  }
                  break;
                }

              case EffectType.DamageImmunityIncrease:
              case EffectType.DamageImmunityDecrease:
                {
                  int nDamageType = eff.IntParams.ElementAt(0);
                  DamageType nDamageTypeFromIcon = DamageTypeFromEffectIconDamageImmunity(nEffectIcon.RowIndex);

                  if (nDamageTypeFromIcon != DamageType.BaseWeapon && nDamageType != (int)nDamageTypeFromIcon)
                    bSkipDisplay = true;

                  sModifier = GetModifierType(eff.EffectType, EffectType.DamageImmunityIncrease, EffectType.DamageImmunityDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(1) + "% " + DamageTypeToString(nDamageType) + " Damage Immunity";
                  break;
                }

              case EffectType.SpellImmunity:
                {
                  sStats = "Immunité au sort : " + NwSpell.FromSpellId(eff.IntParams.ElementAt(0)).Name;
                  break;
                }

              case EffectType.SpellLevelAbsorption:
                {
                  int nMaxSpellLevelAbsorbed = eff.IntParams.ElementAt(0);
                  bool bUnlimited = eff.IntParams.ElementAt(3).ToBool();

                  var sSpellLevel = nMaxSpellLevelAbsorbed switch
                  {
                    0 => "Tours de magie",
                    _ => "Niveau " + nMaxSpellLevelAbsorbed,
                  };

                  sSpellLevel += " Level" + (nMaxSpellLevelAbsorbed == 0 ? "" : " and Below");
                  string sSpellSchool = SpellSchoolToString(eff.IntParams.ElementAt(2));
                  string sRemainingSpellLevels = bUnlimited ? "" : "(" + eff.IntParams.ElementAt(1) + " Niveaux de sorts restants)";
                  sStats = sSpellLevel + " " + sSpellSchool + " Spell Immunity " + sRemainingSpellLevels;

                  if (bIsSpellLevelAbsorptionPretendingToBeSpellImmunity)
                    nIconEffectType = EffectType.SpellImmunity;
                  else if (bUnlimited && !bIsSpellLevelAbsorptionPretendingToBeSpellImmunity)
                    bSkipDisplay = true;

                  break;
                }

              case EffectType.Regenerate:
                {
                  sStats = "+" + eff.IntParams.ElementAt(0) + " HP / " + (eff.IntParams.ElementAt(1) / 1000.0f).ToString("0.00") + "s";
                  break;
                }

              case EffectType.Poison:
                {
                  sStats = "Poison: " + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("poison", "Name", eff.IntParams.ElementAt(0))));
                  break;
                }

              case EffectType.Disease:
                {
                  sStats = "Disease: " + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("disease", "Name", eff.IntParams.ElementAt(0))));
                  break;
                }

              case EffectType.Curse:
                {
                  int nAbility;
                  string sAbilityDecrease = "";

                  for (nAbility = 0; nAbility < 6; nAbility++)
                  {
                    int nAbilityMod = eff.IntParams.ElementAt(nAbility);
                    if (nAbilityMod > 0)
                    {
                      string sAbility = StringUtils.TranslateAttributeToFrench((Ability)nAbility)[..3];
                      sAbilityDecrease += "-" + nAbilityMod + " " + sAbility + ", ";
                    }
                  }

                  sAbilityDecrease = sAbilityDecrease[0..^2];
                  sStats = sAbilityDecrease;
                  break;
                }

              case EffectType.MovementSpeedIncrease:
              case EffectType.MovementSpeedDecrease:
                {
                  sModifier = GetModifierType(eff.EffectType, EffectType.MovementSpeedIncrease, EffectType.MovementSpeedDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(0) + "% Movement Speed";
                  break;
                }

              case EffectType.ElementalShield:
                {
                  sStats = eff.IntParams.ElementAt(0) + " + " + NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("iprp_damagecost", "Name", eff.IntParams.ElementAt(1)) + " (" + DamageTypeToString(eff.IntParams.ElementAt(2)))) + ")";
                  break;
                }

              case EffectType.NegativeLevel:
                {
                  sStats = "-" + eff.IntParams.ElementAt(0) + " Niveaux";
                  break;
                }

              case EffectType.Concealment:
                {
                  string sMissChance = MissChanceToString(eff.IntParams.ElementAt(4) - 1);
                  sStats = eff.IntParams.ElementAt(0) + "% Concealment" + (sMissChance == "" ? "" : " (" + sMissChance + ")");
                  sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(1), eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3));
                  break;
                }

              case EffectType.SpellResistanceIncrease:
              case EffectType.SpellResistanceDecrease:
                {
                  sModifier = GetModifierType(eff.EffectType, EffectType.SpellResistanceIncrease, EffectType.SpellResistanceDecrease);
                  sStats = sModifier + eff.IntParams.ElementAt(0) + " Résistance aux sorts";
                  break;
                }

              case EffectType.SpellFailure:
                {
                  sStats = eff.IntParams.ElementAt(0) + "% Echec des sorts (Ecole : " + SpellSchoolToString(eff.IntParams.ElementAt(1)) + ")";
                  break;
                }

              case EffectType.Invisibility:
                {
                  int nInvisibilityType = eff.IntParams.ElementAt(0);
                  if (nEffectIcon.RowIndex == NWScript.EFFECT_ICON_INVISIBILITY)
                    bSkipDisplay = nInvisibilityType != NWScript.INVISIBILITY_TYPE_NORMAL;
                  else if (nEffectIcon.RowIndex == NWScript.EFFECT_ICON_IMPROVEDINVISIBILITY)
                    bSkipDisplay = nInvisibilityType != NWScript.INVISIBILITY_TYPE_IMPROVED;
                  if (!bSkipDisplay)
                  {
                    sStats = "Invisibilité" + (nInvisibilityType == NWScript.INVISIBILITY_TYPE_IMPROVED ? "Suprème " : "");
                    sRacialTypeAlignment = GetVersusRacialTypeAndAlignment(eff.IntParams.ElementAt(1), eff.IntParams.ElementAt(2), eff.IntParams.ElementAt(3));
                  }
                  break;
                }
            }

            if (!bSkipDisplay)
            {
              string creator = !eff.Creator.IsValid ? "Inconnue" : eff.Creator.Name;
              string message = $"\n{name.ColorString(ColorConstants.Red)}\n";
              message += $"{durationRemaining}\n";
              message += sStats == "" ? "" : " -> " + sStats + sRacialTypeAlignment + "\n";
              message += $"Source : {creator}";


              oPC.SendServerMessage(message, ColorConstants.Orange);
            }
          }

          break;
      }
    }
    private static string GetModifierType(EffectType nEffectType, EffectType nPlus, EffectType nMinus)
    {
      return nEffectType == nPlus ? "+" : nEffectType == nMinus ? "-" : "";
    }
    private static string ACTypeToString(int nACType)
    {

      return (ACBonus)nACType switch
      {
        ACBonus.Dodge => "Esquive",
        ACBonus.Natural => "Naturelle",
        ACBonus.ArmourEnchantment => "Armure",
        ACBonus.ShieldEnchantment => "Bouclier",
        ACBonus.Deflection => "Parade",
        _ => "",
      };
    }
    private static string SpellSchoolToString(int nSpellSchool)
    {
      return nSpellSchool switch
      {
        NWScript.SPELL_SCHOOL_GENERAL => "Généraliste",
        NWScript.SPELL_SCHOOL_ABJURATION => "Abjuration",
        NWScript.SPELL_SCHOOL_CONJURATION => "Invocation",
        NWScript.SPELL_SCHOOL_DIVINATION => "Divination",
        NWScript.SPELL_SCHOOL_ENCHANTMENT => "Enchantement",
        NWScript.SPELL_SCHOOL_EVOCATION => "Evocation",
        NWScript.SPELL_SCHOOL_ILLUSION => "Illusion",
        NWScript.SPELL_SCHOOL_NECROMANCY => "Nécromancie",
        NWScript.SPELL_SCHOOL_TRANSMUTATION => "Transmutation",
        _ => "Nom manquant",
      };
    }

    private static string MissChanceToString(int nMissChance)
    {
      return (MissChanceType)nMissChance switch
      {
        MissChanceType.VsRanged => "vs. Distance",
        MissChanceType.VsMelee => "vs. Mêlée",
        _ => "Nom manquant",
      };
    }
    private static DamageType DamageTypeFromEffectIconDamageImmunity(int nEffectIcon)
    {
      return nEffectIcon switch
      {
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC_DECREASE => DamageType.Magical,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID_DECREASE => DamageType.Acid,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD_DECREASE => DamageType.Cold,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE_DECREASE => DamageType.Divine,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL_DECREASE => DamageType.Electrical,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE_DECREASE => DamageType.Fire,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE_DECREASE => DamageType.Negative,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE_DECREASE => DamageType.Positive,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC_DECREASE => DamageType.Sonic,
        _ => DamageType.BaseWeapon,
      };
    }
    private static ImmunityType ImmunityTypeFromEffectIconImmunity(int nEffectIcon)
    {
      return nEffectIcon switch
      {
        NWScript.EFFECT_ICON_IMMUNITY_MIND => ImmunityType.MindSpells,
        NWScript.EFFECT_ICON_IMMUNITY_POISON => ImmunityType.Poison,
        NWScript.EFFECT_ICON_IMMUNITY_DISEASE => ImmunityType.Disease,
        NWScript.EFFECT_ICON_IMMUNITY_FEAR => ImmunityType.Fear,
        NWScript.EFFECT_ICON_IMMUNITY_TRAP => ImmunityType.Trap,
        NWScript.EFFECT_ICON_IMMUNITY_PARALYSIS => ImmunityType.Paralysis,
        NWScript.EFFECT_ICON_IMMUNITY_BLINDNESS => ImmunityType.Blindness,
        NWScript.EFFECT_ICON_IMMUNITY_DEAFNESS => ImmunityType.Deafness,
        NWScript.EFFECT_ICON_IMMUNITY_SLOW => ImmunityType.Slow,
        NWScript.EFFECT_ICON_IMMUNITY_ENTANGLE => ImmunityType.Entangle,
        NWScript.EFFECT_ICON_IMMUNITY_SILENCE => ImmunityType.Silence,
        NWScript.EFFECT_ICON_IMMUNITY_STUN => ImmunityType.Stun,
        NWScript.EFFECT_ICON_IMMUNITY_SLEEP => ImmunityType.Sleep,
        NWScript.EFFECT_ICON_IMMUNITY_CHARM => ImmunityType.Charm,
        NWScript.EFFECT_ICON_IMMUNITY_DOMINATE => ImmunityType.Dominate,
        NWScript.EFFECT_ICON_IMMUNITY_CONFUSE => ImmunityType.Confused,
        NWScript.EFFECT_ICON_IMMUNITY_CURSE => ImmunityType.Cursed,
        NWScript.EFFECT_ICON_IMMUNITY_DAZED => ImmunityType.Dazed,
        NWScript.EFFECT_ICON_IMMUNITY_ABILITY_DECREASE => ImmunityType.AbilityDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_ATTACK_DECREASE => ImmunityType.AttackDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_DECREASE => ImmunityType.DamageDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_IMMUNITY_DECREASE => ImmunityType.DamageImmunityDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_AC_DECREASE => ImmunityType.AcDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_MOVEMENT_SPEED_DECREASE => ImmunityType.MovementSpeedDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_SAVING_THROW_DECREASE => ImmunityType.SavingThrowDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_SPELL_RESISTANCE_DECREASE => ImmunityType.SpellResistanceDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_SKILL_DECREASE => ImmunityType.SkillDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_KNOCKDOWN => ImmunityType.Knockdown,
        NWScript.EFFECT_ICON_IMMUNITY_NEGATIVE_LEVEL => ImmunityType.NegativeLevel,
        NWScript.EFFECT_ICON_IMMUNITY_SNEAK_ATTACK => ImmunityType.SneakAttack,
        NWScript.EFFECT_ICON_IMMUNITY_CRITICAL_HIT => ImmunityType.CriticalHit,
        NWScript.EFFECT_ICON_IMMUNITY_DEATH_MAGIC => ImmunityType.Death,
        _ => ImmunityType.None,
      };
    }
    private static string DamageTypeToString(int nDamageType)
    {
      return (DamageType)nDamageType switch
      {
        DamageType.Bludgeoning => "Contondant",
        DamageType.Piercing => "Perçant",
        DamageType.Slashing => "Tranchant",
        DamageType.Magical => "Magique",
        DamageType.Acid => "Acide",
        DamageType.Cold => "Froid",
        DamageType.Divine => "Divin",
        DamageType.Electrical => "Electrique",
        DamageType.Fire => "Feu",
        DamageType.Negative => "Negatif",
        DamageType.Positive => "Positif",
        DamageType.Sonic => "Sonique",
        DamageType.BaseWeapon => "Base",
        _ => "Nom manquant",
      };
    }
    private static Ability AbilityTypeFromEffectIconAbility(int nEffectIcon)
    {
      return nEffectIcon switch
      {
        NWScript.EFFECT_ICON_ABILITY_INCREASE_STR or NWScript.EFFECT_ICON_ABILITY_DECREASE_STR => Ability.Strength,
        NWScript.EFFECT_ICON_ABILITY_INCREASE_DEX or NWScript.EFFECT_ICON_ABILITY_DECREASE_DEX => Ability.Dexterity,
        NWScript.EFFECT_ICON_ABILITY_INCREASE_CON or NWScript.EFFECT_ICON_ABILITY_DECREASE_CON => Ability.Constitution,
        NWScript.EFFECT_ICON_ABILITY_INCREASE_INT or NWScript.EFFECT_ICON_ABILITY_DECREASE_INT => Ability.Intelligence,
        NWScript.EFFECT_ICON_ABILITY_INCREASE_WIS or NWScript.EFFECT_ICON_ABILITY_DECREASE_WIS => Ability.Wisdom,
        NWScript.EFFECT_ICON_ABILITY_INCREASE_CHA or NWScript.EFFECT_ICON_ABILITY_DECREASE_CHA => Ability.Charisma,
        _ => Ability.Strength,
      };
    }

    private static string SavingThrowToString(int nSavingThrow)
    {
      return nSavingThrow switch
      {
        NWScript.SAVING_THROW_ALL => "Universel",
        NWScript.SAVING_THROW_FORT => "Vigueur",
        NWScript.SAVING_THROW_REFLEX => "Réflexes",
        NWScript.SAVING_THROW_WILL => "Volonté",
        _ => "Nom manquant",
      };
    }
    private static string SavingThrowTypeToString(int nSavingThrowType)
    {
      return (SavingThrowType)nSavingThrowType switch
      {
        SavingThrowType.MindSpells => "Sorts affectant l'esprit",
        SavingThrowType.Poison => "Poison",
        SavingThrowType.Disease => "Maladie",
        SavingThrowType.Fear => "Peur",
        SavingThrowType.Sonic => "Sonique",
        SavingThrowType.Acid => "Acide",
        SavingThrowType.Fire => "Feu",
        SavingThrowType.Electricity => "Electricité",
        SavingThrowType.Positive => "Positif",
        SavingThrowType.Negative => "Négatif",
        SavingThrowType.Death => "Mort",
        SavingThrowType.Cold => "Froid",
        SavingThrowType.Divine => "Divin",
        SavingThrowType.Trap => "Pièges",
        SavingThrowType.Spell => "Sorts",
        SavingThrowType.Good => "Bon",
        SavingThrowType.Evil => "Mauvais",
        SavingThrowType.Law => "Loi",
        SavingThrowType.Chaos => "Chaos",
        _ => "Nom manquant",
      };
    }
    private static string GetVersusRacialTypeAndAlignment(int nRacialType, int nLawfulChaotic, int nGoodEvil)
    {
      string sRacialType = nRacialType == NWScript.RACIAL_TYPE_INVALID ? "" : NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("racialtypes", "NamePlural", nRacialType)));
      string sLawfulChaotic = nLawfulChaotic == NWScript.ALIGNMENT_LAWFUL ? "Lawful" : nLawfulChaotic == NWScript.ALIGNMENT_CHAOTIC ? "Chaotic" : "";
      string sGoodEvil = nGoodEvil == NWScript.ALIGNMENT_GOOD ? "Good" : nGoodEvil == NWScript.ALIGNMENT_EVIL ? "Evil" : "";
      string sAlignment = sLawfulChaotic + (sLawfulChaotic == "" ? sGoodEvil : (sGoodEvil == "" ? "" : " " + sGoodEvil));
      return (sRacialType != "" || sAlignment != "") ? (" vs. " + sAlignment + (sAlignment == "" ? sRacialType : (sRacialType == "" ? "" : " " + sRacialType))) : "";
    }
    private static EffectType EffectIconToEffectType(int nEffectIcon)
    {
      return nEffectIcon switch
      {
        NWScript.EFFECT_ICON_INVALID => EffectType.InvalidEffect,
        // *** No Extra Stats
        NWScript.EFFECT_ICON_BLIND => EffectType.Blindness,
        NWScript.EFFECT_ICON_CHARMED => EffectType.Charmed,
        NWScript.EFFECT_ICON_CONFUSED => EffectType.Confused,
        NWScript.EFFECT_ICON_FRIGHTENED => EffectType.Frightened,
        NWScript.EFFECT_ICON_DOMINATED => EffectType.Dominated,
        NWScript.EFFECT_ICON_PARALYZE => EffectType.Paralyze,
        NWScript.EFFECT_ICON_DAZED => EffectType.Dazed,
        NWScript.EFFECT_ICON_STUNNED => EffectType.Stunned,
        NWScript.EFFECT_ICON_SLEEP => EffectType.Sleep,
        NWScript.EFFECT_ICON_SILENCE => EffectType.Silence,
        NWScript.EFFECT_ICON_TURNED => EffectType.Turned,
        NWScript.EFFECT_ICON_HASTE => EffectType.Haste,
        NWScript.EFFECT_ICON_SLOW => EffectType.Slow,
        NWScript.EFFECT_ICON_ENTANGLE => EffectType.Entangle,
        NWScript.EFFECT_ICON_DEAF => EffectType.Deaf,
        NWScript.EFFECT_ICON_DARKNESS => EffectType.Darkness,
        NWScript.EFFECT_ICON_POLYMORPH => EffectType.Polymorph,
        NWScript.EFFECT_ICON_SANCTUARY => EffectType.Sanctuary,
        NWScript.EFFECT_ICON_TRUESEEING => EffectType.TrueSeeing,
        NWScript.EFFECT_ICON_SEEINVISIBILITY => EffectType.SeeInvisible,
        NWScript.EFFECT_ICON_ETHEREALNESS => EffectType.Ethereal,
        NWScript.EFFECT_ICON_PETRIFIED => EffectType.Petrify,
        // ***
        NWScript.EFFECT_ICON_DAMAGE_RESISTANCE => EffectType.DamageResistance,
        NWScript.EFFECT_ICON_REGENERATE => EffectType.Regenerate,
        NWScript.EFFECT_ICON_DAMAGE_REDUCTION => EffectType.DamageReduction,
        NWScript.EFFECT_ICON_TEMPORARY_HITPOINTS => EffectType.TemporaryHitpoints,
        NWScript.EFFECT_ICON_IMMUNITY => EffectType.Immunity,
        NWScript.EFFECT_ICON_POISON => EffectType.Poison,
        NWScript.EFFECT_ICON_DISEASE => EffectType.Disease,
        NWScript.EFFECT_ICON_CURSE => EffectType.Curse,
        NWScript.EFFECT_ICON_ATTACK_INCREASE => EffectType.AttackIncrease,
        NWScript.EFFECT_ICON_ATTACK_DECREASE => EffectType.AttackDecrease,
        NWScript.EFFECT_ICON_DAMAGE_INCREASE => EffectType.DamageIncrease,
        NWScript.EFFECT_ICON_DAMAGE_DECREASE => EffectType.DamageDecrease,
        NWScript.EFFECT_ICON_AC_INCREASE => EffectType.AcIncrease,
        NWScript.EFFECT_ICON_AC_DECREASE => EffectType.AcDecrease,
        NWScript.EFFECT_ICON_MOVEMENT_SPEED_INCREASE => EffectType.MovementSpeedIncrease,
        NWScript.EFFECT_ICON_MOVEMENT_SPEED_DECREASE => EffectType.MovementSpeedDecrease,
        NWScript.EFFECT_ICON_SAVING_THROW_DECREASE => EffectType.SavingThrowDecrease,
        NWScript.EFFECT_ICON_SPELL_RESISTANCE_INCREASE => EffectType.SpellResistanceIncrease,
        NWScript.EFFECT_ICON_SPELL_RESISTANCE_DECREASE => EffectType.SpellResistanceDecrease,
        NWScript.EFFECT_ICON_SKILL_INCREASE => EffectType.SkillIncrease,
        NWScript.EFFECT_ICON_SKILL_DECREASE => EffectType.SkillDecrease,
        NWScript.EFFECT_ICON_ELEMENTALSHIELD => EffectType.ElementalShield,
        NWScript.EFFECT_ICON_LEVELDRAIN => EffectType.NegativeLevel,
        NWScript.EFFECT_ICON_SPELLLEVELABSORPTION => EffectType.SpellLevelAbsorption,
        NWScript.EFFECT_ICON_SPELLIMMUNITY => EffectType.SpellImmunity,
        NWScript.EFFECT_ICON_CONCEALMENT => EffectType.Concealment,
        NWScript.EFFECT_ICON_EFFECT_SPELL_FAILURE => EffectType.SpellFailure,
        NWScript.EFFECT_ICON_INVISIBILITY or NWScript.EFFECT_ICON_IMPROVEDINVISIBILITY => EffectType.Invisibility,
        NWScript.EFFECT_ICON_ABILITY_INCREASE_STR or NWScript.EFFECT_ICON_ABILITY_INCREASE_DEX or NWScript.EFFECT_ICON_ABILITY_INCREASE_CON or NWScript.EFFECT_ICON_ABILITY_INCREASE_INT or NWScript.EFFECT_ICON_ABILITY_INCREASE_WIS or NWScript.EFFECT_ICON_ABILITY_INCREASE_CHA => EffectType.AbilityIncrease,
        NWScript.EFFECT_ICON_ABILITY_DECREASE_STR or NWScript.EFFECT_ICON_ABILITY_DECREASE_CHA or NWScript.EFFECT_ICON_ABILITY_DECREASE_DEX or NWScript.EFFECT_ICON_ABILITY_DECREASE_CON or NWScript.EFFECT_ICON_ABILITY_DECREASE_INT or NWScript.EFFECT_ICON_ABILITY_DECREASE_WIS => EffectType.AbilityDecrease,
        NWScript.EFFECT_ICON_IMMUNITY_ALL or NWScript.EFFECT_ICON_IMMUNITY_MIND or NWScript.EFFECT_ICON_IMMUNITY_POISON or NWScript.EFFECT_ICON_IMMUNITY_DISEASE or NWScript.EFFECT_ICON_IMMUNITY_FEAR or NWScript.EFFECT_ICON_IMMUNITY_TRAP or NWScript.EFFECT_ICON_IMMUNITY_PARALYSIS or NWScript.EFFECT_ICON_IMMUNITY_BLINDNESS or NWScript.EFFECT_ICON_IMMUNITY_DEAFNESS or NWScript.EFFECT_ICON_IMMUNITY_SLOW or NWScript.EFFECT_ICON_IMMUNITY_ENTANGLE or NWScript.EFFECT_ICON_IMMUNITY_SILENCE or NWScript.EFFECT_ICON_IMMUNITY_STUN or NWScript.EFFECT_ICON_IMMUNITY_SLEEP or NWScript.EFFECT_ICON_IMMUNITY_CHARM or NWScript.EFFECT_ICON_IMMUNITY_DOMINATE or NWScript.EFFECT_ICON_IMMUNITY_CONFUSE or NWScript.EFFECT_ICON_IMMUNITY_CURSE or NWScript.EFFECT_ICON_IMMUNITY_DAZED or NWScript.EFFECT_ICON_IMMUNITY_ABILITY_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_ATTACK_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_DAMAGE_IMMUNITY_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_AC_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_MOVEMENT_SPEED_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_SAVING_THROW_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_SPELL_RESISTANCE_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_SKILL_DECREASE or NWScript.EFFECT_ICON_IMMUNITY_KNOCKDOWN or NWScript.EFFECT_ICON_IMMUNITY_NEGATIVE_LEVEL or NWScript.EFFECT_ICON_IMMUNITY_SNEAK_ATTACK or NWScript.EFFECT_ICON_IMMUNITY_CRITICAL_HIT or NWScript.EFFECT_ICON_IMMUNITY_DEATH_MAGIC => EffectType.Immunity,
        NWScript.EFFECT_ICON_SAVING_THROW_INCREASE or NWScript.EFFECT_ICON_REFLEX_SAVE_INCREASED or NWScript.EFFECT_ICON_FORT_SAVE_INCREASED or NWScript.EFFECT_ICON_WILL_SAVE_INCREASED => EffectType.SavingThrowIncrease,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_INCREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC => EffectType.DamageImmunityIncrease,
        NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_MAGIC_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ACID_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_COLD_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_DIVINE_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_ELECTRICAL_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_FIRE_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_NEGATIVE_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_POSITIVE_DECREASE or NWScript.EFFECT_ICON_DAMAGE_IMMUNITY_SONIC_DECREASE => EffectType.DamageImmunityDecrease,
        NWScript.EFFECT_ICON_INVULNERABLE => EffectType.Invulnerable,
        NWScript.EFFECT_ICON_WOUNDING => EffectType.InvalidEffect,
        NWScript.EFFECT_ICON_TAUNTED => EffectType.InvalidEffect,
        NWScript.EFFECT_ICON_TIMESTOP => EffectType.TimeStop,
        NWScript.EFFECT_ICON_BLINDNESS => EffectType.Blindness,
        NWScript.EFFECT_ICON_DISPELMAGICBEST => EffectType.InvalidEffect,
        NWScript.EFFECT_ICON_DISPELMAGICALL => EffectType.InvalidEffect,
        NWScript.EFFECT_ICON_ENEMY_ATTACK_BONUS => EffectType.InvalidEffect,
        NWScript.EFFECT_ICON_FATIGUE => EffectType.InvalidEffect,
        _ => EffectType.InvalidEffect,
      };
    }

    // TODO : Probablement refaire le système de déguisement plus proprement
    /*private static int HandlePlayerPerceived(CallInfo callInfo)
    {
        Player oPC;

        if (Players.TryGetValue(oidSelf, out oPC))
        {
            Player oPerceived;
            if (Players.TryGetValue(NWScript.GetLastPerceived(), out oPerceived))
            {
                if (NWScript.GetIsPC(oPerceived.oid) != 1 || NWScript.GetIsDM(oPerceived.oid) == 1 || NWScript.GetIsDMPossessed(oPerceived.oid) == 1 || oPerceived.disguiseName.Length == 0)
                    return 0;

                if (!oPC.disguiseDetectTimer.ContainsKey(oPC.oid) || (DateTime.Now - oPC.disguiseDetectTimer[oPerceived.oid]).TotalSeconds > 1800)
                {
                    oPC.disguiseDetectTimer[oPerceived.oid] = DateTime.Now;

                    int[] iPCSenseSkill = { NWScript.GetSkillRank(NWScript.SKILL_LISTEN, oPC.oid), NWScript.GetSkillRank(NWScript.SKILL_SEARCH, oPC.oid), NWScript.GetSkillRank(NWScript.SKILL_SPOT, oPC.oid),
        NWScript.GetSkillRank(NWScript.SKILL_BLUFF, oPC.oid) };

                    int[] iPerceivedHideSkill = { NWScript.GetSkillRank(NWScript.SKILL_BLUFF, oPerceived.oid), NWScript.GetSkillRank(NWScript.SKILL_HIDE, oPerceived.oid),
        NWScript.GetSkillRank(NWScript.SKILL_PERFORM, oPerceived.oid), NWScript.GetSkillRank(NWScript.SKILL_PERSUADE, oPerceived.oid) };

                    Random d20 = NWN.Utils.random;
                    int iRollAttack = iPCSenseSkill.Max() + d20.Next(21);
                    int iRollDefense = iPerceivedHideSkill.Max() + d20.Next(21);

                    if (iRollAttack > iRollDefense)
                    {
                        NWScript.SendMessageToPC(oPC.oid, NWScript.GetName(oPerceived.oid) + " fait usage d'un déguisement ! Sous le masque, vous reconnaissez " + NWScript.GetName(oPerceived.oid, 1));
                        //NWNX_Rename_ClearPCNameOverride(oPerceived, oPC);
                    }
                }
            }
        }

        return 0;
    }*/
  }
}
