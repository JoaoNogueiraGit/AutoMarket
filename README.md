# AutoMarket
A full-stack marketplace platform for buying and selling used vehicles.

📝 **Project Overview**
~~
Auto Market is a web-based platform designed to bridge the gap between private sellers and potential buyers in the automotive market. This academic project focuses on creating a seamless user experience for listing vehicles, managing inventories, and searching for the perfect car through advanced filtering.

✨ **Key Features**
~~
User Authentication: Secure sign-up and login for buyers and sellers.

Ad Management: Sellers can create, edit, and delete vehicle listings (including image uploads, pricing, and technical specs).

Advanced Search: Filter vehicles by brand, model, fuel type, year, and price range.

Favorites System: Users can save favorite brands to a personal "Wishlist" to recieve notifications when an ad with a car of that gets uploaded on the website.

🛠 **Tech Stack**
~~
Frontend: HTML, CSS, Javascript

Backend: C# .NET 9.0 with Entity Framewrok

Database: Relational Database SQL

Tools: Bootstrap, Ajax

🚀 **Getting Started**
~~
Before running the project, ensure you have the following installed:

    .NET SDK (v6.0 or higher)
    
    SQL Server (LocalDB or Express)
    
    EF Core Tools: Install by running: "dotnet tool install --global dotnet-ef"

**1. Clone the repository**
  
**2. Configure the connection string**
   Open appsettings.json in the project and chage the connection string to mtch your SQLServer instance.
   
**3. Database Migration & Seeding:**
   run these commands in your terminal to create the database and populate initial data: "dotnet ef database update"

**4. Run the application: "dotnet run"**
