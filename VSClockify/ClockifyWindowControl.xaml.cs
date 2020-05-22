//------------------------------------------------------------------------------
// <copyright file="ClockifyWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace VSClockify
{
    using Services;
    using Services.Models;
    using Services.Models.Azure;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;
    using FontAwesome.WPF;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.ComponentModel;
    using System.Windows.Threading;
    using System.Threading;

    /// <summary>
    /// Interaction logic for ClockifyWindowControl.
    /// </summary>
    public partial class ClockifyWindowControl : UserControl
    {
       // private ClockyifyService _clockifyService;
       // private AzureAPIService _azureService;
        private string _apiKey;
        private User _user;
        private List<Project> _projects;
        private bool _timerStart = false;
        //private DataModel mdl = new DataModel();
        //public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("SpinnerVisibility", typeof(Visibility), typeof(ClockifyWindowControl));

        private string workspaceId
        {
            get
            {
                if (_user != null)
                    return string.IsNullOrEmpty(_user.activeWorkspace) ? _user.defaultWorkspace : _user.activeWorkspace;
                return "";
            }
        }

        public List<string> Statuses { get; set; }
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

                // DataContext explains WPF in which object WPF has to check the binding path. Here Vis is in "this" then:
                DataContext = this;

               // this._clockifyService = new ClockyifyService();
                //this._azureService = new AzureAPIService();
                textBox_apiKey.Text = ServiceUtility.ClockifyApiKey;
                textBox_azurePAT.Text = ServiceUtility.AzurePAT;
                textBox_azureUrl.Text = ServiceUtility.AzureSearchAPIEndPoint;
                textBox_azureApiUrl.Text = ServiceUtility.AzureAPIEndPoint;
                spinner.Icon = FontAwesomeIcon.Spinner;
                Statuses = new List<string>() { "Proposed", "Active", "Resolved", "Closed" };


                if (textBox_apiKey.Text.Length > 0)
                {
                    var res = loadClockifyData().Result;
                }
                else
                {
                    //  ExpanderTimePanel.Visibility = Visibility.Hidden;
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

            try
            {
                spinner.Visibility = Visibility.Visible;
                spinner.Refresh();
                if (textBox_apiKey.Text.Length == 0)
                {
                    MessageBox.Show("Please fill the Clockify API Key");
                }
                else if (comboBoxDesc.Text.Length == 0)
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

                    var _res = ClockifyUtility.StartClockify(workspaceId, new TimeEntryRequest()
                    {
                        start = DateTime.UtcNow,
                        description = comboBoxDesc.Text,
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
                    var _res = ClockifyUtility.StopClockify(workspaceId, _user.id, new StopTimeEntryRequest()
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

            spinner.Visibility = Visibility.Hidden;
            
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                spinner.Visibility = Visibility.Visible;
                spinner.Refresh();
                
                if (textBox_apiKey.Text.Length == 0)
                {
                    MessageBox.Show("Please fill the Clockify API Key");
                }
                else
                {
                    //progressbar.Value = 15;
                    UCLoadingControl.Visibility = Visibility.Visible;
                    ServiceUtility.ClockifyApiKey = textBox_apiKey.Text;
                    ServiceUtility.AzurePAT = textBox_azurePAT.Text;
                    ServiceUtility.AzureSearchAPIEndPoint = textBox_azureUrl.Text;
                    ServiceUtility.AzureAPIEndPoint = textBox_azureApiUrl.Text;

                    var res = loadClockifyData().Result;
                    UCLoadingControl.Visibility = Visibility.Hidden;
                    if (string.IsNullOrEmpty(_user?.id))
                    {
                        MessageBox.Show("Invalild Clockify API Key");
                    }
                }
                //buttonApply.Content = "Apply";
                //progressbar.Visibility = Visibility.Hidden;

                //SpinnerVisibility = false;
            }
            catch (Exception ex)
            {
                //buttonApply.Content = "Apply";
                //progressbar.Visibility = Visibility.Hidden;
                MethodLogger.SaveLogToFile("buttonApply_Click exception:" + ex.Message);
                MethodLogger.SaveLogToFile("buttonApply_Click exception:" + ex.StackTrace);
            }

            spinner.Visibility = Visibility.Hidden;
            spinner.Refresh();
        }

        private async Task<bool> loadClockifyData()
        {


            _user = ClockifyUtility.GetUser();
            //progressbar.Value = 45;
            if (!string.IsNullOrEmpty(_user?.id))
            {
                labelTitle.Content = _user.name;
                ExpanderTimePanel.IsExpanded = false;
                ExpanderTimeLog.IsExpanded = true;
                expander.IsExpanded = false;
                TimerPanel.Visibility = Visibility.Visible;

                _projects = ClockifyUtility.GetProjects(workspaceId);
                // progressbar.Value = 80;
                if ((_projects?.Count ?? 0) > 0)
                {
                    //comboBoxProject.Items.Clear();
                    comboBoxProject.DisplayMemberPath = "name";
                    comboBoxProject.SelectedValuePath = "id";

                    var _proj = _projects.FindAll(a => !a.archived);
                    comboBoxProject.ItemsSource = _proj;

                    for (int i = 0; i < _proj.Count; i++)
                    {
                        if (ServiceUtility.ClockifyDefaultProject == _proj[i].id)
                        {
                            comboBoxProject.SelectedItem = _proj[i];
                            comboBoxProject.Text = _proj[i].name;
                            //comboBoxProject.SelectedIndex = i;                            
                        }
                    }
                    if (comboBoxProject.SelectedIndex <= 0 && comboBoxProject.HasItems)
                    {
                        // comboBoxProject.SelectedIndex = 0;
                        comboBoxProject.SelectedItem = _proj[0];
                        comboBoxProject.Text = _proj[0].name;
                    }
                    setDate();
                    LoadWeeklyTimeLog();


                }

            }
            else
            {
                //ExpanderTimePanel.Visibility = Visibility.Hidden;
                // TimerPanel.Visibility = Visibility.Hidden;
                ExpanderTimePanel.IsExpanded = false;
                ExpanderTimeLog.IsExpanded = false;
                expander.IsExpanded = true;
            }
            return true;
        }

        private void comboBoxProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxProject.SelectedValue != null)
            {
                ServiceUtility.ClockifyDefaultProject = comboBoxProject.SelectedValue.ToString();
            }
        }

        private void comboBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            comboTxtChanged(sender, e.Text);
        }

        private void comboBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            comboTxtChanged(sender, "");
        }

        private void comboTxtChanged(object sender, string txt)
        {
            ComboBox cmb = (ComboBox)sender;
            cmb.IsDropDownOpen = true;
            if (_timerStart)
            {
                _timerStart = !_timerStart;
                buttonTimer.Content = "Start Timer";

            }
            var _txt = !string.IsNullOrEmpty(cmb.Text) ? cmb.Text : (!string.IsNullOrEmpty(txt) ? txt : "");
            if (!string.IsNullOrEmpty(_txt) && _txt.Length >= 3)
            {
                var res = ClockifyUtility.GetWorkItems(_txt, _user.name, _user.email);
                if (res != null)
                {
                    cmb.ItemsSource = res.OrderByDescending(a => Int32.Parse(a.Id)).ToList();
                    //cmb.DisplayMemberPath = "Desc";
                    //cmb.SelectedValuePath = "Title";
                }
            }

        }

        private void comboBoxDesc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;            
        }

        private void lblLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void setDate()
        {          
            dpStart.SelectedDate =ClockifyUtility.GetWeekStart();
            dpEnd.SelectedDate = ClockifyUtility.GetWeekEnd();
        }
        private void LoadWeeklyTimeLog()
        {
            lblStatus.Content = "";
            
            var thisWeekStart =dpStart.SelectedDate;
            var thisWeekEnd = dpEnd.SelectedDate;
            if (thisWeekStart.HasValue && thisWeekEnd.HasValue)
            {
                //ListTimeEntries.ItemsSource = dt;
                var res= ClockifyUtility.GetTimeEntries(workspaceId, _user.id, thisWeekStart.Value, thisWeekEnd.Value);
                dataGrid1.ItemsSource = res;
                lblTot.Content = "Total: " + Math.Round(res.Sum(r => r.durationD), 2);
            }
        }

        private void buttonSync_Click(object sender, RoutedEventArgs e)
        {
            spinner.Visibility = Visibility.Visible;
            spinner.Refresh();
            if (dataGrid1.ItemsSource != null)
            {
                var count = 0;
                foreach (var itm in dataGrid1.ItemsSource)
                {
                    var i = itm as TimeEntryResult;
                    if (i != null && i.selected && !string.IsNullOrEmpty(i.taskId) && i.durationD > 0)
                    {
                        var changes = new List<WorkItemChange>()
                        {
                            new WorkItemChange() {op=Constants.ActionType.ADD,path=Constants.Attributes.CompletedHours,value=i.durationD.ToString() },
                            new WorkItemChange() {op=Constants.ActionType.ADD,path=Constants.Attributes.RemainingHours,value=i.remaining.ToString() }
                        };
                        if (i.state != i.azureItem.State)
                        {
                            changes.Add(new WorkItemChange() { op = Constants.ActionType.ADD, path = Constants.Attributes.State, value = i.state.ToString() });
                        }
                        var st = ClockifyUtility.PatchWorkItems(i.taskId, changes);
                        if (st)
                        {
                            count++;
                        }
                    }
                }
                LoadWeeklyTimeLog();
                lblStatus.Content = count + " record/s has been updated";
            }
            spinner.Visibility = Visibility.Hidden;
           
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            spinner.Visibility = Visibility.Visible;
            spinner.Refresh();
            LoadWeeklyTimeLog();
            spinner.Visibility = Visibility.Hidden;
           
        }

        private void comboBoxProject_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            comboTxtChanged(sender, "");
        }
        private void NotifyTargetUpdated(object sender, DataTransferEventArgs e)
        {
        }
    }

    public class DataModel : INotifyPropertyChanged
    {
        private Visibility _spinnerVisibility = Visibility.Hidden;
        [Bindable(true)]
        public Visibility SpinnerVisibility
        {
            get
            {
                return this._spinnerVisibility;
            }

            set
            {
                this._spinnerVisibility = value;
                //  var d = new DependencyPropertyChangedEventArgs(ValueProperty, this._spinnerVisibility,value);
                //  OnProcessing(this,null);
                //  this.OnPropertyChanged(d);
                NotifyPropertyChanged("SpinnerVisibility");
            }
        }

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

    public static class ExtensionMethods

    {
        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)

        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}