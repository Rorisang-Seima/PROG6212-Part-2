CREATE DATABASE RaceDayDb;
GO
USE RaceDayDb;
GO
CREATE TABLE Users (
 UserId int IDENTITY(1,1) PRIMARY KEY,
 FirstName nvarchar(80) NOT NULL, LastName nvarchar(80) NOT NULL,
 Email nvarchar(180) NOT NULL UNIQUE, PasswordHash nvarchar(max) NOT NULL,
 Role nvarchar(20) NOT NULL, PhoneNumber nvarchar(30) NULL, CreatedAt datetime2 NOT NULL
);
CREATE TABLE Events (
 EventId int IDENTITY(1,1) PRIMARY KEY,
 OrganiserId int NOT NULL, Name nvarchar(150) NOT NULL, Description nvarchar(2000) NULL,
 EventDate datetime2 NOT NULL, Location nvarchar(250) NOT NULL, RegistrationDeadline datetime2 NOT NULL,
 ImageUrl nvarchar(500) NULL, RouteInfoUrl nvarchar(500) NULL, DistanceKm decimal(8,2) NOT NULL,
 EventType nvarchar(20) NOT NULL, CreatedAt datetime2 NOT NULL,
 CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserId) REFERENCES Users(UserId)
);
CREATE TABLE Categories (
 CategoryId int IDENTITY(1,1) PRIMARY KEY, Name nvarchar(100) NOT NULL UNIQUE, Description nvarchar(500) NULL
);
CREATE TABLE EventCategories (
 EventCategoryId int IDENTITY(1,1) PRIMARY KEY, EventId int NOT NULL, CategoryId int NOT NULL,
 EntryFee decimal(10,2) NOT NULL, MaximumParticipants int NOT NULL,
 CONSTRAINT FK_EventCategories_Events FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE,
 CONSTRAINT FK_EventCategories_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
 CONSTRAINT UQ_EventCategories_Event_Category UNIQUE(EventId, CategoryId)
);
CREATE TABLE Enrolments (
 EnrolmentId int IDENTITY(1,1) PRIMARY KEY, ParticipantId int NOT NULL, EventCategoryId int NOT NULL,
 EnrolmentDate datetime2 NOT NULL, Status nvarchar(20) NOT NULL,
 CONSTRAINT FK_Enrolments_Users FOREIGN KEY (ParticipantId) REFERENCES Users(UserId),
 CONSTRAINT FK_Enrolments_EventCategories FOREIGN KEY (EventCategoryId) REFERENCES EventCategories(EventCategoryId) ON DELETE CASCADE,
 CONSTRAINT UQ_Enrolments_Participant_Category UNIQUE(ParticipantId, EventCategoryId)
);
CREATE TABLE Results (
 ResultId int IDENTITY(1,1) PRIMARY KEY, EnrolmentId int NOT NULL UNIQUE,
 FinishPosition int NULL, FinishTime time NULL, ResultStatus nvarchar(30) NOT NULL, RecordedAt datetime2 NOT NULL,
 CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId) ON DELETE CASCADE
);
