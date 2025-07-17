using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Google.Apis.Drive.v3.Data;
using static Anvil.API.Events.ModuleEvents;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class MainMenuWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly List<NuiElement> rootChildren = new();
        private readonly List<NuiListTemplateCell> rowTemplate = new();
        private readonly NuiBind<string> buttonName = new("buttonName");
        private readonly NuiBind<string> buttonTooltip = new("buttonTooltip");
        private readonly NuiBind<int> listCount = new("listCount");
        private readonly NuiBind<string> search = new("search");

        //private NwObject selectionTarget;

        private Dictionary<string, Utils.MainMenuCommand> myCommandList = new();
        private Dictionary<string, Utils.MainMenuCommand> currentList;

        public MainMenuWindow(Player player) : base(player)
        {
          windowId = "mainMenu";

          rootColumn.Children = rootChildren;
          rowTemplate.Add(new NuiListTemplateCell(new NuiButton(buttonName) { Id = "command", Tooltip = buttonTooltip, Height = 35 }) { VariableSize = true });
          rootChildren.Add(new NuiRow() { Children = new List<NuiElement>() { new NuiTextEdit("Recherche", search, 50, false) { Width = 370 } } });
          rootChildren.Add(new NuiRow() { Height = 385, Children = new List<NuiElement>() { new NuiList(rowTemplate, listCount) { RowHeight = 35 } } });

          CreateWindow();
        }
        public void CreateWindow()
        {
          myCommandList.Clear();

          if (player.oid.PlayerName == "Chim" || player.oid.PlayerName == "dodozik" || player.oid.PlayerName == "WingsOfJoy")
            myCommandList = Utils.mainMenuCommands.ToDictionary(m => m.Key, m => m.Value);
          else if (player.oid.IsDM)
            myCommandList = Utils.mainMenuCommands.Where(m => m.Value.rank < Utils.CommandRank.Admin).ToDictionary(m => m.Key, m => m.Value);
          else
            myCommandList = Utils.mainMenuCommands.Where(m => m.Value.rank < Utils.CommandRank.DM).ToDictionary(m => m.Key, m => m.Value);

          NuiRect windowRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(10, player.oid.GetDeviceProperty(PlayerDeviceProperty.GuiHeight) * 0.01f, 410, 500);

          window = new NuiWindow(rootColumn, "Menu principal")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = collapsed,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleMainMenuEvents;

            search.SetBindValue(player.oid, nuiToken.Token, "");
            search.SetBindWatch(player.oid, nuiToken.Token, true);

            collapsed.SetBindValue(player.oid, nuiToken.Token, false);
            geometry.SetBindValue(player.oid, nuiToken.Token, windowRectangle);
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);

            if (myCommandList.TryGetValue("instantLearn", out var instantLearn))
              instantLearn.label = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_INSTANT_LEARN").HasValue ?
                "Désactiver l'apprentissage instantané (Alpha)" : "Activer l'apprentissage instantané (Alpha)";

            currentList = myCommandList;
            LoadMenu(currentList);
          }
        }
        private void HandleMainMenuEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          try
          {
            switch (nuiEvent.EventType)
            {
              case NuiEventType.Click:

                if (nuiEvent.ArrayIndex < 0)
                  return;

                switch (currentList.Keys.ElementAt(nuiEvent.ArrayIndex))
                {
                  case "grimoire":

                    if (!player.windows.TryGetValue("grimoires", out var grimoires)) player.windows.Add("grimoires", new GrimoiresWindow(player));
                    else ((GrimoiresWindow)grimoires).CreateWindow();

                    CloseWindow();

                    break;
                  
                  case "dispelAoE":

                    if (!player.windows.TryGetValue("aoeDispel", out var dispelAoe)) player.windows.Add("aoeDispel", new AoEDispelWindow(player));
                    else ((AoEDispelWindow)dispelAoe).CreateWindow();

                    CloseWindow();

                    break;

                  case "effectDispel":

                    if (!player.windows.TryGetValue("effectDispel", out var dispel)) player.windows.Add("effectDispel", new PlayerEffectDispelWindow(player));
                    else ((PlayerEffectDispelWindow)dispel).CreateWindow();

                    CloseWindow();

                    break;

                  /*case "healthManaBars":

                    if (!player.windows.ContainsKey("healthBar")) player.windows.Add("healthBar", new HealthBarWindow(player));
                    else ((HealthBarWindow)player.windows["healthBar"]).CreateWindow();

                    if (!player.windows.ContainsKey("energyBar")) player.windows.Add("energyBar", new EnergyBarWindow(player));
                    else ((EnergyBarWindow)player.windows["energyBar"]).CreateWindow();

                    CloseWindow();

                    break;*/

                  

                  case "cooldownPosition":

                    if (!player.windows.TryGetValue("cooldownPosition", out var cooldown)) player.windows.Add("cooldownPosition", new CooldownPositionSetter(player));
                    else ((CooldownPositionSetter)cooldown).CreateWindow();

                    CloseWindow();

                    break;

                  case "lootEditor":

                    if (!player.windows.TryGetValue("lootEditor", out var lootEditor)) player.windows.Add("lootEditor", new LootEditorWindow(player));
                    else ((LootEditorWindow)lootEditor).CreateWindow();

                    CloseWindow();

                    break;

                  case "shortRest":

                    //CreatureUtils.HandleShortRest(player);
                    //SpellUtils.DispelConcentrationEffects(player.oid.LoginCreature);

                    break;

                  case "longRest":

                    /*var nbInspiHeroique = player.oid.LoginCreature.GetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique);

                    if (Utils.In(player.oid.LoginCreature.Race.RacialType, RacialType.Human, (RacialType)CustomRace.WoodHalfElf, (RacialType)CustomRace.DrowHalfElf, (RacialType)CustomRace.HighHalfElf)
                      && nbInspiHeroique < 3)
                      nbInspiHeroique += 1;

                    player.shortRest = 0;

                    player.oid.LoginCreature.ForceRest();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(EffectSystem.EvocateurSurchargeVariable).Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>("_ILLUSION_SEE_INVI_COOLDOWN").Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(Wizard.TransmutationStoneVariable).Delete();
                    player.oid.LoginCreature.GetObjectVariable<LocalVariableString>(CreatureUtils.CharmeInstinctifVariable).Delete();

                    RangerUtils.RestoreEnnemiJure(player.oid.LoginCreature);
                    FighterUtils.RestoreManoeuvres(player.oid.LoginCreature);
                    FighterUtils.RestoreTirArcanique(player.oid.LoginCreature);
                    FighterUtils.RestoreSecondSouffle(player.oid.LoginCreature);
                    BarbarianUtils.RestoreBarbarianRage(player.oid.LoginCreature);
                    MonkUtils.RestoreKi(player.oid.LoginCreature);
                    WizardUtils.RestaurationArcanique(player.oid.LoginCreature);
                    DruideUtils.RecuperationNaturelle(player.oid.LoginCreature);
                    RangerUtils.RegenerationNaturelle(player.oid.LoginCreature);
                    WizardUtils.ResetAbjurationWard(player.oid.LoginCreature);
                    WizardUtils.ResetPresage(player.oid);
                    FighterUtils.RestoreEldritchKnight(player.oid.LoginCreature);
                    BardUtils.RestoreInspirationBardique(player.oid.LoginCreature);
                    PaladinUtils.RestorePaladinCharges(player.oid.LoginCreature);
                    ClercUtils.RestoreConduitDivin(player.oid.LoginCreature);
                    ClercUtils.RestoreInterventionDivine(player.oid.LoginCreature);
                    ClercUtils.RestoreClercDomaine(player.oid.LoginCreature);
                    EnsoUtils.RestoreSorcerySource(player.oid.LoginCreature);
                    DruideUtils.RestoreFormeSauvage(player.oid.LoginCreature);
                    OccultisteUtils.RestoreFouleeFeerique(player.oid.LoginCreature);
                    OccultisteUtils.RestoreLueurDeGuerison(player.oid.LoginCreature);
                    OccultisteUtils.HandleResilienceCeleste(player.oid.LoginCreature);
                    player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.DonDuProtecteur, (byte)(player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) > 1 ? player.oid.LoginCreature.GetAbilityModifier(Ability.Charisma) : 1));

                    if (player.oid.LoginCreature.Race.Id == CustomRace.HalfOrc)
                    {
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Value = 1;
                      player.ApplyHalfOrcEndurance();
                    }

                    if (player.learnableSkills.ContainsKey(CustomSkill.PaladinSentinelleImmortelle))
                    {
                      player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>(EffectSystem.SentinelleImmortelleVariable).Value = 1;
                      player.ApplySentinelleImmortelle();
                    }

                    if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerFrenziedStrike))
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BersekerFrenziedStrike, 0);

                    if (player.oid.LoginCreature.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Fighter, (ClassType)CustomClass.EldritchKnight) && c.Level < 17))
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.FighterSurge, 1);
                     
                    player.oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.CanPrepareSpells, NwTimeSpan.FromHours(1));

                    foreach (var classInfo in player.oid.LoginCreature.Classes.Where(c => c.Class.IsSpellCaster))
                    {
                      var spellGainTable = classInfo.Class.SpellGainTable[classInfo.Level - 1];
                      byte i = 0;

                      foreach(var spellGain in spellGainTable)
                      {
                        classInfo.SetRemainingSpellSlots(i, spellGain);
                        i++;
                      }
                    }

                    if(player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentDivin))
                    {
                      byte chatimentLevel = (byte)(player.windows.TryGetValue("chatimentLevelSelection", out var chatimentWindow) 
                        ? ((ChatimentLevelSelectionWindow)chatimentWindow).selectedSpellLevel : 1);
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentDivin, player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).GetRemainingSpellSlots(chatimentLevel));
                    }

                    player.oid.LoginCreature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ClercMartialVariable).Delete();

                    EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.DivinationVisionEffectTag);
                    SpellUtils.DispelConcentrationEffects(player.oid.LoginCreature);

                    if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ChatimentOcculte))
                      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ChatimentOcculte, (byte)(player.oid.LoginCreature.GetClassInfo((ClassType)CustomClass.Occultiste).GetRemainingSpellSlots(1)));

                    player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ClercIncantationPuissante, 0);
                    player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.InspirationHeroique, nbInspiHeroique);
                    */
                    break;
                }
                break;

              case NuiEventType.Watch:

                switch (nuiEvent.ElementId)
                {
                  case "search":

                    string currentSearch = search.GetBindValue(player.oid, nuiToken.Token).ToLower();
                    currentList = string.IsNullOrEmpty(currentSearch) ? myCommandList : myCommandList.Where(v => v.Value.label.ToLower().Contains(currentSearch)).ToDictionary(c => c.Key, c => c.Value);
                    LoadMenu(currentList);

                    break;
                }

                break;
            }
          }
          catch(Exception e)
          {
            Utils.LogMessageToDMs($"{e.Message}\n{e.StackTrace}\narray index : {nuiEvent.ArrayIndex} - key collection count {currentList.Count}");
          }
        }
        private void LoadMenu(Dictionary<string, Utils.MainMenuCommand> commandList)
        {
          buttonName.SetBindValues(player.oid, nuiToken.Token, commandList.Values.Select(c => c.label));
          buttonTooltip.SetBindValues(player.oid, nuiToken.Token, commandList.Values.Select(c => c.tooltip));
          listCount.SetBindValue(player.oid, nuiToken.Token, commandList.Count);
        }
        private static bool AreaDescriptionExists(NwPlayer oPlayer)
        {
          try
          {
            var request = ModuleSystem.googleDriveService.Files.List();
            request.Q = $"name = '{oPlayer.ControlledCreature.Area.Name.Replace("'", "")}'";

            LogUtils.LogMessage($"Area - {oPlayer.ControlledCreature.Area.Name} - Description request - {oPlayer.ControlledCreature.Name} - {request.Q}", LogUtils.LogType.AreaManagement);

            FileList list = request.Execute();

            if (list.Files.Count > 0)
              return true;
            else
              return false;
          }
          catch(Exception e)
          {
            Utils.LogMessageToDMs($"{e.Message}\n{e.StackTrace}");
            return false;
          }
        }
      }

      private static void FollowTarget(OnPlayerTarget selection)
      {
        if (selection.IsCancelled || selection.TargetObject is not NwCreature target)
          return;

        if (selection.Player.ControlledCreature.MovementRate == MovementRate.Immobile
            || Encumbrance2da.IsCreatureHeavilyEncumbred(selection.Player.ControlledCreature))
        {
          selection.Player.SendServerMessage("Cette commande ne peut être utilisée en étant surchargé.", ColorConstants.Red);
          return;
        }

        if (selection.Player.ControlledCreature.Area != target.Area || selection.Player.ControlledCreature.DistanceSquared(target) > 2000)
        {
          selection.Player.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} est trop éloigné pour que vous puissiez le suivre.", ColorConstants.Red);
          return;
        }

        _  = selection.Player.ControlledCreature.ActionForceFollowObject(target, 3.0f);
      }

      private static void CommendTarget(OnPlayerTarget selection)
      {
        if (selection.IsCancelled || selection.TargetObject is not NwCreature target 
          || !Players.TryGetValue(target, out Player commendedPlayer) || commendedPlayer.oid.IsDM)
          return;

        if (selection.Player.IsDM)
        {
          if (commendedPlayer.bonusRolePlay > 3)
            return;

          commendedPlayer.bonusRolePlay += 1;
          commendedPlayer.oid.SendServerMessage($"Votre bonus roleplay est désormais de {commendedPlayer.bonusRolePlay}", new Color(32, 255, 32));
          commendedPlayer.oid.ExportCharacter();

          LogUtils.LogMessage($"{selection.Player.LoginCreature.Name} vient d'augmenter le bonus d'investissement de {commendedPlayer.oid.LoginCreature.Name} à {commendedPlayer.bonusRolePlay}", LogUtils.LogType.DMAction);
        }
        else
        {
          if (commendedPlayer.bonusRolePlay < 4)
          {
            LogUtils.LogMessage($"{selection.Player.LoginCreature.Name} vient de recommander {commendedPlayer.oid.LoginCreature.Name} pour une augmentation de bonus d'investissement.", LogUtils.LogType.DMAction);
            commendedPlayer.oid.SendServerMessage("Vous venez d'obtenir une recommandation pour une augmentation de bonus d'investissement !", ColorConstants.Rose);

            if (commendedPlayer.bonusRolePlay == 1)
            {
              LogUtils.LogMessage($"{selection.Player.LoginCreature.Name} vient de faire passer le bonus d'investissement de {commendedPlayer.oid.LoginCreature.Name} à 2", LogUtils.LogType.DMAction);
              commendedPlayer.bonusRolePlay = 2;
              commendedPlayer.oid.SendServerMessage("Votre bonus roleplay est désormais de 2", new Color(32, 255, 32));
              commendedPlayer.oid.ExportCharacter();
            }
          }

          selection.Player.SendServerMessage($"Vous venez de recommander {commendedPlayer.oid.LoginCreature.Name.ColorString(ColorConstants.White)} pour une augmentation de bonus d'investissement !", ColorConstants.Rose);
        }
      }

      private static void RenameTarget(OnPlayerTarget selection)
      {
        if (selection.IsCancelled || !Players.TryGetValue(selection.Player.LoginCreature, out Player player) || selection.TargetObject is not NwGameObject target)
          return;

        if (player.windows.TryGetValue("playerInput", out var rename))
          ((PlayerInputWindow)rename).CreateWindow("Quel nom ?", DmRename, target);
        else
          player.windows.Add("playerInput", new PlayerInputWindow(player, "Quel nom ?", DmRename, target));
      }
      private static bool DmRename(Player player, NwGameObject target, string inputValue)
      {
        if (target == null)
          return false;

        player.oid.SendServerMessage($"{target.Name.ColorString(ColorConstants.White)} a été renommé {inputValue.ColorString(ColorConstants.White)}.", ColorConstants.Green);
        target.Name = inputValue;

        return true;
      }
    }
  }
}
