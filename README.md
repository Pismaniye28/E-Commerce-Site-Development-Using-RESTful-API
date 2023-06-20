# E-Commerce-Site-Development-Using-RESTful-API
E-Commerce Site Development Using RESTful API - Basic Website 
As consumers' behavior shifts toward online purchasing options, electronic commerce has emerged as an essential marketplace facilitating trade over the internet via web-based applications known generally as e-commerce sites. Such portals have grown in popularity due to their potential benefits such as increased accessibility for customers worldwide without geographical barriers at any time; convenient product information browsing with online catalogs; secure transactions using trusted payment systems; effective post-sale customer service through efficient order delivery etcetera. Businesses operating across these platforms may possess distinctive business models like B2B (business-to-business), B2C(business- consumer ), C2C(consumer- consumer) or even curation-oriented markets such as C2B (consumer-to-business). When it comes to the most popular online shopping portals across the globe, Amazon, eBay, Alibaba and Shopify seem to be the usual suspects. Nonetheless, search engine optimization tactics for higher visibility on Google searches; UX designs that offer exceptional browsing experiences; and effective digital marketing methods remain just as critical aspects of successful e-commerce ventures.
Key Words: e-commerce websites include online shopping, digital storefront, mobile commerce, payment gateways, and inventory management.

# Frontend Dependencies

For Front-end Dependencies; I will be using node module. For installation you will need to open terminal and fallow these steps : 
•	Open your terminal or command prompt.
•	Navigate to the root directory of your Node.js project or the directory where you want to install the package. You can use the cd command to change directories. For example: cd my-project
•	To install a package, use the npm install command followed by the package name. For example, to install a package called "example-package," run the following command: npm install example-package
•	If you want to install a specific version of the package, you can specify it by appending @ followed by the version number. For example: npm install example-package@1.2.3
•	NPM will start downloading the package and its dependencies. It will create a folder named node_modules in your project directory and store the installed packages there.
•	Once the installation is complete, you can start using the package in your Node.js code. You will typically need to import or require the package into your script. The specific import or require statement will depend on the package you installed.
•	For example, if you installed a package called "example-package," you would import it like this: const examplePackage = require('example-package');
•	That's it! You have successfully installed a Node.js package using NPM from the terminal.
Here is my npm list:
Table 4: Summary Dependencies Table
Name	Version	Command
popperjs	2.11.6	npm i @popperjs/core
ckeditor	4.12.1	npm i ckeditor
font-awesome	4.7.0	npm i font-awesome
Name	Version	Command
fslightbox	3.3.2-2	npm i fslightbox
jquery	3.6.1	npm i jquery
bootstrap	5.2.3	npm i bootstrap

# Backend Dependencies

First you have to create your first project with .NET you can always start with terminal just like: dotnet new mvc. For starting api: dotnet new webapi.
•	Create a .NET project: Open your terminal or command prompt and navigate to the directory where you want to create your project. Use the ‘dotnet’ new command to create a new project. For example, to create a new console application, you can run: dotnet new console
•	Install the NuGet package: In the terminal or command prompt, navigate to the root directory of your .NET project. Then, run the following command to install the package:	dotnet add package <package-name>
•	Replace <package-name> with the actual name of the NuGet package. For example: dotnet add package Newtonsoft.Json
•	Restore dependencies: After running the previous command, the .NET CLI will download and install the specified package, along with its dependencies. To ensure all dependencies are properly installed, run the following command: dotnet restore
Table 5: Summary Dependencies Table 2 
Name	Version
Microsoft.AspNetCore.Identity.EntityFrameworkCore	6.0.0
Microsoft.EntityFrameworkCore.Design	6.0.10
Microsoft.VisualStudio.Web.CodeGeneration.Design	6.0.10
Newtonsoft.Json	13.0.2
Iyzipay	*newest
  

  
  # FlowChart


  
  •	The customer navigates to the shopping cart page.
•	The system displays the list of products in the shopping cart along with their quantities and prices.
•	The customer reviews the items in the shopping cart and makes any necessary modifications, such as updating quantities or removing items.
•	The customer clicks on the "Proceed to Checkout" button.
•	The system prompts the customer to provide shipping and billing information, such as name, address, and payment method.
•	The customer enters the required information and submits the order.
•	The system validates the provided information and calculates the total order amount, including taxes and shipping fees.
•	The system generates an order confirmation page, displaying the order details, total amount, and a unique order number.
•	The customer receives an email notification with the order confirmation and relevant information.
•	The system updates the inventory to reflect the purchased items and quantities.
•	The system initiates the payment process using the customer's selected payment method.
•	The payment gateway processes the payment transaction securely.
•	The system updates the order status to "Paid" and sends a payment confirmation email to the customer.
•	The system notifies the shipping department to prepare the order for shipment.
•	The shipping department packs the items, generates a shipping label, and hands over the package to the assigned shipping carrier.
•	The system updates the order status to "Shipped" and provides the customer with a tracking number.
•	The customer can track the shipment by entering the provided tracking number on the KokoMija website or the shipping carrier's website.
•	The customer receives the package and verifies the contents for accuracy and condition.
•	The customer may choose to leave feedback or rate the shopping experience through the KokoMija website or other review platforms.
•	The system updates the order status to "Delivered" and sends a delivery confirmation email to the customer.
•	The use case ends.

  # Iyzi Api
  
 •Register and obtain API credentials: Visit the iyzico website and sign up for an account. Once registered, you'll need to obtain the necessary API credentials, such as API key and secret key, which will be used to authenticate your requests.
 •	Install the required NuGet package: In your .NET Core application, use NuGet Package Manager or the .NET CLI to install the iyzico SDK package. The official iyzico SDK for .NET can be found on NuGet.org. You can install it by running the following command in the NuGet Package Manager Console or the terminal: dotnet add package Iyzipay

  # Project About
  A quick start for developeing such an app 
  

