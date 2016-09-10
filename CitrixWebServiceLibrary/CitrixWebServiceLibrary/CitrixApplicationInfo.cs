using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitrixWebServiceLibrary
{
    /// <summary>
    /// metadate describing a Storefront App. These get populated from the JSON response after a successful query to get resources
    /// </summary>
    public class CitrixApplicationInfo
    {
        public string ID { get; set; }
        public String AppTitle { get; set; }
        public String AppLaunchURL { get; set; }
        public String AppIcon { get; set; }
        public String AppDesc { get; set; }
        public String Subscribed { get; set; }
        public String SubscriptionUrl { get; set; }
        public int Position { get; set; }
    }
}
