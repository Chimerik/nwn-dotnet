using System;
using System.Numerics;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;
using NWN.Systems.Craft;
using NWN.API;

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
      public Dictionary<int, Skill> learnableSkills = new Dictionary<int, Skill>();
      public Dictionary<int, LearnableSpell> learnableSpells = new Dictionary<int, LearnableSpell>();
      public Dictionary<int, Skill> removeableMalus = new Dictionary<int, Skill>();
      public Dictionary<string, int> materialStock = new Dictionary<string, int>();
      public List<API.Effect> effectList = new List<API.Effect>();
      public List<QuickBarSlot> savedQuickBar = new List<QuickBarSlot>();
      public Dictionary<int, MapPin> mapPinDictionnary = new Dictionary<int, MapPin>();
      public Dictionary<string, string> areaExplorationStateDictionnary = new Dictionary<string, string>();

      public Action OnCollectCycleCancel = delegate { };
      public Action OnCollectCycleComplete = delegate { };
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
      public void DoActionOnTargetSelected(uint oTarget, Vector3 vTarget)
      {
        if (Convert.ToBoolean(NWScript.GetIsObjectValid(oTarget)))
          this.OnSelectTarget(oTarget, vTarget);
      }
      private Action<uint, Vector3> OnSelectTarget = delegate { };
      public void SelectTarget(Action<uint, Vector3> callback)
      {
        this.OnSelectTarget = callback;

        switch (this.targetEvent)
        {
          case TargetEvent.SitTarget:
            NWScript.EnterTargetingMode(this.oid);
            break;
          case TargetEvent.Creature:
            NWScript.EnterTargetingMode(this.oid, NWScript.OBJECT_TYPE_CREATURE);
            targetEvent = TargetEvent.LootSaverTarget;
            break;
          default:
            NWScript.EnterTargetingMode(this.oid);
            break;
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
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuDOWN);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuUP);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuSELECT);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuEXIT);

              this.savedQuickBar.Clear();
              emptyQBS.nObjectType = 0;

              for (int i = 0; i < 12; i++)
              {
                this.savedQuickBar.Add(PlayerPlugin.GetQuickBarSlot(this.oid, i));
                PlayerPlugin.SetQuickBarSlot(this.oid, i, emptyQBS);
              }

              emptyQBS.nObjectType = 4;
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuDOWN;
              PlayerPlugin.SetQuickBarSlot(this.oid, 0, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuUP;
              PlayerPlugin.SetQuickBarSlot(this.oid, 1, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuSELECT;
              PlayerPlugin.SetQuickBarSlot(this.oid, 2, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuEXIT;
              PlayerPlugin.SetQuickBarSlot(this.oid, 3, emptyQBS);

              this.loadedQuickBar = QuickbarType.Menu;
              break;
            case QuickbarType.Sit:
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuDOWN);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuUP);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomPositionRight);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomPositionLeft);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomPositionForward);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomPositionBackward);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomPositionRotateRight);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomPositionRotateLeft);
              CreaturePlugin.AddFeat(this.oid, (int)Feat.CustomMenuEXIT);

              this.savedQuickBar.Clear();
              emptyQBS = new QuickBarSlot();
              emptyQBS.nObjectType = 0;

              for (int i = 0; i < 12; i++)
              {
                this.savedQuickBar.Add(PlayerPlugin.GetQuickBarSlot(this.oid, i));
                PlayerPlugin.SetQuickBarSlot(this.oid, i, emptyQBS);
              }
              emptyQBS.nObjectType = 4;
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuDOWN;
              PlayerPlugin.SetQuickBarSlot(this.oid, 0, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuUP;
              PlayerPlugin.SetQuickBarSlot(this.oid, 1, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomPositionLeft;
              PlayerPlugin.SetQuickBarSlot(this.oid, 2, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomPositionRight;
              PlayerPlugin.SetQuickBarSlot(this.oid, 3, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomPositionForward;
              PlayerPlugin.SetQuickBarSlot(this.oid, 4, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomPositionBackward;
              PlayerPlugin.SetQuickBarSlot(this.oid, 5, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomPositionRotateLeft;
              PlayerPlugin.SetQuickBarSlot(this.oid, 6, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomPositionRotateRight;
              PlayerPlugin.SetQuickBarSlot(this.oid, 7, emptyQBS);
              emptyQBS.nINTParam1 = (int)Feat.CustomMenuEXIT;
              PlayerPlugin.SetQuickBarSlot(this.oid, 8, emptyQBS);

              this.loadedQuickBar = QuickbarType.Sit;
              this.OnKeydown += this.menu.HandleMenuFeatUsed;
              break;
          }
        }
      }
      public void UnloadMenuQuickbar()
      {
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomMenuUP);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomMenuDOWN);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomMenuSELECT);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomMenuEXIT);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomPositionLeft);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomPositionRight);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomPositionForward);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomPositionBackward);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomPositionRotateLeft);
        CreaturePlugin.RemoveFeat(this.oid, (int)Feat.CustomPositionRotateRight);

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
          craftJob.remainingTime = craftJob.remainingTime - (float)(DateTime.Now - dateLastSaved).TotalSeconds;

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
            uint improvedTEBlueprint = NWScript.CopyItem(ObjectPlugin.Deserialize((craftJob.craftedItem)), oid, 1);
            NWScript.SetLocalInt(improvedTEBlueprint, "_BLUEPRINT_TIME_EFFICIENCY", NWScript.GetLocalInt(improvedTEBlueprint, "_BLUEPRINT_TIME_EFFICIENCY") + 1);
            break;
          case Job.JobType.Enchantement:
            NwItem enchantedItem = NwObject.Deserialize<NwItem>(craftJob.craftedItem);
            oid.AcquireItem(enchantedItem);

            enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value -= 1;
            if (enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Value <= 0)
              enchantedItem.GetLocalVariable<int>("_AVAILABLE_ENCHANTEMENT_SLOT").Delete();
            Craft.Collect.System.AddCraftedEnchantementProperties(enchantedItem, craftJob.material);

            break;
          case Job.JobType.Recycling:
            NwItem recycledItem = NwObject.Deserialize<NwItem>(craftJob.craftedItem);
            int recycledValue = recycledItem.GetLocalVariable<int>("_BASE_COST").Value;

            if (materialStock.ContainsKey(craftJob.material))
              materialStock[craftJob.material] += recycledValue;
            else
              materialStock.Add(craftJob.material, recycledValue);

            oid.SendServerMessage($"Recyclage de {recycledItem.Name.ColorString(Color.WHITE)} terminé. Vous en retirez {recycledValue} unité(s) de {craftJob.material}", Color.GREEN) ;
            recycledItem.Destroy();

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
            if (this.learnableSkills.TryGetValue(this.currentSkillJob, out Skill skill))
            {
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
        CreaturePlugin.RemoveFeat(oid, skill.oid);

        if (RegisterRemoveCustomFeatEffect.TryGetValue(skill.oid, out Func<Player, int, int> handler))
          handler.Invoke(this, skill.oid);

        ObjectPlugin.DeleteInt(oid, "_CURRENT_JOB");
        // NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco

        this.removeableMalus.Remove(skill.oid);
      }
      public void CancelCollectCycle()
      {
        OnCollectCycleCancel();
      }
      public void CompleteCollectCycle()
      {
        // AssignCommand permet de "patcher" un bug de comportement undéfinie
        // qui apparait en appelant une callback depuis l'event de la GUI TIMING BAR
        NWScript.AssignCommand(
          NWScript.GetModule(),
          () => OnCollectCycleComplete()
        );
      }
      public void UpdateJournal()
      {
        JournalEntry journalEntry;

        if (playerJournal.craftJobCountDown != null
            && NWScript.GetArea(oid).ToNwObject<NwArea>().GetLocalVariable<int>("_AREA_LEVEL")?.Value == 0)
        {
          journalEntry = PlayerPlugin.GetJournalEntry(oid, "craft_job");
          if (journalEntry.nUpdated != -1)
          {
            journalEntry.sName = $"Travail artisanal - {NWN.Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.craftJobCountDown - DateTime.Now))}";
            PlayerPlugin.AddCustomJournalEntry(oid, journalEntry, 1);
          }
          this.CraftJobProgression();
        }

        if (playerJournal.skillJobCountDown != null)
        {
          journalEntry = PlayerPlugin.GetJournalEntry(oid, "skill_job");
          if (journalEntry.nUpdated != -1)
          {
            journalEntry.sName = $"Entrainement - {NWN.Utils.StripTimeSpanMilliseconds((TimeSpan)(playerJournal.skillJobCountDown - DateTime.Now))}";
            PlayerPlugin.AddCustomJournalEntry(oid, journalEntry, 1);
          }

          switch (currentSkillType)
          {
            case SkillType.Skill:
              Skill skill;
              if (learnableSkills.TryGetValue(currentSkillJob, out skill))
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
        int pocketGold = NWScript.GetGold(oid);

        if (pocketGold >= price)
        {
          CreaturePlugin.SetGold(oid, pocketGold - price);
          return;
        }

        var borrowedGold = price - pocketGold;
        bankGold -= borrowedGold;
      }
    }
  }
}
