
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

### Testing the Retry Mechanism

1. **Simulate Failure in SendGridEmailSender**:
   Modify the `SendGridEmailSender` to simulate a failure with a random chance to trigger the retry logic:
   ```csharp
   public async Task<bool> SendEmailAsync(SurveyEmailInfo emailInfo)
   {
       // Simulate a failure on the first attempt to test retry logic :) 
       // change to 1 to 4 if you want to test the chance of failure and test the retry logic :)
       if (new Random().Next(2, 4) == 1)  // 25% chance to simulate failure 
       {
           throw new SmtpException(SmtpStatusCode.ServiceNotAvailable, "Simulated transient failure");
       }

       // Continue with the normal email sending process
       var client = new SendGridClient(_sendGridConfig.ApiKey);
       var from = new EmailAddress(_sendGridConfig.FromEmail, _sendGridConfig.FromName);
       var subject = $"Survey for {emailInfo.DomainName}";
       var to = new EmailAddress(emailInfo.AdminEmail);
       var plainTextContent = $"Please take the survey at {emailInfo.SurveyUrl}.";
       var htmlContent = $"<strong>Please take the survey at {emailInfo.SurveyUrl}.</strong>";
       var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
       var response = await client.SendEmailAsync(msg);

       return response.StatusCode == System.Net.HttpStatusCode.Accepted;
   }


2. **Run the Application and Observe Logs**:
   Start the application and monitor the logs to ensure the retry mechanism is functioning:
   ```
   Transient SMTP exception occurred. Retrying 1/3...
   Transient SMTP exception occurred. Retrying 2/3...
   Email sent successfully on attempt 3.
   ```

3. **Remove or Adjust the Failure Simulation**:
   After testing, remove or adjust the simulated failure to return to normal operations.

### Deployment

For deploying this application to a live environment, consider using cloud platforms like Azure App Service or AWS Elastic Beanstalk, which support .NET Core applications. Ensure that environment variables for the SendGrid API key and other configurations are set in your production environment.

## Contributing

Contributions are welcome! Please fork this repository, create a new branch, and submit a pull request with your changes. Make sure to follow the coding standards and write tests for new functionality.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

---

For any questions or support, please open an issue on GitHub or contact the maintainers.
