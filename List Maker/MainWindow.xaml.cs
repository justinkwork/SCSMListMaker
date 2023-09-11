using Microsoft.EnterpriseManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;


namespace List_Maker
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.Description = "Select a folder";
            folderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputPath.Text = folderBrowser.SelectedPath;
            }
            
        }

        private void btnAddList_Click(object sender, RoutedEventArgs e)
        {
            List<string> internalNames = (lstInternal.ItemsSource == null) ? new List<string>() : (List<string>)lstInternal.ItemsSource;
            internalNames.Add(txtListName.Text);
            lstInternal.ItemsSource = internalNames;

            List<string> displayNames = (lstDisplay.ItemsSource == null) ? new List<string>() : (List<string>)lstDisplay.ItemsSource;
            displayNames.Add(txtListDisplayName.Text);
            lstDisplay.ItemsSource = displayNames;

            ICollectionView internalView = CollectionViewSource.GetDefaultView(lstInternal.ItemsSource);
            internalView.Refresh();

            ICollectionView displayView = CollectionViewSource.GetDefaultView(lstDisplay.ItemsSource);
            displayView.Refresh();

        }

        private void btnCreateXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnterpriseManagementGroup emg = EnterpriseManagementGroup.Connect(txtSCSMServer.Text);
                JKW.MPTools.SMManagementPack mp = new JKW.MPTools.SMManagementPack(txtMpName.Text, emg);

                foreach (var enumList in lstInternal.Items)
                {
                    var enuma = new JKW.MPTools.MPEnum();
                    enuma.ID = enumList.ToString();
                    mp.AddEnum(enuma, lstDisplay.Items[lstInternal.Items.IndexOf(enumList)].ToString(), null);

                }

                mp.Save(txtOutputPath.Text + "\\" + mp.Name + ".xml");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            
        }

        private void btnCreateSealed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.DefaultExt = ".snk";
                dialog.Filter = "Signing Key (.snk)|*.snk";

                if ((bool)dialog.ShowDialog()) 
                {

                    EnterpriseManagementGroup emg = EnterpriseManagementGroup.Connect(txtSCSMServer.Text);
                    JKW.MPTools.SMManagementPack mp = new JKW.MPTools.SMManagementPack(txtMpName.Text, emg);

                    foreach (var enumList in lstInternal.Items)
                    {
                        var enuma = new JKW.MPTools.MPEnum();
                        enuma.ID = enumList.ToString();
                        mp.AddEnum(enuma, lstDisplay.Items[lstInternal.Items.IndexOf(enumList)].ToString(), null);

                    }

                    mp.Seal(dialog.FileName, "Company", txtOutputPath.Text);

                }
                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
