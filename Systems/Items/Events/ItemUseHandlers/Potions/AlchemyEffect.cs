using System.Text.Json;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    private partial class Potion
    {
      public static void AlchemyEffect(OnItemUse onUse)
      {
        string[] jsonArray = onUse.Item.GetObjectVariable<LocalVariableString>("_SERIALIZED_PROPERTIES").Value.Split("|");

        foreach (string json in jsonArray)
        {
          CustomUnpackedEffect customUnpackedEffect = JsonSerializer.Deserialize<CustomUnpackedEffect>(json);
          
          if (onUse.TargetObject == null)
            customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(onUse.UsedBy, onUse.Item);
          else
            customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(onUse.TargetObject, onUse.Item);
        }
      }
    }
  }
}
