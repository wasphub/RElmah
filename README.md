RElmah - Reactive ELMAH
======

If you already used [ElmahR] in the past, you already know what **RElmah** is about. We want to monitor applications and receive real time notifications about unhandled exceptions. But the way things are done behind the scenes is totally redesigned to offer a true **reactive** experience to the developer. 

How is it done?
------

RElmah has clear goals: being a scalable streaming system for errors, with a modular and extensible design, and nevertheless minimal and simple. In order to achieve those goals a strong focus has been put on specific characteristics:

- clean separation between server and client portions
- true and independent client libraries for both JavaScript and .NET
- [Rx] at the core on both the server side the clients side libraries
- Errors, sources, connection status, basically anything that can be delivered to a client will be managed through push streams

Do you want a sneak preview? What about being able to do something like this in Linqpad?

```c#
var c = new Connection("http://localhost:9100/");
await c.Start(new ClientToken("u01"));

var q = 
	from error in c.Errors
	where error.Error.Type.IndexOf("Argument", StringComparison.OrdinalIgnoreCase) > -1
	select new { error.Error, error.Error.Time, error.Error.Type };
	  
await q.DumpLatest(true);
```

You get the idea :)

Where are we?
------

Current version is 0.6, which means some foundations are there, but it's not really ready to be used yet in production. The main features of the current implementation are:

* A deeply reactive-based structure pervades the server side logic: errors streams and configuration deltas are all managed through queries over observables
* A *backend* system allows for horizontal scalability
* Clean .NET and  JS clients are available, sharing a very similar API and again providing observable streams of information
* A fluent API is available in order to bootstrap RElmah correctly

Can I play with it?
-----

Inside this repository you find a VS2013 solution that you can build easily, and then you'll have 3 web applications inside a solution folder called Samples:

* *RElmah.Server*: this is a sample server application, which would receive errors and broadcast them to clients
* *RElmah.Dashboard.Basic*: this is a sample dashboard, very minimal and visually disappointing, but once connected it will display some basic info about each received error, and some running totals
* *RElmah.Source*: this application has just one page generating random errors, which will be posted to the server and from there broadcast to the connected dashboards

It's quite simple, you just launch the server first, then the dashboard from where you'll "login" into the server, and finally the errors source. You'll just need to refresh the failing page multiple times to see what happens to the dashboard. There are some visibility rules setup, so not every error is displayed, but if you insist in refreshing the failing page you'll definitely see some on the dashboard.

The solution also contains some Linqpad scripts, so you could for example run `basic.linq` and see how errors are streamed to it when triggered by the failing page (again not all of them because of visibility rules I already mentioned, but also because the script itself is doing some filtering, can you guess how?).

So, you might want to have a look and see if you like what's happening, and maybe contribute with ideas or coding. And I just added a [Trello] board where I'll try to keep ideas and progress up to date.

That's it?
------

Well, yes, but there are several other things it does which require more context to be tried and understood. Without documentation it might be difficult to discover them, but it's also true that the API definition (and the implementation too) is not yet 100% stable, so better wait a bit for that. I'll just mention a few anyway:

* RElmah is not about building a full fledged dashboard, is more about providing reactive APIs on top of error streams. You don't like the current dashboard? Who would anyway?? Use the client APIs to build your own! Are you more interested in building reactive automation tools to build *non visual* workflows on errors? Again, the client libraries are all you need.
* RElmah has a simple yet powerful visibility system, which allows you to aggregate sources of errors in clusters, and provide users access to them. Both a code and an HTTP API are available to interact with the visibility system. It means, as of today RElmah does not take care of persisting that information, you do it in your host the way you prefer and then you just pick the preferred API to tell RElmah how to setup visibility. 

Are these limitations? Maybe, but they also provide more flexibility, and freedom! For example, visibility does not require you to setup any extra database or deal with `.config` files if you already have that logic in your host application. Is it not the case? You white a bunch of lines of code in the bootstrap phase to setup visibility as you like it and it's done, like this:

```c#
async conf =>
{
	var c01 = await conf.AddCluster("c01");

	var s01 = await conf.AddSource("s01", "Source 01");

	var u01 = await conf.AddUser("u01");

	Task.WaitAll(
		conf.AddSourceToCluster(c01.Value.Name, s01.Value.SourceId),

		conf.AddUserToCluster(c01.Value.Name, wcu.Value.Name),
		conf.AddUserToCluster(c01.Value.Name, u01.Value.Name)
	);
}
```

It's easy! And you can write similar code to change those rules *live* while the application runs, RElmah takes care of streaming the changes where it makes sense!

This is just a short list of stuff already there, but several other ideas are there waiting for some time slot to be implemented:

* Introduction of [Akka.NET] for higher scalability and fault tolerance while receiving errors
* Support for [ASP.NET 5]
* Implementation of satellite services (like persistence), and hopefully a better default dashboard too
* Improving unit tests: there are already several, but more are needed, and I'll get rid of Microsoft Fakes...
* And what about Nuget packages for the server portion and the .NET client, and maybe even a Bower one for the JS client? I know, we'll get there too... :)

A lot to do!

Yeah, but... what's your goal here?
------

This is a disclaimer: **RElmah**, as it was for ElmahR at the beginning, is for me a playground where I try to experiment with new stuff. This does not mean I don't aim to get, at some point, to something as complete and reliable as possible. But time and energy are not on my side, so it will grow quite slowly, and it might always stay at a basic stage. Unless anybody out there wants to join :)


[ElmahR]:http://elmahr.apphb.com/
[Rx]:http://msdn.microsoft.com/en-us/data/gg577609.aspx
[Trello]:https://trello.com/b/ZBdjmxld/relmah
[Akka.NET]:http://getakka.net/
[ASP.NET 5]:http://www.asp.net/vnext
