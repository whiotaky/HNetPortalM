using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("HNetPortal-M")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("WSH Computing")]
[assembly: AssemblyProduct("HNetPortal-M")]
[assembly: AssemblyCopyright("Copyright © 2006-2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0066d424-5768-4385-a6f8-3199068db857")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

//WSH-Custom so we can grab appDate from here
[assembly: CustomAssemblyAppDate("01/05/2019")]

//WSH-Custom Because it supports letters
[assembly: CustomAssemblyAppVersion("3.7.6")]

//WSH-Custom 
[assembly: CustomAssemblyAuthor("William S. Hiotaky")]

[AttributeUsage(AttributeTargets.Assembly)]
public class CustomAssemblyAppDate : Attribute {
	public string Date;
	public CustomAssemblyAppDate() : this(string.Empty) { }
	public CustomAssemblyAppDate(string txt) { Date = txt; }
}

public class CustomAssemblyAppVersion : Attribute {
	public string Version;
	public CustomAssemblyAppVersion() : this(string.Empty) { }
	public CustomAssemblyAppVersion(string txt) { Version = txt; }
}

public class CustomAssemblyAuthor : Attribute {
	public string Author;
	public CustomAssemblyAuthor() : this(string.Empty) { }
	public CustomAssemblyAuthor(string txt) { Author = txt; }
}
