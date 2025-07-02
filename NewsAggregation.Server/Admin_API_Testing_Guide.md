# Admin API Testing Guide

## Overview
This guide provides testing instructions for the Admin menu backend functionality that has been implemented.

## Prerequisites
1. Ensure the server is running on `http://localhost:5078`
2. You need an Admin user account (use the seeded admin account)
3. Get a valid JWT token by logging in as admin

## Admin User Login
First, login as admin to get the JWT token:

**POST** `http://localhost:5078/api/AuthManagement/login`
```json
{
  "username": "admin",
  "password": "admin123"
}
```

Copy the `token` from the response for use in subsequent requests.

## Admin API Endpoints

### 1. View External Servers and Status
**GET** `http://localhost:5078/api/Admin/servers`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Expected Response:**
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
      "lastAccessed": "21 Mar 2025",
      "apiUrl": "https://newsapi.org/v2/top-headlines",
      "requestsPerHour": 1000,
      "currentHourRequests": 0,
      "createdAt": "2025-01-19T08:01:13.000Z"
    }
  ]
}
```

### 2. View External Server Details
**GET** `http://localhost:5078/api/Admin/servers/{id}`

**Example:** `http://localhost:5078/api/Admin/servers/1`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Expected Response:**
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
    "lastAccessed": "2025-01-19T08:01:13.000Z",
    "requestsPerHour": 1000,
    "currentHourRequests": 0,
    "lastHourReset": "2025-01-19T08:01:13.000Z",
    "createdAt": "2025-01-19T08:01:13.000Z"
  }
}
```

### 3. Update External Server Details
**PUT** `http://localhost:5078/api/Admin/servers/{id}`

**Example:** `http://localhost:5078/api/Admin/servers/1`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Updated NewsAPI",
  "apiKey": "NEW_API_KEY_HERE",
  "isActive": true,
  "requestsPerHour": 1500
}
```

**Expected Response:**
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

### 4. Create New External Server
**POST** `http://localhost:5078/api/Admin/servers`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

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

**Expected Response:**
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

### 5. Toggle Server Status
**PATCH** `http://localhost:5078/api/Admin/servers/{id}/toggle-status`

**Example:** `http://localhost:5078/api/Admin/servers/1`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Expected Response:**
```json
{
  "message": "Server status toggled successfully. New status: Inactive",
  "isActive": false
}
```

### 6. Delete Server
**DELETE** `http://localhost:5078/api/Admin/servers/{id}`

**Example:** `http://localhost:5078/api/Admin/servers/4`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Expected Response:**
```json
{
  "message": "Server deleted successfully"
}
```

### 7. Add New News Category
**POST** `http://localhost:5078/api/Admin/categories`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Science",
  "description": "Science and research news"
}
```

**Expected Response:**
```json
{
  "message": "Category created successfully",
  "category": {
    "id": 6,
    "name": "Science",
    "description": "Science and research news",
    "isActive": true,
    "createdAt": "2025-01-19T10:30:00.000Z"
  }
}
```

### 8. View Category Details
**GET** `http://localhost:5078/api/Admin/categories/{id}`

**Example:** `http://localhost:5078/api/Admin/categories/6`

**Headers:**
```
Authorization: Bearer <your_jwt_token>
```

**Expected Response:**
```json
{
  "message": "Category retrieved successfully",
  "category": {
    "id": 6,
    "name": "Science",
    "description": "Science and research news",
    "isActive": true,
    "createdAt": "2025-01-19T10:30:00.000Z"
  }
}
```

## Testing Tools
You can use any of these tools to test the APIs:
1. **Swagger UI**: `http://localhost:5078/swagger`
2. **Postman**
3. **curl** commands
4. **Thunder Client** (VS Code extension)

## Error Handling
The API includes proper error handling for:
- Invalid JWT tokens (401 Unauthorized)
- Missing admin role (403 Forbidden)
- Server not found (404 Not Found)
- Invalid request data (400 Bad Request)
- Server errors (500 Internal Server Error)

## Notes
- All endpoints require Admin role authentication
- API keys are masked in responses for security
- Soft delete is used for servers (sets IsActive to false)
- All timestamps are in UTC 