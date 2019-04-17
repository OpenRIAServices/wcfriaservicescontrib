# Project Description
WCF RIA Services Contrib is a collection of tools for WCF RIA Services. Contributions are welcome.

WCF RIA Services Contrib has the following features:

|Library | Description|
|---|---|
| [ComboBoxExtensions](http://blogs.msdn.com/b/kylemc/archive/2010/06/18/combobox-sample-for-ria-services.aspx) | Is a collection of extensions to ComboBox that add asynchronous loading, cascading ComboBoxes and support for Entity associations to ComboBoxes.| 
| [Data Validation Framework](EntityGraphs)| Is a generic validation framework that is much more flexible and feature-rich than the validation framework of WCF RIA Services.|
| [Entity Tools](EntityTools) | Is a collection of extension methods that add state manipulation functionality (such as cloning) to entities. |
| [EntityGraph](EntityGraphs) | Is a technology to separate generic operations (such as cloning iterating, validation) from your data model. |
| [FluentMetadata](FluentMetadata-for-WCF-RIA-Services) | A Fluent API for defining metadata for WCF RIA Services entities.|
| [T4RIA](T4RIA) |is a T4 template that generates domain services, metadata and localization for WCF RIA Services from ADO.NET Entity Data Model.|

# Download and install
WCF RIA Services is available in the following forms:
* Source code. If you need the most recent stuff of WCF RIA Services Contrib, or if you want to make (and contribute) your own changes, checking out the source code [here](http://riaservicescontrib.codeplex.com/SourceControl/list/changesets) is the way to go.
* NuGet. This is the prefered way of using WCF RIA Services Contrib  because it allows you to use only those parts that you need, because installation is very simple, and because most NuGet packages are released more frequently then the binary release of WCF RIA Services Contrib, described below. The following sub-components of WCF RIA Services Contrib are distributed as NuGet Packages:
	* [EntityTools](http://nuget.org/List/Packages/RiaServicesContrib.EntityTools)
	* [DataValidationFramework](http://nuget.org/List/Packages/DataValidationFramework)
	* [EntityGraph](http://nuget.org/List/Packages/EntityGraph)
	* [FluentMetadata](http://nuget.org/List/Packages/FluentMetadata)
* Binary zip file. This bundles EntityTools, ComboBoxExtensions, and T4RIA. You can obtain this distribution via the [Downloads](http://riaservicescontrib.codeplex.com/releases) tab. The zip file is no longer being maintained.

# Please Contribute
If you have any RIA Services related code which you would like to add to this project please let me know. I welcome anyone willing to contribute.

