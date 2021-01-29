using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWN.Systems
{
  class ScriptPerf
  {
    public int nbExecution { get; set; }
    public double cumulatedExecutionTime { get; set; }
    public ScriptPerf(double executionTime)
    {
      nbExecution = 1;
      cumulatedExecutionTime = executionTime;
    }
  }
}
