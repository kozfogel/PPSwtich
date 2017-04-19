using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace PPSwitch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string selectedProcess = "";
        string selectedChosen = "";
        string saveFilepath = "HighPerformanceProcesses.txt";
        List<string> chosenOnes = new List<string>();
        NotifyIcon nIcon;

        Guid HPPScheme = Guid.Parse("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
        Guid BALScheme = Guid.Parse("381b4222-f694-41f0-9685-ff5bb260df2e");

        Boolean hpp;
        Boolean closing = false;

        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.ComponentModel.IContainer components;

        public Boolean HPP
        {
            get { return hpp; }
            set {
                if(hpp != value)
                {
                    hpp = value;
                    if(hpp)
                    {
                        PowerSchemeHelper.SetPowerScheme(HPPScheme);
                        this.nIcon.Icon = new Icon(@"ico/highperformance.ico");
                    }     
                    else
                    {
                        PowerSchemeHelper.SetPowerScheme(BALScheme);
                        this.nIcon.Icon = new Icon(@"ico/balance.ico");
                    }
                        
                }
                
            }
        }


        System.Timers.Timer pTimer;


        public MainWindow()
        {
            InitializeComponent();

            initIcon();
     

            if (File.Exists(saveFilepath))
                chosenOnes = File.ReadAllLines(saveFilepath).ToList();

            lblChosen.ItemsSource = chosenOnes;


            fillList();

            pTimer = new System.Timers.Timer(5000);
            pTimer.Elapsed += PTimer_Elapsed;
            pTimer.Start();

            Visibility = Visibility.Hidden;
        }

        private void PTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            { fillList();  });

        }

        void fillList()
        {
            List<Process> localAll = Process.GetProcesses().ToList();

            List<String> pNames = new List<string>();
            bool switchHpp = false;
            foreach (var p in localAll)
                if (!pNames.Contains(p.ProcessName))
                {
                    pNames.Add(p.ProcessName);
                    if (chosenOnes.Contains(p.ProcessName))
                        switchHpp = true;
                }

            HPP = switchHpp;
            lblChosen.ItemsSource = chosenOnes;
            pNames.Sort();

            lblRunning.ItemsSource = pNames;
            lblRunning.SelectedItem = selectedProcess;
        }

        void saveAndRefreshList()
        {
            lblChosen.ItemsSource = null;
            lblChosen.ItemsSource = chosenOnes;
            File.WriteAllLines(saveFilepath, chosenOnes);
        }

        private void lblRunning_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
                selectedProcess = e.AddedItems[0].ToString();
        }

        private void lblRunning_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!chosenOnes.Contains(selectedProcess))
            {
                chosenOnes.Add(selectedProcess);
                saveAndRefreshList();
            }
        }

        private void lblChosen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 1)
                selectedChosen = e.AddedItems[0].ToString();
        }

        private void lblChosen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            chosenOnes.Remove(selectedChosen);
            saveAndRefreshList();
            selectedChosen = "";
        }


        private void initIcon()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();

            this.nIcon = new NotifyIcon(components);
            this.nIcon.Icon = new Icon(@"ico/balance.ico");
            this.nIcon.Visible = true;
            this.contextMenu1.MenuItems.AddRange(
            new System.Windows.Forms.MenuItem[] { this.menuItem1 });

            // Initialize menuItem1
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += MenuItem1_Click;

            // Set up how the form should be displayed.

            nIcon.Text = "PPSwitch";
            nIcon.ContextMenu = this.contextMenu1;

            nIcon.DoubleClick += NIcon_DoubleClick;
        }

        private void NIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Activate();
            Visibility = Visibility.Visible;
            this.Activate();
        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            closing = true;
            this.Close();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!closing)
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
            }
            else
                nIcon.Visible = false;
        }
    }
}
