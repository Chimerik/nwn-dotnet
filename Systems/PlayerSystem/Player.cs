using System;
using System.Numerics;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;

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
      public CraftJob craftJob { get; set; }
      public uint autoAttackTarget { get; set; }
      public Boolean isFrostAttackOn { get; set; }
      public uint previousArea { get; set; }
      public DateTime lycanCurseTimer { get; set; }
      public Feat activeLanguage { get; set; }
      public Menu menu { get; }

      private uint blockingBoulder;
      public string disguiseName { get; set; }
      public uint deathCorpse { get; set; }
      public int setValue { get; set; }

      private List<uint> _selectedObjectsList = new List<uint>();
      public List<uint> selectedObjectsList
      {
        get => _selectedObjectsList;
        // set => _selectedObjectsList.Add(value);
      }

      public Dictionary<uint, Player> listened = new Dictionary<uint, Player>();
      public Dictionary<uint, Player> blocked = new Dictionary<uint, Player>();
      public Dictionary<uint, DateTime> disguiseDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> pickpocketDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> inviDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> inviEffectDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, uint> summons = new Dictionary<uint, uint>();
      public Dictionary<int, SkillSystem.Skill> learnableSkills = new Dictionary<int, SkillSystem.Skill>();
      public Dictionary<int, SkillSystem.Skill> removeableMalus = new Dictionary<int, SkillSystem.Skill>();
      public Dictionary<string, int> materialStock = new Dictionary<string, int>();
      public List<Effect> effectList = new List<Effect>();

      public Action OnMiningCycleCancelled = delegate { };
      public Action OnMiningCycleCompleted = delegate { };

      public Player(uint nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
        
        if(ObjectPlugin.GetInt(this.oid, "accountId") == 0)
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

      public void EmitKeydown(KeydownEventArgs e)
      {
        OnKeydown(this, e);
      }

      public event EventHandler<KeydownEventArgs> OnKeydown = delegate { };

      public class KeydownEventArgs : EventArgs
      {
        public string key { get; }

        public KeydownEventArgs(string key)
        {
          this.key = key;
        }
      }
      public void DoActionOnTargetSelected(uint oPC, Vector3 vTarget)
      {
        this.OnSelectTarget(oPC, vTarget);
      }
      private Action<uint, Vector3> OnSelectTarget = delegate { };
      public void SelectTarget(Action<uint, Vector3> callback)
      {
        this.OnSelectTarget = callback;

        NWScript.EnterTargetingMode(this.oid, NWScript.OBJECT_TYPE_CREATURE);
      }
      public void OnFrostAutoAttackTimedEvent() // conservé pour mémoire, à retravailler
      {
        if (NWScript.GetIsObjectValid(this.autoAttackTarget) == 1)
        {
          NWScript.AssignCommand(this.oid, () => NWScript.ActionCastSpellAtObject(NWScript.SPELL_RAY_OF_FROST, this.autoAttackTarget));
          NWScript.DelayCommand(6.0f, () => this.OnFrostAutoAttackTimedEvent());
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

      public void BoulderBlock()
      {
        BoulderUnblock();
        var location = NWScript.GetLocation(oid);
        blockingBoulder = NWScript.CreateObject(NWScript.OBJECT_TYPE_PLACEABLE, "plc_boulder", location);
        ObjectPlugin.SetPosition(oid, NWScript.GetPositionFromLocation(location));
        NWScript.ApplyEffectToObject(
          NWScript.DURATION_TYPE_PERMANENT,
          NWScript.EffectVisualEffect(NWScript.VFX_DUR_CUTSCENE_INVISIBILITY),
          blockingBoulder
        );
      }

      public void BoulderUnblock()
      {
        NWScript.DestroyObject(blockingBoulder);
      }
      public void CraftJobProgression()
      {
        float RemainingTime = this.craftJob.remainingTime - (float)(DateTime.Now - this.dateLastSaved).TotalSeconds;

        if (RemainingTime < 0)
        {
          this.AcquireCraftedItem();
        }
      }

      public void AcquireCraftedItem()
      {
        if(this.craftJob.name == "blueprint")
        {
          NWScript.SetLocalInt(NWScript.CopyItem(NWScript.StringToObject(this.craftJob.craftedItem), this.oid, 1), "_BLUEPRINT_RUNS", 10);
          NWScript.PostString(oid, $"La copie de votre patron est terminée !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
          // TODO : changer les sons et effets visuels
          PlayerPlugin.PlaySound(oid, "gui_level_up");
          PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);

          this.craftJob.isActive = false;
          return;
        }

        Blueprint blueprint;
        BlueprintType blueprintType = GetBlueprintTypeFromName(this.craftJob.name);

        if (blueprintType == BlueprintType.Invalid)
        {
          Utils.LogMessageToDMs($"AcquireCraftedItem : {NWScript.GetName(this.oid)} - Blueprint invalid - {this.craftJob.name}");
          return;
        }

        if (CollectSystem.blueprintDictionnary.ContainsKey(blueprintType))
          blueprint = CollectSystem.blueprintDictionnary[blueprintType];
        else
          blueprint = new Blueprint(blueprintType);
        
        NWScript.DelayCommand(10.0f, () => this.PlayCraftJobCompletedEffects(blueprint)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco
      }
      public void AcquireSkillPoints()
      {
        SkillSystem.Skill skill;
        if (this.learnableSkills.TryGetValue(this.currentSkillJob, out skill))
        {
          float skillPointRate = this.CalculateSkillPointsPerSecond(skill);
          skill.acquiredPoints += skillPointRate * (float)(DateTime.Now - this.dateLastSaved).TotalSeconds;
          double RemainingTime = skill.GetTimeToNextLevel(skillPointRate);

          if (RemainingTime <= 0)
          {
            this.LevelUpSkill(skill);
          }
        }
        else
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
        }
      }
      public void RefreshAcquiredSkillPoints(int skillId)
      {
        SkillSystem.Skill skill;
        if (this.learnableSkills.TryGetValue(skillId, out skill))
        {
          float skillPointRate = this.CalculateSkillPointsPerSecond(skill);
          skill.acquiredPoints += skillPointRate * (float)(DateTime.Now - this.dateLastSaved).TotalSeconds;
          this.dateLastSaved = DateTime.Now;
          double remainingTime = skill.GetTimeToNextLevel(skillPointRate);
          this.playerJournal.skillJobCountDown = DateTime.Now.AddSeconds(remainingTime);

          if (remainingTime <= 0)
            this.LevelUpSkill(skill);
        }
      }
      public float CalculateSkillPointsPerSecond(SkillSystem.Skill skill)
      {
        float SP = (float)(NWScript.GetAbilityScore(oid, skill.primaryAbility) + (NWScript.GetAbilityScore(oid, skill.secondaryAbility) / 2)) / 60;

        switch (this.bonusRolePlay)
        {
          case 0:
            SP = SP * 80 / 100;
            break;
          case 1:
            SP = SP * 90 / 100;
            break;
          case 3:
            SP = SP * 110 / 100;
            break;
          case 4:
            SP = SP * 120 / 100;
            break;
        }

        if (!this.isConnected)
          SP = SP * 60 / 100;
        else if (this.isAFK)
          SP = SP * 80 / 100;

        return SP;
      }
      public void LevelUpSkill(SkillSystem.Skill skill)
      {
        if (!Convert.ToBoolean(CreaturePlugin.GetKnowsFeat(oid, skill.oid)))
        {
          CreaturePlugin.AddFeat(oid, skill.oid);
          this.PlayNewSkillAcquiredEffects(skill);
        }
        else
        {
          int value;
          int skillCurrentLevel = CreaturePlugin.GetHighestLevelOfFeat(oid, skill.oid);
          if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", skillCurrentLevel), out value))
          {
            CreaturePlugin.AddFeat(oid, value);
            CreaturePlugin.RemoveFeat(oid, value);
          }
          else
          {
            Utils.LogMessageToDMs($"SKILL LEVEL UP ERROR - Player : {NWScript.GetName(oid)}, Skill : {skill.name} ({skill.oid}), Current level : {skillCurrentLevel}");
          }
        }

        Func<PlayerSystem.Player, int, int> handler;
        if (SkillSystem.RegisterAddCustomFeatEffect.TryGetValue(skill.oid, out handler))
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

        skill.trained = true;
        this.currentSkillJob = (int)Feat.Invalid;

        if (skill.successorId > 0)
        {
          this.learnableSkills.Add(skill.successorId, new SkillSystem.Skill(skill.successorId, 0, this));
        }
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
        NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco

        this.removeableMalus.Remove(skill.oid);
      }

      public void PlayNewSkillAcquiredEffects(SkillSystem.Skill skill)
      {
        NWScript.PostString(oid, $"Votre apprentissage {skill.name} est terminé !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        PlayerPlugin.PlaySound(oid, "gui_level_up");
        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);
        skill.CloseSkillJournalEntry();
      }

      public void PlayCraftJobCompletedEffects(Blueprint blueprint)
      {
        NWScript.PostString(oid, $"La création de votre {this.craftJob.name} est terminée !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        // TODO : changer les sons et effets visuels
        PlayerPlugin.PlaySound(oid, "gui_level_up");
        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);

        CollectSystem.AddCraftedItemProperties(NWScript.CreateItemOnObject(blueprint.craftedItemTag, oid), blueprint, this.craftJob.material);
        this.craftJob.isActive = false;
        this.craftJob.CloseCraftJournalEntry();
      }

      public void PlayNoCurrentTrainingEffects()
      {
        NWScript.PostString(oid, $"Vous n'avez aucun apprentissage en cours !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        NWScript.SendMessageToPC(oid, "Vous n'avez aucun apprentissage en cours !");
        PlayerPlugin.PlaySound(oid, "gui_dm_drop");
        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_REDUCE_ABILITY_SCORE);
      }
      public void DoActionOnMiningCycleCancelled()
      {
        this.OnMiningCycleCancelled();
      }
      public void DoActionOnMiningCycleCompleted()
      {
        this.OnMiningCycleCompleted();
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

        if (this.playerJournal.craftJobCountDown != null && Convert.ToBoolean(NWScript.GetLocalInt(NWScript.GetArea(this.oid), "_REST")))
        {
          journalEntry = PlayerPlugin.GetJournalEntry(this.oid, "craft_job");
          journalEntry.sName = $"Travail artisanal - {Utils.StripTimeSpanMilliseconds((TimeSpan)(this.playerJournal.craftJobCountDown - DateTime.Now))}";
          PlayerPlugin.AddCustomJournalEntry(this.oid, journalEntry, 1);

          this.CraftJobProgression();
        }

        if (this.playerJournal.skillJobCountDown != null)
        {
          this.RefreshAcquiredSkillPoints(this.currentSkillJob);
          journalEntry = PlayerPlugin.GetJournalEntry(this.oid, "skill_job");
          journalEntry.sName = $"Entrainement - {Utils.StripTimeSpanMilliseconds((TimeSpan)(this.playerJournal.skillJobCountDown - DateTime.Now))}";
          PlayerPlugin.AddCustomJournalEntry(this.oid, journalEntry, 1);
        }

        if(this.DoJournalUpdate)
          NWScript.DelayCommand(1.0f, () => this.UpdateJournal());
      }
      public void rebootUpdate()
      {
        JournalEntry journalEntry = PlayerPlugin.GetJournalEntry(this.oid, "reboot");
        journalEntry.sName = $"REBOOT SERVEUR - {Utils.StripTimeSpanMilliseconds((TimeSpan)(this.playerJournal.rebootCountDown - DateTime.Now))}";
        PlayerPlugin.AddCustomJournalEntry(this.oid, journalEntry);

        NWScript.DelayCommand(1.0f, () => this.rebootUpdate());
      }
    }
  }
}
