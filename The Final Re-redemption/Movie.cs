using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace The_Final_Re_redemption
{
    public class Movie : INotifyPropertyChanged 
    {
        public string Id { get; set; }
        public double Rating { get; set; }
        private double myrating;
        public double MyRating
        {
            get { return myrating; }
            set
            {
                if (value < 10.01 && value >= 0)
                {
                    myrating = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("myrating"));
                    }
                }
                else
                {
                    throw new Exception("Your new value must be between 0 and 10.");
                }
            }
        }
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
        [XmlAttribute] //put name as an attribute in XML-movie-element
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
