using System;
using System.Numerics;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.SkillSystem;
using NWN.Systems.Craft;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public class Player
    { 
      public readonly uint oid;
      public readonly int accountId;
      public readonly int characterId;
      public int bonusRolePlay { get; set; }
      public Location location { get; set; }
      public Boolean isConnected { get; set; }
      public Boolean isAFK { get; set; }
      public Boolean DoJournalUpdate { get; set; }
      public int currentHP { get; set; }
      public int bankGold { get; set; }
      public PlayerJournal playerJournal { get; set; }
      public DateTime dateLastSaved { get; set; }
      public int currentSkillJob { get; set; }
      public SkillType currentSkillType { get; set; }
      public Job craftJob { get; set; }
      public uint autoAttackTarget { get; set; }
      public Boolean isFrostAttackOn { get; set; }
      public uint previousArea { get; set; }
      public DateTime lycanCurseTimer { get; set; }
      public Feat activeLanguage { get; set; }
      public TargetEvent targetEvent { get; set; }
      public Menu menu { get; }
      public string disguiseName { get; set; }
      public uint deathCorpse { get; set; }
      public int setValue { get; set; }
      public QuickbarType loadedQuickBar { get; set; }

      public Dictionary<uint, Player> listened = new Dictionary<uint, Player>();
      public Dictionary<uint, Player> blocked = new Dictionary<uint, Player>();
      public Dictionary<uint, DateTime> disguiseDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> pickpocketDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> inviDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> inviEffectDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, uint> summons = new Dictionary<uint, uint>();
      public Dictionary<int, SkillSystem.Skill> learnableSkills = new Dictionary<int, SkillSystem.Skill>();
      public Dictionary<int, SkillSystem.LearnableSpell> learnableSpells = new Dictionary<int, SkillSystem.LearnableSpell>();
      public Dictionary<int, SkillSystem.Skill> removeableMalus = new Dictionary<int, SkillSystem.Skill>();
      public Dictionary<string, int> materialStock = new Dictionary<string, int>();
      public List<Effect> effectList = new List<Effect>();
      public List<QuickBarSlot> savedQuickBar = new List<QuickBarSlot>();
      public Dictionary<int, MapPin> mapPinDictionnary = new Dictionary<int, MapPin>();

      public Action OnCollectCycleCancel = delegate { };
      public Action OnCollectCycleComplete = delegate { };

      public Player(uint nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
       
        if (ObjectPlugin.GetInt(this.oid, "accountId") == 0)
          InitializeNewPlayer(this.oid);

        this.accountId = ObjectPlugin.GetInt(this.oid, "accountId");
        
        if (ObjectPlugin.GetInt(this.oid, "characterId") == 0)
          InitializeNewCharacter(this);

        this.characterId = ObjectPlugin.GetInt(this.oid, "characterId");
        this.isConnected = true;

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
          default:
            NWScript.EnterTargetingMode(this.oid);
            break;
        }
      }
      public void RemoveLycanCurse()
      {
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_SUPER_HEROISM), this.oid);
        RenamePlugin.ClearPCNameOverride(this.oid, NWScript.OBJECT_INVALID, 1);
        CreaturePlugin.SetMovementRate(this.oid, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_PC);

        EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", this.oid);
      }
      public void ApplyLycanCurse()
      {
        Effect ePoly = NWScript.EffectPolymorph(107, 1);
        Effect eLink = NWScript.SupernaturalEffect(ePoly);
        eLink = NWScript.TagEffect(eLink, "lycan_curse");

        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eLink, this.oid, 900.0f);
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_INSTANT, NWScript.EffectVisualEffect(NWScript.VFX_IMP_SUPER_HEROISM), this.oid);
        
        RenamePlugin.SetPCNameOverride(this.oid, "Loup-garou", "", "", RenamePlugin.NWNX_RENAME_PLAYERNAME_OVERRIDE);
        CreaturePlugin.SetMovementRate(this.oid, CreaturePlugin.NWNX_CREATURE_MOVEMENT_RATE_FAST);

        EventsPlugin.AddObjectToDispatchList("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", this.oid);
      }

      /*public void BoulderBlock()
      {
        NWScript.SendMessageToPC(this.oid, $"Creating boulders");
        BoulderUnblock();
        var location = NWScript.GetLocation(oid);
        blockingBoulder = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "plc_boulder", location, 0, $"block_rock_{NWScript.GetPCPublicCDKey(this.oid)}");
        ObjectPlugin.SetPosition(oid, NWScript.GetPositionFromLocation(location));
        NWScript.ApplyEffectToObject(
          NWScript.DURATION_TYPE_PERMANENT,
          NWScript.EffectVisualEffect(NWScript.VFX_DUR_CUTSCENE_INVISIBILITY),
          blockingBoulder
        );
      }*/
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

        this.savedQuickBar.Clear();
        this.loadedQuickBar = QuickbarType.Invalid;
      }
      public void CraftJobProgression()
      {
        if (craftJob.IsActive())
        {
          craftJob.remainingTime = craftJob.remainingTime - (float)(DateTime.Now - dateLastSaved).TotalSeconds;
          
          if (craftJob.remainingTime < 0)
          {
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
            uint improvedTEBlueprint = NWScript.CopyItem(ObjectPlugin.Deserialize((this.craftJob.craftedItem)), this.oid, 1);
            NWScript.SetLocalInt(improvedTEBlueprint, "_BLUEPRINT_TIME_EFFICIENCY", NWScript.GetLocalInt(improvedTEBlueprint, "_BLUEPRINT_TIME_EFFICIENCY") + 1);
            break;
          default:
            Blueprint blueprint;
            if (Craft.Collect.System.blueprintDictionnary.ContainsKey(this.craftJob.baseItemType))
            {
              blueprint = Craft.Collect.System.blueprintDictionnary[this.craftJob.baseItemType];
              Craft.Collect.System.AddCraftedItemProperties(NWScript.CreateItemOnObject(blueprint.craftedItemTag, oid), blueprint, this.craftJob.material);
            }
            else
            {
              NWScript.SendMessageToPC(this.oid, "[ERREUR HRP] Il semble que votre dernière création soit invalide. Le staff a été informé du problème.");
              Utils.LogMessageToDMs($"AcquireCraftedItem : {NWScript.GetName(this.oid)} - Blueprint invalid - {this.craftJob.baseItemType} - For {NWScript.GetName(this.oid)}");
            }
            break;
        }

        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);
        craftJob.CloseCraftJournalEntry();
        craftJob = new Job(-10, "", 0, this);
      }
      public void AcquireSkillPoints()
      {
        switch(currentSkillType)
        {
          case SkillType.Skill:
            Skill skill;
            if (this.learnableSkills.TryGetValue(this.currentSkillJob, out skill))
            {
              float skillPointRate = skill.CalculateSkillPointsPerSecond();
              skill.acquiredPoints += skillPointRate * (float)(DateTime.Now - this.dateLastSaved).TotalSeconds;
              double RemainingTime = skill.GetTimeToNextLevel(skillPointRate);

              if (RemainingTime <= 0)
              {
                skill.LevelUpSkill();
              }
            }
            break;
          case SkillType.Spell:
            LearnableSpell spell;
            if (this.learnableSpells.TryGetValue(this.currentSkillJob, out spell))
            {
              float skillPointRate = spell.CalculateSkillPointsPerSecond();
              spell.acquiredPoints += skillPointRate * (float)(DateTime.Now - this.dateLastSaved).TotalSeconds;
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
      public void RemoveMalus(SkillSystem.Skill skill)
      {
        CreaturePlugin.RemoveFeat(oid, skill.oid);

        Func<PlayerSystem.Player, int, int> handler;
        if (SkillSystem.RegisterRemoveCustomFeatEffect.TryGetValue(skill.oid, out handler))
        {
          try
          {
            handler.Invoke(this, skill.oid);
          }
          catch (Exception e)
          {
            Utils.LogException(e);
          }
        }

        ObjectPlugin.DeleteInt(oid, "_CURRENT_JOB");
       // NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco

        this.removeableMalus.Remove(skill.oid);
      }
      public void PlayNoCurrentTrainingEffects()
      {
        NWScript.PostString(oid, $"Vous n'avez aucun apprentissage en cours !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        NWScript.SendMessageToPC(oid, "Vous n'avez aucun apprentissage en cours !");
        PlayerPlugin.PlaySound(oid, "gui_dm_drop");
        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_REDUCE_ABILITY_SCORE);
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
      
      public Effect GetPartySizeEffect(int iPartySize = 0)
      {
        var oPartyMember = NWScript.GetFirstFactionMember(oid, 1);
        while (NWScript.GetIsObjectValid(oPartyMember) == 1)
        {
          iPartySize++;
          oPartyMember = NWScript.GetNextFactionMember(oid, 1);
        }

        Effect eParty = null;

        switch (iPartySize) // déterminer quel est l'effet de groupe à appliquer
        {
          case 1:
            break;
          case 2:
            eParty = NWScript.TagEffect(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), "PartyEffect");
            break;
          case 3:
            eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), NWScript.EffectAttackIncrease(1));
            eParty = NWScript.TagEffect(eParty, "PartyEffect");
            break;
          case 4:
          case 5:
            eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), NWScript.EffectAttackIncrease(1));
            eParty = NWScript.EffectLinkEffects(NWScript.EffectDamageIncrease(1, NWScript.DAMAGE_TYPE_BLUDGEONING), eParty);
            eParty = NWScript.TagEffect(eParty, "PartyEffect");
            break;
          case 6:
            eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), NWScript.EffectAttackIncrease(1));
            eParty = NWScript.TagEffect(eParty, "PartyEffect");
            break;
          case 7:
            eParty = NWScript.TagEffect(NWScript.EffectACIncrease(1, NWScript.AC_DODGE_BONUS), "PartyEffect");
            break;
          default:
            break;
        }

        return eParty;
      }
      public void UpdateJournal()
      {
        JournalEntry journalEntry;
        Area area;

        if (playerJournal.craftJobCountDown != null 
          && AreaSystem.areaDictionnary.TryGetValue(NWScript.GetObjectUUID(NWScript.GetArea(oid)), out area) && area.level == 0)
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

          switch(currentSkillType)
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
      public void rebootUpdate()
      {
        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(this.oid, "reboot");
        journalEntry.sName = $"REBOOT SERVEUR - {Utils.StripTimeSpanMilliseconds((TimeSpan)(this.playerJournal.rebootCountDown - DateTime.Now))}";
        PlayerPlugin.AddCustomJournalEntry(this.oid, journalEntry);

        NWScript.DelayCommand(1.0f, () => this.rebootUpdate());
      }
      public Boolean IsDialogQuickbarOn()
      {
        QuickBarSlot qbs = PlayerPlugin.GetQuickBarSlot(this.oid, 0);
        if (qbs.nObjectType == 4 && qbs.nINTParam1 == (int)Feat.CustomMenuDOWN)
          return true;
        return false;
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
