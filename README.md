# ISEC-PWEB-Rental4You

I have been given a project to develop a web application using ASP.NET Core, C#, and SQL Server. 

The purpose of this application is to create a SaaS (Software as a Service) platform that allows car rental companies to make their vehicles available for reservation through a common web platform in a convenient and efficient manner.

# Features that were proposed to be implemented:

![image](https://user-images.githubusercontent.com/72463113/211181217-8c0ac44f-3f76-471e-872a-8765b2c65f2b.png)
![image](https://user-images.githubusercontent.com/72463113/211181233-c291f21f-b1fc-46b5-b603-4fb56767efcf.png)


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

○ Register as a Customer;

○ Search for vehicles by location, vehicle type, and date (pick-up date and return date);




● Customer area (Customer):

○ Login and edit his account.

○ Perform a search (similar to anonymous user);

○ Make a vehicle reservation;

○ Consult his reservations;

○ Consult the details of a reservation;




● Employee area of a rental company (Employee):

○ Login and edit his account.

○ Manage the company's fleet of vehicles:
  ■ List vehicles - with filters (category, state) and with sorting.
  ■ Add vehicles;
  ■ Edit vehicles;
  ■ Delete vehicles (only if there are no reservations for that vehicle);
  ■ Activate/deactivate vehicles;
  
○ Manage the company's reservations:
  ■ List reservations - with filters (pick-up date, return date, category,
vehicle, customer);
  ■ Edit a reservation
  ■ Delete a reservation




● Manager area of a car rental company (Manager):

○ Employee management:
  ■ Create users with the employee profile, associated with their company.
  ■ Create users with the manager profile, associated with their company.
  ■ List employees.
  ■ Activate/deactivate employees.
  ■ Cannot delete nor deactivate their own user.
  
○ Reservation management (same as Employee profile).

○ Fleet management (same as Employee profile).




● Platform administrator area (Administrator)

○ Company management:
  ■ List companies - with filters (name, subscription state) and with sorting.
  ■ Add companies - automatically creates a user with the manager profile, associated with
the company.
  ■ Edit companies;
  ■ Delete companies (only if they do not have any vehicles yet)
  ■ Activate/deactivate companies.

○ Vehicle category management
  ■ List, create, edit, delete categories

○ User management
  ■ List, edit, activate, deactivate users

○ According to the previously mentioned Dashboards for Managers
  ■ Dashboard with information on reservation performance

○ Charts
  ■ Number of daily reservations in the past 30 days
  ■ Number of monthly reservations (last 12 months).
  ■ Number of new customers per month (last 12 months).

# Features that weren't developed :(

● It isn't possible to add images about the damages of the car to the details of a reservation
● It isn't possible to search vehicles filtered by dates
● It isn't possible to search for reservations with filters

# Known bugs

● None...

# My personal analysis of the project

I enjoyed a lot working on this project, however, It could have been way better, by far, if I had more time to work on it. Since I had more subjects with more projects to be done, I couldn't focus on 100% on this project. Taking this in consideration, for what is done, I think it is very good.
It couldn't have been done this good without the help of my team mate and teatchers.

# Final grade

To be evaluated...





