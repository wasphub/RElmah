<Query Kind="Statements">
  <Reference Relative="..\src\RElmah.Client\bin\Debug\RElmah.Client.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\RElmah.Client.dll</Reference>
  <Reference Relative="..\src\RElmah.Domain\bin\Debug\RElmah.Domain.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Domain\bin\Debug\RElmah.Domain.dll</Reference>
  <Reference Relative="..\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll">&lt;MyDocuments&gt;\RElmah\repo\src\RElmah.Client\bin\Debug\System.Reactive.Linq.dll</Reference>
  <Namespace>RElmah.Client</Namespace>
  <Namespace>RElmah.Domain</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
</Query>

var c = new Connection("http://localhost:50360/");
c.Start();

var q = 
	from error in c.Clusters
	//where error.Targets.First().Name.StartsWith("B")
	select error.Targets.First().Name;
	
q.DumpLive();