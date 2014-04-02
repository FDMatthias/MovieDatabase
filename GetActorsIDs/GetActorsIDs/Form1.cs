using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace GetActorsIDs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
        }


        public class Actor
        {
            public string name;
            public string id;

            public override string ToString()
            {
                return name + "#" + id;
            }
        }

        ObservableCollection<Actor> AllActors = new ObservableCollection<Actor>();
        StreamWriter fi;
        int toDownload;

        string indexSite = "nm";

        WebClient wc = new WebClient();
        private int index = 1;
        private void StartButton_Click(object sender, EventArgs e)
        {
            
            toDownload = int.Parse(textBox1.Text) + 1;
            for (int x = 0; x < (7 - index.ToString().ToCharArray().Count()); x++)
                indexSite += "0";
            indexSite += index.ToString();
                wc.DownloadStringAsync(new Uri("http://m.imdb.com/name/" + indexSite));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            label1.Text = "Progress: " + index + "/" + toDownload;
            if(index != toDownload)
            {
                
                Actor tempactor = new Actor();
                if (e.Error == null)
                {
                    var scrape = new HtmlDocument();
                    scrape.LoadHtml(e.Result);
                    HtmlNode docnode = scrape.DocumentNode;
                    HtmlNode[] divnodes = docnode.Descendants("div").ToArray();
                    foreach (var t in divnodes)
                    {
                        if (t.Attributes["class"] != null)
                        {
                            if (t.Attributes["class"].Value == "col-xs-12") //informatie kader localiseren
                            {
                                HtmlNode hNode = t.Descendants("h1").FirstOrDefault();
                                if (hNode != null)
                                {
                                    string[] getname = (hNode.InnerText.Replace("\n", "#")).Split('#');
                                    tempactor.name = getname[0].ToString() + getname[1].ToString();
                                    tempactor.id = indexSite;
                                    AllActors.Add(tempactor);
                                    ListOfActors.Items.Add(tempactor.ToString());
                                }
                            }
                        }
                    }
                }
                wc.DownloadStringCompleted -= wc_DownloadStringCompleted;
                wc.DownloadStringCompleted += wc_DownloadStringCompleted2;
                index++;

                indexSite = "nm";
                for (int x = 0; x < (7 - index.ToString().ToCharArray().Count()); x++)
                    indexSite += "0";
                indexSite += index.ToString();
                wc.DownloadStringAsync(new Uri("http://m.imdb.com/name/" + indexSite));
            }
            else
            {
                MakeFile();
            }
        }

        private void wc_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            label1.Text = "Progress: " + index + "/" + toDownload;
            if (index != toDownload)
            {
                Actor tempactor = new Actor();
                if (e.Error == null)
                {
                    var scrape = new HtmlDocument();
                    scrape.LoadHtml(e.Result);
                    HtmlNode docnode = scrape.DocumentNode;
                    HtmlNode[] divnodes = docnode.Descendants("div").ToArray();
                    foreach (var t in divnodes)
                    {
                        if (t.Attributes["class"] != null)
                        {
                            if (t.Attributes["class"].Value == "col-xs-12") //informatie kader localiseren
                            {
                                HtmlNode hNode = t.Descendants("h1").FirstOrDefault();
                                if (hNode != null)
                                {
                                    string[] getname = (hNode.InnerText.Replace("\n", "#")).Split('#');
                                    tempactor.name = getname[0].ToString() + getname[1].ToString();
                                    tempactor.id = indexSite;
                                    AllActors.Add(tempactor);
                                    ListOfActors.Items.Add(tempactor.ToString());
                                }
                            }
                        }
                    }
                }
                wc.DownloadStringCompleted -= wc_DownloadStringCompleted2;
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                index++;

                indexSite = "nm";
                for (int x = 0; x < (7 - index.ToString().ToCharArray().Count()); x++)
                    indexSite += "0";
                indexSite += index.ToString();
                wc.DownloadStringAsync(new Uri("http://m.imdb.com/name/" + indexSite));
            }
            else
            {
                MakeFile();
            }
        }

        private void MakeFile()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            var chosenFolder = dlg.SelectedPath;
            fi = File.CreateText(chosenFolder + "\\actorsDB.txt"); //txt file aanmaken
            
            //inhoud txt-document
            for (int x = 0; x < AllActors.Count; x++)
            {
                fi.WriteLine(AllActors[x].ToString());
            }

            //einde inhoud html-document
            fi.Close();

            MessageBox.Show("Done");
        }

    }
}
