using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMVoteApp
{
    public class VotingLocation
    {
        public string ID { get; set;  }
        public string Name { get; set;  }
        public string Address { get; set;  }
        public int WaitTime { get; set; }
        public Geometry Location { get; set;  }
    }

    public class Twilio
    {
        public string Message { get; set; }
    }
}
