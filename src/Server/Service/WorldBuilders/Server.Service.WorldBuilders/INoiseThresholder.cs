using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Service.WorldBuilders;

internal interface INoiseThresholder {

	float[,] Threshold( float[,] source, float threshold );

	float[,] Range( float[,] source, float min, float max );
}

