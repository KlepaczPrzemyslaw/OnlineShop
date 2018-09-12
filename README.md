# OnlineShop_ENG
- Project Date: 27-08-2018 to 12-09-2018
- Description: Project after 6 weeks of study C# at CodersLab
- Short Description: A simple ASP.NET Core MVC application with SQL database.
- To run:
  1) Download this repository.
  2) Run OnlineShop.sln in Visual Studio.
  3) Press F5 to start debugging (You will need a MSSQL database!).
    
## Description: 
  1) Not logged user can:
  - Check products list
  - Check products details
  2) Logged user can:
  - Everything what can not logged user do
  - Add product to cart
  - Buy products from cart
  - Edit personal data (in user menu)
  - Check bought product (in user menu)
  - Cancel order for product (but only before admin's order confirmation!)
  3) Admin can:
  - Everything what can logged user do
  - Create, edit and delete products
  - Confirm, that the goods was shipped to customer
  
  - Default user account:
  Login: `user@user.com` ,
  Password: `UserPass1.`
  
  - Default admin account:
  Login: `admin@admin.com` ,
  Password: `AdminPass1.`
  
  - Email Sender doesn't work - because I removed password from appsettings.json. If You want to check it, please upadte email    informations in appsettings.json as example inside 
