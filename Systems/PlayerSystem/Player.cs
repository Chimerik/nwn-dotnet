﻿using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;
using NWN.Systems.Craft;
using NWN.API;
using System.Threading.Tasks;
using Skill = NWN.Systems.SkillSystem.Skill;
using Action = System.Action;
using NWN.API.Constants;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public class Player
    {
      public NwPlayer oid { get; set; }
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
      public int setValue { get; set; }
      public string setString { get; set; }
      public QuickbarType loadedQuickBar { get; set; }
      public Arena.PlayerData pveArena = new Arena.PlayerData();

      public List<NwPlayer> listened = new List<NwPlayer>();
      public Dictionary<uint, Player> blocked = new Dictionary<uint, Player>();
      public Dictionary<Feat, int> learntCustomFeats = new Dictionary<Feat, int>();
      public Dictionary<Feat, Skill> learnableSkills = new Dictionary<Feat, Skill>();
      public Dictionary<int, LearnableSpell> learnableSpells = new Dictionary<int, LearnableSpell>();
      public Dictionary<Feat, Skill> removeableMalus = new Dictionary<Feat, Skill>();
      public Dictionary<string, int> materialStock = new Dictionary<string, int>();
      public List<API.Effect> effectList = new List<API.Effect>();
      public List<QuickBarSlot> savedQuickBar = new List<QuickBarSlot>();
      public Dictionary<int, MapPin> mapPinDictionnary = new Dictionary<int, MapPin>();
      public Dictionary<string, string> areaExplorationStateDictionnary = new Dictionary<string, string>();

      //public Action OnCollectCycleCancel = delegate { };
      //public Action OnCollectCycleComplete = delegate { };
      public Player(NwPlayer nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);

        if (ObjectPlugin.GetInt(this.oid, "accountId") == 0 && !oid.IsDM)
          InitializeNewPlayer(this.oid);

        this.accountId = ObjectPlugin.GetInt(this.oid, "accountId");

        if (ObjectPlugin.GetInt(this.oid, "characterId") == 0 && !oid.IsDM)
          InitializeNewCharacter(this);

        this.characterId = ObjectPlugin.GetInt(this.oid, "characterId");

        if (!Convert.ToBoolean(NWScript.GetIsDM(this.oid)))
          InitializePlayer(this);
        else
          InitializeDM(this);

        Log.Info($"Player initialization : DONE");
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

      public event EventHandler<DeathEventArgs> OnDeath = delegate { };
      public void EmitDeath(DeathEventArgs e)
      {
        OnDeath(this, e);
      }

      public class DeathEventArgs : EventArgs
      {
        public uint oKiller;
        public Player player;

        public DeathEventArgs(Player player, uint oKiller)
        {
          this.oKiller = oKiller;
          this.player = player;
        }
      }
      public void LoadMenuQuickbar(QuickbarType type)
      {
        if (this.loadedQuickBar == QuickbarType.Invalid)
        {
          QuickBarSlot emptyQBS = new QuickBarSlot();

          switch (type)
          {
            case QuickbarType.Menu:

              this.savedQuickBar.Clear();
              emptyQBS.nObjectType = 4;

              for (int i = 0; i < 12; i++)
              {
                this.savedQuickBar.Add(PlayerPlugin.GetQuickBarSlot(this.oid, i));
                PlayerPlugin.SetQuickBarSlot(this.oid, i, emptyQBS);
              }

              if (menu.choices.Count > 0)
              {
                oid.AddFeat(CustomFeats.CustomMenuDOWN);
                oid.AddFeat(CustomFeats.CustomMenuUP);
                oid.AddFeat(CustomFeats.CustomMenuSELECT);

                if (ObjectPlugin.GetInt(this.oid, "_MENU_HOTKEYS_SWAPPED") == 0)
                {
                  emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuDOWN;
                  PlayerPlugin.SetQuickBarSlot(this.oid, 0, emptyQBS);
                  emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuUP;
                  PlayerPlugin.SetQuickBarSlot(this.oid, 1, emptyQBS);
                }
                else
                {
                  emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuDOWN;
                  PlayerPlugin.SetQuickBarSlot(this.oid, 1, emptyQBS);
                  emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuUP;
                  PlayerPlugin.SetQuickBarSlot(this.oid, 0, emptyQBS);
                }
                  
                emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuSELECT;
                PlayerPlugin.SetQuickBarSlot(this.oid, 2, emptyQBS);
              }

              oid.AddFeat(CustomFeats.CustomMenuEXIT);
              
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuEXIT;
              PlayerPlugin.SetQuickBarSlot(this.oid, 3, emptyQBS);

              this.loadedQuickBar = QuickbarType.Menu;
              break;
            case QuickbarType.Sit:
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomMenuDOWN);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomMenuUP);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomPositionRight);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomPositionLeft);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomPositionForward);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomPositionBackward);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomPositionRotateRight);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomPositionRotateLeft);
              CreaturePlugin.AddFeat(this.oid, (int)CustomFeats.CustomMenuEXIT);

              this.savedQuickBar.Clear();
              emptyQBS = new QuickBarSlot();
              emptyQBS.nObjectType = 0;

              for (int i = 0; i < 12; i++)
              {
                this.savedQuickBar.Add(PlayerPlugin.GetQuickBarSlot(this.oid, i));
                PlayerPlugin.SetQuickBarSlot(this.oid, i, emptyQBS);
              }
              emptyQBS.nObjectType = 4;
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuDOWN;
              PlayerPlugin.SetQuickBarSlot(this.oid, 0, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuUP;
              PlayerPlugin.SetQuickBarSlot(this.oid, 1, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomPositionLeft;
              PlayerPlugin.SetQuickBarSlot(this.oid, 2, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomPositionRight;
              PlayerPlugin.SetQuickBarSlot(this.oid, 3, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomPositionForward;
              PlayerPlugin.SetQuickBarSlot(this.oid, 4, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomPositionBackward;
              PlayerPlugin.SetQuickBarSlot(this.oid, 5, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomPositionRotateLeft;
              PlayerPlugin.SetQuickBarSlot(this.oid, 6, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomPositionRotateRight;
              PlayerPlugin.SetQuickBarSlot(this.oid, 7, emptyQBS);
              emptyQBS.nINTParam1 = (int)CustomFeats.CustomMenuEXIT;
              PlayerPlugin.SetQuickBarSlot(this.oid, 8, emptyQBS);

              this.loadedQuickBar = QuickbarType.Sit;
              this.OnKeydown += this.menu.HandleMenuFeatUsed;
              break;
          }
        }
      }
      public void UnloadMenuQuickbar()
      {
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomMenuUP);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomMenuDOWN);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomMenuSELECT);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomMenuEXIT);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomPositionLeft);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomPositionRight);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomPositionForward);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomPositionBackward);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomPositionRotateLeft);
        CreaturePlugin.RemoveFeat(this.oid, (int)CustomFeats.CustomPositionRotateRight);

        int i = 0;
        foreach (QuickBarSlot qbs in this.savedQuickBar)
        {
          PlayerPlugin.SetQuickBarSlot(this.oid, i, qbs);
          i++;
        }

        savedQuickBar.Clear();
        loadedQuickBar = QuickbarType.Invalid;
      }
      public void CraftJobProgression()
      {
        if (craftJob.IsActive())
        {
          Log.Info($"remaining : {craftJob.remainingTime}");
          Log.Info($"Time since last save : {(float)(DateTime.Now - dateLastSaved).TotalSeconds}");

          craftJob.remainingTime = craftJob.remainingTime - (float)(DateTime.Now - dateLastSaved).TotalSeconds;

          Log.Info($"result : {craftJob.remainingTime}");

          if (craftJob.remainingTime < 0)
          {
            Log.Info($"craft job done. Acquiring item - Type : {craftJob.type} - BaseItem : {craftJob.baseItemType}");
            AcquireCraftedItem();
          }
        }
      }

      public void AcquireCraftedItem()
      {
        switch (craftJob.type)
        {
          case Job.JobType.BlueprintCopy:
            uint bpCopy = NWScript.CopyItem(ObjectPlugin.Deserialize((this.craftJob.craftedItem)), this.oid, 1);
            NWScript.SetLocalInt(bpCopy, "_BLUEPRINT_RUNS", 10);
            NWScript.SetName(bpCopy, $"Copie de {NWScript.GetName(bpCopy)}");
            break;
          case Job.JobType.BlueprintResearchMaterialEfficiency:
            uint improvedMEBP = NWScript.CopyItem(ObjectPlugin.Deserialize((this.craftJob.craftedItem)), this.oid, 1);
            NWScript.SetLocalInt(improvedMEBP, "_BLUEPRINT_MATERIAL_EFFICIENCY", NWScript.GetLocalInt(improvedMEBP, "_BLUEPRINT_MATERIAL_EFFICIENCY") + 1);
            break;
          case Job.JobType.BlueprintResearchTimeEfficiency:
            ((NwItem)NwItem.Deserialize(craftJob.craftedItem.ToByteArray())).Clone(oid).GetLocalVariable<int>("_BLUEPRINT_TIME_EFFICIENCY").Value += 1;
            break;
          case Job.JobType.Enchantement:
            NwItem enchantedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            oid.AcquireItem(enchantedItem);

            int enchanteurChanceuxLevel = 0;
            if (learntCustomFeats.ContainsKey(CustomFeats.EnchanteurChanceux))
              enchanteurChanceuxLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.EnchanteurChanceux, learntCustomFeats[CustomFeats.EnchanteurChanceux]);

            if (NwRandom.Roll(Utils.random, 100) > enchanteurChanceuxLevel)
            {
              enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
              if (enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
                enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();
            }

            int enchanteurExpertLevel = 0;
            if (learntCustomFeats.ContainsKey(CustomFeats.EnchanteurExpert))
              enchanteurExpertLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.EnchanteurExpert, learntCustomFeats[CustomFeats.EnchanteurExpert]);

            int boost = 0;
            if (NwRandom.Roll(Utils.random, 100) <= enchanteurExpertLevel * 2)
              boost = 1;

            Craft.Collect.System.AddCraftedEnchantementProperties(enchantedItem, craftJob.material, boost);

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

            oid.SendServerMessage($"Recyclage de {recycledItem.Name.ColorString(Color.WHITE)} terminé. Vous en retirez {recycledValue} unité(s) de {craftJob.material}", Color.GREEN) ;
            recycledItem.Destroy();

            break;
          case Job.JobType.Renforcement:
            NwItem reinforcedItem = NwItem.Deserialize(craftJob.craftedItem.ToByteArray());
            oid.AcquireItem(reinforcedItem);

            reinforcedItem.GetLocalVariable<int>("_DURABILITY").Value += reinforcedItem.GetLocalVariable<int>("_DURABILITY").Value * 5 / 100;
            reinforcedItem.GetLocalVariable<int>("_REINFORCEMENT_LEVEL").Value += 1;

            oid.SendServerMessage($"Renforcement de {reinforcedItem.Name.ColorString(Color.WHITE)} terminé.", Color.GREEN);

            break;
          default:
            if (Craft.Collect.System.blueprintDictionnary.TryGetValue(craftJob.baseItemType, out Blueprint blueprint))
            {
              NwItem craftedItem = NwItem.Create(blueprint.craftedItemTag, oid);

              if (craftedItem == null)
              {
                oid.SendServerMessage($"Votre fabrication artisanale est terminée. Ouvrez votre journal pour obtenir le résultat de votre travail !");
                return;
              }

              Craft.Collect.System.AddCraftedItemProperties(craftedItem, craftJob.material);
              craftedItem.GetLocalVariable<string>("_ORIGINAL_CRAFTER_NAME").Value = oid.Name;

              int artisanExceptionnelLevel = 0;
              if (learntCustomFeats.ContainsKey(CustomFeats.ArtisanExceptionnel))
                artisanExceptionnelLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ArtisanExceptionnel, learntCustomFeats[CustomFeats.ArtisanExceptionnel]);

              if (NwRandom.Roll(Utils.random, 100) <= artisanExceptionnelLevel)
              {
                craftedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value += 1;
                oid.SendServerMessage("Votre talent d'artisan vous a permis de créer un objet exceptionnel disposant d'un emplacement d'enchantement supplémentaire !", Color.NAVY);
              }

              int artisanAppliqueLevel = 0;
              if (learntCustomFeats.ContainsKey(CustomFeats.ArtisanApplique))
                artisanAppliqueLevel += SkillSystem.GetCustomFeatLevelFromSkillPoints(CustomFeats.ArtisanApplique, learntCustomFeats[CustomFeats.ArtisanApplique]);

              if (NwRandom.Roll(Utils.random, 100) <= artisanAppliqueLevel * 3)
              {
                craftedItem.GetLocalVariable<int>("_DURABILITY").Value += craftedItem.GetLocalVariable<int>("_DURABILITY").Value * 20 / 100;
                oid.SendServerMessage("En travaillant de manière particulièrement appliquée, vous parvenez à fabriquer un objet plus résistant !", Color.NAVY);
              }
            }
            else
            {
              NWScript.SendMessageToPC(oid, "[ERREUR HRP] Il semble que votre dernière création soit invalide. Le staff a été informé du problème.");
              Utils.LogMessageToDMs($"AcquireCraftedItem : {NWScript.GetName(oid)} - Blueprint invalid - {craftJob.baseItemType} - For {NWScript.GetName(oid)}");
            }
            break;
        }

        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);

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
              int pooledPoints = ObjectPlugin.GetInt(oid, "_STARTING_SKILL_POINTS");
              if (pooledPoints > 0)
              {
                if (pooledPoints > skill.pointsToNextLevel)
                {
                  ObjectPlugin.SetInt(oid, "_STARTING_SKILL_POINTS", pooledPoints - skill.pointsToNextLevel, 1);
                  skill.acquiredPoints += skill.pointsToNextLevel;
                }
                else
                {
                  skill.acquiredPoints += pooledPoints;
                  ObjectPlugin.DeleteInt(oid, "_STARTING_SKILL_POINTS");
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
        oid.RemoveFeat(skill.oid);

        if (RegisterRemoveCustomFeatEffect.TryGetValue(skill.oid, out Func<Player, Feat, int> handler))
          handler.Invoke(this, skill.oid);

        ObjectPlugin.DeleteInt(oid, "_CURRENT_JOB");
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

        if (oid.Location.Area == null && DoJournalUpdate)
        {
          Task waitAreaLoaded = NwTask.Run(async () =>
          {
            await NwTask.WaitUntil(() => oid.Location.Area != null);
            await NwTask.Delay(TimeSpan.FromSeconds(1));
            UpdateJournal();
          });

          return;
        }

        if (playerJournal.craftJobCountDown != null && oid.Area.GetLocalVariable<int>("_AREA_LEVEL").Value == 0)
        {
          journalEntry = PlayerPlugin.GetJournalEntry(oid, "craft_job");
          if (journalEntry.nUpdated != -1)
          {
            journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.craftJobCountDown - DateTime.Now))}";
            PlayerPlugin.AddCustomJournalEntry(oid, journalEntry, 1);
          }
          this.CraftJobProgression();
        }

        if (playerJournal.skillJobCountDown != null)
        {
          journalEntry = PlayerPlugin.GetJournalEntry(oid, "skill_job");
          if (journalEntry.nUpdated != -1)
          {
            journalEntry.sName = $"Entrainement - {Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.skillJobCountDown - DateTime.Now))}";
            PlayerPlugin.AddCustomJournalEntry(oid, journalEntry, 1);
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

        if (!this.oid.IsValid)
          return;

        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(this.oid, "reboot");
        journalEntry.sName = $"REBOOT SERVEUR - {countDown}";
        PlayerPlugin.AddCustomJournalEntry(this.oid, journalEntry);
        
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
        int pocketGold = (int)oid.Gold;

        if (pocketGold >= price)
        {
          oid.TakeGold(price);
          return;
        }

        var borrowedGold = price - pocketGold;
        bankGold -= borrowedGold;

        oid.SendServerMessage($"Vous ne disposez pas de la somme requise. {price} pièces d'or ont donc été prélevées sur votre compte.");
      }
    }
  }
}
