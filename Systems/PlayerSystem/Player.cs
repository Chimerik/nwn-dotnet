using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.Enums.VisualEffect;

namespace NWN.Systems
{
  public static partial class PlayerSystem
  {
    public class Player : NWPlayer
    {
      public readonly uint oid;
      public readonly Boolean IsNewPlayer;
      public Boolean isConnected { get; set; }
      public Boolean isAFK { get; set; }
      public uint AutoAttackTarget { get; set; }
      public DateTime LycanCurseTimer { get; set; }
      public Menu menu { get; }

      private uint blockingBoulder;
      public string DisguiseName { get; set; }
      private List<uint> _SelectedObjectsList = new List<uint>();
      public List<uint> SelectedObjectsList
      {
        get => _SelectedObjectsList;
        // set => _SelectedObjectsList.Add(value);
      }

      public Dictionary<uint, Player> Listened = new Dictionary<uint, Player>();
      public Dictionary<uint, Player> Blocked = new Dictionary<uint, Player>();
      public Dictionary<uint, DateTime> DisguiseDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> PickpocketDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> InviDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, DateTime> InviEffectDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, NWCreature> Summons = new Dictionary<uint, NWCreature>();
      public Dictionary<int, SkillSystem.Skill> LearnableSkills = new Dictionary<int, SkillSystem.Skill>();

      public Player(uint nwobj) : base(nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
        //TODO : ajouter IsNewPlayer = résultat de la requête en BDD pour voir si on a déjà des infos sur lui ou pas !
        
        // TODO : charger la liste à partir de la BDD et l'état d'avancement des SP. Mettre en place le système d'apprentissage via livre
        this.LearnableSkills.Add(1116, new SkillSystem.Skill(1116, NWNX.Object.GetFloat(this, "_JOB_SP_1116")));
        this.LearnableSkills.Add(122, new SkillSystem.Skill(122, NWNX.Object.GetFloat(this, "_JOB_SP_122")));
        this.LearnableSkills.Add(128, new SkillSystem.Skill(128, NWNX.Object.GetFloat(this, "_JOB_SP_128")));
        this.LearnableSkills.Add(133, new SkillSystem.Skill(133, NWNX.Object.GetFloat(this, "_JOB_SP_133")));
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
      public void OnFrostAutoAttackTimedEvent() // conservé pour mémoire, à retravailler
      {
        if (this.AutoAttackTarget.AsObject().IsValid)
        {
          this.CastSpellAtObject(Spell.RayOfFrost, this.AutoAttackTarget);
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
        if (this.LearnableSkills.TryGetValue(NWNX.Object.GetInt(this, "_CURRENT_JOB"), out skill))
        {
          skill.AcquiredPoints += this.CalculateAcquiredSkillPoints(skill);

          double RemainingTime = skill.GetTimeToNextLevel(this);
          NWNX.Object.SetFloat(this, $"_JOB_SP_{skill.oid}", skill.AcquiredPoints, true);
          if (RemainingTime < 0)
          {
            this.LevelUpSkill(skill);
          }
          else if (RemainingTime < 600)
          {
            NWScript.AssignCommand(this, () => NWScript.DelayCommand((float)RemainingTime, () => LevelUpSkill(skill)));
          }
        }
      }
      public double RefreshAcquiredSkillPoints()
      {
        SkillSystem.Skill skill;
        if (this.LearnableSkills.TryGetValue(NWNX.Object.GetInt(this, "_CURRENT_JOB"), out skill))
        {
          skill.AcquiredPoints += this.CalculateAcquiredSkillPoints(skill);

          double RemainingTime = skill.GetTimeToNextLevel(this);
          NWNX.Object.SetFloat(this, $"_JOB_SP_{skill.oid}", skill.AcquiredPoints, true);
          NWNX.Object.SetString(this, "_DATE_LAST_SAVED", DateTime.Now.ToString(), true);

          return RemainingTime;
        }

        return 0;
      }

      private float CalculateAcquiredSkillPoints(SkillSystem.Skill skill)
      {
        var ElapsedSeconds = (float)(DateTime.Now - DateTime.Parse(NWNX.Object.GetString(this, "_DATE_LAST_SAVED"))).TotalSeconds;
        float SP = (float)(NWScript.GetAbilityScore(this, skill.PrimaryAbility) + (NWScript.GetAbilityScore(this, skill.SecondaryAbility) / 2)) * ElapsedSeconds / 60;

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
          NWNX.Object.DeleteInt(this, "_CURRENT_JOB");
          NWScript.DelayCommand(10.0f, () => this.PlayNewSkillAcquiredEffects(skill)); // Décalage de 10 secondes pour être sur que le joueur a fini de charger la map à la reco
        }

        skill.CurrentLevel += 1;

        if (skill.CurrentLevel >= skill.MaxLevel)
          this.LearnableSkills.Remove(skill.oid);
      }

      public void PlayNewSkillAcquiredEffects(SkillSystem.Skill skill)
      {
        NWScript.PostString(this, $"Votre apprentissage {skill.Name} {skill.CurrentLevel} est terminé !", 80, 10, ScreenAnchor.TopLeft, 5.0f, unchecked((int)0xC0C0C0FF), unchecked((int)0xC0C0C0FF), 9, "fnt_galahad14");
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
    }
  }
}
