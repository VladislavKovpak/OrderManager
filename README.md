Order Manager App
This is a .NET Core MVC web application implementing authentication and authorization for managing products, supermarkets, customers, and orders.

Features
✅ Public access:

View products

View supermarkets

✅ Authenticated users:

Access home page

✅ Admin users:

Create/edit/delete supermarkets

Create/edit/delete products

View & manage all customers

View all user orders

Access the Admin tab to:

See the list of all users

Edit user roles and buyer type claims

✅ Buyer users:

View only their own orders (no modify/create/delete)

Access the Discount page if their buyerType claim is gold or wholesale

✅ Account management:

Register page → creates a user with the default role Buyer and buyer type Regular

Login/logout functionality with dynamic navigation links

Technologies Used
ASP.NET Core MVC

Entity Framework Core

Identity for authentication & authorization

Razor Views

Bootstrap (for UI styling)
