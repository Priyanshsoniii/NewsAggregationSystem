# News Aggregation Server - Unit Tests

This directory contains comprehensive unit tests for all services in the News Aggregation Server project.

## Test Structure

### Services Tests
- **AuthServiceTests.cs** - Tests for user authentication and registration
- **NewsServiceTests.cs** - Tests for news article management and recommendations
- **CategoryServiceTests.cs** - Tests for category CRUD operations
- **UserServiceTests.cs** - Tests for user management operations
- **EmailServiceTests.cs** - Tests for email sending functionality
- **NotificationServiceTests.cs** - Tests for notification management and settings
- **ExternalNewsServiceTests.cs** - Tests for external news aggregation
- **ExternalServerServiceTests.cs** - Tests for external server management

### External Clients Tests
- **NewsApiClientTests.cs** - Tests for NewsAPI integration
- **TheNewsApiClientTests.cs** - Tests for The News API integration (to be implemented)
- **BbcRssClientTests.cs** - Tests for BBC RSS integration (to be implemented)

## Running Tests

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Basic Test Execution

1. **Run all tests:**
   ```bash
   dotnet test
   ```

2. **Run tests with detailed output:**
   ```bash
   dotnet test --verbosity normal
   ```

3. **Run tests with coverage:**
   ```bash
   dotnet test --collect:"XPlat Code Coverage"
   ```

### Running Specific Test Classes

1. **Run AuthService tests only:**
   ```bash
   dotnet test --filter "FullyQualifiedName~AuthServiceTests"
   ```

2. **Run NewsService tests only:**
   ```bash
   dotnet test --filter "FullyQualifiedName~NewsServiceTests"
   ```

3. **Run all service tests:**
   ```bash
   dotnet test --filter "FullyQualifiedName~ServiceTests"
   ```

4. **Run external client tests:**
   ```bash
   dotnet test --filter "FullyQualifiedName~ClientTests"
   ```

### Running Specific Test Methods

1. **Run a specific test method:**
   ```bash
   dotnet test --filter "FullyQualifiedName~LoginAsync_WithValidCredentials_ReturnsSuccess"
   ```

2. **Run tests with specific pattern:**
   ```bash
   dotnet test --filter "FullyQualifiedName~LoginAsync"
   ```

## Test Categories

### Unit Tests
- **Constructor Tests** - Verify proper initialization
- **Happy Path Tests** - Test normal operation with valid data
- **Error Handling Tests** - Test exception scenarios
- **Edge Case Tests** - Test boundary conditions and invalid inputs
- **Mock Verification Tests** - Ensure dependencies are called correctly

### Test Patterns Used

1. **Arrange-Act-Assert (AAA)** - Standard test structure
2. **Mocking** - Using Moq for dependency isolation
3. **Theory Tests** - Parameterized tests for multiple scenarios
4. **Exception Testing** - Verify proper exception handling
5. **Async Testing** - Proper async/await patterns

## Test Coverage

### AuthService
- ✅ User login with valid/invalid credentials
- ✅ User registration with validation
- ✅ JWT token generation
- ✅ Password hashing and verification
- ✅ Error handling for database failures

### NewsService
- ✅ Article filtering and retrieval
- ✅ Article interactions (like, unlike, save, read)
- ✅ Recommendation algorithm
- ✅ Search functionality
- ✅ Article reporting
- ✅ Category-based filtering

### CategoryService
- ✅ CRUD operations
- ✅ Active category filtering
- ✅ Error handling

### UserService
- ✅ User management operations
- ✅ Email-based user lookup
- ✅ Active user filtering
- ✅ Error handling

### EmailService
- ✅ Email sending functionality
- ✅ Notification emails
- ✅ Welcome emails
- ✅ Configuration validation

### NotificationService
- ✅ Notification CRUD operations
- ✅ User notification settings
- ✅ Email notification sending
- ✅ Keyword-based notifications

### ExternalNewsService
- ✅ Multi-source news aggregation
- ✅ Error handling for individual sources
- ✅ Article deduplication
- ✅ Cancellation token support

### ExternalServerService
- ✅ Server management operations
- ✅ Server status toggling
- ✅ Configuration validation

### NewsApiClient
- ✅ API configuration validation
- ✅ HTTP request handling
- ✅ JSON response parsing
- ✅ Error handling for various scenarios

## Interpreting Test Results

### Test Output
- **Passed** - Test executed successfully
- **Failed** - Test failed (check error details)
- **Skipped** - Test was skipped (usually due to configuration)

### Common Test Failures
1. **Mock Setup Issues** - Verify mock configurations
2. **Async/Await Issues** - Check for proper async patterns
3. **Configuration Issues** - Ensure test data is properly set up
4. **Exception Handling** - Verify expected exceptions are thrown

## Best Practices

### Writing New Tests
1. Follow the AAA pattern (Arrange, Act, Assert)
2. Use descriptive test names
3. Test both happy path and error scenarios
4. Mock external dependencies
5. Use theory tests for parameterized scenarios

### Test Maintenance
1. Keep tests independent
2. Avoid test interdependencies
3. Clean up test data
4. Update tests when service contracts change

## Continuous Integration

### GitHub Actions
Tests can be integrated into CI/CD pipelines:

```yaml
- name: Run Unit Tests
  run: dotnet test --verbosity normal --collect:"XPlat Code Coverage"
```

### Code Coverage
Generate coverage reports:

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Troubleshooting

### Common Issues
1. **Test Discovery Issues** - Ensure test classes are public
2. **Mock Setup Problems** - Verify mock configurations match service expectations
3. **Async Test Failures** - Check for proper async/await usage
4. **Configuration Errors** - Verify test configuration is correct

### Debugging Tests
1. Use `dotnet test --verbosity detailed` for more information
2. Add logging to test methods
3. Use debugger breakpoints in test methods
4. Check test output for detailed error messages

## Contributing

When adding new services or modifying existing ones:

1. Create corresponding test classes
2. Follow existing test patterns
3. Ensure comprehensive coverage
4. Update this README with new test information
5. Run all tests before submitting changes

## Test Data

Test data is created inline within each test method to ensure:
- Test independence
- Clear test scenarios
- Easy maintenance
- No external dependencies

## Performance Considerations

- Tests are designed to run quickly
- Mock external dependencies to avoid network calls
- Use in-memory test data
- Avoid database connections in unit tests 