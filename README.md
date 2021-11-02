# CitrixStorefrontDotNet
Citrix Storefront API with C#

Derived from the Citrix Developer VS entension at https://visualstudiogallery.msdn.microsoft.com/6587c715-fff3-4e2b-bf7f-10331840c552

You don't technically need this extension to run this code, it contains some handy project boilerplates, and this code is derived from those boilerplates

The Storefront API is all REST-based anyway, and this code just abstracts away some of the REST internals necessary to interact with Citrix

Developed to work with the Citrix Storefront Web API v3.0, available here: https://www.citrix.com/downloads/storefront-web-interface/sdks.html  If you're using a different version of Storefront, well, your mileage may vary.

# What's inside
The solution consists of two projects: a code library containing the static helper methods, and a simple console app that authenticates, grabs a collection of apps(resources), and outputs the content's of the fourth app's *.ica file onto the console.

No flexibility, no error handling. You can do all that yourself. Also, there are orphaned parameters and junk that should probably be cleaned up. 

# setting it up
1. Your app must have connectivity to Storefront. Im my experience, even having Storefront accessible over the web is not enough; you need to be inside the resident domain in order for this to work. Your experience might vary.
2. The parameters you'll need are in the App Settings of the Console App's app.config.  The parameters are Storefront Url, username, password, and domain of the calling user. Unfortunately you need to pass credentials to the web service in order for this to work; supposedly, this will work with pass-through and Kerberos but I have no clue on how to set that up.

