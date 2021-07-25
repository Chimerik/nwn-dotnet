using Anvil.API;

using Newtonsoft.Json;

namespace NWN.Systems
{
  class PotionAlchimisteEffect
  {
    public PotionAlchimisteEffect(NwItem potion, NwPlayer oPC)
    {
      string[] jsonArray = potion.GetObjectVariable<LocalVariableString>("_SERIALIZED_PROPERTIES").Value.Split("|") ;

      foreach(string json in jsonArray)
      {
        CustomUnpackedEffect customUnpackedEffect = JsonConvert.DeserializeObject<CustomUnpackedEffect>(json);
        customUnpackedEffect.ApplyCustomUnPackedEffectToTarget(oPC.ControlledCreature);
      }
    }
  }
}
