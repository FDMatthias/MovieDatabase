using System;
using System.Collections.Generic;
using System.Windows;

namespace The_Final_Re_redemption
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        public Statistics()
        {
            InitializeComponent();
            UpdateStats();
        }

        public void UpdateStats()
        {
            int SeenMovies = 0;
            int wishlistMovies = 0;
            double highestRating = 0.0;
            double lasthighRating = 0.0;
            double lastlowRating = 10.0;
            double lowestRating = 10.0;
            string lowestmovie = "";
            string highestmovie = "";
           /* foreach (KeyValuePair<string, MainWindow.Movie> statItem in MainWindow.movies)
            {
                if (statItem.Value.SeenIt)
                {
                    SeenMovies += 1;
                }
                if (statItem.Value.Wishlisted)
                    wishlistMovies += 1;
                if (statItem.Value.Rating > lasthighRating) //calc + define highest rating
                {
                    lasthighRating = statItem.Value.Rating;
                    highestRating = lasthighRating;
                    highestmovie = statItem.Value.Name;
                }
                if (statItem.Value.Rating < lastlowRating) //calc + define lowest rating
                {
                    lastlowRating = statItem.Value.Rating;
                    lowestRating = lastlowRating;
                    lowestmovie = statItem.Value.Name;
                }
                
            }*/

           // LblActorsInDb.Content = "Actors in database: " + MainWindow.AllActors.Count;
          //  LblMoviesInDb.Content = "Movies in database: " + MainWindow.movies.Count;
            LblSeenMovies.Content = "Movies seen: " + SeenMovies;
            LblWishlist.Content = "Movies in wishlist: " + MainWindow.wishlistedmovies;
            LblLowestRating.Content = "Lowest rating: " + lowestRating + " (" + lowestmovie + ")";
            LblHighestRating.Content = "Highest rating: " + highestRating + " (" + highestmovie + ")";
        }
    }
}
