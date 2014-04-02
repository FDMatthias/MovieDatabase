using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Final_Re_redemption
{
    public class PlayedIn
    {
        public string MovieName { get; set; }
        public string MovieId { get; set; }
        public PlayedIn(string name, string id)
        {
            MovieName = name;
            MovieId = id;
        }
        public PlayedIn(PlayedIn item)
        {
            MovieName = item.MovieName;
            MovieId = item.MovieId;
        }
        public PlayedIn()
        {

        }
        public override string ToString()
        {
            return "#" + MovieId + "#" + MovieName;
        }
    }
}
