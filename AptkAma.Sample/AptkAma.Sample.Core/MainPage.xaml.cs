using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            ToDoItems.ItemsSource = await GetTodoItemsAsync();
        }

        async Task AddItem(TodoItem item)
        {
            await _aptkAmaService.Data.LocalTable<TodoItem>().InsertAsync(item);

            // File
            var image = await _mediaService.PickPhotoAsync();
            if (image != null)
            {
                try
                {
                    var targetPath = _aptkAmaService.Data.FileManagementService().CopyFileToAppDirectory(item.Id, image.Path);
                    var fileName = Path.GetFileName(targetPath);
                    var file = await _aptkAmaService.Data.LocalTable<TodoItem>().AddFileAsync(item, Path.GetFileName(targetPath));
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
            ToDoItems.ItemsSource = await GetTodoItemsAsync();
        }

        async Task<List<TodoItem>> GetTodoItemsAsync()
        {
            return await _aptkAmaService.Data.LocalTable<TodoItem>().Where(i => !i.Complete).ToListAsync();
        }

        public async void OnLog(object sender, EventArgs e)
        {
            if (!_aptkAmaService.Identity.EnsureLoggedIn())
            {
                await _aptkAmaService.Identity.LoginAsync("CustomLogin", "Your login here", "Your password here");
            }
        }
    }
}
