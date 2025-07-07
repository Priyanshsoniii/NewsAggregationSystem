# News Aggregation Server - Use Case Specifications

This document provides detailed specifications for all use cases in the News Aggregation Server system.

## 📋 Use Case Categories

### 1. Authentication & User Management
### 2. News Browsing & Search
### 3. User Interactions
### 4. Personalization
### 5. Admin Management
### 6. Background Services
### 7. Content Moderation

---

## 🔐 Authentication & User Management

### UC1: Login
**Actor**: User, Admin  
**Preconditions**: User has a registered account  
**Main Flow**:
1. User enters email/username and password
2. System validates credentials
3. System generates JWT token
4. System returns token and user information
5. User is authenticated and can access system features

**Alternative Flows**:
- A1: Invalid credentials → Return error message
- A2: Account inactive → Return account disabled message
- A3: Account locked → Return account locked message

**Postconditions**: User is logged in with appropriate role-based access

### UC2: Register
**Actor**: User  
**Preconditions**: User does not have an existing account  
**Main Flow**:
1. User provides username, email, and password
2. System validates email format
3. System checks for existing username/email
4. System hashes password
5. System creates user account with "User" role
6. System sends welcome email
7. System returns success message

**Alternative Flows**:
- A1: Invalid email format → Return validation error
- A2: Username/email already exists → Return conflict error
- A3: Weak password → Return password requirements error

**Postconditions**: New user account is created and user can login

### UC3: Update Profile
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User requests profile update
2. User provides new profile information
3. System validates input data
4. System updates user profile
5. System returns updated profile

**Alternative Flows**:
- A1: Invalid data → Return validation error
- A2: Email already in use → Return conflict error

**Postconditions**: User profile is updated

### UC4: Validate Email
**Actor**: System  
**Preconditions**: User registration in progress  
**Main Flow**:
1. System receives email address
2. System checks email format using regex
3. System verifies email domain
4. System returns validation result

**Alternative Flows**:
- A1: Invalid format → Return false
- A2: Invalid domain → Return false

**Postconditions**: Email validation result is determined

### UC5: Manage User Accounts
**Actor**: Admin  
**Preconditions**: Admin is logged in  
**Main Flow**:
1. Admin views list of all users
2. Admin can view user details
3. Admin can activate/deactivate users
4. Admin can change user roles
5. System updates user status

**Alternative Flows**:
- A1: User not found → Return error
- A2: Invalid role → Return validation error

**Postconditions**: User account status is updated

---

## 📰 News Browsing & Search

### UC6: View Headlines
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User requests headlines
2. System retrieves non-hidden articles
3. System applies user preferences and filters
4. System returns paginated results
5. User views headlines with metadata

**Alternative Flows**:
- A1: No articles available → Return empty list
- A2: Filtered content → Apply keyword/content filters

**Postconditions**: User can view and interact with headlines

### UC7: View Today's News
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User requests today's news
2. System filters articles by current date
3. System applies visibility and moderation filters
4. System returns today's articles
5. User views today's news

**Alternative Flows**:
- A1: No articles today → Return empty list

**Postconditions**: User views today's news articles

### UC8: View News by Category
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User selects category
2. System validates category exists and is active
3. System retrieves articles for selected category
4. System applies filters and pagination
5. User views category-specific news

**Alternative Flows**:
- A1: Category not found → Return error
- A2: Category hidden → Return access denied

**Postconditions**: User views category-specific articles

### UC9: View News by Date Range
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User specifies start and end dates
2. System validates date range
3. System retrieves articles within date range
4. System applies filters and sorting
5. User views date-filtered news

**Alternative Flows**:
- A1: Invalid date range → Return error
- A2: No articles in range → Return empty list

**Postconditions**: User views date-range filtered articles

### UC10: Search News
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User enters search query
2. User optionally specifies date range
3. System performs full-text search
4. System applies relevance scoring
5. System returns search results
6. User views search results

**Alternative Flows**:
- A1: No results found → Return empty list
- A2: Query too short → Return validation error

**Postconditions**: User views search results

### UC11: View Article Details
**Actor**: User, Admin  
**Preconditions**: User is logged in  
**Main Flow**:
1. User selects article
2. System retrieves full article details
3. System records article read (for personalization)
4. System returns article with engagement options
5. User views complete article

**Alternative Flows**:
- A1: Article not found → Return 404 error
- A2: Article hidden → Return access denied

**Postconditions**: User views article details and read is recorded

---

## 👤 User Interactions

### UC12: Save Article
**Actor**: User  
**Preconditions**: User is logged in, article exists  
**Main Flow**:
1. User requests to save article
2. System checks if already saved
3. System creates saved article record
4. System returns success confirmation
5. Article appears in user's saved list

**Alternative Flows**:
- A1: Already saved → Return already saved message
- A2: Article not found → Return error

**Postconditions**: Article is saved to user's collection

### UC13: View Saved Articles
**Actor**: User  
**Preconditions**: User is logged in  
**Main Flow**:
1. User requests saved articles
2. System retrieves user's saved articles
3. System returns saved articles list
4. User views saved articles

**Alternative Flows**:
- A1: No saved articles → Return empty list

**Postconditions**: User views saved articles

### UC14: Delete Saved Article
**Actor**: User  
**Preconditions**: User is logged in, article is saved  
**Main Flow**:
1. User requests to delete saved article
2. System removes saved article record
3. System returns success confirmation
4. Article removed from saved list

**Alternative Flows**:
- A1: Article not in saved list → Return error

**Postconditions**: Article is removed from saved collection

### UC15: Like Article
**Actor**: User  
**Preconditions**: User is logged in, article exists  
**Main Flow**:
1. User requests to like article
2. System checks if already liked
3. System creates like record
4. System increments article like count
5. System updates recommendations
6. System returns success

**Alternative Flows**:
- A1: Already liked → Return already liked message

**Postconditions**: Article is liked and count updated

### UC16: Unlike Article
**Actor**: User  
**Preconditions**: User is logged in, article is liked  
**Main Flow**:
1. User requests to unlike article
2. System removes like record
3. System decrements article like count
4. System updates recommendations
5. System returns success

**Alternative Flows**:
- A1: Not liked → Return not liked message

**Postconditions**: Article is unliked and count updated

### UC17: Mark Article as Read
**Actor**: User  
**Preconditions**: User is logged in, article exists  
**Main Flow**:
1. User views article (automatic)
2. System creates read record
3. System updates reading history
4. System updates recommendations
5. System returns success

**Alternative Flows**:
- A1: Already read → Update read timestamp

**Postconditions**: Article is marked as read

### UC18: Report Article
**Actor**: User  
**Preconditions**: User is logged in, article exists  
**Main Flow**:
1. User reports article with reason
2. System creates report record
3. System increments article report count
4. System checks report threshold
5. System notifies admin if threshold exceeded
6. System returns success

**Alternative Flows**:
- A1: Already reported → Return already reported message
- A2: Threshold exceeded → Auto-hide article

**Postconditions**: Article is reported and moderation triggered

---

## 🎯 Personalization

### UC19: Get Personalized Recommendations
**Actor**: User  
**Preconditions**: User is logged in  
**Main Flow**:
1. System analyzes user behavior
2. System calculates user preferences
3. System generates personalized recommendations
4. System returns recommended articles
5. User views personalized content

**Alternative Flows**:
- A1: No user history → Return popular articles
- A2: Insufficient data → Return category-based recommendations

**Postconditions**: User receives personalized recommendations

### UC20: Configure Notification Settings
**Actor**: User  
**Preconditions**: User is logged in  
**Main Flow**:
1. User accesses notification settings
2. User configures category preferences
3. User sets email notification preferences
4. System saves notification settings
5. System returns confirmation

**Alternative Flows**:
- A1: Invalid settings → Return validation error

**Postconditions**: Notification settings are updated

### UC21: Set Keywords for Notifications
**Actor**: User  
**Preconditions**: User is logged in  
**Main Flow**:
1. User enters keywords of interest
2. System validates keyword format
3. System saves keyword preferences
4. System monitors articles for keywords
5. System sends notifications when matches found

**Alternative Flows**:
- A1: Invalid keywords → Return validation error
- A2: Too many keywords → Return limit error

**Postconditions**: Keyword-based notifications are configured

### UC22: View Notifications
**Actor**: User  
**Preconditions**: User is logged in  
**Main Flow**:
1. User requests notifications
2. System retrieves user notifications
3. System returns notifications list
4. User views notifications

**Alternative Flows**:
- A1: No notifications → Return empty list

**Postconditions**: User views notifications

### UC23: Mark Notification as Read
**Actor**: User  
**Preconditions**: User is logged in, notification exists  
**Main Flow**:
1. User marks notification as read
2. System updates notification status
3. System returns success
4. Notification marked as read

**Alternative Flows**:
- A1: Already read → Return already read message

**Postconditions**: Notification is marked as read

---

## ⚙️ Admin Management

### UC24: View External Servers
**Actor**: Admin  
**Preconditions**: Admin is logged in  
**Main Flow**:
1. Admin requests server list
2. System retrieves all external servers
3. System includes status and metrics
4. System returns server information
5. Admin views server status

**Alternative Flows**:
- A1: No servers configured → Return empty list

**Postconditions**: Admin views server status

### UC25: Update Server Configuration
**Actor**: Admin  
**Preconditions**: Admin is logged in, server exists  
**Main Flow**:
1. Admin selects server to update
2. Admin provides new configuration
3. System validates configuration
4. System updates server settings
5. System returns confirmation

**Alternative Flows**:
- A1: Invalid configuration → Return validation error
- A2: Server not found → Return error

**Postconditions**: Server configuration is updated

### UC26: Create News Category
**Actor**: Admin  
**Preconditions**: Admin is logged in  
**Main Flow**:
1. Admin provides category details
2. System validates category name
3. System checks for duplicates
4. System creates new category
5. System returns confirmation

**Alternative Flows**:
- A1: Category already exists → Return conflict error
- A2: Invalid name → Return validation error

**Postconditions**: New category is created

### UC27: View Reported Articles
**Actor**: Admin  
**Preconditions**: Admin is logged in  
**Main Flow**:
1. Admin requests reported articles
2. System retrieves articles with reports
3. System includes report details
4. System returns reported articles
5. Admin reviews reported content

**Alternative Flows**:
- A1: No reported articles → Return empty list

**Postconditions**: Admin views reported articles

### UC28: Hide/Show Articles
**Actor**: Admin  
**Preconditions**: Admin is logged in, article exists  
**Main Flow**:
1. Admin selects article to moderate
2. Admin chooses hide/show action
3. System updates article visibility
4. System returns confirmation
5. Article visibility is changed

**Alternative Flows**:
- A1: Article not found → Return error

**Postconditions**: Article visibility is updated

### UC29: Hide/Show Categories
**Actor**: Admin  
**Preconditions**: Admin is logged in, category exists  
**Main Flow**:
1. Admin selects category to moderate
2. Admin chooses hide/show action
3. System updates category visibility
4. System returns confirmation
5. Category visibility is changed

**Alternative Flows**:
- A1: Category not found → Return error

**Postconditions**: Category visibility is updated

### UC30: Manage Filtered Keywords
**Actor**: Admin  
**Preconditions**: Admin is logged in  
**Main Flow**:
1. Admin adds/removes filtered keywords
2. System validates keyword format
3. System updates keyword list
4. System applies filters to content
5. System returns confirmation

**Alternative Flows**:
- A1: Invalid keyword → Return validation error
- A2: Keyword already exists → Return conflict error

**Postconditions**: Filtered keywords are updated

### UC31: View System Statistics
**Actor**: Admin  
**Preconditions**: Admin is logged in  
**Main Flow**:
1. Admin requests system statistics
2. System calculates various metrics
3. System returns statistics
4. Admin views system performance

**Alternative Flows**:
- A1: No data available → Return empty statistics

**Postconditions**: Admin views system statistics

---

## 🔄 Background Services

### UC32: Aggregate News from External Sources
**Actor**: System  
**Preconditions**: External APIs are configured  
**Main Flow**:
1. Background service triggers (every 4 hours)
2. System calls multiple external APIs
3. System processes and deduplicates articles
4. System categorizes uncategorized articles
5. System saves new articles to database
6. System logs aggregation results

**Alternative Flows**:
- A1: API unavailable → Log error and continue
- A2: Rate limit exceeded → Wait and retry
- A3: Invalid response → Skip and continue

**Postconditions**: New articles are aggregated and stored

### UC33: Send Email Notifications
**Actor**: System  
**Preconditions**: Email service is configured  
**Main Flow**:
1. System identifies notification events
2. System generates email content
3. System sends emails via SMTP
4. System logs email delivery
5. System updates notification status

**Alternative Flows**:
- A1: Email service unavailable → Queue for retry
- A2: Invalid email address → Skip and log

**Postconditions**: Email notifications are sent

### UC34: Process User Reports
**Actor**: System  
**Preconditions**: User reports exist  
**Main Flow**:
1. System monitors report counts
2. System checks report thresholds
3. System auto-hides articles if threshold exceeded
4. System notifies admin of actions
5. System logs moderation actions

**Alternative Flows**:
- A1: Threshold not reached → Continue monitoring
- A2: Article already hidden → Skip action

**Postconditions**: Reported content is moderated

### UC35: Generate Recommendations
**Actor**: System  
**Preconditions**: User interaction data exists  
**Main Flow**:
1. System analyzes user behavior patterns
2. System calculates content similarity
3. System generates personalized recommendations
4. System stores recommendation data
5. System serves recommendations to users

**Alternative Flows**:
- A1: Insufficient data → Use default recommendations
- A2: No similar content → Use popular articles

**Postconditions**: Personalized recommendations are generated

---

## 🛡️ Content Moderation

### UC36: Auto-hide Articles by Report Count
**Actor**: System  
**Preconditions**: Article has reports  
**Main Flow**:
1. System monitors article report count
2. System checks against threshold
3. System automatically hides article if threshold exceeded
4. System notifies admin
5. System logs moderation action

**Alternative Flows**:
- A1: Threshold not reached → Continue monitoring
- A2: Article already hidden → Skip action

**Postconditions**: Article is automatically hidden

### UC37: Filter Content by Keywords
**Actor**: System  
**Preconditions**: Filtered keywords are configured  
**Main Flow**:
1. System checks articles against filtered keywords
2. System hides articles containing filtered keywords
3. System logs filtering actions
4. System notifies admin of filtered content

**Alternative Flows**:
- A1: No keywords configured → Skip filtering
- A2: Article already hidden → Skip action

**Postconditions**: Content is filtered based on keywords

### UC38: Monitor System Health
**Actor**: System  
**Preconditions**: System is running  
**Main Flow**:
1. System checks database connectivity
2. System monitors external API health
3. System checks background service status
4. System logs health metrics
5. System alerts admin of issues

**Alternative Flows**:
- A1: Service unavailable → Send alert
- A2: Performance degraded → Log warning

**Postconditions**: System health is monitored and reported

---

## 📊 Use Case Summary

| Category | Use Cases | Primary Actors |
|----------|-----------|----------------|
| Authentication & User Management | 5 | User, Admin |
| News Browsing & Search | 6 | User, Admin |
| User Interactions | 7 | User |
| Personalization | 5 | User |
| Admin Management | 8 | Admin |
| Background Services | 4 | System |
| Content Moderation | 3 | System, Admin |

**Total Use Cases**: 38

This comprehensive use case specification provides detailed information for system development, testing, and documentation purposes. 