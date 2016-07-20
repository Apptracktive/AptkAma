# AptkAma 
### aka Azure Mobile Apps Plugin for Xamarin & Windows

The main purpose is to access any Azure Mobile Apps API functionality by a single line of code from anywhere in the solution (PCL or platform project), like for example:
	
	// Getting data from Azure
    await _aptkAmaService.Data.RemoteTable<TodoItem>().Where(t => !t.Complete).ToListAsync();
	
	// Syncing data for offline use
	await _aptkAmaService.Data.LocalTable<TodoItem>().PullAsync();

	// Syncing files for offline use
	await _aptkAmaService.Data.LocalTable<TodoItem>().PullFilesAsync(item);

	// Asking user to log in with Facebook
    await _aptkAmaService.Identity.LoginAsync(AptkAmaAuthenticationProvider.Facebook);
	
	// Asking user to log in with credentials
	await _aptkAmaService.Identity.LoginAsync ("CustomLogin", login, password);
	
	// Registering for push notifications
	await _aptkAmaService.Notification.RegisterAsync(AptkAmaNotificationHandler.TestNotificationTemplate);
	

Everything available everywhere from IAptkAmaService.
	

## Setup

Only 2 steps mandatory:

1. Update/Create your Model classes so that they all inherit from EntityData abstract class 
or ITableData interface if there's another parent class yet.

2. Configure the plugin from any first called class in the app (PCL or platform project) with at list:

    var configuration = new AptkAmaPluginConfiguration("YOUR URL", typeof("ONE OF YOUR MODEL CLASS").GetTypeInfo().Assembly);
    AptkAmaPluginLoader.Init(configuration);


## These steps are optional:
	
3. (Optional) Registering an intance of this plugin with IoC is a better way to use it than calling the static AptkAmaPluginLoader.Instance each time.
Please look at MvvmCross, MvvmLight, FreshMvvm or any Mvvm/IoC framework of your choice and see online documentation and tutorials

4. (Optional) If you want to manage local data thanks to LocalTable<T> and other methods
you have to install the AptkAma Plugin LocalStore Extension nuget package.

5. (Optional) If you want to manage file sync for offline access
you have to install the AptkAma Plugin FileStore Extension nuget package.

6. (Optional) You can save some parameters with local caching by implementing the IAptkAmaCacheService interface. 
If so, add this line BEFORE AptkAmaPluginLoader.Init(configuration); :

	configuration.CacheService = new AptkAmaCacheService();

where AptkAmaCacheService is the name of your implementatation class.

It is useful when trying to handle authentication token expiration, auto-login or notification auto-register (see samples, online documentation and tutorials).
	
7. (Optional) If you plan to use notifications, you can handle its life cycle by implementing the AptkAmaBaseNotificationHandler class.
If so, add this line BEFORE AptkAmaPluginLoader.Init(configuration); :

	configuration.NotificationHandler = new AptkAmaNotificationHandler();

where AptkAmaNotificationHandler is the name of your implementation class, wich could be like:

	public class AptkAmaNotificationHandler : AptkAmaBaseNotificationHandler
    {
        public static IAptkAmaNotificationTemplate TestNotificationTemplate = new AptkAmaNotificationTemplate("MyTemplate")
        {
            {"name", "$(name)"},
            {"alert", "$(message)"},
            {"sound", "default"}
        };

        public AptkAmaNotificationHandler()
        {
            GoogleSenderIds = Constants.GoogleSenderIds;
        }

        public override void OnNotificationReceived(IAptkAmaNotification notification)
        {
            if (notification.IsTypeOf(TestNotificationTemplate))
            {
            }
        }
    }

where TestNotificationTemplate is an example of a notification custom template and GoogleSenderIds your Google project id.

Also, for notifications to work with iOS, into the AppDelegate, add these overrides:

	public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
    {
        AptkAmaPluginLoader.Instance.Notification.RegisteredForRemoteNotifications(deviceToken);
    }

    public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
    {
        AptkAmaPluginLoader.Instance.Notification.FailedToRegisterForRemoteNotifications(error);
    }

    public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
    {
        AptkAmaPluginLoader.Instance.Notification.ReceivedRemoteNotification(userInfo);
    }

	
## Usage

An instance of the plugin give you access by default to:

#### Data

Data give you access by default to RemoteTable< T >() where T could be one of your Model class.
From there, you can do what you used to with standard MobileServiceTable and manage online Azure data (please refer to Azure Mobile Apps documentation or see samples), like this:

    var openItems = await _aptkAmaService.Data.RemoteTable<TodoItem>().Where(t => !t.Complete).ToListAsync();

If you want to manage local data thanks to LocalTable<T>, you need to install the AptkAma Plugin LocalStore Extension nuget package.
	
#### Identity

Identity offers methods to manage the login process.
For example, it allows you to ask user for social login directly from a PCL.

Authenticate user with social identity provider like this (ex Facebook):

    await _aptkAmaService.Identity.LoginAsync(AptkAmaAuthenticationProvider.Facebook);
    
You can also register a new user with custom authentication like this:

    await _aptkAmaService.Identity.RegisterAsync<YOUR_REGISTRATION_REQUEST_CLASS, YOUR_REGISTRATION_RESULT_CLASS>("NAME_OF_YOUR_REGISTRATION_CONTROLER", YOUR_REGISTRATION_REQUEST_CLASS instance);

And then log in user like this:

    await _aptkAmaService.Identity.LoginAsync("NAME_OF_YOUR_LOGIN_CONTROLER", "USER_LOGIN", "USER_PASSWORD");
    
But if you need to, you might want to log in specifying your own request and result class like we do with registration.
    
Please see the backend sample for more details about the custom controlers itself.

#### Notification

Notification offers methods to manage the notification life cycle (from the PCL project or not), such as:

Registration:

	await _aptkAmaService.Notification.RegisterAsync(AptkAmaNotificationHandler.TestNotificationTemplate);
	
Unregistration:

	await _aptkAmaService.Notification.UnregisterAllAsync();
	
Reception:

	public override void OnNotificationReceived(IAptkAmaNotification notification)
	{
		if (notification.IsTypeOf(TestNotificationTemplate))
		{
		}
	}

#### Api

Api is here to send custom requests to custom Azure controllers.


## Advanced

#### Handling authentication token expiration

You can specify custom handlers.

One thing you can do with handler is automaticaly use cached token to authenticate unauthorized request and then ask user to log in again if his token expired or if not yet logged in thanks to callback action.
Here is this handler provided by the plugin:

This code:

1. Send your request to Azure
2. _Check server response and if unauthorized
3. _Load last cached credentials and if exist, resend the request with it
4. __Check server response and if unauthorized
5. __Try to refresh the token, resend the request with it
6. ___Check server response and if unauthorized
7. ___Check last used identity provider and if exist, automaticaly ask your user to log in with it again
8. _If not yet logged in, execute the callback action if defined (like showing an identity provider picker login view for example)
9. If logged in again, send the original request
10. If still unauthorized, throw UnauthorizedException

This handler is not activated by default. If you want to use it, you have to tell the plugin thanks to each platform configuration.

Between var configuration = new AptkAmaPluginConfiguration(...); and AptkAmaPluginLoader.Init(...); add:

    var identityHandler = new AptkAmaIdentityHandler(configuration, [Optional] YOUR_ACTION_IF_LOGGED_OUT);
    configuration.Handlers = new HttpMessageHandler[] { identityHandler };

Then, after AptkAmaPluginLoader.Init(...); add:

    identityHandler.AptkAmaService = AptkAmaPluginLoader.Instance;
    
#### Local caching

You can tell the plugin how to store some parameters for further use.

To do so, implement a storage feature of your choice like I did in samples with Xamarin.Settings and then implement the IAptkAmaCacheService to use it:



Note that you'll have to create each property UserId, AuthToken, IdentityProvider and NotificationRegistrationId on the settings feature side.

Also, you have to set this cache service implementation to each platform configuration:

Between var configuration = new AptkAmaPluginConfiguration(...); and AptkAmaPluginLoader.Init(...); add:

    configuration.CacheService = new AptkAmaCacheService();
    
The plugin will deal with each methods by itself before login and after login and logout.

The same with notification registration process.

More details on samples.



# AptkAma Plugin LocalStore Extension

You can manage local data and sync by adding the AptkAma Plugin LocalStore Extension from Nuget and then follow the ToDo-AptkAma instructions.

Then you'll get access to LocalTable< T >() extension from Data where T could be one of your Model class and use it as you used to with the standard MobileServiceSyncTable (please refer to Microsoft official documentation) like:

	// Getting some local data
    await _aptkAmaService.Data.LocalTable<TodoItem>().Where(t => !t.Complete).ToListAsync();

	// Pushing data to Azure
    await _aptkAmaService.Data.PushAsync();
    
	
## Setup

Nothing mandatory.


## These steps are optional

1. (Optional) You can change the database short path
2. (Optional) You can change the database file name
3. (Optional) You can set your own IMobileServiceSyncHandler implementation
4. (Optional) You can set an IAptkAmaFileStoreService instance for file syncing



# AptkAma Plugin FileStore Extension

You can manage local data and sync by adding the AptkAma Plugin FileStore Extension from Nuget (currently in beta) and then follow the ToDo-AptkAma instructions.

Then you'll get access to file sync extension methods and use it as you used to with the standard MobileServiceSyncTable (please refer to Microsoft official documentation) like:

	// Associating a file with a table item
	await _aptkAmaService.Data.LocalTable<TodoItem>().AddFileAsync(item, Path.GetFileName(image.Path));

	// Pushing files to Azure
	await _aptkAmaService.Data.LocalTable<TodoItem>().PushFileChangesAsync();
    
	
## Setup

1. Update/Create your Model classes used with files, so that they all inherit from FileSyncEntityData abstract class 
or IFileSyncTableData interface if there's another parent class yet (previous EntityData could be replaced by FileSyncEntityData).

2. You have to provide a FileStore plugin extension instance to the LocalStore plugin extension with this line :

	AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(AptkAmaFileStorePluginLoader.Instance));


/!\ As the File libs from Microsoft are still in beta, here are some workarounds:

3. Id property inherited from a parent class is not yet supported so please add the "new" keyword into each previous class:

    public new string Id { get; set; }

4. Non US culture info are not yet supported by the file sync process, so please add these lines:

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

and use it like so each time you play with files:

	await ExecWithSpecificCultureAsync(async () => await _aptkAmaService.Data.LocalTable<TodoItem>().PullFilesAsync(items.First()), new CultureInfo("en-US"));



## These steps are optional:

5. (Optional) You can set your own files download folder name 
by initializing the FileStore plugin extension with this line (before initializing LocalStore):

    AptkAmaFileStorePluginLoader.Init(new AptkAmaFileStorePluginConfiguration(new AptkAmaFileManagementService("FOLDER_OF_YOUR_CHOICE")));

6. (Optional) You can set your own IAptkAmaFileManagementService and IFileSyncTriggerFactory implementations 
by initializing the FileStore plugin extension with this line (before initializing LocalStore):

    AptkAmaFileStorePluginLoader.Init(new AptkAmaFileStorePluginConfiguration(YourCustomFileManagementService, YourCustomTriggerFactory));
