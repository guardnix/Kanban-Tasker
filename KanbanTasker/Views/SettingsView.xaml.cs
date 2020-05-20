﻿using System.Collections.Generic;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using KanbanTasker.Services.SQLite;
using KanbanTasker.Helpers;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading.Tasks;
using KanbanTasker.Helpers.Authentication;
using KanbanTasker.ViewModels;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KanbanTasker.Views
{
    public sealed partial class SettingsView : ContentDialog
    {
        private string appClientId;
        private string[] scopes = new string[] { "files.readwrite"};
        private string appId = "422b281b-be2b-4d8a-9410-7605c92e4ff1";
        private AuthenticationProvider authProvider;
        public IPublicClientApplication MsalClient { get; }
        public const string DataFilename = "ktdatabase.db";
        public const string BackupFolderName = "Kanban Tasker";
        public User CurrentUser { get; set; }
        public BoardViewModel CurrentViewModel { get; set; }
        public SettingsView()
        {
            this.InitializeComponent();
            txtResults.Text = "Not Logged In";

            // Initialize the Authentication  Provider
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void BtnCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void SettingsDialog_ViewUpdatesClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
            var dialog = new AppUpdatedDialogView();
            var result = await dialog.ShowAsync();
        }

        private async void btnBackupDb_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationResult authResult = null;

            progressRing.IsActive = true;

            // Initialize Authentication Provider
            authProvider = new AuthenticationProvider(appId, scopes);

            // Request a token to sign in the user
            var accessToken = await authProvider.GetAccessToken();
          
            // Initialize Graph Client
            GraphServiceHelper.Initialize(authProvider);
            var graphClient = new GraphServiceClient(authProvider);
            var user = await GraphServiceHelper.GetMeAsync();
            var backupFolder = await GraphServiceHelper.GetOneDriveFolderAsync("Kanban Tasker");

            // Create backup folder in OneDrive if not exists
            if (backupFolder == null)
                backupFolder = await GraphServiceHelper.CreateNewOneDriveFolder("Kanban Tasker") as DriveItem;
           
            // Backup datafile (or overwrite)
            var uploadedFile = await GraphServiceHelper.UploadFileToOneDrive(backupFolder.Id, DataFilename);

            // Debug Results
            progressRing.IsActive = false;
            var displayName = await GraphServiceHelper.GetMyDisplayName();
            txtResults.Text = "Welcome, " + displayName;
            await DisplayMessageAsync("Data backed up successfully.");
            #region OLD
            // Signed-in user
            //var user = Task.Run(() => GraphHelper.GetMeAsync()).Result;
            //var user = await GraphHelper.GetMeAsync();

            //var test = user.AboutMe;

            // Windows Community Toolkit Version
            //var provider = ProviderManager.Instance.GlobalProvider;

            //if (provider != null && provider.State == ProviderState.SignedIn)
            //{
            //    // Do graph call here with provider.Graph...
            //    //var graphClient = provider.Graph;
            //    //var children = await graphClient.Me.Drive.Root.Children
            //    //                .Request()
            //    //                .GetAsync();

            //    //foreach (var child in children)
            //    //{
            //    //    var test = child.Name;
            //    //}

            //    // Search for folder inside of OneDrive
            //    //var search = await graphClient.Me.Drive.Root
            //    //    .Search("Documents")
            //    //    .Request()
            //    //    .GetAsync();
            //    //            var driveItem = await graphClient.Me.Drive.Root
            //    //.Request()
            //    //.GetAsync();
            //    //var stream = "The contents of the file goes here.";

            //    //await graphClient.Me.Drive.Items["test.txt"].Content
            //    //    .Request()
            //    //    .PutAsync(stream);
            //    //var test = children.Count;

            //    // Create new folder in OneDrive Root Folder
            //    //            var driveItem = new Microsoft.Graph.DriveItem
            //    //            {
            //    //                Name = "KanbanTasker",
            //    //                Folder = new Microsoft.Graph.Folder
            //    //                {
            //    //                },
            //    //                AdditionalData = new Dictionary<string, object>()
            //    //{
            //    //    {"@microsoft.graph.conflictBehavior","rename"}
            //    //}
            //    //            };

            //    //            await graphClient.Me.Drive.Root.Children
            //    //                .Request()
            //    //                .AddAsync(driveItem);
            //}
            //else
            //{
            //    //Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification message = new Microsoft.Toolkit.Uwp.UI.Controls.InAppNotification();
            //    //message.Show("Login failed. Please try again.", 3000);
            //}
            #endregion OLD
        }

        private async void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            await authProvider.SignOut();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                txtResults.Text = "User has signed-out";
                //this.CallGraphButton.Visibility = Visibility.Visible;
                //this.SignOutButton.Visibility = Visibility.Collapsed;
            });
        }


        /// <summary>
        /// Displays a message in the InAppNotification. Can be called from any thread.
        /// </summary>
        private async Task DisplayMessageAsync(string message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       var frame = (Frame)Window.Current.Content;
                       (frame.Content as MainView).KanbanInAppNotification.Show(message, 3000);
                       //SettingsAppNotification.Show(message, 3000);
                   });
        }

    }
}
