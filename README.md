# AptkAma 
### aka Azure Mobile Apps Plugin for Xamarin & Windows

The main purpose is to access any Azure Mobile Apps API functionality by a single line of code from anywhere in the project (PCL or not), like:

    var openItems = await _aptkAmaService.Data.RemoteTable<TodoItem>().Where(t => !t.Complete).ToListAsync();

Or:

	await _aptkAmaService.Data.LocalTable<TodoItem>().PullAsync();

Or:

    await _aptkAmaService.Identity.LoginAsync(AptkAmaAuthenticationProvider.Facebook);
	
Or:

	await _service.Identity.LoginAsync ("CustomLogin", login, password);
	
Or:

	await _aptkAmaService.Notification.RegisterAsync(AptkAmaNotificationHandler.TestNotificationTemplate);

	
## Setup

Just install the AptkAma package from nuget and then follow the ToDo-AptkAma instructions.

Basic configuration:

. Update/Create your Model classes so that they all inherit from EntityData abstract class 
or ITableData interface if there's another parent class yet.

. Install, configure, initialize and optionaly register the plugin on each platform:

#### Android

Add these lines into the OnCreate of the first launching activity (ex MainActivity or SplashScreen) and complete it:

    var configuration = new AptkAmaPluginConfiguration("YOUR URL", "YOUR KEY", typeof("ONE OF YOUR MODEL CLASS").GetTypeInfo().Assembly);
    AptkAmaPluginLoader.Init(configuration, ApplicationContext);

#### iOS

Add these lines into the AppDelegate FinishedLaunching method and complete it:

    var configuration = new AptkAmaPluginConfiguration("YOUR URL", "YOUR KEY", typeof("ONE OF YOUR MODEL CLASS").GetTypeInfo().Assembly);
    AptkAmaPluginLoader.Init(configuration, app);
    
#### WindowsPhone and Windows (any version)

    var configuration = new AptkAmaPluginConfiguration("YOUR URL", "YOUR KEY", typeof("ONE OF YOUR MODEL CLASS").GetTypeInfo().Assembly);
    AptkAmaPluginLoader.Init(configuration);
	
. (Optional) If you plan to use notifications, you can handle its life cycle by implementing the AptkAmaBaseNotificationHandler class like:

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

where TestNotificationTemplate is an example of a notification custom template.
Then you'll have to configure each platform:

#### Android

Add this line BEFORE AptkAmaPluginLoader.Init(configuration, ApplicationContext); :

	configuration.NotificationHandler = new AptkAmaNotificationHandler();

where AptkAmaNotificationHandler is the name of your implementation class.

#### iOS

Add this line BEFORE AptkAmaPluginLoader.Init(configuration, app); :

	configuration.NotificationHandler = new AptkAmaNotificationHandler();

where AptkAmaNotificationHandler is the name of your implementation class.

Then still in the AppDelegate, add these overrides:

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

#### WindowsPhone and Windows (any version)

Add this line BEFORE AptkAmaPluginLoader.Init(configuration); :

	configuration.NotificationHandler = new AptkAmaNotificationHandler();

where AptkAmaNotificationHandler is the name of your implementation class.


## MVVM

After plugin installed and configured, you'd better register an instance of it to then resolve it when needed or use dependency injection.
Here are some examples:

#### MVVMCross

    Mvx.RegisterSingleton(AptkAmaPluginLoader.Instance);

#### FreshMVVM

    FreshIOC.Container.Register<IAptkAmaService>(AptkAmaPluginLoader.Instance);

#### MvvmLight

    SimpleIoc.Default.Register<IAptkAmaService>(AptkAmaPluginLoader.Instance);
    
	
## Usage

An instance of the plugin give you access by default to:

#### Data

Data give you access by default to RemoteTable< T >() where T could be one of your Model class.
From there, you can do what you used to with standard MobileServiceTable and manage online Azure data (please refer to Azure Mobile Services documentation or see samples), like this:

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

    /// <summary>
    /// DelegatingHandler to automaticaly log in user again if its auth token expired
    /// </summary>
    public class AptkAmaIdentityHandler : DelegatingHandler
    {
        private readonly IAptkAmaPluginConfiguration _configuration;
        private readonly Action _onLoggedOut;
        private IAptkAmaCredentials _credentials;
        public IAptkAmaService AptkAmaService;

        public AptkAmaIdentityHandler(IAptkAmaPluginConfiguration configuration, Action onLoggedOut = null)
        {
            _configuration = configuration;
            _onLoggedOut = onLoggedOut;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Resolve IMvxAmsService if not yet defined
                if (AptkAmaService == null)
                {
                    throw new InvalidOperationException("Make sure to configure AptkAma plugin properly before using it.");
                }

                // Cloning the request
                var clonedRequest = await CloneRequest(request);

                // Load saved user if possible
                if (_configuration.CacheService != null
                    && _configuration.CacheService.TryLoadCredentials(out _credentials)
                    && (AptkAmaService.Identity.CurrentUser == null
                    || (AptkAmaService.Identity.CurrentUser.UserId != _credentials.User.UserId
                    && AptkAmaService.Identity.CurrentUser.MobileServiceAuthenticationToken != _credentials.User.MobileServiceAuthenticationToken)))
                {
                    AptkAmaService.Identity.CurrentUser = _credentials.User;

                    clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                    // Set the authentication header
                    clonedRequest.Headers.Add("X-ZUMO-AUTH", _credentials.User.MobileServiceAuthenticationToken);

                    // Resend the request
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized
                    && _credentials != null
                    && _credentials.Provider != AptkAmaAuthenticationProvider.None
                    && _credentials.Provider != AptkAmaAuthenticationProvider.Custom)
                {
                    try
                    {
                        // Login user again
                        var user = await AptkAmaService.Identity.LoginAsync(_credentials.Provider);

                        // Save the user if possible
                        if (_credentials == null) _credentials = new AptkAmaCredentials(_credentials.Provider, user);
                        _configuration.CacheService?.SaveCredentials(_credentials);

                        clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                        // Set the authentication header
                        clonedRequest.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                        // Resend the request
                        response = await base.SendAsync(clonedRequest, cancellationToken);
                    }
                    catch (InvalidOperationException)
                    {
                        // user cancelled auth, so lets return the original response
                        return response;
                    }
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _onLoggedOut?.Invoke();
                }
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var result = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null && request.Content.Headers.ContentType != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var mediaType = request.Content.Headers.ContentType.MediaType;
                result.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
                foreach (var header in request.Content.Headers)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }
    }


This code example:

1. Send your request to Azure
2. Check server response and if unauthorized
3. Load last cached credentials and if exist, resend the request with it
4. Check server response and if unauthorized
5. Check last used identity provider and if exist, automaticaly ask your user to log in with it again
6. If not yet logged in, execute the callback action if defined (like showing an identity provider picker login view for example)
7. If logged in again, send the original request
8. If still unauthorized, throw UnauthorizedException



This handler is not activated by default. If you want to use it, you have to tell the plugin thanks to each platform configuration.

Between var configuration = new AptkAmaPluginConfiguration(...); and AptkAmaPluginLoader.Init(...); add:

    var identityHandler = new AptkAmaIdentityHandler(configuration, [Optional] YOUR_ACTION_IF_LOGGED_OUT);
    configuration.Handlers = new HttpMessageHandler[] { identityHandler };

Then, after AptkAmaPluginLoader.Init(...); add:

    identityHandler.AptkAmaService = AptkAmaPluginLoader.Instance;
    
#### Local caching

You can tell the plugin how to store some parameters for further use.

To do so, implement a storage feature of your choice like I did in samples with Xamarin.Settings and then implement the IAptkAmaCredentialCacheService to use it:

    /// <summary>
    /// This IAptkAmaCredentialsCacheService implementation is a working example 
    /// requiring the installation of Xamarin Settings plugin.
    /// </summary>
    public class AptkAmaCacheService : IAptkAmaCacheService
    {
        #region Identity
        public bool TryLoadCredentials(out IAptkAmaCredentials credentials)
        {
            credentials = !string.IsNullOrEmpty(Settings.AptkAmaIdentityUserId)
                          && !string.IsNullOrEmpty(Settings.AptkAmaIdentityAuthToken)
                          && Settings.AptkAmaIdentityProvider != AptkAmaAuthenticationProvider.None
                ? new AptkAmaCredentials(Settings.AptkAmaIdentityProvider, new MobileServiceUser(Settings.AptkAmaIdentityUserId)
                {
                    MobileServiceAuthenticationToken = Settings.AptkAmaIdentityAuthToken
                })
                : null;

            return credentials != null;
        }

        public void SaveCredentials(IAptkAmaCredentials credentials)
        {
            if (credentials == null)
                return;

            Settings.AptkAmaIdentityProvider = credentials.Provider;
            Settings.AptkAmaIdentityUserId = credentials.User.UserId;
            Settings.AptkAmaIdentityAuthToken = credentials.User.MobileServiceAuthenticationToken;
        }

        public void ClearCredentials()
        {
            Settings.AptkAmaIdentityProvider = AptkAmaAuthenticationProvider.None;
            Settings.AptkAmaIdentityUserId = string.Empty;
            Settings.AptkAmaIdentityAuthToken = string.Empty;
        }
        #endregion

        #region Notification
        public bool TryLoadRegistrationId(out string registrationId)
        {
            registrationId = Settings.AptkAmaNotificationRegistrationId;
            return !string.IsNullOrEmpty(registrationId);
        }

        public void SaveRegistrationId(string registrationId)
        {
            if (string.IsNullOrEmpty(registrationId))
                return;

            Settings.AptkAmaNotificationRegistrationId = registrationId;
        }

        public void ClearRegistrationId()
        {
            Settings.AptkAmaNotificationRegistrationId = string.Empty;
        }
        #endregion
    }

Note that you'll have to create each property UserId, AuthToken, IdentityProvider and NotificationRegistrationId on the settings feature side.

Also, you have to set this credential cache service implementation to each platform configuration:

Between var configuration = new AptkAmaPluginConfiguration(...); and AptkAmaPluginLoader.Init(...); add:

    configuration.CredentialsCacheService = new AptkAmaCredentialCacheService();
    
The plugin will deal with each methods by itself before login and after login and logout.

More details on samples.



# AptkAma Plugin LocalStore Extension

You can manage local data and sync by adding the AptkAma Plugin LocalStore Extension from Nuget and then follow the ToDo-AptkAma instructions.

Then you'll get access to LocalTable< T >() extension from Data where T could be one of your Model class and use it as you used to with the standard MobileServiceSyncTable (please refer to Microsoft official documentation) like:

    var openItems = await _aptkAmaService.Data.LocalTable<TodoItem>().Where(t => !t.Complete).ToListAsync();

Or

    await _aptkAmaService.Data.PushAsync();
    
	
## Setup

Add the AptkAma Plugin LocalStore Extension from Nuget and configure it.

Basic configuration:

After AptkAmaPluginLoader.Init(...); add on each platform:

#### Android

    AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(AptkAmaPluginLoader.Instance, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)));

#### iOS

    SQLitePCL.CurrentPlatform.Init();
    AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(AptkAmaPluginLoader.Instance, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)));

#### WindowsPhone & Windows

    AptkAmaLocalStorePluginLoader.Init(new AptkAmaLocalStorePluginConfiguration(AptkAmaPluginLoader.Instance, Windows.Storage.ApplicationData.Current.LocalFolder.Path));
    
## MVVM

Nothing to register as it's an extension of the main plugin instance

## Advanced

1. You can change the database path
2. You can change the database file name (default AptkAma.db)
3. You can change the initialization timeout (default 30s)
