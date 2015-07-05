using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using TwitterSearcher.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwitterSearcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private ObservableCollection<MyTweet> _tweets = new ObservableCollection<MyTweet>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TweetList.ItemsSource = _tweets;
            LoadTweets();
        }

  

        private async void LoadTweets()
        {
            Uri twitterApiQuery = new Uri("http://search.twitter.com/search.atom?q=win10+OR+windows10");
            var client = new HttpClient();
            var response = await client.GetAsync(twitterApiQuery);

            string content = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(content);

            var doc = XDocument.Parse(content);
            XNamespace ns = "http://www.w3.org/2005/Atom";

            var items = from item in doc.Descendants(ns + "entry")
                        select new MyTweet()
                        {
                            Message = item.Element(ns + "title").Value,
                            Image =
                                (from XElement xe in item.Descendants(ns + "link")
                                 where xe.Attribute("type").Value == "image/png"
                                 select xe.Attribute("href").Value).First()
                        };

            _tweets.Clear();

            foreach(MyTweet t in items)
            {
                _tweets.Add(t);
            }

         }


    }
}
