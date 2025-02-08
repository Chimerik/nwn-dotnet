using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyPolymorph()
      {
        PolymorphType type = (PolymorphType)oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_SAVED_SHAPE").Value;

        if (type > PolymorphType.Werewolf)
        {
          if(oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_SHAPECHANGE_SHAPE_EXPIRATION").HasValue
            && DateTime.Compare(DateTime.Parse(oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_SHAPECHANGE_SHAPE_EXPIRATION").Value), DateTime.Now) > 0)
          {
            oid.LoginCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.Polymorph(oid.LoginCreature, type), DateTime.Parse(oid.LoginCreature.GetObjectVariable<PersistentVariableString>("_SHAPECHANGE_SHAPE_EXPIRATION").Value) - DateTime.Now);
          }
          else
          {
            oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(oid.LoginCreature, type));
          }

          oid.LoginCreature.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_SAVED_SHAPE").Delete();
        }
      }
    }
  }
}
