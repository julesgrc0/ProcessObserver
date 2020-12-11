using System;
using System.Windows;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace ObserverLogsReader
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
       
        private void ReadLine(string line)
        {
            string log = @"((Processstarted\:)\s+([a-zA-Z0-9]+\.exe)\s+(at)\s+([0-9]+\/[0-9]+\/[0-9]+\s+[0-9]+\:[0-9]+\:[0-9]+)|(\[(START|STOP)\]\s+at\s+([0-9]+\/[0-9]+\/[0-9]+\s+[0-9]+\:[0-9]+\:[0-9]+)))";
            string processR = @"(Processstarted\:)\s+([a-zA-Z0-9]+\.exe)\s+(at)\s+([0-9]+\/[0-9]+\/[0-9]+\s+[0-9]+\:[0-9]+\:[0-9]+)";
            string comR = @"(\[(START|STOP)\]\s+at\s+([0-9]+\/[0-9]+\/[0-9]+\s+[0-9]+\:[0-9]+\:[0-9]+))";
            if (Regex.Match(line, log) != null)
            {
                string process = Regex.Replace(line, processR, "$1");
                string com = Regex.Replace(line, comR, "$1");
                if(process == "$1")
                {
                    ComParser(com);
                }
                else
                {
                    ProcessParser(process);
                }
            }
        }

        public Dictionary<int, ProcessList> processList = new Dictionary<int, ProcessList>()
        {
        };
        private void ProcessParser(string pro)
        {
            if (Regex.IsMatch(pro, @"(Process started\:)\s*([a-zA-Z0-9]+\.exe)\s*(at)\s*([0-9\:\/\s*]+)"))
            {
                string processName = Regex.Replace(pro, @"(Process started\:)\s*([a-zA-Z0-9]+\.exe)\s*(at)\s*([0-9\:\/\s*]+)", "$2");
                if (processList.Count.Equals(0))
                {
                    ProcessList list = new ProcessList { processName = processName, number = 1 };
                    processList.Add(processList.Count + 1, list);
                }
                else
                {
                    bool isFind = false;
                    int key = 0;

                    foreach (KeyValuePair<int, ProcessList> proc in processList)
                    {
                        if (proc.Value.processName.Trim().ToLower() == processName.Trim().ToLower())
                        {
                            isFind = true;
                            key = proc.Key;
                        }
                    }
                    if (!isFind)
                    {
                        ProcessList list = new ProcessList { processName = processName, number = 1 };
                        processList.Add(processList.Count + 1, list);
                    }
                    else
                    {
                        processList[key].number += 1;
                    }
                }
            }
        }
        private void Lister()
        {
            listItems.SetValue(
ScrollViewer.HorizontalScrollBarVisibilityProperty,
ScrollBarVisibility.Disabled);
            foreach (KeyValuePair<int, ProcessList> proc in processList)
            {
                ListBoxItem box = new ListBoxItem();
                box.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
                StackPanel panel = new StackPanel();
                TextBlock t1 = new TextBlock();
                t1.Text = proc.Value.processName;
                TextBlock t2 = new TextBlock();
                t2.Text = " Ouvert "+proc.Value.number;
                t2.Margin = new Thickness(0,0,60,0);
                panel.Children.Add(t1);
                panel.Children.Add(t2);
                panel.Orientation = Orientation.Horizontal;
                box.Content = panel;
                listItems.Items.Add(box);
                /*
                 <ListBoxItem Background="#FF3E3E3E">
                <StackPanel Orientation="Horizontal">
                    <TextBlock>Chrome.exe</TextBlock>
                    <TextBlock Margin="40,0,0,0">Ouvert 2543</TextBlock>
                </StackPanel>
            </ListBoxItem>
                 */
            }
        }

        private void ComParser(string com)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog()==true)
            {
                if(File.Exists(openFileDialog.FileName) && Path.GetExtension(openFileDialog.FileName) == ".log")
                {
                   string[] lines= File.ReadAllLines(openFileDialog.FileName);
                    if (!noReset)
                    {
                        listItems.Items.Clear();
                        processList.Clear();
                    }
                    foreach (string line in lines)
                    {
                        ReadLine(line);
                    }
                    Lister();
                }
            }
        }

        private void choseplace_Click(object sender, RoutedEventArgs e)
        {
            if(filename.Text != String.Empty)
            {
                string finaF = Regex.Replace(filename.Text, @"[^0-9a-zA-Z_-]+", "");
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = finaF + ".log";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Create(saveFileDialog.FileName);
                }
                filename.Text = "";
            }
        }
        private bool noReset = true;

        private void noreset_Checked(object sender, RoutedEventArgs e)
        {
            noReset = !noReset;
        }
    }

    public class ProcessList
    {
        public string processName { get; set; }
        public int number { get; set; }

    }
}
