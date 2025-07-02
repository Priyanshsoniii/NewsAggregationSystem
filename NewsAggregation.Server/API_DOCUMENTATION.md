# News Aggregation API Documentation

## Overview
The News Aggregation API provides a comprehensive set of endpoints for managing news articles, user authentication, notifications, and administrative functions. The API follows RESTful principles and uses JWT authentication.

## Base URL
```
https://localhost:7000/api
```

## Authentication
All protected endpoints require a JWT token in the Authorization header:
```
Authorization: Bearer <your_jwt_token>
```

## API Endpoints

### Authentication

#### POST /api/Auth/login
**Description:** Authenticate user and get JWT token

**Request Body:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "role": "Admin",
  "message": "Login successful"
}
```

#### POST /api/Auth/register
**Description:** Register a new user

**Request Body:**
```json
{
  "username": "newuser",
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully"
}
```

### News Articles

#### GET /api/News/headlines
**Description:** Get all news headlines (filtered)

**Response:**
```json
{
  "success": true,
  "count": 10,
  "articles": [
    {
      "id": 1,
      "title": "Breaking News",
      "description": "Article description",
      "url": "https://example.com/article",
      "source": "News Source",
      "publishedAt": "2025-01-19T10:00:00Z",
      "imageUrl": "https://example.com/image.jpg",
      "author": "John Doe",
      "likes": 15,
      "dislikes": 2,
      "categoryName": "Business",
      "categoryId": 1
    }
  ]
}
```

#### GET /api/News/category/{categoryId}/headlines
**Description:** Get headlines by category

**Response:** Same as headlines endpoint

#### GET /api/News/today
**Description:** Get today's headlines

**Response:** Same as headlines endpoint

#### GET /api/News/search
**Description:** Search news articles

**Query Parameters:**
- `query` (required): Search term
- `startDate` (optional): Start date filter
- `endDate` (optional): End date filter

**Response:** Same as headlines endpoint

#### GET /api/News/recommendations
**Description:** Get personalized article recommendations

**Query Parameters:**
- `count` (optional): Number of recommendations (default: 10)

**Response:** Same as headlines endpoint

#### GET /api/News/user-preferences
**Description:** Get user preferences and behavior data

**Response:**
```json
{
  "success": true,
  "preferences": {
    "likedArticlesCount": 5,
    "savedArticlesCount": 3,
    "readArticlesCount": 12,
    "notificationSettings": [
      {
        "categoryId": 1,
        "categoryName": "Business",
        "isEnabled": true,
        "emailNotifications": true,
        "keywords": "[\"finance\", \"economy\"]"
      }
    ],
    "userKeywords": ["tech", "AI", "business"],
    "preferredCategories": [
      {
        "category": "Technology",
        "count": 8
      }
    ]
  }
}
```

#### GET /api/News/category/{categoryId}/personalized
**Description:** Get personalized articles by category

**Query Parameters:**
- `count` (optional): Number of articles (default: 10)

**Response:** Same as headlines endpoint

#### POST /api/News/{id}/like
**Description:** Like an article

**Response:**
```json
{
  "success": true,
  "message": "Article liked successfully"
}
```

#### POST /api/News/{id}/read
**Description:** Mark article as read

**Response:**
```json
{
  "success": true,
  "message": "Article marked as read"
}
```

#### POST /api/News/save
**Description:** Save an article

**Request Body:**
```json
{
  "articleId": 1
}
```

**Response:**
```json
{
  "success": true,
  "message": "Article saved successfully"
}
```

#### DELETE /api/News/saved/{id}
**Description:** Remove saved article

**Response:**
```json
{
  "success": true,
  "message": "Article removed from saved list"
}
```

#### GET /api/News/saved
**Description:** Get user's saved articles

**Response:** Same as headlines endpoint

#### POST /api/News/report
**Description:** Report an article

**Request Body:**
```json
{
  "articleId": 1,
  "reason": "Inappropriate content"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Article reported successfully"
}
```

### Notifications

#### GET /api/Notification
**Description:** Get user notifications

**Response:**
```json
{
  "success": true,
  "count": 5,
  "notifications": [
    {
      "id": 1,
      "title": "New Article",
      "message": "A new article matching your keywords",
      "isRead": false,
      "notificationType": "Article",
      "relatedArticleId": 1,
      "createdAt": "2025-01-19T10:00:00Z"
    }
  ]
}
```

#### GET /api/Notification/unread
**Description:** Get unread notifications

**Response:** Same as notifications endpoint

#### PATCH /api/Notification/{id}/mark-read
**Description:** Mark notification as read

**Response:**
```json
{
  "success": true,
  "message": "Notification marked as read"
}
```

#### PATCH /api/Notification/mark-all-read
**Description:** Mark all notifications as read

**Response:**
```json
{
  "success": true,
  "message": "All notifications marked as read"
}
```

#### DELETE /api/Notification/{id}
**Description:** Delete notification

**Response:**
```json
{
  "success": true,
  "message": "Notification deleted successfully"
}
```

#### GET /api/Notification/settings
**Description:** Get notification settings

**Response:**
```json
{
  "success": true,
  "settings": [
    {
      "categoryId": 1,
      "categoryName": "Business",
      "isEnabled": true,
      "keywords": "[\"finance\"]",
      "emailNotifications": true
    }
  ]
}
```

#### PATCH /api/Notification/settings/{categoryId}/toggle
**Description:** Toggle notification setting

**Response:**
```json
{
  "success": true,
  "isEnabled": true,
  "message": "Notifications enabled successfully"
}
```

#### PUT /api/Notification/settings/{categoryId}/keywords
**Description:** Update notification keywords

**Request Body:**
```json
{
  "keywords": ["tech", "AI", "machine learning"]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Keywords updated successfully"
}
```

#### POST /api/Notification/test-email
**Description:** Send test email notification

**Response:**
```json
{
  "success": true,
  "message": "Test email sent successfully."
}
```

### Admin Endpoints

#### GET /api/Admin/servers
**Description:** Get external servers and status

**Response:**
```json
{
  "message": "External servers retrieved successfully",
  "count": 3,
  "servers": [
    {
      "id": 1,
      "name": "NewsAPI",
      "serverType": "NewsAPI",
      "status": "Active",
      "lastAccessed": "2025-01-19T10:00:00Z",
      "apiUrl": "https://newsapi.org/v2/top-headlines",
      "requestsPerHour": 1000,
      "currentHourRequests": 0
    }
  ]
}
```

#### GET /api/Admin/servers/{id}
**Description:** Get external server details

**Response:**
```json
{
  "message": "Server details retrieved successfully",
  "server": {
    "id": 1,
    "name": "NewsAPI",
    "serverType": "NewsAPI",
    "apiUrl": "https://newsapi.org/v2/top-headlines",
    "apiKey": "***_KEY",
    "isActive": true,
    "lastAccessed": "2025-01-19T10:00:00Z",
    "requestsPerHour": 1000,
    "currentHourRequests": 0
  }
}
```

#### PUT /api/Admin/servers/{id}
**Description:** Update external server

**Request Body:**
```json
{
  "name": "Updated NewsAPI",
  "apiKey": "NEW_API_KEY",
  "isActive": true,
  "requestsPerHour": 1500
}
```

**Response:**
```json
{
  "message": "Server updated successfully",
  "server": {
    "id": 1,
    "name": "Updated NewsAPI",
    "serverType": "NewsAPI",
    "apiUrl": "https://newsapi.org/v2/top-headlines",
    "isActive": true,
    "requestsPerHour": 1500
  }
}
```

#### POST /api/Admin/servers
**Description:** Create new external server

**Request Body:**
```json
{
  "name": "New News Source",
  "apiUrl": "https://api.newsource.com/v1/news",
  "apiKey": "API_KEY_HERE",
  "serverType": "CustomAPI",
  "isActive": true,
  "requestsPerHour": 500
}
```

**Response:**
```json
{
  "message": "Server created successfully",
  "server": {
    "id": 4,
    "name": "New News Source",
    "serverType": "CustomAPI",
    "apiUrl": "https://api.newsource.com/v1/news",
    "isActive": true,
    "requestsPerHour": 500
  }
}
```

#### PATCH /api/Admin/servers/{id}/toggle-status
**Description:** Toggle server status

**Response:**
```json
{
  "message": "Server status toggled successfully. New status: Inactive",
  "isActive": false
}
```

#### DELETE /api/Admin/servers/{id}
**Description:** Delete server

**Response:**
```json
{
  "message": "Server deleted successfully"
}
```

#### POST /api/Admin/categories
**Description:** Create new category

**Request Body:**
```json
{
  "name": "Science",
  "description": "Science and research news"
}
```

**Response:**
```json
{
  "message": "Category created successfully",
  "category": {
    "id": 6,
    "name": "Science",
    "description": "Science and research news",
    "isActive": true,
    "createdAt": "2025-01-19T10:30:00Z"
  }
}
```

#### GET /api/Admin/reported-articles
**Description:** Get reported articles

**Response:**
```json
{
  "success": true,
  "count": 2,
  "reportedArticles": [
    {
      "id": 1,
      "title": "Reported Article",
      "reportCount": 3,
      "isHidden": true
    }
  ]
}
```

#### PATCH /api/Admin/articles/{id}/hide
**Description:** Hide/unhide article

**Query Parameters:**
- `hide` (optional): true to hide, false to unhide (default: true)

**Response:**
```json
{
  "success": true,
  "message": "Article hidden successfully"
}
```

#### PATCH /api/Admin/categories/{id}/hide
**Description:** Hide/unhide category

**Query Parameters:**
- `hide` (optional): true to hide, false to unhide (default: true)

**Response:**
```json
{
  "success": true,
  "message": "Category hidden successfully"
}
```

#### GET /api/Admin/filtered-keywords
**Description:** Get filtered keywords

**Response:**
```json
{
  "success": true,
  "count": 2,
  "keywords": [
    {
      "id": 1,
      "keyword": "spam",
      "createdAt": "2025-01-19T10:00:00Z"
    }
  ]
}
```

#### POST /api/Admin/filtered-keywords
**Description:** Add filtered keyword

**Request Body:**
```json
"spam"
```

**Response:**
```json
{
  "success": true,
  "keyword": {
    "id": 1,
    "keyword": "spam",
    "createdAt": "2025-01-19T10:00:00Z"
  }
}
```

#### DELETE /api/Admin/filtered-keywords/{id}
**Description:** Delete filtered keyword

**Response:**
```json
{
  "success": true,
  "message": "Keyword deleted successfully"
}
```

### External News

#### GET /api/ExternalNews/latest
**Description:** Fetch latest news from external sources

**Response:**
```json
{
  "success": true,
  "count": 15,
  "message": "Successfully imported 15 articles to database"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "message": "Invalid request data",
  "errors": {
    "field": ["Error message"]
  }
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid token"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied. Admin role required."
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred while processing the request"
}
```

## Rate Limiting
- API endpoints are subject to rate limiting
- External server requests are limited per hour
- Background services run every 30 minutes

## Database Schema
The API uses a relational database with the following main entities:
- Users (authentication and profiles)
- Categories (news categories)
- NewsArticles (news content)
- SavedArticles (user saved articles)
- Notifications (user notifications)
- Reports (article reports)
- ExternalServers (external API configurations)
- FilteredKeywords (admin filtered content)

## Testing
The API includes comprehensive unit and integration tests covering:
- Service layer functionality
- Controller endpoints
- Authentication and authorization
- Database operations
- Error handling

## Deployment
The API is designed to run on .NET 8.0 with SQL Server database and includes:
- Docker support
- Configuration management
- Logging and monitoring
- Health checks
- Swagger documentation 