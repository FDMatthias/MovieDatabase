using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace The_Final_Re_redemption
{
    public class Actor
    {
        [XmlAttribute] //put name as an attribute in XML-movie-element
        public string Name { get; set; }
        public ObservableCollection<PlayedIn> ActorOf { get; set; }
        public Actor(string name, ObservableCollection<PlayedIn> list)
        {
            Name = name;
            ActorOf = list;
        }
        public Actor(Actor givenactor)
        {
            Name = givenactor.Name;
            ActorOf = givenactor.ActorOf;
        }

        public Actor()
        {
            Name = "No name set";
            ActorOf = new ObservableCollection<PlayedIn>();
        }
        public override string ToString()
        {
            string ToReturn = Name;
            foreach (var playedIn in ActorOf)
            {
                ToReturn += playedIn.ToString();
            }
            return ToReturn;
        }
    }
}
