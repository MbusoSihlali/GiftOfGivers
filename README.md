# Gift of the Givers - Advanced MVC Application

An advanced ASP.NET Core MVC application for the Gift of the Givers Foundation, featuring modern architecture, database integration, and comprehensive functionality.

## 🏗️ Architecture

This project implements a modern MVC architecture with the following advanced features:

### **Layered Architecture**
- **Models**: Entity Framework Core models with data annotations
- **Data Access**: Repository pattern with generic and specific repositories
- **Controllers**: MVC controllers with proper separation of concerns
- **Views**: Razor views with clean HTML and Bootstrap integration
- **API Controllers**: RESTful API endpoints for external integrations
- **Middleware**: Custom middleware for logging and exception handling
- **Configuration**: Centralized configuration management

### **Key Features**
- ✅ **Entity Framework Core** with SQL Server database
- ✅ **ASP.NET Core Identity** for advanced authentication and authorization
- ✅ **Repository Pattern** for data access abstraction
- ✅ **Dependency Injection** for loose coupling
- ✅ **RESTful API** with full CRUD operations
- ✅ **Custom Middleware** for logging and error handling
- ✅ **Configuration Management** with strongly-typed settings
- ✅ **Unit Testing** with xUnit, Moq, and InMemory database
- ✅ **Role-Based Authorization** (Admin, Employee, Donor)
- ✅ **Database Seeding** for initial data setup

## 📁 Project Structure

```
GiftOfGivers/
├── Controllers/
│   ├── Api/                    # RESTful API controllers
│   │   ├── DonationsApiController.cs
│   │   ├── VolunteersApiController.cs
│   │   └── ProjectsApiController.cs
│   ├── AccountController.cs   # Authentication & user management
│   ├── DonateController.cs    # Donation processing
│   ├── EmployeeController.cs  # Employee dashboard & management
│   ├── HomeController.cs       # Public pages
│   └── VolunteerController.cs # Volunteer applications
├── Data/
│   └── ApplicationDbContext.cs  # EF Core database context
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Models/
│   ├── ApplicationUser.cs     # Identity user model
│   ├── Donation.cs
│   ├── Volunteer.cs
│   ├── Project.cs
│   ├── ApiDtos.cs             # API data transfer objects
│   └── ViewModels/            # View-specific models
├── Repositories/
│   ├── IRepository.cs          # Generic repository interface
│   ├── Repository.cs           # Generic repository implementation
│   ├── IDonationRepository.cs
│   ├── DonationRepository.cs
│   ├── IVolunteerRepository.cs
│   ├── VolunteerRepository.cs
│   ├── IProjectRepository.cs
│   └── ProjectRepository.cs
├── Configuration/
│   └── AppSettings.cs          # Configuration classes
├── Views/
│   ├── Home/
│   ├── Account/
│   ├── Donate/
│   ├── Volunteer/
│   ├── Employee/
│   └── Shared/
└── Program.cs                  # Application configuration
```

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (or LocalDB)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd GiftGivers/GiftOfGivers
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database connection**
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GiftOfGiversDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run database migrations**
   The application will automatically create the database on first run due to the seeding logic in `Program.cs`.

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - Public site: `https://localhost:5001`
   - Employee login: `employee@giftofthegivers.org` / `Password123!`
   - Admin login: `admin@giftofthegivers.org` / `Admin123!`

## 🔧 Configuration

### Application Settings
Configure application behavior in `appsettings.json`:

```json
{
  "Application": {
    "SiteName": "Gift of the Givers Foundation",
    "SiteUrl": "https://localhost:5001",
    "AdminEmail": "admin@giftofthegivers.org",
    "EnableRegistration": true,
    "EnableVolunteerApplications": true,
    "RecentDonationsCount": 5,
    "RecentVolunteersCount": 10
  },
  "Security": {
    "PasswordMinLength": 8,
    "PasswordRequireDigit": true,
    "PasswordRequireLowercase": true,
    "PasswordRequireUppercase": true,
    "PasswordRequireNonAlphanumeric": true,
    "MaxFailedAccessAttempts": 5,
    "DefaultLockoutTimeSpanMinutes": 5,
    "CookieExpirationHours": 2
  }
}
```

## 🌐 API Endpoints

### Donations API
- `GET /api/donations` - Get all donations
- `GET /api/donations/{id}` - Get specific donation
- `GET /api/donations/user/{userId}` - Get user donations
- `GET /api/donations/recent/{count}` - Get recent donations
- `GET /api/donations/statistics` - Get donation statistics
- `POST /api/donations` - Create new donation
- `PUT /api/donations/{id}` - Update donation (Admin/Employee)
- `DELETE /api/donations/{id}` - Delete donation (Admin)

### Volunteers API
- `GET /api/volunteers` - Get all volunteers
- `GET /api/volunteers/{id}` - Get specific volunteer
- `GET /api/volunteers/status/{status}` - Get volunteers by status
- `GET /api/volunteers/skill/{skill}` - Get volunteers by skill
- `GET /api/volunteers/location/{city}/{province}` - Get volunteers by location
- `GET /api/volunteers/statistics` - Get volunteer statistics
- `POST /api/volunteers` - Create volunteer application
- `PUT /api/volunteers/{id}` - Update volunteer (Admin/Employee)
- `PATCH /api/volunteers/{id}/status` - Update volunteer status (Admin/Employee)
- `DELETE /api/volunteers/{id}` - Delete volunteer (Admin)

### Projects API
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get specific project
- `GET /api/projects/active` - Get active projects
- `GET /api/projects/category/{category}` - Get projects by category
- `GET /api/projects/status/{status}` - Get projects by status (Admin/Employee)
- `GET /api/projects/statistics` - Get project statistics
- `POST /api/projects` - Create project (Admin/Employee)
- `PUT /api/projects/{id}` - Update project (Admin/Employee)
- `PATCH /api/projects/{id}/status` - Update project status (Admin/Employee)
- `DELETE /api/projects/{id}` - Delete project (Admin)

## 🧪 Testing

Run the unit tests:
```bash
cd GiftOfGivers.Tests
dotnet test
```

### Test Coverage
- Repository layer tests with InMemory database
- Controller tests with Moq mocking framework
- Service layer tests for business logic

## 🔐 Security Features

- **Password Strength**: Configurable password requirements
- **Account Lockout**: Configurable lockout policies
- **Role-Based Authorization**: Admin, Employee, Donor roles
- **Cookie Security**: HttpOnly cookies with configurable expiration
- **CSRF Protection**: Built-in anti-forgery token validation
- **Input Validation**: Data annotations on all models

## 📊 Database Schema

### Main Entities
- **Users**: Extended Identity user with profile information
- **Donations**: Financial contributions with tax receipt generation
- **Volunteers**: Volunteer applications with skills and availability
- **Projects**: Humanitarian projects with funding tracking

### Relationships
- Users → Donations (One-to-Many)
- Users → Volunteer Applications (One-to-Many)
- Projects → Donations (One-to-Many)

## 🎨 Frontend Features

- **Responsive Design**: Bootstrap 5 integration
- **Dynamic Navigation**: Role-based menu items
- **Real-time Updates**: AJAX for dashboard statistics
- **Form Validation**: Client-side and server-side validation
- **User-Friendly URLs**: Clean routing structure

## 🚀 Deployment

### Database Deployment
1. Update connection string for production database
2. Run migrations: `dotnet ef database update`
3. Configure environment-specific settings

### Application Deployment
1. Build the application: `dotnet publish -c Release`
2. Deploy to IIS, Azure, or other hosting platforms
3. Configure HTTPS and security headers
4. Set up logging and monitoring

## 📝 Development Notes

### Adding New Features
1. Create model in `Models/` folder
2. Add repository interface and implementation
3. Register repository in `Program.cs`
4. Create controller with appropriate actions
5. Create corresponding views
6. Add API controller if needed
7. Write unit tests

### Database Changes
1. Update model properties
2. Create migration: `dotnet ef migrations add MigrationName`
3. Apply migration: `dotnet ef database update`

## 🤝 Contributing

This project was developed by Mbuso Sihlali/Simphiwe Dladla/ Hlamulo Ndhlovu as a final-year project

## 📄 License

This project is for educational purposes and demonstrates modern ASP.NET Core development practices.

## 🙏 Acknowledgments

- Gift of the Givers Foundation for the inspiration
- Microsoft ASP.NET Core team for the excellent framework
- Bootstrap for the responsive UI components
