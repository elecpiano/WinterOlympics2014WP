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
using Microsoft.Phone.Net.NetworkInformation;
using System.IO;
using WinterOlympics2014WP.Utility;
using WinterOlympics2014WP.Models;
using System.Windows.Input;
using WinterOlympics2014WP.Controls;

namespace WinterOlympics2014WP.Pages
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        #region Property

        private string categoryID = string.Empty;

        #endregion

        #region Lifecycle

        public CategoryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            categoryID = NavigationContext.QueryString[NaviParam.CATEGORY_ID];
            SetTitle(categoryID);
            LoadSchedules();
        }

        #endregion

        #region Sub Title

        private void SetTitle(string cateId)
        {
            this.topBar.SecondaryHeader = "短道速滑";
        }

        #endregion

        #region VirtualizingStackPanel

        StackPanel scheduleItemsPanel = null;
        private void scheduleListBoxItemsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            scheduleItemsPanel = sender as StackPanel;
        }

        #endregion

        #region Schedule List

        ListDataLoader<GameSchedule> scheduleloader = new ListDataLoader<GameSchedule>();
        List<GameSchedule> scheduleList = new List<GameSchedule>();

        private void LoadSchedules()
        {
            if (scheduleloader.Loaded || scheduleloader.Busy)
            {
                return;
            }

            snow1.IsBusy = true;

            scheduleloader.Load("getschedule", "&id=" + categoryID,
                list =>
                {
                    this.scheduleListBox.ItemsSource = scheduleList = list;
                    snow1.IsBusy = false;
                }, true, Constants.SCHEDULE_MODULE, string.Format(Constants.SCHEDULE_FILE_NAME_FORMAT, categoryID));
        }

        #endregion

        #region Result

        DataLoader<GameResult> resultLoader = new DataLoader<GameResult>();
        GameResultPanel gameResultPanel = null;
        GameSchedule expandedSchedule = null;

        private void LoadGameResult(GameSchedule schedule)
        {
            if (resultLoader.Busy)
            {
                return;
            }

            snow1.IsBusy = true;

            resultLoader.Load("getresult", "&id=" + schedule.ID,
                list =>
                {
                    ShowResultPanel(schedule, list);
                    snow1.IsBusy = false;
                }, true, Constants.SCHEDULE_MODULE, string.Format(Constants.RESULT_FILE_NAME_FORMAT, schedule.ID));
        }

        private void Result_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var schedule = sender.GetDataContext<GameSchedule>();
            if (schedule == expandedSchedule)
            {
                HideResultPanel();
            }
            else
            {
                if (expandedSchedule!=null)
                {
                    HideResultPanel();
                }
                LoadGameResult(schedule);
            }
        }

        private void ShowResultPanel(GameSchedule schedule, GameResult result)
        {
            if (gameResultPanel == null)
            {
                gameResultPanel = new GameResultPanel();
            }
            if (scheduleItemsPanel.Children.Contains(gameResultPanel))
            {
                scheduleItemsPanel.Children.Remove(gameResultPanel);
            }

            scheduleItemsPanel.DataContext = result;

            int index = scheduleList.IndexOf(schedule);
            scheduleItemsPanel.Children.Insert(index + 1, gameResultPanel);
            gameResultPanel.Show();
            expandedSchedule = schedule;
        }

        private void HideResultPanel()
        {
            gameResultPanel.Hide(scheduleItemsPanel);
            expandedSchedule = null;
        }

        #endregion

        #region Subscribe

        private void Subscribe_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GameSchedule schedule = sender.GetDataContext<GameSchedule>();
            schedule.StartTime = DateTime.Now.AddSeconds(30);
            ReminderHelper.AddReminder(schedule.ID, schedule.Category, schedule.Match, schedule.StartTime, "/Pages/LivePage.xaml");
        }

        #endregion

    }
}