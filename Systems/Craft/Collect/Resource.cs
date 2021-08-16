using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWN.Systems
{
  public enum ResourceType
  {
    Ore,
    Mineral,
    Wood,
    Plank,
    Pelt,
    Leather,
    Plant
  }
  public class Resource
  {
    public readonly ResourceType type;
    public readonly int id;
    public double quantity;
  }
}
