<Query Kind="Statements">
  <Reference Relative="..\..\RElmah.Client\bin\Debug\RElmah.Client.dll">&lt;MyDocuments&gt;\RElmah\RElmah\relmah\src\RElmah.Client\bin\Debug\RElmah.Client.dll</Reference>
  <Reference Relative="..\..\RElmah.Client\bin\Debug\RElmah.Common.dll">&lt;MyDocuments&gt;\RElmah\RElmah\relmah\src\RElmah.Client\bin\Debug\RElmah.Common.dll</Reference>
  <Reference Relative="..\..\RElmah.Client\bin\Debug\System.Reactive.Core.dll">&lt;MyDocuments&gt;\RElmah\RElmah\relmah\src\RElmah.Client\bin\Debug\System.Reactive.Core.dll</Reference>
  <Reference Relative="..\..\RElmah.Client\bin\Debug\System.Reactive.Interfaces.dll">&lt;MyDocuments&gt;\RElmah\RElmah\relmah\src\RElmah.Client\bin\Debug\System.Reactive.Interfaces.dll</Reference>
  <Reference Relative="..\..\RElmah.Client\bin\Debug\System.Reactive.Linq.dll">&lt;MyDocuments&gt;\RElmah\RElmah\relmah\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll</Reference>
  <Namespace>RElmah.Client</Namespace>
  <Namespace>RElmah.Common</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

var c = new Connection("http://localhost:9100/");

var apps = 
	from app in c.Sources
	select app;
apps.Subscribe(o => o.Dump());

var sts = 
	from s in c.States
	select s;
sts.Subscribe(o => o.Dump());

await c.Start(new ClientToken("u01"));