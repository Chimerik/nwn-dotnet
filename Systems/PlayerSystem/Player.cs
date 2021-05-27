using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;
using NWN.Systems.Craft;
using NWN.API;
using System.Threading.Tasks;
using Skill = NWN.Systems.SkillSystem.Skill;
using NWN.API.Constants;
using System.Threading;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public class Player
    {
      public NwPlayer oid { get; set; }
      public DateTime mapLoadingTime { get; set; }
      public readonly int accountId;
      public readonly int characterId;
      public API.Location location { get; set; }
      public int bonusRolePlay { get; set; }
      public Boolean isAFK { get; set; }
      public Boolean DoJournalUpdate { get; set; }
      public int currentHP { get; set; }
      public int bankGold { get; set; }
      public PlayerJournal playerJournal { get; set; }
      public DateTime dateLastSaved { get; set; }
      public int currentSkillJob { get; set; }
      public SkillType currentSkillType { get; set; }
      public Job craftJob { get; set; }
      public Boolean isFrostAttackOn { get; set; }
      public API.Location previousLocation { get; set; }
      public TargetEvent targetEvent { get; set; }
      public Menu menu { get; }
      public NwCreature deathCorpse { get; set; }
      public QuickbarType loadedQuickBar { get; set; }
      public string serializedQuickbar { get; set; }
      public Arena.PlayerData pveArena { get; set; }

      public List<NwPlayer> listened = new List<NwPlayer>();
      public Dictionary<uint, Player> blocked = new Dictionary<uint, Player>();
      public Dictionary<Feat, int> learntCustomFeats = new Dictionary<Feat, int>();
      public Dictionary<Feat, Skill> learnableSkills = new Dictionary<Feat, Skill>();
      public Dictionary<int, LearnableSpell> learnableSpells = new Dictionary<int, LearnableSpell>();
      public Dictionary<Feat, Skill> removeableMalus = new Dictionary<Feat, Skill>();
      public Dictionary<string, int> materialStock = new Dictionary<string, int>();
      public List<API.Effect> effectList = new List<API.Effect>();
      public Dictionary<int, MapPin> mapPinDictionnary = new Dictionary<int, MapPin>();
      public Dictionary<string, string> areaExplorationStateDictionnary = new Dictionary<string, string>();
      public Dictionary<int, Color> chatColors = new Dictionary<int, Color>();

      public Player(NwPlayer nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
        this.pveArena = new Arena.PlayerData();

        if (ObjectPlugin.GetInt(this.oid.LoginCreature, "accountId") == 0 && !oid.IsDM)
          InitializeNewPlayer(this.oid);

        this.accountId = ObjectPlugin.GetInt(this.oid.LoginCreature, "accountId");

        if (ObjectPlugin.GetInt(this.oid.LoginCreature, "characterId") == 0 && !oid.IsDM)
          InitializeNewCharacter(this);

        this.characterId = ObjectPlugin.GetInt(this.oid.LoginCreature, "characterId");

        if (!Convert.ToBoolean(NWScript.GetIsDM(this.oid.LoginCreature)))
          InitializePlayer(this);
        else
          InitializeDM(this);

        Log.Info($"Player first initialization : DONE");
      }

      public void EmitKeydown(MenuFeatEventArgs e)
      {
        OnKeydown(this, e);
      }

      public event EventHandler<MenuFeatEventArgs> OnKeydown = delegate { };

      public class MenuFeatEventArgs : EventArgs
      {
        public Feat feat { get; }

        public MenuFeatEventArgs(Feat feat)
        {
          this.feat = feat;
        }
      }
      public void LoadMenuQuickbar(QuickbarType type)
      {
        if (this.loadedQuickBar == QuickbarType.Invalid)
        {
          PlayerQuickBarButton emptyQBS = new PlayerQuickBarButton();

          switch (type)
          {
            case QuickbarType.Menu:

              this.serializedQuickbar = oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString();
              emptyQBS.ObjectType = QuickBarButtonType.Empty;

              for (byte i = 0; i < 12; i++)
              {
                oid.ControlledCreature.SetQuickBarButton(i, emptyQBS);
              }

              if (menu.choices.Count > 0)
              {
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuDOWN);
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuUP);
                oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuSELECT);

                emptyQBS.ObjectType = QuickBarButtonType.Feat;

                if (ObjectPlugin.GetInt(this.oid.LoginCreature, "_MENU_HOTKEYS_SWAPPED") == 0)
                {
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuDOWN;
                  oid.ControlledCreature.SetQuickBarButton(0, emptyQBS);
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuUP;
                  oid.ControlledCreature.SetQuickBarButton(1, emptyQBS);
                }
                else
                {
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuDOWN;
                  oid.ControlledCreature.SetQuickBarButton(1, emptyQBS);
                  emptyQBS.Param1 = (int)CustomFeats.CustomMenuUP;
                  oid.ControlledCreature.SetQuickBarButton(0, emptyQBS);
                }

                emptyQBS.Param1 = (int)CustomFeats.CustomMenuSELECT;
                oid.ControlledCreature.SetQuickBarButton(2, emptyQBS);
              }

              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuEXIT);

              emptyQBS.Param1 = (int)CustomFeats.CustomMenuEXIT;
              oid.ControlledCreature.SetQuickBarButton(3, emptyQBS);

              this.loadedQuickBar = QuickbarType.Menu;
              break;
            case QuickbarType.Sit:
              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuDOWN);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuUP);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionRight);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionLeft);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionForward);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionBackward);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionRotateRight);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomPositionRotateLeft);
              oid.ControlledCreature.AddFeat(CustomFeats.CustomMenuEXIT);

              this.serializedQuickbar = oid.ControlledCreature.SerializeQuickbar().ToBase64EncodedString();

              for (int i = 0; i < 12; i++)
                oid.ControlledCreature.SetQuickBarButton((byte)i, emptyQBS);

              emptyQBS.ObjectType = QuickBarButtonType.Feat;
              emptyQBS.Param1 = (int)CustomFeats.CustomMenuDOWN;
              oid.ControlledCreature.SetQuickBarButton(0, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomMenuUP;
              oid.ControlledCreature.SetQuickBarButton(1, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionLeft;
              oid.ControlledCreature.SetQuickBarButton(2, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionRight;
              oid.ControlledCreature.SetQuickBarButton(3, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionForward;
              oid.ControlledCreature.SetQuickBarButton(4, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionBackward;
              oid.ControlledCreature.SetQuickBarButton(5, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionRotateLeft;
              oid.ControlledCreature.SetQuickBarButton(6, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomPositionRotateRight;
              oid.ControlledCreature.SetQuickBarButton(7, emptyQBS);
              emptyQBS.Param1 = (int)CustomFeats.CustomMenuEXIT;
              oid.ControlledCreature.SetQuickBarButton(8, emptyQBS);

              this.loadedQuickBar = QuickbarType.Sit;
              this.OnKeydown += this.menu.HandleMenuFeatUsed;
              break;
          }
        }
      }
      public void UnloadMenuQuickbar()
      {
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuUP);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuDOWN);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuSELECT);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomMenuEXIT);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionLeft);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionRight);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionForward);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionBackward);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionRotateLeft);
        oid.ControlledCreature.RemoveFeat(CustomFeats.CustomPositionRotateRight);

        bool returned = oid.ControlledCreature.DeserializeQuickbar(this.serializedQuickbar.ToByteArray());
        loadedQuickBar = QuickbarType.Invalid;
      }
      public void CraftJobProgression()
      {
        if (craftJob.IsActive())
        {
          craftJob.remainingTime = craftJob.remainingTime - (float)(DateTime.Now - dateLastSaved).TotalSeconds;

          if (craftJob.remainingTime < 0)
          {
            Log.Info($"craft job done. Acquiring item - Type : {craftJob.type} - BaseItem : {craftJob.baseItemType}");
            AcquireCraftedItem();
          }
        }
      }

      public async void AcquireCraftedItem()
      {
        switch (craftJob.type)
        {
          case Job.JobType.BlueprintCopy:
            uint bpCopy = NWScript.CopyItem(ObjectPlugin.Deserialize((this.craftJob.craftedItem)), this.oid.LoginCreature, 1);
            NWScript.SetLocalInt(bpCopy, "_BLUEPRINT_RUNS", 10);
            NWScript.SetName(bpCopy, $"Copie de {NWScript.GetName(bpCopy)}");
            break;
          case Job.JobType.BlueprintResearchMaterialEfficiency:
            uint improvedMEBP = NWScript.CopyItem(ObjectPlugin.Deserialize((this.craftJob.craftedItem)), this.oid.LoginCreature, 1);
            NWScript.SetLocalInt(improvedMEBP, "_BLUEPRINT_MATERIAL_EFFICIENCY", NWScript.GetLocalInt(improvedMEBP, "_BLUEPRINT_MATERIAL_EFFICIENCY") + 1);
            break;
          case Job.JobType.BlueprintResearchTimeEfficiency:
            ((NwItem)NwItem.Deserialize(craftJob.craftedItem.ToByteArray())).Clone(oid.LoginCreature).GetLocalVariable<int>("_BLUEPRINT_TIME_EFFICIENCY").Value += 1;
            break;
          case Job.JobType.Enchantement:
            NwItem enchantedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            oid.LoginCreature.AcquireItem(enchantedItem);

            int enchanteurChanceuxLevel = 0;
            if (learntCustomFeats.ContainsKey(CustomFeats.EnchanteurChanceux))
              enchanteurChanceuxLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.EnchanteurChanceux, learntCustomFeats[CustomFeats.EnchanteurChanceux]);

            if (NwRandom.Roll(Utils.random, 100) > enchanteurChanceuxLevel)
            {
              enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
              if (enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
                enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();
            }

            int enchanteurExpertLevel = 0;
            if (learntCustomFeats.ContainsKey(CustomFeats.EnchanteurExpert))
              enchanteurExpertLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.EnchanteurExpert, learntCustomFeats[CustomFeats.EnchanteurExpert]);

            int boost = 0;
            if (NwRandom.Roll(Utils.random, 100) <= enchanteurExpertLevel * 2)
              boost = 1;

            Craft.Collect.System.AddCraftedEnchantementProperties(enchantedItem, craftJob.material, boost, characterId);

            break;
          case Job.JobType.EnchantementReactivation:

            NwItem reactivatedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            oid.LoginCreature.AcquireItem(reactivatedItem);

            API.ItemProperty reactivatedIP = reactivatedItem.ItemProperties.FirstOrDefault(ip => ip.Tag.StartsWith($"ENCHANTEMENT_{craftJob.material}") && ip.Tag.Contains("INACTIVE"));

            Task waitLoopEnd = NwTask.Run(async () =>
            {
              API.ItemProperty deactivatedIP = reactivatedIP;
              await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
              reactivatedItem.RemoveItemProperty(deactivatedIP);
              await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
              deactivatedIP.Tag = reactivatedIP.Tag.Replace("_INACTIVE", "");
              reactivatedItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
              await NwTask.Delay(TimeSpan.FromSeconds(0.1f));

              if (!reactivatedItem.ItemProperties.Any(ip => ip.Tag.Contains("_INACTIVE")) && reactivatedItem.GetLocalVariable<int>("_REPAIR_DONE").HasValue)
              {
                reactivatedItem.GetLocalVariable<int>("_DURABILITY").Value = reactivatedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value;
                reactivatedItem.GetLocalVariable<int>("_REPAIR_DONE").Delete();
                oid.SendServerMessage($"Réactivation de {reactivatedItem.Name.ColorString(Color.WHITE)} terminée. L'objet est comme neuf !", new Color(32, 255, 32));
              }
            });

            break;
          case Job.JobType.Recycling:
            NwItem recycledItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            int recycledValue = recycledItem.GetLocalVariable<int>("_BASE_COST").Value;

            if (learntCustomFeats.ContainsKey(CustomFeats.Recycler))
              recycledValue +=  recycledValue * 1 * GetCustomFeatLevelFromSkillPoints(CustomFeats.Recycler, learntCustomFeats[CustomFeats.Recycler]) / 100;

            if (materialStock.ContainsKey(craftJob.material))
              materialStock[craftJob.material] += recycledValue;
            else
              materialStock.Add(craftJob.material, recycledValue);

            oid.SendServerMessage($"Recyclage de {recycledItem.Name.ColorString(Color.WHITE)} terminé. Vous en retirez {recycledValue} unité(s) de {craftJob.material}", new Color(32, 255, 32)) ;
            recycledItem.Destroy();

            break;
          case Job.JobType.Renforcement:
            NwItem reinforcedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            oid.LoginCreature.AcquireItem(reinforcedItem);

            reinforcedItem.GetLocalVariable<int>("_DURABILITY").Value += reinforcedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value * 5 / 100;
            reinforcedItem.GetLocalVariable<int>("_REINFORCEMENT_LEVEL").Value += 1;

            oid.SendServerMessage($"Renforcement de {reinforcedItem.Name.ColorString(Color.WHITE)} terminé.", new Color(32, 255, 32));

            break;
          case Job.JobType.Repair:
            NwItem repairedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            oid.LoginCreature.AcquireItem(repairedItem);

            if(!repairedItem.ItemProperties.Any(ip => ip.Tag.Contains("_INACTIVE")))
            {
              repairedItem.GetLocalVariable<int>("_DURABILITY").Value = repairedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value;
              oid.SendServerMessage($"Réparation de {repairedItem.Name.ColorString(Color.WHITE)} terminée. L'objet est comme neuf !", new Color(32, 255, 32));
            }
            else
            {
              repairedItem.GetLocalVariable<int>("_REPAIR_DONE").Value = 1;
              oid.SendServerMessage($"Réparation de {repairedItem.Name.ColorString(Color.WHITE)} terminée. Reste cependant à réactiver les enchantements.", Color.ORANGE);
            }
            break;
          default:
            if (Craft.Collect.System.blueprintDictionnary.TryGetValue(craftJob.baseItemType, out Blueprint blueprint))
            {
              await NwModule.Instance.WaitForObjectContext();
              NwItem craftedItem;
              if (craftJob.craftedItem != "")
              {
                craftedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
                oid.LoginCreature.AcquireItem(craftedItem);
              }
              else
                craftedItem = NwItem.Create(blueprint.craftedItemTag, oid.LoginCreature);

              if (craftedItem == null)
              {
                oid.SendServerMessage($"Votre fabrication artisanale est terminée. Ouvrez votre journal pour obtenir le résultat de votre travail !");
                return;
              }

              Craft.Collect.System.AddCraftedItemProperties(craftedItem, craftJob.material);
              craftedItem.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value = oid.LoginCreature.Name;

              int artisanExceptionnelLevel = 0;
              if (learntCustomFeats.ContainsKey(CustomFeats.ArtisanExceptionnel))
                artisanExceptionnelLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.ArtisanExceptionnel, learntCustomFeats[CustomFeats.ArtisanExceptionnel]);

              if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
              {
                craftedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
                oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", Color.NAVY);
              }

              int artisanAppliqueLevel = 0;
              if (learntCustomFeats.ContainsKey(CustomFeats.ArtisanApplique))
                artisanAppliqueLevel += GetCustomFeatLevelFromSkillPoints(CustomFeats.ArtisanApplique, learntCustomFeats[CustomFeats.ArtisanApplique]);

              if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
              {
                craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value += craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value * 20 / 100;
                oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", Color.NAVY);
              }

              craftedItem.GetLocalVariable<int>("_DURABILITY").Value = craftedItem.GetLocalVariable<int>("_MAX_DURABILITY").Value;
              craftedItem.GetLocalVariable<int>("_REPAIR_DONE").Delete();

              foreach (API.ItemProperty ip in craftedItem.ItemProperties.Where(ip => ip.Tag.Contains("INACTIVE")))
              {
                Task waitLoop = NwTask.Run(async () =>
                {
                  API.ItemProperty deactivatedIP = ip;
                  await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
                  craftedItem.RemoveItemProperty(deactivatedIP);
                  await NwTask.Delay(TimeSpan.FromSeconds(0.1f));
                  deactivatedIP.Tag = deactivatedIP.Tag.Replace("_INACTIVE", "");
                  craftedItem.AddItemProperty(deactivatedIP, EffectDuration.Permanent);
                });
              }
            }
            else
            {
              oid.SendServerMessage("[ERREUR HRP] Il semble que votre dernière création soit invalide. Le staff a été informé du problème.");
              Utils.LogMessageToDMs($"AcquireCraftedItem : {oid.LoginCreature.Name} - Blueprint invalid - {craftJob.baseItemType} - For {oid.LoginCreature.Name}");
            }
            break;
        }

        PlayerPlugin.ApplyInstantVisualEffectToObject(oid.LoginCreature, oid.LoginCreature, NWScript.VFX_IMP_GLOBE_USE);

        craftJob.CloseCraftJournalEntry();
        craftJob = new Job(-10, "", 0, this);
      }
      public void AcquireSkillPoints()
      {
        switch (currentSkillType)
        {
          case SkillType.Skill:
            if (this.learnableSkills.TryGetValue((Feat)currentSkillJob, out Skill skill))
            {
              int pooledPoints = ObjectPlugin.GetInt(oid.LoginCreature, "_STARTING_SKILL_POINTS");
              if (pooledPoints > 0)
              {
                if (pooledPoints > skill.pointsToNextLevel)
                {
                  ObjectPlugin.SetInt(oid.LoginCreature, "_STARTING_SKILL_POINTS", pooledPoints - skill.pointsToNextLevel, 1);
                  skill.acquiredPoints += skill.pointsToNextLevel;
                }
                else
                {
                  skill.acquiredPoints += pooledPoints;
                  ObjectPlugin.DeleteInt(oid.LoginCreature, "_STARTING_SKILL_POINTS");
                }
              }

              double skillPointRate = skill.CalculateSkillPointsPerSecond();
              skill.acquiredPoints += skillPointRate * (DateTime.Now - this.dateLastSaved).TotalSeconds;
              double RemainingTime = skill.GetTimeToNextLevel(skillPointRate);

              if (RemainingTime <= 0)
              {
                skill.LevelUpSkill();
              }
            }
            break;
          case SkillType.Spell:
            if (this.learnableSpells.TryGetValue(this.currentSkillJob, out LearnableSpell spell))
            {
              double skillPointRate = spell.CalculateSkillPointsPerSecond();
              spell.acquiredPoints += skillPointRate * (DateTime.Now - this.dateLastSaved).TotalSeconds;
              double RemainingTime = spell.GetTimeToNextLevel(skillPointRate);

              if (RemainingTime <= 0)
              {
                spell.LevelUpSkill();
              }
            }
            break;
        }

        /*else
        {
          if (this.removeableMalus.TryGetValue(this.currentSkillJob, out skill))
          {
            float skillPointRate = this.CalculateSkillPointsPerSecond(skill);
            skill.acquiredPoints += skillPointRate * (float)(DateTime.Now - this.dateLastSaved).TotalSeconds; ;
            double RemainingTime = skill.GetTimeToNextLevel(skillPointRate);

            if (RemainingTime < 0)
            {
              this.RemoveMalus(skill);
            }
          }
        }*/
      }
      public void RemoveMalus(Skill skill)
      {
        oid.LoginCreature.RemoveFeat(skill.oid);

        if (RegisterRemoveCustomFeatEffect.TryGetValue(skill.oid, out Func<Player, Feat, int> handler))
          handler.Invoke(this, skill.oid);

        ObjectPlugin.DeleteInt(oid.LoginCreature, "_CURRENT_JOB");
        // NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco

        this.removeableMalus.Remove(skill.oid);
      }
      /*public void CancelCollectCycle()
      {
        OnCollectCycleCancel();
      }*/
      /*public void CompleteCollectCycle()
      {
        Log.Info("on cycle complete");
        // AssignCommand permet de "patcher" un bug de comportement undéfinie
        // qui apparait en appelant une callback depuis l'event de la GUI TIMING BAR
        NWScript.AssignCommand(
          NWScript.GetModule(),
          () => OnCollectCycleComplete()
        );
      }*/
      public void UpdateJournal()
      {
        JournalEntry journalEntry;

        if (oid.LoginCreature.Location.Area == null && DoJournalUpdate)
        {
          Task waitAreaLoaded = NwTask.Run(async () =>
          {
            await NwTask.WaitUntil(() => oid.LoginCreature.Location.Area != null);
            await NwTask.Delay(TimeSpan.FromSeconds(1));
            UpdateJournal();
          });

          return;
        }

        if (playerJournal.craftJobCountDown != null && oid.LoginCreature.Area.GetLocalVariable<int>("_AREA_LEVEL").Value == 0)
        {
          journalEntry = PlayerPlugin.GetJournalEntry(oid.LoginCreature, "craft_job");
          if (journalEntry.nUpdated != -1)
          {
            journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.craftJobCountDown - DateTime.Now))}";
            PlayerPlugin.AddCustomJournalEntry(oid.LoginCreature, journalEntry, 1);
          }
          this.CraftJobProgression();
        }

        if (playerJournal.skillJobCountDown != null)
        {
          journalEntry = PlayerPlugin.GetJournalEntry(oid.LoginCreature, "skill_job");
          if (journalEntry.nUpdated != -1)
          {
            journalEntry.sName = $"Entrainement - {Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.skillJobCountDown - DateTime.Now))}";
            PlayerPlugin.AddCustomJournalEntry(oid.LoginCreature, journalEntry, 1);
          }

          switch (currentSkillType)
          {
            case SkillType.Skill:
              Skill skill;
              if (learnableSkills.TryGetValue((Feat)currentSkillJob, out skill))
                skill.RefreshAcquiredSkillPoints();
              break;
            case SkillType.Spell:
              LearnableSpell spell;
              if (learnableSpells.TryGetValue(currentSkillJob, out spell))
                spell.RefreshAcquiredSkillPoints();
              break;
          }
        }

        dateLastSaved = DateTime.Now;

        if (DoJournalUpdate)
          NWScript.DelayCommand(1.0f, () => UpdateJournal());
      }
      public async void rebootUpdate(int countDown)
      {
        await NwTask.Delay(TimeSpan.FromSeconds(1));

        if (!this.oid.LoginCreature.IsValid)
          return;

        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(this.oid.LoginCreature, "reboot");
        journalEntry.sName = $"REBOOT SERVEUR - {countDown}";
        PlayerPlugin.AddCustomJournalEntry(this.oid.LoginCreature, journalEntry);
        
        if (countDown >= 0)
          this.rebootUpdate(countDown - 1);
      }
      public string CheckDBPlayerAccount()
      {
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"select accountName from PlayerAccounts  where rowId = @accountId");
        NWScript.SqlBindInt(query, "@accountId", accountId);
        NWScript.SqlStep(query);

        return NWScript.SqlGetString(query, 0);
      }
      // Take gold from the PC or from his bank account
      public void PayOrBorrowGold(int price)
      {
        int pocketGold = (int)oid.LoginCreature.Gold;

        if (pocketGold >= price)
        {
          oid.LoginCreature.TakeGold(price);
          return;
        }

        var borrowedGold = price - pocketGold;
        bankGold -= borrowedGold;

        oid.SendServerMessage($"Vous ne disposez pas de la somme requise. {price} pièces d'or ont donc été prélevées sur votre compte.");
      }
      public async Task<bool> WaitForPlayerInputInt()
      {
        this.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").Value = 1;

        this.oid.OnPlayerChat -= ChatSystem.HandlePlayerInputInt;
        this.oid.OnPlayerChat += ChatSystem.HandlePlayerInputInt;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task awaitPlayerInput = NwTask.WaitUntil(() => this.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue, tokenSource.Token);
        Task awaitPlayerCancellation = NwTask.WaitUntil(() => this.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasValue, tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerInput, awaitPlayerCancellation);
        tokenSource.Cancel();

        this.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();

        if (awaitPlayerCancellation.IsCompletedSuccessfully)
          return false;
        else
          return true;
      }
      public async Task<bool> WaitForPlayerInputByte()
      {
        this.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").Value = 1;

        this.oid.OnPlayerChat -= ChatSystem.HandlePlayerInputByte;
        this.oid.OnPlayerChat += ChatSystem.HandlePlayerInputByte;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task awaitPlayerInput = NwTask.WaitUntil(() => this.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue, tokenSource.Token);
        Task awaitPlayerCancellation = NwTask.WaitUntil(() => this.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasValue, tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerInput, awaitPlayerCancellation);
        tokenSource.Cancel();

        this.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();

        if (awaitPlayerCancellation.IsCompletedSuccessfully)
          return false;
        else
          return true;
      }
      public async Task<bool> WaitForPlayerInputString()
      {
        this.oid.LoginCreature.GetLocalVariable<int>("_AWAITING_PLAYER_INPUT").Value = 1;

        this.oid.OnPlayerChat -= ChatSystem.HandlePlayerInputString;
        this.oid.OnPlayerChat += ChatSystem.HandlePlayerInputString;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task awaitPlayerInput = NwTask.WaitUntil(() => this.oid.LoginCreature.GetLocalVariable<string>("_PLAYER_INPUT").HasValue, tokenSource.Token);
        Task awaitPlayerCancellation = NwTask.WaitUntil(() => this.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").HasValue, tokenSource.Token);

        await NwTask.WhenAny(awaitPlayerInput, awaitPlayerCancellation);
        tokenSource.Cancel();

        this.oid.LoginCreature.GetLocalVariable<int>("_PLAYER_INPUT_CANCELLED").Delete();

        if (awaitPlayerCancellation.IsCompletedSuccessfully)
          return false;
        else
          return true;
      }
    }
  }
}
