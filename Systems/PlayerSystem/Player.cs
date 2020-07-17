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
      public virtual Boolean isConnected { get; set; }
      public virtual uint AutoAttackTarget { get; set; }
      public virtual DateTime LycanCurseTimer { get; set; }
      public Menu menu { get; }

      private uint blockingBoulder;
      public virtual string DisguiseName { get; set; }
      private List<uint> _SelectedObjectsList = new List<uint>();
      public virtual List<uint> SelectedObjectsList
      {
        get => _SelectedObjectsList;
        // set => _SelectedObjectsList.Add(value);
      }

      public Dictionary<uint, Player> Listened = new Dictionary<uint, Player>();
      public Dictionary<uint, DateTime> DisguiseDetectTimer = new Dictionary<uint, DateTime>();
      public Dictionary<uint, NWCreature> Summons = new Dictionary<uint, NWCreature>();

      public Player(uint nwobj) : base(nwobj)
      {
        this.oid = nwobj;
        this.menu = new PrivateMenu(this);
        //TODO : ajouter IsNewPlayer = résultat de la requête en BDD pour voir si on a déjà des infos sur lui ou pas !
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
    }
  }
}
