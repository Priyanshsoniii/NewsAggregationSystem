# News Aggregation Server

A robust .NET 8 Web API server application that aggregates news from multiple external sources, provides user authentication, and delivers personalized news content with advanced features for both users and administrators.

## 🏗️ Architecture Overview

The application follows **Clean Architecture** principles with a **layered architecture** design:

```
NewsAggregation.Server/
├── Controllers/          # API endpoints (Presentation Layer)
├── Services/            # Business logic (Application Layer)
├── Repository/          # Data access (Infrastructure Layer)
├── Models/              # Data models and DTOs
├── Data/                # Database context and migrations
├── BackgroundServices/  # Background tasks
├── Configuration/       # Application settings
├── Exceptions/          # Custom exception handling
└── Utilities/           # Helper classes
```

### Key Architectural Principles

- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Clean Code**: DRY principle, meaningful naming, proper separation of concerns
- **RESTful APIs**: Standard HTTP methods (GET, POST, PUT, DELETE) with proper status codes
- **Dependency Injection**: IoC container for loose coupling
- **Repository Pattern**: Abstraction layer for data access
- **Service Layer**: Business logic encapsulation

## 🚀 Features

### Core Functionality
- **Multi-Source News Aggregation**: Fetches news from NewsAPI, The News API, and BBC RSS
- **Background Processing**: Automated news collection every 4 hours
- **User Authentication**: JWT-based authentication with role-based access
- **Email Notifications**: SMTP-based email delivery for user notifications
- **Personalized Recommendations**: AI-driven content recommendations based on user behavior

### User Features
- **News Browsing**: View headlines by category, date range, and search
- **Article Management**: Save, like, dislike, and mark articles as read
- **Personalization**: Configure notification preferences and keywords
- **Search & Filter**: Advanced search with date range and sorting options
- **Article Reporting**: Flag inappropriate content

### Admin Features
- **External Server Management**: Monitor and configure news sources
- **Content Moderation**: Hide articles and categories based on reports
- **Category Management**: Create and manage news categories
- **Keyword Filtering**: Filter out content based on specific keywords
- **System Monitoring**: Track server status and performance

## 🛠️ Technology Stack

- **Framework**: .NET 8.0
- **Database**: SQL Server (Relational Database)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer Tokens
- **Email**: MailKit with SMTP
- **HTTP Client**: Built-in HttpClient with Polly for resilience
- **Background Services**: Hosted Services with dependency injection
- **API Documentation**: Swagger/OpenAPI
- **Testing**: xUnit with Moq for mocking
- **Password Hashing**: BCrypt.Net-Next
- **JSON Processing**: System.Text.Json

### Why SQL Server (Relational Database)?

The application uses SQL Server for the following reasons:

1. **ACID Compliance**: Ensures data integrity for user accounts, saved articles, and financial transactions
2. **Complex Relationships**: News articles have multiple relationships (categories, users, likes, reports)
3. **Data Consistency**: Foreign key constraints prevent orphaned records
4. **Transaction Support**: Critical for operations like user registration and article saving
5. **Mature Ecosystem**: Excellent tooling, monitoring, and backup solutions
6. **Performance**: Optimized for read-heavy workloads with proper indexing

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (Local or Azure)
- SMTP Server (Gmail, SendGrid, etc.)
- API Keys for external news sources:
  - [NewsAPI](https://newsapi.org/)
  - [The News API](https://www.thenewsapi.com/)

## 🔧 Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd NewsAggregation.Server
```

### 2. Database Setup
```bash
# Update connection string in appsettings.json
# Run Entity Framework migrations
dotnet ef database update
```

### 3. Configuration
Update `appsettings.json` with your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=NewsAggregationDB;Trusted_Connection=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-here-make-it-long-and-complex",
    "Issuer": "NewsAggregationSystem",
    "Audience": "NewsAggregationClients",
    "ExpirationHours": 24
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "News Aggregation System"
  },
  "NewsApiSettings": {
    "NewsApiKey": "your-news-api-key",
    "TheNewsApiKey": "your-the-news-api-key",
    "FirebaseApiKey": "your-firebase-api-key",
    "FetchIntervalHours": 3
  }
}
```

### 4. Run the Application
```bash
dotnet run
```

The application will be available at:
- **API**: https://localhost:7000/api
- **Swagger UI**: https://localhost:7000/swagger

## 🔐 Authentication & Authorization

### User Roles
- **Admin**: Full system access, content moderation, server management
- **User**: News browsing, personalization, article interactions

### JWT Token Structure
```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "role": "Admin|User",
  "exp": 1640995200,
  "iss": "NewsAggregationSystem",
  "aud": "NewsAggregationClients"
}
```

## 📚 API Documentation

### Authentication Endpoints

#### POST /api/Auth/login
User login with email and password.
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

#### POST /api/Auth/register
User registration with validation.
```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

### News Endpoints

#### GET /api/News/headlines
Retrieve all headlines with pagination and filtering.

#### GET /api/News/headlines/today
Get today's headlines.

#### GET /api/News/headlines/category/{categoryId}
Get headlines by category.

#### GET /api/News/search?query={searchTerm}
Search articles with optional date range filtering.

#### GET /api/News/saved
Get user's saved articles.

#### POST /api/News/saved
Save an article to user's collection.

#### POST /api/News/{id}/like
Like an article.

#### POST /api/News/{id}/unlike
Unlike an article.

#### POST /api/News/{id}/read
Mark article as read.

#### GET /api/News/recommendations
Get personalized article recommendations.

### Admin Endpoints

#### GET /api/Admin/servers
List all external news servers with status.

#### PUT /api/Admin/servers/{id}
Update external server configuration.

#### POST /api/Admin/categories
Create new news category.

#### GET /api/Admin/reported-articles
Get articles reported by users.

#### PATCH /api/Admin/articles/{id}/hide
Hide/show reported articles.

#### POST /api/Admin/filtered-keywords
Add keywords for content filtering.

### User Management

#### GET /api/Users/profile
Get current user profile.

#### PUT /api/Users/profile
Update user profile.

### Notifications

#### GET /api/Notifications
Get user notifications.

#### POST /api/Notifications/configure
Configure notification preferences.

#### POST /api/Notifications/test-email
Send test email notification.

## 🔄 Background Services

### News Aggregation Service
- **Frequency**: Every 4 hours
- **Purpose**: Fetch latest news from external APIs
- **Features**: 
  - Multi-threaded API calls
  - Duplicate detection
  - Error handling and retry logic
  - Category identification for uncategorized articles

### Email Service
- **Purpose**: Send notifications and welcome emails
- **Features**:
  - SMTP configuration
  - HTML email templates
  - Batch processing
  - Error logging

## 🧪 Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~AuthServiceTests"
```

### Test Coverage
- **Unit Tests**: 90%+ coverage across all services
- **Integration Tests**: Database and external API testing
- **Mock Testing**: External dependencies mocked with Moq

## 🔒 Security Features

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control (RBAC)
- Password hashing with BCrypt
- Token expiration and refresh

### Data Protection
- Input validation and sanitization
- SQL injection prevention (EF Core)
- XSS protection
- CORS configuration

### Content Moderation
- User reporting system
- Automatic content hiding based on report thresholds
- Admin content moderation tools
- Keyword-based filtering

## 📊 Performance & Scalability

### Database Optimization
- Proper indexing on frequently queried columns
- Connection pooling
- Query optimization with EF Core
- Database migration management

### Caching Strategy
- In-memory caching for frequently accessed data
- Response caching for static content
- Background service optimization

### Multi-threading
- Parallel API calls to external services
- Background service with cancellation token support
- Async/await patterns throughout the application

## 🚨 Error Handling

### Custom Exceptions
- `NewsApiException`: External API errors
- `TheNewsApiException`: The News API specific errors
- `BbcRssException`: RSS feed errors

### Centralized Exception Handling
- Global exception middleware
- Structured error responses
- Comprehensive logging
- Graceful degradation

## 📈 Monitoring & Logging

### Logging
- Structured logging with Serilog
- Different log levels (Debug, Info, Warning, Error)
- Performance metrics logging
- External API call logging

### Health Checks
- Database connectivity
- External API availability
- Background service status
- Email service health

## 🔧 Configuration Management

### Environment-Specific Settings
- `appsettings.json`: Default configuration
- `appsettings.Development.json`: Development overrides
- Environment variables support
- Secure configuration storage

### External API Configuration
- API key management
- Rate limiting configuration
- Retry policies
- Timeout settings

## 🚀 Deployment

### Docker Support
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/NewsAggregation.Server.dll"]
```

### Azure Deployment
- Azure App Service
- Azure SQL Database
- Azure Key Vault for secrets
- Application Insights for monitoring

## 🤝 Contributing

### Code Standards
- Follow SOLID principles
- Implement clean code practices
- Add unit tests for new features
- Update API documentation

### Development Workflow
1. Create feature branch
2. Implement changes with tests
3. Update documentation
4. Submit pull request
5. Code review and merge

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Check the API documentation at `/swagger`
- Review the unit tests for usage examples

## 🔄 Version History

### v1.0.0
- Initial release with core functionality
- Multi-source news aggregation
- User authentication and authorization
- Admin content moderation tools
- Personalized recommendations
- Email notifications
- Comprehensive unit testing 