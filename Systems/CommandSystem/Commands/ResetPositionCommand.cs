using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  class ResetPosition
  {
    public ResetPosition(NwPlayer oPC)
    {
      NWScript.SetObjectVisualTransform(oPC.ControlledCreature, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_X, 0.0f);
      NWScript.SetObjectVisualTransform(oPC.ControlledCreature, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Y, 0.0f);
      NWScript.SetObjectVisualTransform(oPC.ControlledCreature, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 0.0f);
      NWScript.SetObjectVisualTransform(oPC.ControlledCreature, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0.0f);
      NWScript.SetObjectVisualTransform(oPC.ControlledCreature, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Y, 0.0f);
      NWScript.SetObjectVisualTransform(oPC.ControlledCreature, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_Z, 0.0f);
    }
  }
}
