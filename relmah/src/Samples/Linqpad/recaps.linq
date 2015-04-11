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
await c.Start(new ClientToken("u01"));

var q = 
	from r in c.Recaps
	select new { r.SourceId, r.Type, r.Measure };
	 
await q.DumpLatest(true);