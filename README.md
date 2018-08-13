# MUnique.Log4Net.CoreSignalR

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://travis-ci.org/MUnique/Log4Net.CoreSignalR.svg?branch=master)](https://travis-ci.org/MUnique/Log4Net.CoreSignalR)

**MUnique.Log4Net.CoreSignalR** provides a [Log4Net Appender](http://logging.apache.org/log4net/release/manual/introduction.html#appenders) 
which forwards Log4Net log events to a [SignalR](https://github.com/aspnet/SignalR) hub, which is also provided as [LogHub](src/LogHub.cs).

From this hub it gets forwarded to its registered SignalR clients, e.g. to a JavaScript client in the browser.

The main use case for MUnique.Log4Net.CoreSignalR is building a log viewer on your website that gives easy visibility to diagnostic information and errors logged on the server.

## .NET Standard / Core
This project uses the new [SignalR](https://github.com/aspnet/SignalR) library for .NET Standard/Core. It requires .NET Standard 2.0, because SignalR requires it as well.
You can find a similar project for the regular version of the .net framework [here](https://github.com/MUnique/log4net.SignalR).

## Getting started

### Add a reference to MUnique.Log4Net.CoreSignalR

To use this project, the easiest way is adding a reference to the nuget package, because it adds all the required dependencies itself.
The package name is *munique.log4net.coresignalr* and can be added with the following command in the Package Manager Console:

* `Tools --> NuGet Package Manager --> Package Manager Console`
* Run ``Install-Package munique.log4net.coresignalr``

### Configure log appender

Configure MUnique.Log4Net.CoreSignalR as a Log4Net appender by adding this to your log4net configuration:

```xml
<log4net debug="true">
    <appender name="SignalrAppender" type="MUnique.Log4Net.CoreSignalR.SignalrAppender, MUnique.Log4Net.CoreSignalR">
        <HubUrl>http://localhost/signalr/hubs/logHub/</HubUrl>
        <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%date %-5level - %message%newline" />
        </layout>
    </appender>
    <root>
        <appender-ref ref="SignalrAppender" />
    </root>
</log4net>
```

Since SignalR for .net core doesn't provide a method to get something like the "GlobalHost" (and workarounds are ugly), the appender will always have to connect to a hub.

Additionally, the LogHub needs to be added to your application startup:

```csharp
public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Faster pings for testing
            options.KeepAliveInterval = TimeSpan.FromSeconds(5);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseSignalR(routes =>
        {
            routes.MapHub<LogHub>("/signalr/hubs/logHub");
        });
    }

    // If you want to transmit older messages to newly connected clients, you can configure the maximum number of cached log entries.
    // By default, this is 0 - therefore no caching happens
    LogHub.MaximumCachedEntries = 200;
}
```

A complete example about adding SignalR to your application can be found [here](https://github.com/aspnet/SignalR/blob/release/2.2/samples/SignalRSamples/Startup.cs).


### Usage Example: Alert when an error was logged

Add the [ASP.NET Core SignalR javascript client library](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-2.1&tabs=visual-studio#add-the-signalr-client-library) to your web page to listen out for error log events raised on the server.
Once the SignalrAppender is set up, all events logged on the server using Log4Net will be transmitted to the hub and a browser can listen to it by executing a JavaScript function.

Here we're handling log events of level 'ERROR':

```javascript
let connection = new signalR.HubConnectionBuilder().withUrl("/signalr/hubs/logHub").build();

connection.on("OnLoggedEvent", function (formattedEvent, loggedEvent, id) {
    if (loggedEvent.Level.Name === "ERROR") {
      alert(formattedEvent);
    }
});

connection.start()
    .then(() => connection.send("Subscribe"))
    .catch(error => console.error(error.toString()));
```

The parameters of `OnLoggedEvent` are simply:
  * formattedEvent: The log entry formatted as string as configured in your log4net configuration
  * loggedEvent: An object which contains details about the logged event, such as `Message`, `Level`, `TimeStamp` or `ExceptionString`. TypeScript definitions are included, you can find them [here](src/types.ts).
  * id: Sequentially ordered identifier. May be helpful if you want to cache log messages and later retrieve them by a client which connects after the event happend.


### Groups
Additionally, you can specifiy a group name. This can be handy, e.g. if you want to provide different appenders for different parts of your application or to provide only specific log messages to a specific group of clients.

The default group name is "DefaultGroup". E.g. if we want to set it to "MyGroup", you can set it in the configuration:

```xml
<configSections>
  <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
</configSections>

<log4net debug="true">
    <appender name="SignalrAppender" type="MUnique.Log4Net.CoreSignalR.SignalrAppender, MUnique.Log4Net.CoreSignalR">
        <HubUrl>http://localhost/signalr/hubs/logHub/</HubUrl>        
        <GroupName>MyGroup</GroupName>
        <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%date %-5level - %message%newline" />
        </layout>
    </appender>
    <root>
        <appender-ref ref="SignalrAppender" />
    </root>
</log4net>
```

To subscribe for log events of this group, we have to specify the group accordingly when subscribing, e.g. Javascript:


```javascript
connection.start()
    .then(() => connection.send("Subscribe", "MyGroup"))
    .catch(error => console.error(error.toString()));
```


## License

This project is open source under the [The MIT License (MIT)](http://www.opensource.org/licenses/mit-license.php).
