RElmah - Reactive ELMAH
======

If you already used [ElmahR] in the past, you already know what **RElmah** is about. We want to monitor applications and receive real time notifications about unhandled exceptions. But the way things are done behind the scenes is totally redesigned to offer a true **reactive** experience to the developer. 

How is it done?
------

RElmah has clear goals: being a scalable streaming system for errors, with a modular and extensible design and nevertheless minimal and simple. In order to achieve those goals a strong focus has been put on specific characteristics:

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

Where we are?
------

Current version is 0.6, which means some foundations are there, but it's not really ready to be used yet in production. The main features of the current implementation are:

* A deeply reactive-based structure pervades the server side logic: errors streams and configuration deltas are all managed through queries over observables
* A *backend* system allows for horizontal scalability
* Clean .NET and  JS clients are available, sharing a very similar API and again providing observable streams of information
* A fluent API is available in order to bootstrap RElmah correctly

You might want to have a look and see if you like what's happening, and maybe contribute with ideas or coding. And I just added a [Trello] board where I'll try to keep ideas and progress up to date.

Yeah, but... what's your goal here?
------

This is a disclaimer: **RElmah**, as it was for ElmahR at the beginning, is for me a playground where I try to experiment with new stuff. This does not mean I don't aim to get, at some point, to something as complete and reliable as possible. But time and energy are not on my side, so it will grow quite slowly, and it might always stay at a basic stage. Unless anybody out there wants to join :)


[ElmahR]:http://elmahr.apphb.com/
[Rx]:http://msdn.microsoft.com/en-us/data/gg577609.aspx
[Trello]:https://trello.com/b/ZBdjmxld/relmah
