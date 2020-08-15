using System;
using System.Collections.Generic;
using System.Security.Authentication;
using NWN.Enums;
using NWN.Enums.VisualEffect;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public class Player : NWPlayer
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
      public string lastTargetedCommandArgument { get; set; }
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
      public Dictionary<uint, NWCreature> summons = new Dictionary<uint, NWCreature>();
      public Dictionary<int, SkillSystem.Skill> learnableSkills = new Dictionary<int, SkillSystem.Skill>();
      public Dictionary<int, SkillSystem.Skill> removeableMalus = new Dictionary<int, SkillSystem.Skill>();

      public Player(uint nwobj) : base(nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
        // TODO : ajouter IsNewPlayer = résultat de la requête en BDD pour voir si on a déjà des infos sur lui ou pas !

        // TODO : charger la liste à partir de la BDD et l'état d'avancement des SP. Mettre en place le système d'apprentissage via livre
        this.learnableSkills.Add(1116, new SkillSystem.Skill(1116, NWNX.Object.GetFloat(this, "_JOB_SP_1116")));
        this.learnableSkills.Add(122, new SkillSystem.Skill(122, NWNX.Object.GetFloat(this, "_JOB_SP_122")));
        this.learnableSkills.Add(128, new SkillSystem.Skill(128, NWNX.Object.GetFloat(this, "_JOB_SP_128")));
        this.learnableSkills.Add(133, new SkillSystem.Skill(133, NWNX.Object.GetFloat(this, "_JOB_SP_133")));
        int successorId;
        if (int.TryParse(NWScript.Get2DAString("feat", "SUCCESSOR", NWNX.Creature.GetHighestLevelOfFeat(this, 1132)), out successorId))
        {
          this.learnableSkills.Add(successorId, new SkillSystem.Skill(successorId, NWNX.Object.GetFloat(this, $"_JOB_SP_{successorId}")));
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
      public void EmitTargetSelection(TargetSelectionEventArgs e)
      {
        OnTargetSelection(this, e);
      }

      public event EventHandler<TargetSelectionEventArgs> OnTargetSelection = delegate { };
      public class TargetSelectionEventArgs : EventArgs
      {
        public uint target { get; }
        public Vector position { get; }

        public TargetSelectionEventArgs(uint target, Vector position)
        {
          this.target = target;
          this.position = position;
        }
      }

      public void OnFrostAutoAttackTimedEvent() // conservé pour mémoire, à retravailler
      {
        if (this.autoAttackTarget.AsObject().IsValid)
        {
          this.CastSpellAtObject(Spell.RayOfFrost, this.autoAttackTarget);
          NWScript.DelayCommand(6.0f, () => this.OnFrostAutoAttackTimedEvent());
        }
      }
      public void RemoveLycanCurse()
      {
        this.ApplyEffect(DurationType.Instant, NWScript.EffectVisualEffect((VisualEffect)Impact.SuperHeroism));
        NWNX.Rename.ClearPCNameOverride(this, null, true);
        NWNX.Creature.SetMovementRate(this, MovementRate.PC);

        NWNX.Events.RemoveObjectFromDispatchList("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", this);
      }
      public void ApplyLycanCurse()
      {
        Effect ePoly = NWScript.EffectPolymorph(107, true);
        Effect eLink = NWScript.SupernaturalEffect(ePoly);
        eLink = NWScript.TagEffect(eLink, "lycan_curse");

        this.ApplyEffect(DurationType.Temporary, eLink, 900.0f);
        this.ApplyEffect(DurationType.Instant, NWScript.EffectVisualEffect((VisualEffect)Impact.SuperHeroism));

        NWNX.Rename.SetPCNameOverride(this, "Loup-garou", "", "", NWNX.Enum.NameOverrideType.Override);
        NWNX.Creature.SetMovementRate(this, MovementRate.Fast);

        NWNX.Events.AddObjectToDispatchList("NWNX_ON_EFFECT_REMOVED_AFTER", "event_effects", this);
      }

      public void BoulderBlock()
      {
        BoulderUnblock();
        var location = NWScript.GetLocation(oid);
        blockingBoulder = NWScript.CreateObject(Enums.ObjectType.Placeable, "plc_boulder", location, false);
        NWNX.Object.SetPosition(oid, NWScript.GetPositionFromLocation(location));
        NWScript.ApplyEffectToObject(
          Enums.DurationType.Permanent,
          NWScript.EffectVisualEffect((VisualEffect)Temporary.CutsceneInvisibility),
          blockingBoulder
        );
      }

      public void BoulderUnblock()
      {
        NWScript.DestroyObject(blockingBoulder);
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
            Utils.LogException(new Exception($"SKILL LEVEL UP ERROR - Player : {this.Name}, Skill : {skill.name} ({skill.oid}), Current level : {skillCurrentLevel}"));
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
        NWScript.PostString(this, $"Votre apprentissage {skill.name} est terminé !", 80, 10, ScreenAnchor.TopLeft, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        NWNX.Player.PlaySound(this, "gui_level_up", this);
        NWNX.Player.ApplyInstantVisualEffectToObject(this, this, (int)Impact.GlobeUse);
      }

      public void PlayNoCurrentTrainingEffects()
      {
        NWScript.PostString(this, $"Vous n'avez aucun apprentissage en cours !", 80, 10, ScreenAnchor.TopLeft, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
        this.SendMessage("Vous n'avez aucun apprentissage en cours !");
        NWNX.Player.PlaySound(this, "gui_dm_drop", this);
        NWNX.Player.ApplyInstantVisualEffectToObject(this, this, (int)Impact.ReduceAbilityScore);
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
        
        if (NWNX.Creature.GetHighestLevelOfFeat(this, iRandomMalus) != 65535)
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
