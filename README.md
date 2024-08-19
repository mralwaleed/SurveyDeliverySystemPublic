
# Survey Delivery System

## Overview

The **Survey Delivery System** automates the delivery of survey links to domain administrators. It features reliable email delivery with built-in retry logic, ensuring that surveys are sent even under challenging conditions. The system is built using ASP.NET Core and integrates with SendGrid for email delivery.

## Features

- **Automated Email Delivery:** Sends survey links to administrators of specified domain names.
- **Retry Mechanism:** Retries email sending in case of transient failures.
- **Validation:** Ensures email addresses, URLs, and domain names are correct before sending.
- **Logging:** Logs all email attempts, validation results, and errors.
- **API Versioning:** Supports versioning for future updates without breaking existing clients.

## Technologies Used

- **ASP.NET Core 6**: The web framework used to build the application.
- **SendGrid**: A cloud-based email service provider used for sending emails.
- **Scrutor**: A library for advanced dependency injection scenarios like service decoration.

## Setup Instructions

### Prerequisites

- .NET 6 SDK or later
- SendGrid account (sign up at [SendGrid](https://sendgrid.com))
- Visual Studio or any other C# IDE

### Link SendGrid with Your Project

1. **Sign up for a SendGrid account** at [SendGrid](https://sendgrid.com).

2. **Create an API Key** in the SendGrid Dashboard:
   - Go to the [API Keys](https://app.sendgrid.com/settings/api_keys) page.
   - Click "Create API Key", give it a name, and choose the "Full Access" option.
   - Copy the API Key.

3. **Configure `appsettings.json`**:
   Update the `appsettings.json` file with your SendGrid API key and other configurations:
   ```json
   {
   "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning",
       }
     }
     "SendGridSettings": {
       "ApiKey": "<your-sendgrid-api-key>",
       "FromEmail": "no-reply@yourdomain.com",
       "FromName": "Your Company Name"
     },
     "EmailSettings": {
       "MaxRetryAttempts": 3,
       "RetryBaseDelaySeconds": 2
     },
     
   }
   ```

### Install Dependencies

1. **Install Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the Project**:
   ```bash
   dotnet build
   ```

### Running the Application

1. **Run the Application**:
   ```bash
   dotnet run
   ```
   The application will be available at `https://localhost:<port>`.

2. **Access the API**:
   Use an API client like Postman to interact with the API. Example endpoint:
   ```
   POST https://localhost:<port>/api/v1/survey/create-survey
   ```
   Example request body:
   ```json
   {
     "SurveyUrl": "https://example.com/survey",
     "Domains": [
       {
         "DomainName": "example.com",
         "AdminEmail": "admin@example.com"
       },
       {
         "DomainName": "anotherdomain.com",
         "AdminEmail": "admin@anotherdomain.com"
       }
     ]
   }
   ```


### Testing Frameworks Used

- **NUnit**: A popular and robust testing framework for .NET.
- **Moq**: A library for creating mock objects and setting up expectations in unit tests.
- **FluentValidation.TestHelper**: Assists in testing FluentValidation validators.
- **Microsoft.NET.Test.Sdk**: Required for running tests in .NET projects.

### Running Unit Tests

To run the unit tests included in this project, follow these steps:

1. **Navigate to the Test Project Directory**:
   In your terminal or command prompt, navigate to the directory containing the test project:
   ```bash
   cd SurveyDeliverySystem.Tests
   ```

2. **Run the Tests**:
   Use the following command to run all the unit tests:
   ```bash
   dotnet test
   ```
   This command will build the test project and execute all the tests. The results, including any test failures, will be displayed in the terminal.

3. **Understanding the Test Results**:
   After running the tests, you'll see output indicating which tests passed and which failed. The output includes details about any failures, making it easier to identify and fix issues in the code.

4. **Test Coverage**:
   To check the code coverage of your tests, you can use the `coverlet.collector` package included in the project. This can help ensure that your tests are covering the necessary parts of your codebase.
   ```bash
   dotnet test /p:CollectCoverage=true
   ```
   This command will generate a coverage report, showing you which parts of your code are covered by the tests.

### Writing New Tests

If you want to contribute by adding new tests:

1. **Create a New Test Class**:
   Add new test classes in the `SurveyDeliverySystem.Tests` project under appropriate folders like `Services` or `Validators`.

2. **Use NUnit's `[Test]` Attribute**:
   Mark your test methods with the `[Test]` attribute.

   Example:
   ```csharp
   [Test]
   public void ExampleTest()
   {
       // Arrange
       var expected = 1;

       // Act
       var actual = 1;

       // Assert
       Assert.AreEqual(expected, actual);
   }
   ```

3. **Mock Dependencies**:
   Use the Moq library to mock dependencies in your unit tests.

   Example:
   ```csharp
   var mockEmailSender = new Mock<IEmailSender>();
   mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<SurveyEmailInfo>())).ReturnsAsync(true);
   ```


### Deployment

For deploying this application to a live environment, consider using cloud platforms like Azure App Service or AWS Elastic Beanstalk, which support .NET Core applications. Ensure that environment variables for the SendGrid API key and other configurations are set in your production environment.

## Contributing

Contributions are welcome! Please fork this repository, create a new branch, and submit a pull request with your changes. Make sure to follow the coding standards and write tests for new functionality.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

---

For any questions or support, please open an issue on GitHub or contact the maintainers.
