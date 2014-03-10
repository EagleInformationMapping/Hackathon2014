using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace SAMVoteApp
{
    public class Geofeedia
    {
        public string Title { get; set;  }
        public BitmapImage Source { get; set;  }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
    }
}
