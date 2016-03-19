using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBID
{
    public class ImagePost
    {
        public string ImageUrl;
        public int Id;
        public string BlogUrl;

        public ImagePost(string url, int id, string blogUrl)
        {
            ImageUrl = url;
            this.Id = id;
            this.BlogUrl = blogUrl;
        }

        public static List<ImagePost> CreateImagePostsFromResponse(string response)
        {
            List<ImagePost> ResultList = new List<ImagePost>();



            return ResultList;
        }
    }
}
