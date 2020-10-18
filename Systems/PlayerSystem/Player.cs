using System;
using System.Numerics;
using System.Collections.Generic;
using System.Security.Authentication;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public class Player
    { 
      public readonly uint oid;
      public readonly Boolean isNewPlayer;
      public Boolean isConnected { get; set; }
      public Boolean isAFK { get; set; }
      public uint autoAttackTarget { get; set; }
      public DateTime lycanCurseTimer { get; set; }
      public Menu menu { get; }

      private uint blockingBoulder;
      public string disguiseName { get; set; }
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

      public Action OnMiningCycleCancelled = delegate { };
      public Action OnMiningCycleCompleted = delegate { };

      public Player(uint nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
        // TODO : ajouter IsNewPlayer = résultat de la requête en BDD pour voir si on a déjà des infos sur lui ou pas !

        // TODO : charger la liste à partir de la BDD et l'état d'avancement des SP. Mettre en place le système d'apprentissage via livre
        this.learnableSkills.Add(1116, new SkillSystem.Skill(1116, ObjectPlugin.GetFloat(this.oid, "_JOB_SP_1116")));
        this.learnableSkills.Add(122, new SkillSystem.Skill(122, ObjectPlugin.GetFloat(this.oid, "_JOB_SP_122")));
        this.learnableSkills.Add(128, new SkillSystem.Skill(128, ObjectPlugin.GetFloat(this.oid, "_JOB_SP_128")));
        this.learnableSkills.Add(133, new SkillSystem.Skill(133, ObjectPlugin.GetFloat(this.oid, "_JOB_SP_133")));
        int successorId;
        if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", CreaturePlugin.GetHighestLevelOfFeat(this.oid, 1132)), out successorId))
        {
          this.learnableSkills.Add(successorId, new SkillSystem.Skill(successorId, ObjectPlugin.GetFloat(this.oid, $"_JOB_SP_{successorId}")));
        }
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
        float RemainingTime = ObjectPlugin.GetFloat(oid, "_CURRENT_CRAFT_JOB_REMAINING_TIME") - (float)(DateTime.Now - DateTime.Parse(ObjectPlugin.GetString(oid, "_DATE_LAST_SAVED"))).TotalSeconds;

        if (RemainingTime < 0)
        {
          this.AcquireCraftedItem();
        }
        else if (RemainingTime < 600)
        {
          NWScript.AssignCommand(oid, () => NWScript.DelayCommand((float)RemainingTime, () => AcquireCraftedItem()));
        }
      }

      public void AcquireCraftedItem()
      {
        CollectSystem.Blueprint blueprint;
        CollectSystem.BlueprintType blueprintType = CollectSystem.GetBlueprintTypeFromName(NWNX.Object.GetString(this, "_CURRENT_CRAFT_JOB"));

        if (blueprintType == CollectSystem.BlueprintType.Invalid)
        {
          // TODO : envoyer l'erreur sur Discord
          return;
        }

        if (CollectSystem.blueprintDictionnary.ContainsKey(blueprintType))
          blueprint = CollectSystem.blueprintDictionnary[blueprintType];
        else
          blueprint = new CollectSystem.Blueprint(blueprintType);
        
        NWScript.DelayCommand(10.0f, () => this.PlayCraftJobCompletedEffects(blueprint)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco
      }
      public void AcquireSkillPoints()
      {
        SkillSystem.Skill skill;
        if (this.learnableSkills.TryGetValue(NWNX.Object.GetInt(this, "_CURRENT_JOB"), out skill))
        {
          skill.acquiredPoints += this.CalculateAcquiredSkillPoints(skill);

          double RemainingTime = skill.GetTimeToNextLevel(this);
          NWNX.Object.SetFloat(this, $"_JOB_SP_{skill.oid}", skill.acquiredPoints, true);

          if (RemainingTime < 0)
          {
            this.LevelUpSkill(skill);
          }
          else if (RemainingTime < 600)
          {
            NWScript.AssignCommand(this, () => NWScript.DelayCommand((float)RemainingTime, () => LevelUpSkill(skill)));
          }
        }
        else
        {
          if (this.removeableMalus.TryGetValue(NWNX.Object.GetInt(this, "_CURRENT_JOB"), out skill))
          {
            skill.acquiredPoints += this.CalculateAcquiredSkillPoints(skill);

            double RemainingTime = skill.GetTimeToNextLevel(this);
            NWNX.Object.SetFloat(this, $"_JOB_SP_{skill.oid}", skill.acquiredPoints, true);
            if (RemainingTime < 0)
            {
              this.RemoveMalus(skill);
            }
            else if (RemainingTime < 600)
            {
              NWScript.AssignCommand(this, () => NWScript.DelayCommand((float)RemainingTime, () => RemoveMalus(skill)));
            }
          }
        }
      }
      public double RefreshAcquiredSkillPoints()
      {
        SkillSystem.Skill skill;
        if (this.learnableSkills.TryGetValue(NWNX.Object.GetInt(this, "_CURRENT_JOB"), out skill))
        {
          skill.acquiredPoints += this.CalculateAcquiredSkillPoints(skill);

          double RemainingTime = skill.GetTimeToNextLevel(this);
          NWNX.Object.SetFloat(this, $"_JOB_SP_{skill.oid}", skill.acquiredPoints, true);
          NWNX.Object.SetString(this, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);

          return RemainingTime;
        }

        return 0;
      }

      private float CalculateAcquiredSkillPoints(SkillSystem.Skill skill)
      {
        var ElapsedSeconds = (float)(DateTime.Now - DateTime.Parse(NWNX.Object.GetString(this, "_DATE_LAST_SAVED"))).TotalSeconds;
        float SP = (float)(NWScript.GetAbilityScore(this, skill.primaryAbility) + (NWScript.GetAbilityScore(this, skill.secondaryAbility) / 2)) * ElapsedSeconds / 60;

        switch (NWNX.Object.GetInt(this, "_BRP"))
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
        if (!this.HasFeat((Feat)skill.oid))
        {
          this.AddFeat((Feat)skill.oid);
          NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco
        }
        else
        {
          int value;
          int skillCurrentLevel = NWNX.Creature.GetHighestLevelOfFeat(this, skill.oid);
          if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", skillCurrentLevel), out value))
          {
            this.AddFeat((Feat)value);
            this.RemoveFeat((Feat)skill.oid);
          }
          else
          {
            Utils.LogMessageToDMs($"SKILL LEVEL UP ERROR - Player : {this.Name}, Skill : {skill.name} ({skill.oid}), Current level : {skillCurrentLevel}");
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

        NWNX.Object.DeleteInt(this, "_CURRENT_JOB");
        this.learnableSkills.Remove(skill.oid);

        if (skill.successorId > 0)
        {
          this.learnableSkills.Add(skill.successorId, new SkillSystem.Skill(skill.successorId, 0));
        }
      }

      public void RemoveMalus(SkillSystem.Skill skill)
      {
        this.RemoveFeat((Feat)skill.oid);

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

        NWNX.Object.DeleteInt(this, "_CURRENT_JOB");
        NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco

        this.removeableMalus.Remove(skill.oid);
      }

      public void PlayNewSkillAcquiredEffects(SkillSystem.Skill skill)
      {
        NWScript.PostString(oid, $"Votre apprentissage {skill.name} est terminé !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        PlayerPlugin.PlaySound(oid, "gui_level_up");
        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);
      }

      public void PlayCraftJobCompletedEffects(CollectSystem.Blueprint blueprint)
      {
        NWScript.PostString(oid, $"La création de votre {ObjectPlugin.GetString(oid, "_CURRENT_CRAFT_JOB")} est terminée !", 80, 10, NWScript.SCREEN_ANCHOR_TOP_LEFT, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        // TODO : changer les sons et effets visuels
        PlayerPlugin.PlaySound(oid, "gui_level_up");
        PlayerPlugin.ApplyInstantVisualEffectToObject(oid, oid, NWScript.VFX_IMP_GLOBE_USE);

        CollectSystem.AddCraftedItemProperties(NWScript.CreateItemOnObject(blueprint.craftedItemTag, oid), blueprint, ObjectPlugin.GetString(oid, "_CURRENT_CRAFT_JOB_MATERIAL"));

        ObjectPlugin.DeleteString(oid, "_CURRENT_CRAFT_JOB");
        ObjectPlugin.DeleteFloat(oid, "_CURRENT_CRAFT_JOB_REMAINING_TIME");
        ObjectPlugin.DeleteString(oid, "_CURRENT_CRAFT_JOB_MATERIAL");
      }

      public void PlayNoCurrentTrainingEffects()
      {
        NWScript.PostString(this, $"Vous n'avez aucun apprentissage en cours !", 80, 10, ScreenAnchor.TopLeft, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
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
      public void SendToLimbo()
      {
        // Heal PC
        this.ApplyEffect(DurationType.Instant, NWScript.EffectVisualEffect((VisualEffect)Impact.RestorationGreater));
        this.ApplyEffect(DurationType.Instant, NWScript.EffectResurrection());
        this.ApplyEffect(DurationType.Instant, NWScript.EffectHeal(this.MaxHP));

        // TP PC
        NWScript.AssignCommand(this, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP__RESPAWN_AREA"))));
      }
      public void Respawn()
      {
        // TODO : Appliquer les bonus en fonction de l'entité choisie pour respawn (+augmentation du niveau d'influence de l'entité)
        // TODO : Diminuer la durabilité de tous les objets équipés et dans l'inventaire du PJ

        this.DestroyCorpses();
        NWScript.AssignCommand(this, () => NWScript.JumpToLocation(NWScript.GetLocation(NWScript.GetWaypointByTag("WP_START_NEW_CHAR")))); // TODO : le respawn se fera plutôt à l'hospice des taudis
        this.SendMessage("Votre récente déconvenue vous a affligé d'une blessure durable. Il va falloir passer du temps en rééducation pour vous en débarrasser");

        int iRandomMalus = Utils.random.Next(1130, 1130); // TODO : il faudra mettre en paramètre de conf le range des feat ID pour les malus
        
        if (NWNX.Creature.GetHighestLevelOfFeat(this, iRandomMalus) != (int)Feat.INVALID_FEAT)
        {  
          int successorId;
          if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", iRandomMalus), out successorId))
          {
            this.AddFeat((Feat)successorId);
            iRandomMalus = successorId;
          }
        }
        else
          this.AddFeat((Feat)iRandomMalus); 

        Func<PlayerSystem.Player, int, int> handler;
        if (SkillSystem.RegisterAddCustomFeatEffect.TryGetValue(iRandomMalus, out handler))
        {
          try
          {
            handler.Invoke(this, iRandomMalus);
          }
          catch (Exception e)
          {
            Utils.LogException(e);
          }
        }
      }
      public void DestroyCorpses()
      {
        NWPlaceable oCorpse = NWScript.GetObjectByTag("pccorpse").AsPlaceable();
        int i = 1;
        int PcId = NWNX.Object.GetInt(this, "_PC_ID");
        while (oCorpse.IsValid)
        {
          if (PcId == oCorpse.Locals.Int.Get("_PC_ID"))
          {
            oCorpse.Destroy();
            // TODO : supprimer l'objet serialized de la BDD where _PC_ID
            break;
          }
          oCorpse = NWScript.GetObjectByTag("pccorpse", i++).AsPlaceable();
        }

        NWItem oCorpseItem = NWScript.GetObjectByTag("item_pccorpse").AsItem();
        i = 1;
        while (oCorpseItem.IsValid)
        {
          if (PcId == oCorpseItem.Locals.Int.Get("_PC_ID"))
          {
            oCorpseItem.Destroy();
            break;
          }
          oCorpseItem = NWScript.GetObjectByTag("item_pccorpse", i++).AsItem();
        }
      }
      public Effect GetPartySizeEffect(int iPartySize = 0)
      {
        NWPlayer oPartyMember = NWScript.GetFirstFactionMember(this, true).AsPlayer();
        while (oPartyMember.IsValid)
        {
          iPartySize++;
          oPartyMember = NWScript.GetNextFactionMember(this, true).AsPlayer();
        }

        Effect eParty = null;

        switch (iPartySize) // déterminer quel est l'effet de groupe à appliquer
        {
          case 1:
            break;
          case 2:
            eParty = NWScript.TagEffect(NWScript.EffectACIncrease(1, Enums.Item.Property.ArmorClassModiferType.Dodge), "PartyEffect");
            break;
          case 3:
            eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, Enums.Item.Property.ArmorClassModiferType.Dodge), NWScript.EffectAttackIncrease(1));
            eParty = NWScript.TagEffect(eParty, "PartyEffect");
            break;
          case 4:
          case 5:
            eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, Enums.Item.Property.ArmorClassModiferType.Dodge), NWScript.EffectAttackIncrease(1));
            eParty = NWScript.EffectLinkEffects(NWScript.EffectDamageIncrease(1, DamageType.Bludgeoning), eParty);
            eParty = NWScript.TagEffect(eParty, "PartyEffect");
            break;
          case 6:
            eParty = NWScript.EffectLinkEffects(NWScript.EffectACIncrease(1, Enums.Item.Property.ArmorClassModiferType.Dodge), NWScript.EffectAttackIncrease(1));
            eParty = NWScript.TagEffect(eParty, "PartyEffect");
            break;
          case 7:
            eParty = NWScript.TagEffect(NWScript.EffectACIncrease(1, Enums.Item.Property.ArmorClassModiferType.Dodge), "PartyEffect");
            break;
          default:
            break;
        }

        return eParty;
      }
    }
  }
}
