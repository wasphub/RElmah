<Query Kind="Statements">
  <Reference Relative="..\src\RElmah.Client\bin\Debug\RElmah.Client.dll">&lt;MyDocuments&gt;\RElmah\src\RElmah.Client\bin\Debug\RElmah.Client.dll</Reference>
  <Namespace>RElmah.Client</Namespace>
</Query>

var c = new Connection("http://localhost:50360/");
var q = 
	from x in c.Errors
	where x < 50
	select x;
q.DumpLive();