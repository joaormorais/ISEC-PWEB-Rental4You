# ISEC-PWEB-Rental4You

I have been given a project to develop a web application using ASP.NET Core, C#, and SQL Server. 

The purpose of this application is to create a SaaS (Software as a Service) platform that allows car rental companies to make their vehicles available for reservation through a common web platform in a convenient and efficient manner.

# Features:

There are five types of users:

● Unauthenticated user;

● Customer;

● Employee;

● Manager;

● Administrator.




The platform is divided into five distinct areas:

● Public area - unauthenticated users;

● Customer area - registered users with a customer profile;

● Employee area of a rental company - registered users with an employee profile and associated to one or more companies;

● Manager area - registered users with a manager profile and associated to one company;

● Administrator area - administrator of the platform.


The separation of the features per areas was implemented to accommodate the different types of users and their permissions within the application. Users with different profiles will have different levels of access and functionality within the platform.




● Public area - (Anonymous user):

○ Search for vehicles by location, vehicle type, and date (pick-up date and return date).

○ The search result should be a list of available vehicles, indicating the cost and the rental company it belongs to, as well as the company's rating.

○ It should be possible to filter the results by vehicle category and by company.

○ It should be possible to sort the results by lowest/highest price and/or by company rating.

○ Register as a Customer.




● Customer area (Customer):

○ Perform a search (similar to anonymous user)

○ Make a vehicle reservation;

○ Consult the reservation history;

○ Consult the details of a reservation;




● Employee area of a rental company (Employee):

○ Manage the company's fleet of vehicles:
  ■ List vehicles - with filters (category, state) and with sorting.
  ■ Add vehicles;
  ■ Edit vehicles;
  ■ Delete vehicles (only if there are no reservations for that vehicle);
  ■ Activate/deactivate vehicles;
  
○ Manage the company's reservations:
  ■ List reservations - with filters (pick-up date, return date, category,
vehicle, customer);
  ■ Confirm a reservation;
  ■ Reject a reservation;
  ■ Deliver a vehicle to the customer (customer vehicle pick-up)
  
○ Mark the state in which the vehicle is delivered to the customer

○ Number of vehicle kilometers

○ Vehicle damage (Y/N)

○ Notes

○ Employee who made the delivery
■ Receive a vehicle from the customer (customer vehicle return)
● Mark the state in which the vehicle is delivered by the customer
○ Number of vehicle kilometers
○ Vehicle damage (Y/N)
○ If "Yes" is marked, photographs must be attached
that prove the damage.
○ Employee who received the vehicle.
○ Notes.


Manager area:




Administrator area:





