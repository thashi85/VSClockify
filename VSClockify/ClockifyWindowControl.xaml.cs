//------------------------------------------------------------------------------
// <copyright file="ClockifyWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VSClockify
{
    using Services;
    using Services.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ClockifyWindowControl.
    /// </summary>
    public partial class ClockifyWindowControl : UserControl
    {
        private ClockyifyService _clockifyService;
        private string _apiKey;
        private User _user;
        private List<Project> _projects;
        private bool _timerStart=false;

        private string workspaceId
        {
            get
            {
                if (_user != null)
                    return string.IsNullOrEmpty(_user.activeWorkspace) ? _user.defaultWorkspace : _user.activeWorkspace;
                return "";
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ClockifyWindowControl"/> class.
        /// </summary>
        public ClockifyWindowControl()
        {
            try
            {
                //MethodLogger.SaveLogToFile("ClockifyWindowControl ClockifyApiKey:" + ServiceUtility.ClockifyApiKey);
                //MethodLogger.SaveLogToFile("ClockifyWindowControl ClockifyApiUrl:" + ServiceUtility.ClockifyApiUrl);
                this.InitializeComponent();
                this._clockifyService = new ClockyifyService();
                textBox_apiKey.Text = ServiceUtility.ClockifyApiKey;
                if (textBox_apiKey.Text.Length > 0)
                {
                    loadClockifyData();
                }else
                {
                    TimerPanel.Visibility = Visibility.Hidden;
                }
                
            }
            catch (Exception ex)
            {
                MethodLogger.SaveLogToFile("ClockifyWindowControl exception:" + ex.Message);
                MethodLogger.SaveLogToFile("ClockifyWindowControl exception:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Timer_Click(object sender, RoutedEventArgs e)
        {
           
            try {
               // buttonApply.Content = !_timerStart ? "Starting..." : "Stopping...";
                //progressbar.Visibility = Visibility.Visible;
                //progressbar.Value = 5;
                if (textBox_apiKey.Text.Length == 0)
                {
                    MessageBox.Show("Please fill the Clockify API Key");
                }
                else if (textBoxDesc.Text.Length == 0)
                {
                    MessageBox.Show("Please add description");
                }
                else if (comboBoxProject.SelectedValue.ToString().Length == 0)
                {
                    MessageBox.Show("Please select a project");
                }
                else if (!_timerStart)
                {
                   
                    //progressbar.Value = 30;
                   
                    var _res = _clockifyService.PostTimeEntry(workspaceId, new TimeEntryRequest()
                    {
                        start = DateTime.UtcNow,
                        description = textBoxDesc.Text,
                        projectId = comboBoxProject.SelectedValue.ToString()
                    });
                    //progressbar.Value = 90;
                    if (!string.IsNullOrEmpty(_res?.id))
                    {
                        _timerStart = !_timerStart;
                        buttonTimer.Content = "Stop Timer";
                    }
                }
                else
                {
                    var _res = _clockifyService.StopTimeEntry(workspaceId, _user.id, new StopTimeEntryRequest()
                    {
                        end = DateTime.UtcNow
                    });
                    if (!string.IsNullOrEmpty(_res?.id))
                    {
                        _timerStart = !_timerStart;
                        buttonTimer.Content = "Start Timer";
                    }
                }

                //progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                //buttonApply.Content = !_timerStart ? "Start Timer" : "Stop Timer";
                //progressbar.Visibility = Visibility.Hidden;
                MethodLogger.SaveLogToFile("Timer_Click exception:" + ex.Message);
                MethodLogger.SaveLogToFile("Timer_Click exception:" + ex.StackTrace);
            }
           
            //MessageBox.Show(
            // string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", textBox.Text),
            // "ClockifyWindow");
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //progressbar.Visibility = Visibility.Visible;
                //progressbar.Value = 5;
                //buttonApply.Content = "Processing...";
                if (textBox_apiKey.Text.Length == 0)
                {
                    MessageBox.Show("Please fill the Clockify API Key");
                }
                else
                {
                    //progressbar.Value = 15;
                    UCLoadingControl.Visibility = Visibility.Visible;
                    ServiceUtility.ClockifyApiKey = textBox_apiKey.Text;
                    loadClockifyData();
                    UCLoadingControl.Visibility = Visibility.Hidden;
                    if (string.IsNullOrEmpty(_user?.id))
                    {
                        MessageBox.Show("Invalild Clockify API Key");
                    }
                }
                //buttonApply.Content = "Apply";
                //progressbar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                //buttonApply.Content = "Apply";
                //progressbar.Visibility = Visibility.Hidden;
                MethodLogger.SaveLogToFile("buttonApply_Click exception:" + ex.Message);
                MethodLogger.SaveLogToFile("buttonApply_Click exception:" + ex.StackTrace);
            }
        }

        private void loadClockifyData()
        {
            _user = _clockifyService.GetUser();
            //progressbar.Value = 45;
            if (!string.IsNullOrEmpty(_user?.id))
            {
                labelTitle.Content = _user.name;
                TimerPanel.Visibility = Visibility.Visible;
                // SetupPanel.Visibility = Visibility.Collapsed;

                _projects = _clockifyService.GetProjects(workspaceId);
               // progressbar.Value = 80;
                if ((_projects?.Count ?? 0) >0)
                {
                    comboBoxProject.Items.Clear();
                    comboBoxProject.DisplayMemberPath = "name";
                    comboBoxProject.SelectedValuePath = "id";

                    var _proj = _projects.FindAll(a => !a.archived);
                    comboBoxProject.ItemsSource = _proj;

                    for (int i=0;i< _proj.Count;i++)
                    {                        
                        if (ServiceUtility.ClockifyDefaultProject== _proj[i].id)
                        {
                            comboBoxProject.SelectedItem = _proj[i];
                            comboBoxProject.Text = _proj[i].name;
                            //comboBoxProject.SelectedIndex = i;                            
                        }
                    }                   
                    if (comboBoxProject.SelectedIndex<=0 && comboBoxProject.HasItems)
                    {
                       // comboBoxProject.SelectedIndex = 0;
                        comboBoxProject.SelectedItem = _proj[0];
                        comboBoxProject.Text = _proj[0].name;
                    }
                    
                    
                }

            }else
            {
                TimerPanel.Visibility = Visibility.Hidden;
                
            }
        }

        private void comboBoxProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxProject.SelectedValue!=null)
            ServiceUtility.ClockifyDefaultProject = comboBoxProject.SelectedValue.ToString();
        }
    }
}