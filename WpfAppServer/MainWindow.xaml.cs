﻿using DatabaseLib;
using DataService;
using HMIControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ZedGraph;

namespace WpfAppServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double x1 = SystemParameters.PrimaryScreenWidth;
        double y1 = SystemParameters.PrimaryScreenHeight;
        ObservableCollection<TagItem> listArchive = new ObservableCollection<TagItem>();

        public MainWindow()
        {
            InitializeComponent();
            this.Width = x1;
            this.Height = y1;
        }

        List<TagNodeHandle> _valueChangedList;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Login.IsOpen)
            {
                Login frm1 = new Login();
                frm1.ShowDialog();
            }

            if (App.Principal != null)
            {
                txtuser.Text = string.Format("Current user: {0} permission {1}", App.Principal.Identity.Name, App.Principal.ToString());           
            }

            #region  Display default interface
            if (Tag != null && !string.IsNullOrEmpty(Tag.ToString()))
            {
                var Wintypes = Tag.ToString().TrimEnd(';');
                ContentControl ctrl = Activator.CreateInstance(Type.GetType(Wintypes)) as ContentControl;
                if (ctrl != null)
                {
                    ScaleControl(ctrl);
                    ctrl.Loaded += new RoutedEventHandler(ctrl_Loaded);
                    ctrl.Unloaded += new RoutedEventHandler(ctrl_Unloaded);
                    dict[Wintypes] = ctrl;
                    cvs1.Child = ctrl;
                    this.Title = Wintypes;
                }
            }
            #endregion

            #region  Display status bar time, display PLC connection status, communication with PLC watchdog
            DispatcherTimer ShowTimer = new DispatcherTimer();
            ShowTimer.Interval = new TimeSpan(0, 0, 1);
            ShowTimer.Tick += (s, e1) =>
            {
                txttime.InvokeAsynchronously(delegate { txttime.Text = DateTime.Now.ToString(); });
                p1_lamp1.Fill = App.Server.Drivers.Any(x => x.IsClosed) ? Brushes.Red : Brushes.Green;
            };
            ShowTimer.Start();
            #endregion

            #region  Bind to Server
            lock (this)
            {
                _valueChangedList = this.BindingToServer(App.Server);
            }
            BindingTagWindow(this);
            CommandBindings.AddRange(BindingCommandHandler());
             var condlist = App.Server.ActivedConditionList as ObservableCollection<ICondition>;
            if (condlist != null)
            {
                condlist.CollectionChanged += new NotifyCollectionChangedEventHandler(condlist_CollectionChanged);
            }
            var tag = App.Server["__CoreEvent"];
            if (tag != null)
            {
                tag.ValueChanged += (s, e1) =>
                {
                    if (tag != null)
                    {
                        App.Events.ReverseEnqueue(string.Format("{0}     {1}     {2}", tag.GetTagName(), DateTime.Now, tag.ToString()));
                        if (tag.ToString().Contains("Error:"))
                            MessageBox.Show(tag.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };
            }

            #endregion

            #region Archive
            var metalist = App.Server.MetaDataList;
            string ColumsCreate = "";
            for (int i = 0; i < metalist.Count; i++)
            {
                if (metalist[i].Archive == true)
                {
                    listArchive.Add(new TagItem(metalist[i]));
                    ColumsCreate += metalist[i].Name + " REAL NULL, ";
                }
            }

            string CheckTable = "SELECT TABLE_NAME FROM SCADA.INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            using (var reader = DataHelper.Instance.ExecuteReader(CheckTable))
            {
                bool CheckOK = false;
                while (reader.Read())
                {
                    if (reader[0].ToString() == "SaveDataHistory")
                    {
                        if (MessageBox.Show("You want to delete old data?", "Notification", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            string DeleteTable = "DROP TABLE SaveDataHistory";
                            DataHelper.Instance.ExecuteReader(DeleteTable);
                        }
                        else
                        {
                            CheckOK = true;
                        }
                        Console.WriteLine(reader.FieldCount.ToString());
                    }
                }

                if (CheckOK == false)
                {
                    string TABLE = "CREATE TABLE " + "SaveDataHistory" + "([myId] BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL, " + "[Ngay] DATETIME NOT NULL, " + ColumsCreate + "[NgayDouble] FLOAT NULL)";
                    DataHelper.Instance.ExecuteReader(TABLE);
                }
            }

            DispatcherTimer TimerArchive = new DispatcherTimer();
            TimerArchive.Interval = new TimeSpan(0, 0, 60);
            TimerArchive.Tick += (s, e1) =>
            {
                string SaveTime = DateTime.Now.ToString("mm");
                string ColumsInsert = "";
                string ValueInsert = "";
                double DoubleNgay = ((double)new XDate(DateTime.Now));

                for (int i = 0; i < listArchive.Count; i++)
                {
                    ColumsInsert += listArchive[i].TagName + ", ";
                    ValueInsert += listArchive[i].TagValue + ", ";
                }

                if (SaveTime == "15" || SaveTime == "30" || SaveTime == "45" || SaveTime == "59")
                {
                    string INSERT = "INSERT INTO " + "SaveDataHistory" + "(Ngay, " + ColumsInsert + "NgayDouble) " + "VALUES ('" + DateTime.Now + "', " + ValueInsert + DoubleNgay + ")";
                    DataHelper.Instance.ExecuteReader(INSERT);
                }
            };
            if (listArchive.Count > 0) TimerArchive.Start();
            #endregion
        }

        void condlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var condlist = sender as ObservableCollection<ICondition>;
            AlarmConverter convert = new AlarmConverter();
            txtAlarm.Inlines.Clear();
            for (int i = 0; i < condlist.Count; i++)
            {
                txtAlarm.Inlines.Add(new Run(string.Concat(i.ToString(), ":", condlist[i].Message, " ")) { Foreground = convert.Convert(condlist[i].Severity, null, null, null) as Brush });
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ICondition cond in e.NewItems)
                {
                    if (cond.Severity == Severity.Error)
                    {
                        var tag = App.Server["Sys_Alarm"];
                        tag.Write(true);
                        MessageBox.Show(cond.Message, "Warning!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        Dictionary<string, ContentControl> dict = new Dictionary<string, ContentControl>();

        void BindingTagWindow(DependencyObject container)
        {
            if (container == null) return;
            foreach (var item in container.FindChildren<ITagWindow>())
            {
                if (!string.IsNullOrEmpty(item.TagWindowText))
                {
                    UIElement element = item as UIElement;
                    element.AddHandler(UIElement.MouseLeftButtonUpEvent,
                                       new MouseButtonEventHandler(item_MouseLeftButtonUp));
                }
            }
        }

        private void ShowContent(ITagWindow tagwindow)
        {
            if (tagwindow == null || string.IsNullOrEmpty(tagwindow.TagWindowText)) return;
            var windows = tagwindow.TagWindowText.TrimEnd(';').Split(';');
            foreach (string txt in windows)
            {
                if (dict.ContainsKey(txt))
                {
                    if (dict[txt].Tag.ToString() != "YES")
                    {
                        cvs1.Child = dict[txt];
                    }
                    continue;
                }
                if (tagwindow.IsUnique)
                {
                    foreach (var win in App.Current.Windows)
                    {
                        if (win.ToString() == txt)
                            goto lab1;
                    }
                }
                try
                {
                    ContentControl ctrl = Activator.CreateInstance(Type.GetType(txt)) as ContentControl;
                    if (ctrl != null)
                    {
                        var win = ctrl as Window;
                        if (win == null)
                            ScaleControl(ctrl);
                        ctrl.Loaded += new RoutedEventHandler(ctrl_Loaded);
                        ctrl.Unloaded += new RoutedEventHandler(ctrl_Unloaded);
                        if (win != null)
                        {
                            win.Owner = this;
                            win.ShowInTaskbar = false;
                            if (tagwindow.IsModel)
                                win.ShowDialog();
                            else
                                win.Show();
                        }
                        else
                        {
                            dict[txt] = ctrl;
                            cvs1.Child = ctrl;
                            this.Title = txt;
                        }
                    }
                }
                catch (Exception e)
                {
                    App.AddErrorLog(e);
                }
                lab1:
                continue;
            }
        }

        void ScaleControl(ContentControl ctrl)
        {
            var transform = ctrl.RenderTransform as MatrixTransform;
            if (transform != null && !double.IsNaN(ctrl.Width) && !double.IsNaN(ctrl.Height))
            {
                var matrix = transform.Matrix;
                matrix.Scale(x1 / ctrl.Width, y1 / ctrl.Height);
                ctrl.RenderTransform = new MatrixTransform(matrix);
                ctrl.Width = x1;
                ctrl.Height = y1;
                this.Background = ctrl.Background;
            }
        }

        void ctrl_Unloaded(object sender, RoutedEventArgs e)
        {
            ContentControl uie = sender as ContentControl;
            if (uie != null)
            {
                uie.Tag = "NO";
                var windows = uie.FindChildren<ITagWindow>();
                foreach (ITagWindow item in windows)
                {
                    if (!string.IsNullOrEmpty(item.TagWindowText))
                    {
                        ((UIElement)item).RemoveHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(item_MouseLeftButtonUp));
                    }
                }
            }
        }

        void ctrl_Loaded(object sender, RoutedEventArgs e)
        {
            ContentControl uie = sender as ContentControl;
            if (uie != null)
            {
                uie.Tag = "YES";
                BindingTagWindow(uie);
            }
        }

        void item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowContent(sender as ITagWindow);
            e.Handled = true;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you want to Exit?", "Information",  MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.Cancel) return;
            //SystemLog.AddLog(new SystemLog(EventType.Simple, DateTime.Now, App.LogSource, "Drop out"));
            App.Current.Shutdown();
            e.Handled = true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (cvs3.Visibility == Visibility.Hidden)
                cvs3.Visibility = Visibility.Visible;
            else if (cvs3.Visibility == Visibility.Visible)
                cvs3.Visibility = Visibility.Hidden;

        }


        CommandBindingCollection BindingCommandHandler()
        {
            var srv = App.Server;
            CommandBindingCollection CommandBindings = new CommandBindingCollection();
         
            return CommandBindings;
        }

    }
}

