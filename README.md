NetworkReplayer
===============

Very basic tool to record and then later replay HTTP request. Useful during demo to not depend on conference network.

Usage
=====

The first thing to do is to change the code of your application to pass through the recorder.

Make sure that both your device and your computer are on the same network and that the latter is accessible from the outside.

In your client code, use the following pattern to do HTTP request:

```csharp
// The base of your URL should reach your computer
var url = "http://192.168.1.27:8080";
// Add the rest of the URL as if you were connecting normally to your ressource
url += "/orgs/xamarin/events";
// You can also add query parameters
url += "?page=" + offset;

var client = new WebClient ();
// And before issuing the request, setup the following header containing the real protocol + hostname to use
client.Headers.Add ("X-Forward-To", "https://api.github.com");
```

On your computer, launch the tool like that:

```shell
mono --debug NetworkReplayer.exe --record -p 8080
```

This will create a `replay_cache` folder where the content of each request will be stored.

You can then use your application normally as if you were on stage.

When you are happy with yourself, Ctrl+C the previous program and then relaunch it with the following updated command line arguments:

```shell
mono --debug NetworkReplayer.exe --replay -p 8080
```

Now when you run your application, it will automatically fetch the previously saved response content.

Tips for connecting your laptop and mobile device
=================================================

- Use a dedicated network for your laptop and device
  * Your own router
  * Ad-Hoc network (on Mac, Airport status icon > Create Network...) (Careful, Android doesn't support Ad-Hoc WiFi connection)
  * Laptop in router mode (Connection sharing with Mac, better solution existing for other OS)
- Prefer using the [5GHz band](http://en.wikipedia.org/wiki/List_of_WLAN_channels#5.C2.A0GHz_.28802.11a.2Fh.2Fj.2Fn.29.5B13.5D) (either WiFi A or N) which corresponds to channels like 40, 44, 48 (important to use those for Android).
  This will prevent interferences with the attendees WiFi gear since most places/conferences only offer 2.4GHz based-networks (for now).
