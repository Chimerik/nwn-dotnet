using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      public class FicheDePersoDetailsWindow : PlayerWindow
      {
        private readonly NuiColumn rootColumn = new();
        private readonly NuiGroup resistanceGroup = new() { Id = "resistanceGroup", Border = false };
        private readonly NuiRow resistanceRow = new() { Margin = 0.0f, Children = new List<NuiElement>() };
        private readonly NuiGroup classGroup = new() { Id = "classGroup", Border = true };
        private readonly NuiRow classRow = new() { Margin = 0.0f, Children = new List<NuiElement>() };
        private readonly NuiGroup backgroundGroup = new() { Id = "backgroundGroup", Border = true };
        private readonly NuiRow backgroundRow = new() { Margin = 0.0f, Children = new List<NuiElement>() };
        private readonly List<NuiElement> rootChildren = new();
        private readonly NuiBind<string> raceIcon = new("raceIcon");
        private readonly NuiBind<string> raceName = new("raceName");

        private readonly NuiBind<string> backgroundIcon = new("backgroundIcon");
        private readonly NuiBind<string> backgroundName = new("backgroundName");

        private readonly NuiBind<string> str = new("str");
        private readonly NuiBind<string> dex = new("dex");
        private readonly NuiBind<string> con = new("con");
        private readonly NuiBind<string> intel = new("intel");
        private readonly NuiBind<string> wis = new("wis");
        private readonly NuiBind<string> cha = new("cha");

        private readonly NuiBind<string> hp = new("hp");
        private readonly NuiBind<string> ac = new("ac");
        private readonly NuiBind<string> init = new("init");
        private readonly NuiBind<string> proficiency = new("proficiency");
        private readonly NuiBind<string> mainDamage = new("mainDamage");
        private readonly NuiBind<string> mainCriticalRange = new("mainCriticalRange");
        private readonly NuiBind<string> secondaryDamage = new("secondaryDamage");
        private readonly NuiBind<string> secondaryCriticalRange = new("secondaryCriticalRange");

        private readonly NuiBind<string> strSave = new("strSave");
        private readonly NuiBind<string> dexSave = new("dexSave");
        private readonly NuiBind<string> conSave = new("conSave");
        private readonly NuiBind<string> intSave = new("intSave");
        private readonly NuiBind<string> wisSave = new("wisSave");
        private readonly NuiBind<string> chaSave = new("chaSave");

        // List de Row qui contient une list de colonnes ?

        private readonly NuiBind<int> resistanceListCount = new("resistanceListCount");
        private readonly NuiBind<string> resistanceIcon = new("resistanceIcon");
        private readonly NuiBind<string> resistanceName = new("resistanceName");

        private readonly NuiBind<int> conditionsListCount = new("conditionsListCount");
        private readonly NuiBind<string> conditionIcon = new("conditionIcon");
        private readonly NuiBind<string> conditionName = new("conditionName");

        private NwCreature target;

        public FicheDePersoDetailsWindow(Player player, NwCreature target) : base(player)
        {
          windowId = "ficheDePersoDetails";
          windowWidth = player.guiScaledWidth * 0.3f;
          windowHeight = player.guiScaledHeight * 0.8f;
          rootColumn.Children = rootChildren;

          List<NuiListTemplateCell> conditionsTemplate = new List<NuiListTemplateCell>
          {
            new(new NuiSpacer()),
            new(new NuiButtonImage(conditionIcon) { }) {Width = windowWidth / 8},
            new(new NuiSpacer()),
            new(new NuiLabel(conditionName) { VerticalAlign = NuiVAlign.Middle }),
            new(new NuiSpacer()) ,
          };

          /*rootChildren.Add(new NuiRow() { Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiButton("Accueil") { Id = "welcome", Height = 35, Width = windowWidth / 7.5f, ForegroundColor = ColorConstants.Gray },
            new NuiButton("Race") { Id = "race", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_RACE").HasValue },
            new NuiButton("Portrait") { Id = "portrait", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_PORTRAIT").HasValue },
            new NuiButton("Couleurs") { Id = "beauty", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_APPEARANCE").HasValue },
            new NuiButton("Origine") { Id = "histo", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_ORIGIN").HasValue },
            new NuiButton("Classe") { Id = "class", Height = 35, Width = windowWidth / 7.5f, Encouraged = player.oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_IN_CHARACTER_CREATION_CLASS").HasValue },
            new NuiButton("Stats") { Id = "stats", Height = 35, Width = windowWidth / 7.5f , Encouraged = player.oid.LoginCreature.GetObjectVariable < PersistentVariableInt >("_IN_CHARACTER_CREATION_STATS").HasValue},
            new NuiSpacer()
          } });*/

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 7, Children = new List<NuiElement>() 
          {
            new NuiSpacer(),
            new NuiButtonImage(raceIcon) { Width = windowWidth / 8, Height = windowWidth / 8 },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 16, Children = new List<NuiElement>() {
            new NuiLabel(raceName) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
          } });

          classRow.Height = windowWidth / 4;
          classGroup.Height = windowWidth / 4;
          classGroup.Layout = classRow;
          rootChildren.Add(classGroup);

          backgroundRow.Height = windowWidth / 5;
          backgroundGroup.Height = windowWidth / 5;
          backgroundGroup.Layout = backgroundRow;
          rootChildren.Add(backgroundGroup);

          rootChildren.Add(new NuiRow() {  Margin = 0.0f, Height = windowWidth / 5, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>() 
              { 
                new NuiButtonImage("strength") { Id = "strRoll", Tooltip = "Force" },
                new NuiLabel(str) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("dexterity") { Id = "dexRoll", Tooltip = "Dextérité" },
                new NuiLabel(dex) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("constitution") { Id = "conRoll", Tooltip = "Constitution" },
                new NuiLabel(con) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("intelligence") { Id = "intRoll", Tooltip = "Intelligence" },
                new NuiLabel(intel) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("wisdom") { Id = "wisRoll", Tooltip = "Sagesse" },
                new NuiLabel(wis) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiButtonImage("charisma") { Id = "chaRoll", Tooltip = "Charisme" },
                new NuiLabel(cha) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
              } },
            new NuiSpacer()
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>()
          {
            new NuiColumn() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(hp) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } }  },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(init) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(mainDamage) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(secondaryDamage) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } }
            } },
            new NuiColumn() { Margin = 0.0f, Children = new List<NuiElement>()
            {
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(ac) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(proficiency) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(mainCriticalRange) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } },
              new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel(secondaryCriticalRange) { VerticalAlign = NuiVAlign.Middle, HorizontalAlign = NuiHAlign.Center, Height = windowWidth / 16 } } }
            } }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 16, Children = new List<NuiElement>()
          {
            new NuiLabel("Jets de Sauvegarde") { VerticalAlign = NuiVAlign.Bottom, HorizontalAlign = NuiHAlign.Center },
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 5, Children = new List<NuiElement>()
          {
            new NuiSpacer(),
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("strength") { Id = "strSaveRoll", Tooltip = "Force" },
              new NuiLabel(strSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("dexterity") { Id = "dexSaveRoll", Tooltip = "Dextérité" },
              new NuiLabel(dexSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("constitution") { Id = "conSaveRoll", Tooltip = "Constitution" },
              new NuiLabel(conSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("intelligence") { Id = "intSaveRoll", Tooltip = "Intelligence" },
              new NuiLabel(intSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("wisdom") { Id = "wisSaveRoll", Tooltip = "Sagesse" },
              new NuiLabel(wisSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
            {
              new NuiButtonImage("charisma") { Id = "chaSaveRoll", Tooltip = "Charisme" },
              new NuiLabel(chaSave) { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center }
            } },
            new NuiSpacer()
          } });

          /*rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 16, Children = new List<NuiElement>()
          {
            new NuiLabel("Résistances") { VerticalAlign = NuiVAlign.Bottom, HorizontalAlign = NuiHAlign.Center }
          } });

          resistanceGroup.Layout = resistanceRow;
          rootChildren.Add(resistanceGroup);          

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Height = windowWidth / 16, Children = new List<NuiElement>()
          {
            new NuiLabel("Conditions") { VerticalAlign = NuiVAlign.Bottom, HorizontalAlign = NuiHAlign.Center }
          } });

          rootChildren.Add(new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiList(conditionsTemplate, conditionsListCount) { RowHeight = windowWidth / 8 } } });
          */
          CreateWindow(target);
        }

        public void CreateWindow(NwCreature target)
        {
          this.target = target;
          /*ModuleSystem.Log.Info($"windowWidth : {windowWidth}");
          ModuleSystem.Log.Info($"windowHeight : {windowHeight}");
          ModuleSystem.Log.Info($"player.guiWidth : {player.guiWidth}");
          ModuleSystem.Log.Info($"player.guiHeight : {player.guiHeight}");
          ModuleSystem.Log.Info($"player.guiScaledWidth : {player.guiScaledWidth}");
          ModuleSystem.Log.Info($"player.guiScaledHeight : {player.guiScaledHeight}");*/
          NuiRect savedRectangle = player.windowRectangles.TryGetValue(windowId, out var value) ? value : new NuiRect(0, 0, windowWidth, windowHeight);

          window = new NuiWindow(rootColumn, $"{target.Name} - Fiche de perso")
          {
            Geometry = geometry,
            Resizable = false,
            Collapsed = false,
            Closable = true,
            Transparent = false,
            Border = true,
          };

          if (player.oid.TryCreateNuiWindow(window, out NuiWindowToken tempToken, windowId))
          {
            nuiToken = tempToken;
            nuiToken.OnNuiEvent += HandleIntroMirrorEvents;

            raceIcon.SetBindValue(player.oid, nuiToken.Token, Races2da.raceTable[target.Race.Id].icon);
            raceName.SetBindValue(player.oid, nuiToken.Token, target.Race.Name);

            List<string> iconList = new();
            List<string> nameList = new();
            List<string> levelList = new();

            classRow.Children.Clear();
            classRow.Children.Add(new NuiSpacer());

            foreach (var playerClass in target.Classes)
            {
              if (playerClass.Class.Id == CustomClass.Adventurer)
                continue;

              classRow.Children.Add(new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
              {
                new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiButtonImage(playerClass.Class.IconResRef) { Width = windowWidth / 8, Tooltip = $"{playerClass.Level} {playerClass.Class.Name}" } } },
                new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{playerClass.Class.Name}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Tooltip = $"{playerClass.Level} {playerClass.Class.Name}" } } },
                new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{playerClass.Level}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center, Tooltip = $"{playerClass.Level} {playerClass.Class.Name}" } } }
              } });

              classRow.Children.Add(new NuiSpacer());
            }

            classGroup.SetLayout(player.oid, nuiToken.Token, classRow);

            backgroundRow.Children.Clear();
            backgroundRow.Children.Add(new NuiSpacer());

            if (Players.TryGetValue(target, out var targetPlayer))
            {
              foreach (var learnable in targetPlayer.learnableSkills.Values.Where(s => s.category == SkillSystem.Category.StartingTraits))
              {
                backgroundRow.Children.Add(new NuiColumn() { Margin = 0.0f, Width = windowWidth / 8, Children = new List<NuiElement>()
                {
                  new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiButtonImage(learnable.icon) { Width = windowWidth / 8 } } },
                  new NuiRow() { Margin = 0.0f, Children = new List<NuiElement>() { new NuiLabel($"{learnable.name}") { VerticalAlign = NuiVAlign.Top, HorizontalAlign = NuiHAlign.Center } } },
                }});

                backgroundRow.Children.Add(new NuiSpacer());
              }
            }

            backgroundGroup.SetLayout(player.oid, nuiToken.Token, backgroundRow);

            str.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Strength).ToString());
            dex.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Dexterity).ToString());
            con.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Constitution).ToString());
            intel.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Intelligence).ToString());
            wis.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Wisdom).ToString());
            cha.SetBindValue(player.oid, nuiToken.Token, target.GetAbilityScore(Ability.Charisma).ToString());

            hp.SetBindValue(player.oid, nuiToken.Token, $"Points de vie : {target.HP} / {target.MaxHP}");
            ac.SetBindValue(player.oid, nuiToken.Token, $"Classe d'armure : {target.AC}");
            init.SetBindValue(player.oid, nuiToken.Token, $"Initiative : {target.GetAbilityModifier(Ability.Dexterity)}");
            proficiency.SetBindValue(player.oid, nuiToken.Token, $"Bonus de maîtrise : {NativeUtils.GetCreatureProficiencyBonus(target)}");

            NwItem mainWeapon = target.GetItemInSlot(InventorySlot.RightHand);
            NwItem secondaryWeapon = target.GetItemInSlot(InventorySlot.LeftHand);

            if (mainWeapon is not null && ItemUtils.IsWeapon(mainWeapon.BaseItem))
            {
              int damageDie = NativeUtils.GetShillelaghDamageDie(target, mainWeapon.BaseItem);
              damageDie = ItemUtils.IsVersatileWeapon(mainWeapon.BaseItem.ItemType) && secondaryWeapon is null ? damageDie + 2 : damageDie;

              int numDamageDice = NativeUtils.GetShillelaghNumDice(target, mainWeapon.BaseItem);

              mainDamage.SetBindValue(player.oid, nuiToken.Token, $"Dégâts : {numDamageDice}d{damageDie}");
              mainCriticalRange.SetBindValue(player.oid, nuiToken.Token, $"Critique : {NativeUtils.GetCriticalRange(target, mainWeapon, !ItemUtils.IsMeleeWeapon(mainWeapon.BaseItem))} - 20");
            }
            else
            {
              mainDamage.SetBindValue(player.oid, nuiToken.Token, $"Dégâts : {1}d{CreatureUtils.GetUnarmedDamage(target)}");
              mainCriticalRange.SetBindValue(player.oid, nuiToken.Token, $"Critique : 20 - 20");
            }


            if (secondaryWeapon is not null && ItemUtils.IsWeapon(secondaryWeapon.BaseItem))
            {
              int damageDie = NativeUtils.GetShillelaghDamageDie(target, secondaryWeapon.BaseItem);
              int numDamageDice = NativeUtils.GetShillelaghNumDice(target, secondaryWeapon.BaseItem);

              secondaryDamage.SetBindValue(player.oid, nuiToken.Token, $"Main secondaire : {numDamageDice}d{damageDie}");
              secondaryCriticalRange.SetBindValue(player.oid, nuiToken.Token, $"Critique : {NativeUtils.GetCriticalRange(target, secondaryWeapon, !ItemUtils.IsMeleeWeapon(mainWeapon.BaseItem))} - 20");
            }
            else
            {
              secondaryDamage.SetBindValue(player.oid, nuiToken.Token, "Main secondaire : -");
              secondaryCriticalRange.SetBindValue(player.oid, nuiToken.Token, "Critique : -");
            }

            // TODO : penser à prendre en compte les effets et items

            var saveBonus = target.GetAbilityModifier(Ability.Strength) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Strength);
            strSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
            saveBonus = target.GetAbilityModifier(Ability.Dexterity) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Dexterity);
            dexSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
            saveBonus = target.GetAbilityModifier(Ability.Constitution) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Constitution);
            conSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
            saveBonus = target.GetAbilityModifier(Ability.Intelligence) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Intelligence);
            intSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
            saveBonus = target.GetAbilityModifier(Ability.Wisdom) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Wisdom);
            wisSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");
            saveBonus = target.GetAbilityModifier(Ability.Charisma) + SpellUtils.GetSavingThrowProficiencyBonus(target, Ability.Charisma);
            chaSave.SetBindValue(player.oid, nuiToken.Token, saveBonus > 0 ? $"+{saveBonus}" : $"{saveBonus}");

            /*iconList.Clear();
            nameList.Clear();

            resistanceRow.Children.Clear();
            resistanceRow.Children.Add(new NuiList(resistanceTemplate, resistanceListCount) { RowHeight = windowWidth / 8 });

            foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
            {
              if(iconList.Count > 5)
              {
                resistanceIcon.SetBindValues(player.oid, nuiToken.Token, iconList);
                resistanceName.SetBindValues(player.oid, nuiToken.Token, nameList);
                resistanceListCount.SetBindValue(player.oid, nuiToken.Token, iconList.Count);

                iconList.Clear();
                nameList.Clear();
              }

              if(target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetImmunityEffectTagByDamageType(damageType)))
              {
                iconList.Add(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageImmunityEffectIcon[damageType]).Icon);
                nameList.Add(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageImmunityEffectIcon[damageType]).StrRef.ToString());
              }

              if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetResistanceEffectTagByDamageType(damageType)))
              {
                iconList.Add(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageResistanceEffectIcon[damageType]).Icon);
                nameList.Add(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageResistanceEffectIcon[damageType]).StrRef.ToString());
              }

              if (target.ActiveEffects.Any(e => e.IntParams[0] == EffectUtils.GetVulnerabilityEffectTagByDamageType(damageType)))
              {
                iconList.Add(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageVulnerabilityEffectIcon[damageType]).Icon);
                nameList.Add(NwGameTables.EffectIconTable.GetRow((int)DamageType2da.damageVulnerabilityEffectIcon[damageType]).StrRef.ToString());
              }
            }

            resistanceIcon.SetBindValues(player.oid, nuiToken.Token, iconList);
            resistanceName.SetBindValues(player.oid, nuiToken.Token, nameList);
            resistanceListCount.SetBindValue(player.oid, nuiToken.Token, iconList.Count);

            resistanceGroup.SetLayout(player.oid, nuiToken.Token, resistanceRow);

            iconList.Clear();
            nameList.Clear();

            foreach(var eff in target.ActiveEffects)
            {
              if(eff.EffectType == EffectType.Icon && !DamageType2da.damageResistanceEffectIcon.ContainsValue((EffectIcon)eff.IntParams[0]))
              {
                iconList.Add(NwGameTables.EffectIconTable.GetRow(eff.IntParams[0]).Icon);
                nameList.Add(NwGameTables.EffectIconTable.GetRow(eff.IntParams[0]).StrRef.ToString());
              }
            }

            conditionIcon.SetBindValues(player.oid, nuiToken.Token, iconList);
            conditionName.SetBindValues(player.oid, nuiToken.Token, nameList);
            conditionsListCount.SetBindValue(player.oid, nuiToken.Token, iconList.Count);
            */
            geometry.SetBindValue(player.oid, nuiToken.Token, new NuiRect(savedRectangle.X, savedRectangle.Y, windowWidth, windowHeight));
            geometry.SetBindWatch(player.oid, nuiToken.Token, true);
          }
        }
        private void HandleIntroMirrorEvents(ModuleEvents.OnNuiEvent nuiEvent)
        {
          switch (nuiEvent.EventType)
          {
            case NuiEventType.Click:

              switch (nuiEvent.ElementId)
              {
                case "beauty":

                  CloseWindow();

                  if (!player.windows.TryGetValue("bodyColorsModifier", out var body)) player.windows.Add("bodyColorsModifier", new BodyColorWindow(player, player.oid.LoginCreature));
                  else ((BodyColorWindow)body).CreateWindow(player.oid.LoginCreature);

                  break;

                case "class":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introClassSelector", out var classe)) player.windows.Add("introClassSelector", new IntroClassSelectorWindow(player));
                  else ((IntroClassSelectorWindow)classe).CreateWindow();

                  break;

                case "race":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introRaceSelector", out var race)) player.windows.Add("introRaceSelector", new IntroRaceSelectorWindow(player));
                  else ((IntroRaceSelectorWindow)race).CreateWindow();

                  break;

                case "portrait":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introPortrait", out var portrait)) player.windows.Add("introPortrait", new IntroPortraitWindow(player));
                  else ((IntroPortraitWindow)portrait).CreateWindow();

                  break;

                case "histo":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introHistorySelector", out var histo)) player.windows.Add("introHistorySelector", new IntroHistorySelectorWindow(player));
                  else ((IntroHistorySelectorWindow)histo).CreateWindow();

                  break;

                case "stats":

                  CloseWindow();

                  if (!player.windows.TryGetValue("introAbilities", out var stats)) player.windows.Add("introAbilities", new IntroAbilitiesWindow(player));
                  else ((IntroAbilitiesWindow)stats).CreateWindow();

                  break;
              }

              break; 
          }
        }
      }
    }
  }
}
