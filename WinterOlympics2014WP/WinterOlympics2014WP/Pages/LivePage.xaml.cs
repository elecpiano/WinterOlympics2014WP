﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using WinterOlympics2014WP.Models;
using WinterOlympics2014WP.Utility;
using System.Windows.Media.Imaging;
using WinterOlympics2014WP.Controls;

namespace WinterOlympics2014WP.Pages
{
    public partial class LivePage : PhoneApplicationPage
    {
        #region Property

        private string liveID = string.Empty;
        private string liveTitle = string.Empty;
        private string liveImage = string.Empty;

        #endregion

        #region Lifecycle

        public LivePage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            liveID = NavigationContext.QueryString[NaviParam.LIVE_ID];
            liveImage = NavigationContext.QueryString[NaviParam.LIVE_IMAGE];
            liveTitle = NavigationContext.QueryString[NaviParam.LIVE_TITLE];

            this.titleImage.Source = new BitmapImage(new Uri(liveImage, UriKind.RelativeOrAbsolute));
            this.titleTextBlock1.Text = this.titleTextBlock2.Text = liveTitle;

            LoadData();
        }

        #endregion

        #region Data

        GenericDataLoader<LiveData> liveLoader = new GenericDataLoader<LiveData>();

        private void LoadData()
        {
            //TO-DO : remove test line
            liveID = "1000001804";

            if (liveLoader.Loaded || liveLoader.Busy)
            {
                return;
            }

            snow1.IsBusy = true;

            liveLoader.Load("getlivepage", "&id=" + liveID, true, Constants.LIVE_MODULE, string.Format(Constants.LIVE_FILE_NAME_FORMAT, liveID),
                data =>
                {
                    //TO-DO : check where to do this update
                    PopulateLineItems(data);

                    snow1.IsBusy = false;
                });
        }

        private void PopulateLineItems(LiveData data)
        {
            FrameworkElement control = null;
            lineItemsStackPanel.Children.Clear();

            foreach (var item in data.LineItems)
            {
                switch (item.Type)
                {
                    case 0:
                        control = new LivePageItemVideo();
                        break;
                    case 1:
                        control = new LivePageItemLiveText();//TO-DO : check design
                        break;
                    case 2:
                        control = new LivePageItemAlbum() { HostingPage = this };
                        break;
                    case 12:
                        control = new LivePageItemLiveText();
                        break;
                    default:
                        control = null;
                        break;
                }

                if (control != null)
                {
                    control.DataContext = item;
                    lineItemsStackPanel.Children.Add(control);
                }
            }
        }


        #endregion

        #region App Bar

        ApplicationBarIconButton appBarRefresh;

        private void BuildApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            //ApplicationBar.Opacity = 0.9;
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // refresh
            appBarRefresh = new ApplicationBarIconButton(new Uri("/Assets/AppBar/refresh.png", UriKind.Relative));
            appBarRefresh.Text = "刷新";
            appBarRefresh.Click += appBarRefresh_Click;
            ApplicationBar.Buttons.Add(appBarRefresh);
        }

        void appBarRefresh_Click(object sender, System.EventArgs e)
        {
            liveLoader.Loaded = false;
            LoadData();
        }

        #endregion

    }
}