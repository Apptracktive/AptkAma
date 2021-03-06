﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aptk.Plugins.AptkAma;
using Aptk.Plugins.AptkAma.Data;
using Aptk.Plugins.AptkAma.Identity;
using AptkAma.Sample.Core.Model;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace AptkAma.Sample.Core
{
    public partial class MainPage : ContentPage
    {
        private readonly IAptkAmaService _aptkAmaService;
        private readonly IMedia _mediaService;

        public MainPage()
        {
            InitializeComponent();

            _aptkAmaService = AptkAmaPluginLoader.Instance;
            _mediaService = CrossMedia.Current;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _aptkAmaService.Data.LocalTable<TodoItem>().PullAsync(new CancellationToken());
            var items = await _aptkAmaService.Data.LocalTable<TodoItem>().ToListAsync();

            // Culture hack
            if(items.Any())
            {
                await ExecWithSpecificCultureAsync(async () =>
                {
                    foreach (var item in items)
                    {
                        await _aptkAmaService.Data.LocalTable<TodoItem>().PullFilesAsync(item); 
                    }
                }, new CultureInfo("en-US"));
            }
            
            ToDoItems.ItemsSource = await GetTodoItemsAsync();
        }

        async Task AddItem(TodoItem item)
        {
            await _aptkAmaService.Data.LocalTable<TodoItem>().InsertAsync(item);

            //File
            var image = await _mediaService.PickPhotoAsync();
            if (image != null)
            {
                try
                {
                    await _aptkAmaService.Data.FileManagementService().CopyFileToStoreAsync(item.Id, image.Path);

                    await ExecWithSpecificCultureAsync(async () => await _aptkAmaService.Data.LocalTable<TodoItem>().AddFileAsync(item, Path.GetFileName(image.Path)), new CultureInfo("en-US"));
                }
                catch (Exception ex)
                {

                }
            }

            ToDoItems.ItemsSource = await GetTodoItemsAsync();
        }

        async Task CompleteItem(TodoItem item)
        {
            item.Complete = true;
            await _aptkAmaService.Data.LocalTable<TodoItem>().UpdateAsync(item);
            ToDoItems.ItemsSource = await GetTodoItemsAsync();
        }

        public async void OnAdd(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewItemName.Text))
            {
                var todo = new TodoItem { Text = NewItemName.Text };
                await AddItem(todo);
                NewItemName.Text = "";
                NewItemName.Unfocus();
            }
        }

        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as TodoItem;
            await CompleteItem(todo);
        }

        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as TodoItem;
            if (Device.OS != TargetPlatform.iOS && todo != null)
            {
                // Not iOS - the swipe-to-complete is discoverable there
                if (Device.OS == TargetPlatform.Android)
                {
                    await DisplayAlert(todo.Text, "Press-and-hold to complete task " + todo.Text, "Got it!");
                }
                else {
                    // Windows, not all platforms support the Context Actions yet
                    if (await DisplayAlert("Complete?", "Do you wish to complete " + todo.Text + "?", "Complete", "Cancel"))
                    {
                        await CompleteItem(todo);
                    }
                }
            }
            // prevents background getting highlighted
            ToDoItems.SelectedItem = null;
        }

        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            var success = false;
            try
            {
                ToDoItems.ItemsSource = await GetTodoItemsAsync();
                success = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
                // requires C# 6
                //await DisplayAlert ("Refresh Error", "Couldn't refresh data ("+ex.Message+")", "OK");
            }
            list.EndRefresh();
            if (!success)
                await DisplayAlert("Refresh Error", "Couldn't refresh data", "OK");
        }

        public async void OnSync(object sender, EventArgs e)
        {
            await _aptkAmaService.Data.PushAsync();
            await ExecWithSpecificCultureAsync(async () => await _aptkAmaService.Data.LocalTable<TodoItem>().PushFileChangesAsync(), new CultureInfo("en-US"));
            ToDoItems.ItemsSource = await GetTodoItemsAsync();
        }

        async Task<List<TodoItem>> GetTodoItemsAsync()
        {
            var items = await _aptkAmaService.Data.LocalTable<TodoItem>().Where(i => !i.Complete).ToListAsync();
            foreach (var item in items)
            {
                var files = await _aptkAmaService.Data.LocalTable<TodoItem>().GetFilesAsync(item);
                if (files != null && files.Any())
                {
                    item.ImagePath = _aptkAmaService.Data.FileManagementService().GetFullPath(Path.Combine(item.Id, files.First().Name));
                }
            }

            return items;
        }

        public async void OnLog(object sender, EventArgs e)
        {
            if (!_aptkAmaService.Identity.EnsureLoggedIn(true))
            {
                try
                {
                    await _aptkAmaService.Identity.LoginAsync(AptkAmaAuthenticationProvider.Facebook);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Hack for culture handling with file sync, see https://github.com/Azure/azure-mobile-apps-net-files-client/issues/8
        /// </summary>
        /// <param name="action">Action to call with specific culture</param>
        /// <param name="cultureInfo">Culture</param>
        /// <returns></returns>
        private async Task ExecWithSpecificCultureAsync(Func<Task> action, CultureInfo cultureInfo)
        {
            var userCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                await action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Calling action with culture {CultureInfo.DefaultThreadCurrentCulture.Name} failed with message: {ex.Message}");
            }
            finally
            {
                CultureInfo.DefaultThreadCurrentCulture = userCulture;
            }
        }
    }
}
