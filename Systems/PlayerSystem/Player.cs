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
      public virtual uint AutoAttackTarget { get; set; }
      public virtual DateTime LycanCurseTimer { get; set; }

      private List<uint> _SelectedObjectsList = new List<uint>();
      public virtual List<uint> SelectedObjectsList
      {
        get => _SelectedObjectsList;
        // set => _SelectedObjectsList.Add(value);
      }

      public Player(uint nwobj) : base(nwobj)
      {
        this.oid = nwobj;
      }

      public void EmitKeydown(KeydownEventArgs e)
      {
        OnKeydown?.Invoke(this, e);
      }

      public event EventHandler<KeydownEventArgs> OnKeydown;

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
      public void BlockPlayer()
      {
        Location locPlayer = NWScript.GetLocation(this);
        uint oBoulder = NWScript.CreateObject(ObjectType.Placeable, "plc_boulder", locPlayer, false, "_PC_BLOCKER");
        NWNX.Object.SetPosition(this, NWScript.GetPositionFromLocation(locPlayer));
        oBoulder.AsObject().ApplyEffect(DurationType.Permanent, NWScript.EffectVisualEffect((VisualEffect)Temporary.CutsceneInvisibility));
      }

      public void UnblockPlayer()
      {
        uint oBlocker = NWScript.GetNearestObjectByTag("_PC_BLOCKER", this);
        foreach (Effect e in oBlocker.AsObject().Effects)
          NWScript.RemoveEffect(oBlocker, e);

        oBlocker.AsObject().Area.AssignCommand(() => NWScript.DelayCommand(0.01f, () => oBlocker.AsObject().Destroy()));
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
    }
  }
}
