using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
interface IBugBehaviour
{
    float MaxSize { get; set; }
    float MaxEnergy { get; set; }
    float MaxSpeed { get; set; }
    float MaxLifespan { get; set; }
    float MaxMemory { get; set; }
    float MaxDistanceOfView { get; set; }
}