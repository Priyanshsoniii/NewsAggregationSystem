# News Aggregation Server - Architecture Diagrams

This directory contains comprehensive UML diagrams that illustrate the architecture and design of the News Aggregation Server application.

## 📊 Available Diagrams

### 1. Domain Model Class Diagram (`ClassDiagram.puml`)
**Purpose**: Shows the core domain entities and their relationships in the database.

**Key Features**:
- All entity classes with their properties
- Relationships between entities (one-to-many, many-to-many)
- Junction tables for user interactions
- Notes explaining each entity's purpose

**Use Cases**:
- Understanding the data model
- Database schema design
- Entity relationship mapping
- Data flow analysis

### 2. Architecture Class Diagram (`ArchitectureClassDiagram.puml`)
**Purpose**: Illustrates the layered architecture and component interactions.

**Key Features**:
- Presentation Layer (Controllers)
- Application Layer (Services)
- Infrastructure Layer (Repositories)
- External Services (API Clients)
- Background Services
- Data Layer (Entity Framework Context)

**Use Cases**:
- Understanding the application architecture
- Dependency injection patterns
- Service layer design
- Repository pattern implementation

### 3. Entity Relationship Diagram (`ERDiagram.puml`)
**Purpose**: Shows the database schema and entity relationships.

**Key Features**:
- All database entities with attributes
- Primary and foreign key relationships
- Data types and constraints
- Indexes and unique constraints
- Junction tables for many-to-many relationships

**Use Cases**:
- Database design and optimization
- Understanding data relationships
- Schema migration planning
- Performance optimization

### 4. Use Case Diagram (`UseCaseDiagram.puml`)
**Purpose**: Illustrates system functionality and user interactions.

**Key Features**:
- 38 comprehensive use cases
- 4 actor types (User, Admin, External APIs, Email System)
- Include and extend relationships
- Logical grouping by functionality

**Use Cases**:
- Requirements analysis and validation
- System functionality overview
- User interaction mapping
- Development planning and testing



## 🛠️ How to View the Diagrams

### Option 1: Online PlantUML Editor
1. Go to [PlantUML Online Editor](http://www.plantuml.com/plantuml/uml/)
2. Copy the content of any `.puml` file
3. Paste it into the editor
4. The diagram will be generated automatically

### Option 2: Visual Studio Code
1. Install the "PlantUML" extension
2. Open any `.puml` file
3. Press `Alt+Shift+D` to preview the diagram
4. Or right-click and select "Preview Current Diagram"

### Option 3: IntelliJ IDEA / WebStorm
1. Install the "PlantUML integration" plugin
2. Open any `.puml` file
3. The diagram will be displayed automatically
4. Or use `Ctrl+Alt+Shift+U` to generate

### Option 4: Command Line
```bash
# Install PlantUML (requires Java)
java -jar plantuml.jar ClassDiagram.puml
java -jar plantuml.jar ArchitectureClassDiagram.puml
java -jar plantuml.jar ERDiagram.puml
java -jar plantuml.jar UseCaseDiagram.puml
```

## 🏗️ Architecture Overview

### Layered Architecture
The application follows a **Clean Architecture** pattern with clear separation of concerns:

```
┌─────────────────────────────────────┐
│           Presentation Layer        │
│         (Controllers)               │
├─────────────────────────────────────┤
│           Application Layer         │
│         (Services)                  │
├─────────────────────────────────────┤
│         Infrastructure Layer        │
│        (Repositories)               │
├─────────────────────────────────────┤
│            Data Layer               │
│     (Entity Framework)              │
└─────────────────────────────────────┘
```

### Key Design Patterns

1. **Repository Pattern**: Abstracts data access logic
2. **Dependency Injection**: Loose coupling between components
3. **Service Layer**: Business logic encapsulation
4. **Interface Segregation**: Clean contracts between layers
5. **Background Services**: Asynchronous processing

## 🔍 Understanding the Diagrams

### Domain Model Relationships
- **User** ↔ **NewsArticle**: Many-to-many through junction tables
- **Category** → **NewsArticle**: One-to-many categorization
- **User** → **Notification**: One-to-many notifications
- **ExternalServer**: Standalone configuration entity

### Service Dependencies
- Controllers depend on Service interfaces
- Services depend on Repository interfaces
- Repositories depend on Entity Framework Context
- External services are injected into application services

### Data Flow
1. **Request Flow**: Controller → Service → Repository → Database
2. **Response Flow**: Database → Repository → Service → Controller → Client
3. **Background Flow**: Background Service → External Service → Repository

## 📈 System Interactions

### Authentication Flow
```
User → AuthController → AuthService → UserRepository → Database
```

### News Aggregation Flow
```
BackgroundService → ExternalNewsService → [NewsAPI, TheNewsAPI, BBC RSS] → NewsService → NewsRepository → Database
```

### User Interaction Flow
```
User → NewsController → NewsService → NewsRepository → Database
```

## 🎯 Best Practices Demonstrated

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Interfaces can be substituted with implementations
- **Interface Segregation**: Clients depend only on methods they use
- **Dependency Inversion**: High-level modules don't depend on low-level modules

### Clean Code Principles
- **Meaningful Names**: Clear, descriptive class and method names
- **Small Functions**: Each method does one thing well
- **DRY Principle**: No code duplication
- **Separation of Concerns**: Clear boundaries between layers

## 🔧 Maintenance and Updates

### Adding New Features
1. **Domain Model**: Add new entities to `ClassDiagram.puml`
2. **Architecture**: Update `ArchitectureClassDiagram.puml` with new services/repositories
3. **Database Schema**: Update `ERDiagram.puml` with new entities and relationships
4. **System Functions**: Add new use cases to `UseCaseDiagram.puml`

### Updating Diagrams
1. Modify the `.puml` files
2. Regenerate the diagrams using your preferred tool
3. Update this README if needed
4. Commit changes to version control

## 📚 Additional Resources

- [PlantUML Documentation](https://plantuml.com/)
- [UML Class Diagram Guide](https://www.uml-diagrams.org/class-diagrams.html)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)

## 🤝 Contributing

When contributing to the diagrams:
1. Follow the existing naming conventions
2. Maintain consistent styling
3. Add appropriate notes and descriptions
4. Update this README if adding new diagram types
5. Ensure diagrams reflect the actual codebase

---

*These diagrams serve as living documentation and should be updated as the application evolves.* 