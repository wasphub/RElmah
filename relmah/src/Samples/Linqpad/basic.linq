<Query Kind="Statements">
  <Reference Relative="..\..\..\..\..\repo\src\RElmah.Client\bin\Debug\RElmah.Client.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\RElmah.Client.dll</Reference>
  <Reference Relative="..\..\..\..\..\repo\src\RElmah.Common\bin\Debug\RElmah.Common.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Common\bin\Debug\RElmah.Common.dll</Reference>
  <Reference Relative="..\..\..\..\..\repo\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll</Reference>
  <Namespace>RElmah.Client</Namespace>
  <Namespace>RElmah.Common</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

var c = new Connection("http://localhost:9474/");
c.Start();

var q = 
	from error in c.Errors
	select error.Name;
	
q.DumpLive();