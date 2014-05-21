<Query Kind="Statements">
  <Reference Relative="..\src\RElmah.Client\bin\Debug\RElmah.Client.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\RElmah.Client.dll</Reference>
  <Reference Relative="..\src\RElmah.Client\bin\Debug\RElmah.Domain.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\RElmah.Domain.dll</Reference>
  <Reference Relative="..\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll</Reference>
  <Namespace>RElmah.Client</Namespace>
  <Namespace>RElmah.Domain</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

var c = new Connection("http://localhost:50360/");
var errors = c.Clusters;

var q = 
	from error in errors
	//where error.Entry.Name.StartsWith("B")
	select error.Entries.First().Name;
	
q.DumpLive();