## Backend

### API
- `backend/API/AppDbContext.cs` - Entity Framework Core database context for interacting with the `todos` table.
- `backend/API/Program.cs` - Main entry point for the ASP.NET Core backend API, configures services and request pipeline.
- `backend/API/GlobalUsings.cs` - Defines global using directives for the backend API project.

#### API/Todo
- `backend/API/Todo/ITodoRepository.cs` - Defines the `ITodoRepository` interface and its `EntityFrameworkTodoRepository` implementation for data access.
- `backend/API/Todo/Endpoints.cs` - Defines and maps HTTP endpoints for Todo item operations (CRUD).
- `backend/API/Todo/Priority.cs` - Defines the `Priority` enum for Todo items (Lowest, Low, Normal, High, Highest).
- `backend/API/Todo/Todo.cs` - Defines the `TodoItem` entity model with properties like Id, Title, Description.

##### API/Todo/Specifications
- `backend/API/Todo/Specifications/PriorityTodoItemSpecification.cs` - Defines a specification to filter Todo items by priority.

### ServiceDefaults
- `backend/ServiceDefaults/Extensions.cs` - Provides extension methods for .NET Aspire service defaults like telemetry and health checks.
- `backend/ServiceDefaults/HealthCheckHelpers.cs` - Helper class for creating and formatting health check responses.

### API.Integration.Tests
- `backend/API.Integration.Tests/AutoDataProviderAttributes.cs` - Custom NUnit attributes using AutoFixture for integration test data generation.
- `backend/API.Integration.Tests/SetupTests.cs` - Contains setup and health check tests for the integration test environment.
- `backend/API.Integration.Tests/Tests.cs` - Base test fixture for integration tests, manages Testcontainers and database state.

#### API.Integration.Tests/Helpers
- `backend/API.Integration.Tests/Helpers/TestWebApplicationFactory.cs` - Custom `WebApplicationFactory` for integration tests, sets up a test database container.

#### API.Integration.Tests/Todo
- `backend/API.Integration.Tests/Todo/EndpointsTests.cs` - Integration tests for the Todo API endpoints.

### API.Tests
- `backend/API.Tests/AutoDataAttributes.cs` - Custom NUnit attributes using AutoFixture for unit test data generation.
- `backend/API.Tests/GlobalUsings.cs` - Defines global using directives for the API unit tests project.

#### API.Tests/Todo
- `backend/API.Tests/Todo/EndpointsTests.cs` - Unit tests for the Todo API endpoints, focusing on business logic.

## Frontend

### src
- `client/src/App.js` - Root React component for the Todo application, sets up routing/main page.
- `client/src/App.test.js` - Basic test suite for the main App component.
- `client/src/Question.js` - React component for displaying a collapsible question/answer section (not used in Todo app).
- `client/src/index.js` - Entry point for the React application, renders the root App component.

#### src/components
- `client/src/components/AddTodo.js` - React component for the form to add new Todo items.
- `client/src/components/TodoCard.js` - React component for displaying a single Todo item card.
- `client/src/components/TodoList.js` - React component for displaying a list of Todo items.

#### src/pages
- `client/src/pages/DashboardPage.js` - React component for the main dashboard page, manages Todo data and interactions.

#### src/services
- `client/src/services/api.js` - JavaScript module for making API calls to the backend (get, post, delete Todos).

## E2E Tests

### EndToEnd.Tests
- `e2e/EndToEnd.Tests/AutoDataProviderAttributes.cs` - Custom NUnit attributes using AutoFixture for end-to-end test data generation.
- `e2e/EndToEnd.Tests/SetupTests.cs` - Basic setup tests for the e2e environment, checking health and frontend.
- `e2e/EndToEnd.Tests/Tests.cs` - Base test fixture for end-to-end tests, manages the Aspire distributed application lifecycle.
- `e2e/EndToEnd.Tests/HomePageTests.cs` - End-to-end tests for the home page, verifying UI elements and add todo functionality.

#### EndToEnd.Tests/Infrastructure
- `e2e/EndToEnd.Tests/Infrastructure/DistributedApplicationExtensions.cs` - Extension methods for configuring Aspire distributed applications in tests.

## Tools

### AppHost
- `tools/AppHost/Program.cs` - Main entry point for the Aspire AppHost, defines and configures application resources. 