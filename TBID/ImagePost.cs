using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBID
{
    public class ImagePost
    {
        public string ImageURL;
        public int ID;
        public string BlogURL;

        public ImagePost(string URL, int ID, string BlogURL)
        {
            ImageURL = URL;
            this.ID = ID;
            this.BlogURL = BlogURL;
        }
    }
}
