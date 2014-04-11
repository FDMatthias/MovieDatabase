using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HtmlAgilityPack;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Label = System.Windows.Controls.Label;
using ListBox = System.Windows.Controls.ListBox;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using System.ComponentModel;

namespace The_Final_Re_redemption
{
    public partial class MainWindow : Window
    {
        public class DelegateCommand : ICommand
        {
            private Action _executeMethod;
            public DelegateCommand(Action executeMethod)
            {
                _executeMethod = executeMethod;
            }
            public bool CanExecute(object parameter)
            {
                return true;
            }
            public event EventHandler CanExecuteChanged;
            public void Execute(object parameter)
            {
                _executeMethod.Invoke();
            }
        }
        public class ViewModel
        {
            private ObservableCollection<Actor> a_Actors; //make an observablecollection of all actors
            public ObservableCollection<Actor> AllActors
            {
                get
                {
                    return a_Actors;
                }
                set
                {
                    a_Actors = value;
                }
            }
            #region Code for filtering the list
            public ICommand FilterByWish
            {
                get { return new DelegateCommand(this.FilterTheWishes); }
            }
            public ICommand FullList
            {
                get { return new DelegateCommand(this.GiveFullList); }
            } 
            private ListCollectionView GetListCollectionView()
            {
                return (ListCollectionView)CollectionViewSource.GetDefaultView(this.AllMovies);
            }
            public void FilterTheWishes() { this.GetListCollectionView().Filter = this.IsWished; }
            public void GiveFullList() { this.GetListCollectionView().Filter = this.IsMovie; }
            private bool IsWished(object obj)
            {
                if(obj as Movie != null && (obj as Movie).Wishlisted == true)
                {
                    return true;
                }
                return false;
            }
            private bool IsMovie(object obj)
            {
                if (obj as Movie != null)
                {
                    return true;
                }
                return false;
            }
            #endregion
            private ObservableCollection<Movie> a_Movies; //make an observablecollection of all movies
            public ObservableCollection<Movie> AllMovies { get { return a_Movies; } set { a_Movies = value; } }
           
            public ViewModel() //constructor
            {
                a_Actors = new ObservableCollection<Actor>();
                a_Movies = new ObservableCollection<Movie>();
            }
            private Actor m_SelectedPerson; //save selected actor
            public Actor SelectedPerson
            {
                get
                {
                    return m_SelectedPerson;
                }
                set
                {
                    m_SelectedPerson = value;
                }
            }
            private Movie m_SelectedMovie; //save selected movie
            public Movie SelectedMovie
            {
                get
                {
                    return m_SelectedMovie;
                }
                set
                {
                    m_SelectedMovie = value;
                }
            }
        }

        Queue<PlayedIn> TheQ = new Queue<PlayedIn>();
        StreamWriter fi;

        string indexSite;
        string thename;
        string chosenFile;
        string DBline;
        public static int wishlistedmovies = 0;

        OpenFileDialog dlg = new OpenFileDialog();

        WebClient wc = new WebClient();
        WebClient wc2 = new WebClient();

        Movie tempMovie = new Movie();
        DispatcherTimer ts = new DispatcherTimer();

        ViewModel viewModel = new ViewModel();
        public MainWindow()
        {
            InitializeComponent();
            wc.DownloadStringCompleted += downloadString_Completed;
            wc2.DownloadStringCompleted += Wc2OnDownloadStringCompleted;
            ts.Tick += ts_Tick; //downloadevent
            ts.Interval = new TimeSpan(0, 0, 6);
            StatusBarText.Text = "Program succesfully started. Please load a database before proceeding.";
        }

        private void MainWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = viewModel;
        }

        /* SWITCH BETWEEN QUEUE - MOVIES - ACTORS */
        private void MovieInDBLbl_MouseDown(object sender, MouseButtonEventArgs e) { SwitchLists(MovieInDBLbl, QueueLbl, ActorsInDB, WishListLbl);}
        private void QueueLbl_MouseDown(object sender, MouseButtonEventArgs e) { SwitchLists(QueueLbl, MovieInDBLbl, ActorsInDB, WishListLbl); }
        private void ActorsInDB_MouseDown(object sender, MouseButtonEventArgs e) { SwitchLists(ActorsInDB, MovieInDBLbl, QueueLbl, WishListLbl); }
        private void WishListLbl_MouseDown(object sender, MouseButtonEventArgs e) { SwitchLists(WishListLbl, ActorsInDB, MovieInDBLbl, QueueLbl); }
        public void SwitchLists(Label sender, Label other, Label othernr2, Label othernr3)
        {
            StatusBarText.Text = "Selected the menu " + sender.Content + ".";
            sender.Background = Brushes.White;
            sender.Height = 22;
            other.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFFD259"));
            other.Height = 18;
            othernr2.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFFD259"));
            othernr2.Height = 18;
            othernr3.Background = (Brush)(new BrushConverter().ConvertFrom("#FFC3C3C3"));
            othernr3.Height = 18;

            if (sender.Equals(MovieInDBLbl))
            {
                QueueLbl.Margin = new Thickness(10, 12, 0, 0); //change labels
                MovieInDBLbl.Margin = new Thickness(51, 9, 0, 0);
                ActorsInDB.Margin = new Thickness(93, 12, 0, 0);
                WishListLbl.Margin = new Thickness(169, 12, 0, 0);
                MList.Visibility = Visibility.Visible; //listbox Mlist visible, all other listboxes hidden
                AllAList.Visibility = Visibility.Hidden;
                QList.Visibility = Visibility.Hidden;
                viewModel.GiveFullList();
                if(viewModel.SelectedMovie != null)
                {
                    DeleteButton.Visibility = Visibility.Visible;
                    DeleteButton.Opacity = 0.6;
                }
            }
            else if (sender.Equals(QueueLbl))
            {
                QueueLbl.Margin = new Thickness(10, 9, 0, 0);
                MovieInDBLbl.Margin = new Thickness(51, 12, 0, 0);
                ActorsInDB.Margin = new Thickness(93, 12, 0, 0);
                WishListLbl.Margin = new Thickness(169, 12, 0, 0);
                QList.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Hidden;
                AllAList.Visibility = Visibility.Hidden;
                MList.Visibility = Visibility.Hidden;
            }
            else if (sender.Equals(WishListLbl))
            {
                QueueLbl.Margin = new Thickness(10, 12, 0, 0);
                MovieInDBLbl.Margin = new Thickness(51, 12, 0, 0);
                ActorsInDB.Margin = new Thickness(93, 12, 0, 0);
                WishListLbl.Margin = new Thickness(169, 9, 0, 0);
                DeleteButton.Visibility = Visibility.Hidden;
                QList.Visibility = Visibility.Hidden;
                MList.Visibility = Visibility.Visible;
                AllAList.Visibility = Visibility.Hidden;
                QueueLbl.Background = (Brush)(new BrushConverter().ConvertFrom("#FFFFD259"));
                viewModel.FilterTheWishes();
            }
            else
            {
                QueueLbl.Margin = new Thickness(10, 12, 0, 0);
                MovieInDBLbl.Margin = new Thickness(51, 12, 0, 0);
                ActorsInDB.Margin = new Thickness(93, 9, 0, 0);
                WishListLbl.Margin = new Thickness(169, 12, 0, 0);
                AllAList.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Hidden;
                QList.Visibility = Visibility.Hidden;
                MList.Visibility = Visibility.Hidden;
            }
        }
        /*END SWITCH B/W Q - M - A*/

        private void ClearDB(object sender, RoutedEventArgs e)
        {
            StatusBarText.Text = "Database cleared.";
            ActorLbl.Content = "";
            if(viewModel.AllActors != null)
                viewModel.AllActors.Clear();
            if (viewModel.AllMovies != null)
                viewModel.AllMovies.Clear();
            QList.Items.Clear();
            TheQ.Clear();
        }

       
        /* CODE VOOR HINT-TEXT */
        private void ActorBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox)
            {
                //If nothing has been entered yet.
                if (((System.Windows.Controls.TextBox)sender).Foreground.Equals(Brushes.Gray))
                {
                    ((System.Windows.Controls.TextBox)sender).Text = "";
                    ((System.Windows.Controls.TextBox)sender).Foreground = Brushes.Black;
                }
            }
            StatusBarText.Text =
                "Search for an actor by entering a valid name in this box and pressing the button right to it.";
        }
        private void ActorBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Make sure sender is the correct Control.
            if (sender is System.Windows.Controls.TextBox)
            {
                //If nothing was entered, reset default text.
                if (((System.Windows.Controls.TextBox)sender).Text.Trim().Equals(""))
                {
                    ((System.Windows.Controls.TextBox)sender).Foreground = Brushes.Gray;
                    ((System.Windows.Controls.TextBox)sender).Text = "Enter actor's name..";
                }
            }
        }

        private void searchBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox)
            {
                //If nothing has been entered yet.
                if (((System.Windows.Controls.TextBox)sender).Foreground.Equals(Brushes.Gray))
                {
                    ((System.Windows.Controls.TextBox)sender).Text = "";
                    ((System.Windows.Controls.TextBox)sender).Foreground = Brushes.Black;
                }
            }
            StatusBarText.Text =
                "Search for a movie by entering a valid id in this box and pressing the button right to it.";
        }
        private void searchBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Make sure sender is the correct Control.
            if (sender is System.Windows.Controls.TextBox)
            {
                //If nothing was entered, reset default text.
                if (((System.Windows.Controls.TextBox)sender).Text.Trim().Equals(""))
                {
                    ((System.Windows.Controls.TextBox)sender).Foreground = Brushes.Gray;
                    ((System.Windows.Controls.TextBox)sender).Text = "Enter Id..";
                }
            }
        }
        /* END HINT TEXT CODE*/

        /*METHODS TO DO WITH Q*/
        //selecting an item that the user would want to add to the queue.
        private void ActorListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusBarText.Text =
                "Movie has been selected, to add this movie to the queue please press the button 'Add to queue', or to add all the movies to the queue please press 'Add all to queue'.";
        }

        //after selecting an item from main listbox and clicking the add2Q-button
        private void Add2QButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActorListBox.SelectedItem != null)
            {
                var selectedmovie = ActorListBox.SelectedItem as PlayedIn;

                if (selectedmovie != null && viewModel.AllMovies.Any(m => m.Id.Equals(selectedmovie.MovieId)) || TheQ.Contains(selectedmovie))
                {
                    MessageBox.Show("This movie is already in the database or in the queue.");
                    StatusBarText.Text = "The selected movie is already in the database or in the queue.";
                }
                else
                {
                    TheQ.Enqueue(selectedmovie);
                    Update_Q();
                    StatusBarText.Text = "Selected movie is added to the queue.";
                }
            }
            else
            {
                StatusBarText.Text = "No movie selected, nothing was added to the queue.";
            }
        }

        //adding all items from main listbox to the q
        private void AddAll2QButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllAList.SelectedItem != null)
            {
                string SomeMovies = "The following movie(s) were already in the database: ";
                for (int q = 0; q < ActorListBox.Items.Count; q++)
                {
                    ActorListBox.SelectedIndex = q;
                    var selectedmovie = ActorListBox.SelectedItem as PlayedIn;

                    if (selectedmovie != null && viewModel.AllMovies.Any(m => m.Id.Equals(selectedmovie.MovieId)) ||
                        TheQ.Contains(selectedmovie))
                        SomeMovies += "\n" + selectedmovie.MovieName;
                    else
                    {
                        TheQ.Enqueue(selectedmovie);
                        Update_Q();
                        StatusBarText.Text = "Movies are added to the queue.";
                    }
                }
                MessageBox.Show("The queue has been updated.");
                if (SomeMovies != "The following movie(s) were already in the database or queue: ")
                {
                    MessageBox.Show(SomeMovies);

                    StatusBarText.Text += " Some movies were already in the database or queue.";
                }
            }
            else
            {
                StatusBarText.Text = "No movies were added to the queue. Please select an actor first.";
            }
        }

        //method to update the items in the Qlist
        public void Update_Q()
        {
            SwitchLists(QueueLbl, MovieInDBLbl, ActorsInDB, WishListLbl);
            QList.Items.Clear();
            foreach (PlayedIn Qmovie in TheQ)
            {
                QList.Items.Add(new PlayedIn(Qmovie));
            }
        }

        //start downloading items from Q by starting the timer
        private void StartDLQButton_Click(object sender, RoutedEventArgs e)
        {
            if (TheQ.Count != 0)
            {
                StatusBarText.Text = "Downloading queue... Items left in queue: " + QList.Items.Count;
                ts.IsEnabled = true;
                ts.Start();
            }
            else
            {
                MessageBox.Show("No items in queue. Please select items from the left window to add to the queue.");
                StatusBarText.Text = "No items were in the queue. Download not started.";
            }
        }
        
        //code to run at every tick
        public void ts_Tick(object sender, EventArgs eventArgs)
        {
            if (TheQ.Count != 0)
            {
                tempMovie.Id = TheQ.Dequeue().MovieId.ToString();
                    //idmbId opslaan + hieronder standaard waardes meegeven, als deze dan niet gevonden worden op de pagina wordt dit zo goed mogelijk al opgevangen.
                tempMovie.SeenIt = false;
                tempMovie.Wishlisted = false;
                tempMovie.MyRating = 0.0;
                tempMovie.Name = "N/A";
                tempMovie.Rating = 0.0;
                tempMovie.Poster = "Poster_not_available.jpg";
                tempMovie.Year = "????";
                tempMovie.Synopsis = "N/A";
                tempMovie.Genres = "N/A";
                wc2.DownloadStringAsync(new Uri("http://www.imdb.com/title/" + tempMovie.Id.ToString()));
            }
            else
            {
                StatusBarText.Text = "Completed downloading the queue.";
                ts.IsEnabled = false;
                ts.Stop();
            }
        }

        //upon completing a download
        private void Wc2OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var scrape = new HtmlAgilityPack.HtmlDocument();
                scrape.LoadHtml(e.Result);


                HtmlNode docnode = scrape.DocumentNode;
                HtmlNode[] divnodes = docnode.Descendants("div").ToArray();
                foreach (HtmlNode t in divnodes)
                {
                    if (t.Attributes["id"] != null)
                    {
                        if (t.Attributes["id"].Value == "title-overview-widget") //informatie kader localiseren
                        {
                            HtmlNode h1Node = t.Descendants("h1").FirstOrDefault();
                            if (h1Node != null && h1Node.Attributes["class"].Value == "header") //header voor de naam en het jaar localiseren
                            {
                                HtmlNode[] spanNodes = h1Node.Descendants("span").ToArray();
                                foreach (HtmlNode t1 in spanNodes)
                                {
                                    if (t1.Attributes["class"] != null)
                                    {
                                        if (t1.Attributes["class"].Value == "itemprop")
                                        {
                                            tempMovie.Name = t1.InnerText; //naam opslaan
                                        }
                                        if (t1.Attributes["class"].Value == "nobr")
                                        {
                                            char[] hetjaar = t1.InnerText.ToCharArray(); //het jaar opslaan
                                            tempMovie.Year = hetjaar[1].ToString() + hetjaar[2].ToString() + hetjaar[3].ToString() + hetjaar[4].ToString();
                                        }
                                    }
                                }
                            }
                        }
                        HtmlNode[] pNodes = t.Descendants("p").ToArray(); 
                        foreach (HtmlNode t2 in pNodes)
                        {
                            if (t2.Attributes["itemprop"] != null)
                            {
                                if (t2.Attributes["itemprop"].Value == "description") //description opslaan
                                    tempMovie.Synopsis = t2.InnerText.Replace("\n", " ");
                            }
                        }
                        if (t.Attributes["itemprop"] != null && t.Attributes["itemprop"].Value == "description")
                        {
                            tempMovie.Synopsis = t.Attributes["content"].Value;
                        }
                    }
                    if (t.Attributes["class"] != null)
                    {
                        if (t.Attributes["class"].Value == "titlePageSprite star-box-giga-star")
                            tempMovie.Rating = double.Parse(t.InnerText) / 10; //rating opslaan
                        if (t.Attributes["class"].Value == "image")
                        {
                            tempMovie.Poster = "";
                            HtmlNode imagenode = t.Descendants("img").FirstOrDefault();
                            if (tempMovie.Name.Contains(':') || tempMovie.Name.Contains('?') || tempMovie.Name.Contains('/') || tempMovie.Name.Contains('\\') || tempMovie.Name.Contains('"') || tempMovie.Name.Contains('*'))
                            {
                                string[] newpath = new string[2];
                                if (tempMovie.Name.Contains(':'))
                                    newpath = tempMovie.Name.Split(':');
                                if (tempMovie.Name.Contains('?'))
                                    newpath = tempMovie.Name.Split('?');
                                if (tempMovie.Name.Contains('/'))
                                    newpath = tempMovie.Name.Split('/');
                                if (tempMovie.Name.Contains('\\'))
                                    newpath = tempMovie.Name.Split('\\');
                                if (tempMovie.Name.Contains('*'))
                                    newpath = tempMovie.Name.Split('*');
                                for (int m = 0; m < newpath.Length; m++)
                                {
                                    tempMovie.Poster += newpath[m];
                                }
                                tempMovie.Poster += ".jpg";
                            }
                            else
                            {
                                tempMovie.Poster = tempMovie.Name + ".jpg"; //string die als Uri zal gebruikt worden voor de poster vd bijbehorende film
                            }
                            using (WebClient client = new WebClient())
                            { //poster downloaden en opslaan
                                if (imagenode != null)
                                    client.DownloadFile(imagenode.Attributes["src"].Value, Path.GetDirectoryName(chosenFile) + "\\Posters\\" + tempMovie.Poster);
                            }
                        }
                        if (t.Attributes["class"].Value == "infobar")
                        {
                            HtmlNode[] genrenodes = t.Descendants("span").ToArray();
                            tempMovie.Genres = ""; //genres even clearen, aangezien er zijn gevonden moet er niet "N/A" staan.
                            foreach (HtmlNode t1 in genrenodes)
                            {
                                if (t1.Attributes["itemprop"] != null)
                                {
                                    if (t1.Attributes["itemprop"].Value == "genre") //genres opslaan
                                        tempMovie.Genres += t1.InnerText + " ";
                                }
                            }
                        }
                    }
                }

                viewModel.AllMovies.Add(new Movie { Id = tempMovie.Id, Name = tempMovie.Name, Year = tempMovie.Year, Rating = tempMovie.Rating, MyRating = tempMovie.MyRating, SeenIt = tempMovie.SeenIt, Wishlisted =  tempMovie.Wishlisted, Poster = tempMovie.Poster, Genres = tempMovie.Genres, Synopsis = tempMovie.Synopsis});
                //NIET MEER NODIGMList.Items.Add(new Movie { Id = tempMovie.Id, Name = tempMovie.Name, Year = tempMovie.Year, Rating = tempMovie.Rating, MyRating = tempMovie.MyRating, SeenIt = tempMovie.SeenIt, Wishlisted = tempMovie.Wishlisted, Poster = tempMovie.Poster, Genres = tempMovie.Genres, Synopsis = tempMovie.Synopsis });
                QList.Items.RemoveAt(0); //item uit de queue-lijst verwijderen
                SwitchLists(MovieInDBLbl, ActorsInDB, QueueLbl, WishListLbl);
                StatusBarText.Text = "Added the movie " + tempMovie.Name + ". Items left in queue: " + QList.Items.Count;
            }
        }

        /*SEARCH MOVIE IN DB*/
        private void SearchDBButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text != "Enter Id..")
            {
                //var result = from o in MList.Items.OfType<Movie>() where o.Id.Contains(searchBox.Text.ToString()) select o;
                var filteredItems = (ObservableCollection<Movie>)viewModel.AllMovies.Where( p => p.Id != null && p.Id.ToUpper().Contains(searchBox.Text.ToUpper()));
                if (filteredItems != null)
                {
                    Console.WriteLine(filteredItems);
                    SwitchLists(MovieInDBLbl, ActorsInDB, QueueLbl, WishListLbl);
                    StatusBarText.Text = "Movie found in database and selected. You can find the info on the panel to the right.";
                }
                else
                    StatusBarText.Text = "Movie not found in database.";
            }
            else
            {
                StatusBarText.Text = "Invalid id entered, please try again. F.e.: 'tt123456789'.";
            }
        }

        private void GetActorInfo_Click(object sender, RoutedEventArgs e)
        {
           if (ActorTextBox.Text != "Enter actor's name..")
            {
                bool isaanweziginDB = false;
                thename = ActorTextBox.Text;
                StreamReader file = new StreamReader("..\\..\\..\\Database (kies deze om te testen)\\actorsDB.txt");
                while ((DBline = file.ReadLine()) != null)
                {
                    if (DBline.Contains(thename) && isaanweziginDB == false) //isaanweziginDB == false wilt zeggen dat het nog niet gevonden is. Hierdoor wordt er maar gezocht tot de eerste gelijkenis en dan is isaanwezigindeDB = true.
                    {
                        isaanweziginDB = true;
                        string[] splitted2 = DBline.Split('#');
                        DLActor(splitted2[1]);
                    }
                }
               
                file.Close();
                if (!isaanweziginDB)
                {
                    MessageBox.Show("Actor is not found in the database.");
                    StatusBarText.Text = "Actor has not been found in the database. It's because your database is not up to date or you entered the name wrong.";
                }

            }
            else
            {
                MessageBox.Show("Incorrect name entered.");
                StatusBarText.Text = "Incorrect name was entered. Please try again.";
            }
        }

        /*METHODS TO DO WITH ACTOR*/
        //methode om de lijst van films van de acteur te downloade
        public void DLActor(string actorId)
        {
            indexSite = actorId;
     //       wc.DownloadProgressChanged += client_DownloadProgressChanged;
            StatusBarText.Text = "Downloading info from actor..";
            wc.DownloadStringAsync(new Uri("http://m.imdb.com/name/" + indexSite + "/filmotype/actor"));
        }

        //upon completing DLActor
        private void downloadString_Completed(object sender, DownloadStringCompletedEventArgs e)
        {
            Actor chosenActor = new Actor();
            chosenActor.Name = thename;
            if (e.Error == null)
            { //hier worden de films opgezocht en de naam + id van geript, en in een ActorOf lijst gezet.
                var scrape = new HtmlAgilityPack.HtmlDocument();
                scrape.LoadHtml(e.Result);
                HtmlNode docnode = scrape.DocumentNode;
                HtmlNode[] divnodes = docnode.Descendants("div").ToArray();
                foreach (var t in divnodes)
                {
                    if (t.Attributes["class"] != null)
                    {
                        if (t.Attributes["class"].Value == "col-xs-12 col-md-6") //informatie kader localiseren
                        {
                            string filmid = null;
                            HtmlNode aNode = t.Descendants("a").FirstOrDefault();
                            if (aNode != null)
                            {
                                string[] splitted = aNode.Attributes["href"].Value.Split('/');
                                filmid = splitted[2];
                            }
                            HtmlNode spanNode = t.Descendants("span").FirstOrDefault();
                            if (spanNode != null)
                                chosenActor.ActorOf.Add(new PlayedIn(spanNode.InnerText, filmid));
                        }
                    }
                }
                viewModel.AllActors.Add(chosenActor); //acteur aan db van alle acteurs toevoegen
                AllAList.SelectedIndex = AllAList.Items.Count - 1;
                StatusBarText.Text = "Actor has been added to the list.";
                SwitchLists(ActorsInDB, MovieInDBLbl, QueueLbl, WishListLbl);
                
            }
        }

        //selected actor in AllAList changed
        private void AllAList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusBarText.Text = "Actor has been selected, the movies he/she played is in the list on your left.";
        }

        /*LOAD DB FUNCTION*/
        public void loadButton_Click(object sender, RoutedEventArgs e)
        {
            QList.Items.Clear();
            if(viewModel.AllActors != null)
                viewModel.AllActors.Clear();
            if (viewModel.AllMovies != null)
                viewModel.AllMovies.Clear();
            dlg.ShowDialog();
            chosenFile = dlg.FileName; //pad opslaan in string chosenFile. Hierin wordt de database opgeslaan.
            try
            {
                if (chosenFile != null)
                {
                    // Read the file and add it in list 'movies'  line by line.
                    StreamReader file = new StreamReader(chosenFile);
                    while ((DBline = file.ReadLine()) != null)
                    {
                        if (DBline.Contains("Actor:"))
                        {
                            Actor actortoadd = new Actor();
                            string[] splitted2 = DBline.Split('#');
                            actortoadd.Name = splitted2[1];
                            for (int y = 2; y < splitted2.Length; y += 2)
                            {
                                actortoadd.ActorOf.Add(new PlayedIn(splitted2[y + 1], splitted2[y]));
                            }
                            viewModel.AllActors.Add(actortoadd);
                        }
                        else
                        {
                            string[] splitted = DBline.Split('#');
                            Movie movietoadd = new Movie
                            {
                                Id = splitted[0],
                                Name = splitted[1],
                                Year = splitted[2],
                                Rating = double.Parse(splitted[3]),
                                MyRating = double.Parse(splitted[4]),
                                SeenIt = bool.Parse(splitted[5]),
                                Wishlisted = bool.Parse(splitted[6]),
                                Poster = splitted[7],
                                Genres = splitted[8],
                                Synopsis = splitted[9]
                            };
                            viewModel.AllMovies.Add(movietoadd);
                        }

                    }
                    file.Close();
                    //end reader
                  
                    StatusBarText.Text = "Database has been loaded. Number of movies in database: " + MList.Items.Count +
                                         " Actors in database: " + AllAList.Items.Count + ".";
                }
            }
            catch (Exception xException)
            {
                MessageBox.Show(xException.Message);
            }
        }

        /*SAVE DB FUNCTION*/
        public void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenFile == null)
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowDialog();
                var chosenFolder = dlg.SelectedPath;
                fi = File.CreateText(chosenFolder + "\\imdb_database_1.txt"); //txt file aanmaken
            }
            else
            {
                fi = File.CreateText(chosenFile); //txt file in variable laden
                //inhoud van txt file schrijven
                foreach (var movie in viewModel.AllMovies)
                {
                    fi.Write(movie.ToString());
                    fi.WriteLine();
                }
                foreach (var actor in viewModel.AllActors)
                {
                    fi.WriteLine("Actor:#" + actor.ToString());
                }
                //einde inhoud schrijven
                fi.Close();

                StatusBarText.Text = "Database has been saved.";
                System.Windows.MessageBox.Show("Saved database.");
            }
        }

        /*UPDATE INFO FROM SELECTED MOVIE*/
        private void _SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel.SelectedMovie.Poster != null)
            {
                PosterMovie.Source = new BitmapImage(new Uri(Path.GetDirectoryName(chosenFile) + "\\Posters\\" + viewModel.SelectedMovie.Poster));
                StatusBarText.Text = "Movie has been selected, you can find the info in the panel on the right.";
            }
            DeleteButton.Visibility = Visibility.Visible;
            DeleteButton.Opacity = 0.6;
        }
        /*DELETE BUTTON*/
        private void DeleteButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteButton.Opacity = 100;
        }
        //Change opacity to 60%
        private void DeleteButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteButton.Opacity = 0.6;
        }
        //Delete the selected item
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedMovie != null)
            {
                viewModel.AllMovies.Remove(viewModel.SelectedMovie);
            }
        }

        //UPDATE THE SEENIT VALUE
        private void SeenItCheck_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedMovie != null)
            {
                StatusBarText.Text =
                    "Changing 'Seen it'-status.";
            }
            else
            {
                StatusBarText.Text =
                    "Can't change value for 'Seen it' if you haven't selected a movie from the database";
            }
        }
        //ADD TO WISHLIST
        private void Add2Wishlist_Click(object sender, RoutedEventArgs e) 
        {
            if (viewModel.SelectedMovie != null) //er is effectief een movie geselecteerd
            {
                if (Wishlisted.IsChecked == true)
                {
                    SwitchLists(WishListLbl, ActorsInDB, MovieInDBLbl, QueueLbl);
                    StatusBarText.Text = "Movie has been added to wishlist.";
                }
                if (Wishlisted.IsChecked == false)
                {
                    SwitchLists(MovieInDBLbl, ActorsInDB, QueueLbl, WishListLbl);
                    StatusBarText.Text = "Movie has been removed from wishlist.";
                }
            }
            else
            {
                Wishlisted.IsChecked = false;
                StatusBarText.Text =
                    "No movie selected. Select a movie first form the movie list, and then add them to wishlist.";
            }
        }

        /*MYRATING*/
        //update the myrating value
        private void MyNewRatingBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (viewModel.SelectedMovie != null)
                {
                    StatusBarText.Text = "You have changed your rating to: " + viewModel.SelectedMovie.MyRating;
                    MyNewRatingBox.Visibility = Visibility.Hidden;
                    MList.Focus(); //focus on another object so the propertychanged event is raised.
                }
            }
        }
        //change visibility and ability to change the user's rating.
        private void MyRatingLbl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (viewModel.SelectedMovie != null)
            {
            //    MyNewRatingBox.Text = viewModel.SelectedMovie.MyRating.ToString();
                MyNewRatingBox.Visibility = Visibility.Visible;
                MyNewRatingBox.Focus();
                MyNewRatingBox.SelectAll();
                StatusBarText.Text = "To change the rating enter a new value in the box and press 'enter'.";
            }
        }
        //hide mynewratingbox
        private void MyRatingLbl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MyNewRatingBox.Visibility == Visibility.Visible)
            {
                StatusBarText.Text = "You have changed your rating to: " + viewModel.SelectedMovie.MyRating;
                MyNewRatingBox.Visibility = Visibility.Hidden;
                MList.Focus(); //focus on another object so the propertychanged event is raised.
            } 
        }

        /*STATISTICS*/
        //opening the statistics window
        private void ShowStats(object sender, RoutedEventArgs e)
        {
            StatusBarText.Text = "Showing statistics.";
            Statistics Stats = new Statistics(); //new window to show the statistics in.
            Stats.Closed += StatsOnClosed;
            Stats.Show();
        }

        //When the menu statistics is closed.
        private void StatsOnClosed(object sender, EventArgs eventArgs)
        {
            StatusBarText.Text = "Closed statistics, back on main window.";
        }

        //exit the program
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit the program?", "Exit program?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
            else if (result == MessageBoxResult.No)
            {
                StatusBarText.Text = "You chose to stay in the matrix, should've chosen the red pill.";
            }
        }
    }
}
