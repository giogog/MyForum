This project is a comprehensive web application that includes both a back-end built with ASP.NET Core Web API and a front-end developed using Blazor. It serves as an online forum platform where users can create and join forums, such as "Politics in My Country," and engage in discussions by creating topics within those forums. Authorized users can upvote relevant and engaging topics, making them more visible by placing them at the top of the list. Each topic features a comment section with a reply system, allowing for detailed discussions.

Roles
The project defines three roles:

Admin: Can create and approve forums and topics, also can manage users, like ban user or give moderator Role, also can delete or hide topics and forums.

Moderator: Can approve topics, also can hide topics.

User: Can create forums, topics, and comment on topics, upvote liked topic. can check profile and sees all the topics created by him(all user activities can done by admin and moderator !!!)

Architecture

The back-end follows a clean architecture design:
Entity Framework is used for database connectivity, configured in the Infrastructure layer, and injected through API/Extensions/ServiceExtension.cs.
Contracts: Separate layers for abstraction, providing clear boundaries between services and repositories.
Service and Repository Management: IServiceManager is implemented in the Application layer, while IRepositoryManager is implemented in the Infrastructure layer.
Application Layer
The Application layer contains services that handle CRUD (Create, Read, Update, Delete) operations on entities in the database. Global exception handling is set up through a custom middleware (GlobalExceptionHandler), and you will find custom exceptions within various layers of the project.

Authentication and Authorization
The project uses JWT tokens for authentication, with a dedicated service and controller for managing authentication and authorization. Microsoft Identity is configured, where the User class is derived from IdentityUser, and Role is derived from IdentityRole. The Identity setup, along with other service configurations, can be found in ServiceExtension.cs in the API layer.

Email Verification
An Email Verification system is integrated alongside authentication. This system has a dedicated service and an email sender component, configured within the Infrastructure layer.
