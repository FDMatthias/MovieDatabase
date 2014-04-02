using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Final_Re_redemption
{
    public class Movie : INotifyPropertyChanged 
    {
        public string Id { get; set; }
        public double Rating { get; set; }
        public double MyRating { get; set; }
        private bool seenit;
        public bool SeenIt {
            get
            {
                return seenit;
            }
            set
            {
                seenit = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SeenIt"));
                }
            }
        }
        private bool wishlisted;
        public bool Wishlisted
        {
            get
            {
                return wishlisted;
            }
            set
            {
                wishlisted = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Wishlisted"));
                }
            }
        }
        public string Name { get; set; }
        public string Year { get; set; }
        public string Synopsis { get; set; }
        public string Poster { get; set; }
        public string Genres { get; set; }

        public override string ToString()
        {
            return Id + "#" + Name + "#" + Year + "#" + Rating + "#" + MyRating + "#" + SeenIt + "#" + Wishlisted + "#" + Poster + "#" + Genres + "#" + Synopsis;
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
