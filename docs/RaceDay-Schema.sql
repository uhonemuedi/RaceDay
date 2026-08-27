-- ============================================
-- RaceDay Database Script
-- PROG6212 POE Part 1 - Section C
-- ============================================

CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

-- ============================================
-- TABLE: User
-- ============================================
CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    PhoneNumber VARCHAR(20) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================
-- TABLE: Venue
-- ============================================
CREATE TABLE Venue (
    VenueID INT IDENTITY(1,1) PRIMARY KEY,
    VenueName VARCHAR(150) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    City VARCHAR(100) NOT NULL,
    Province VARCHAR(100) NOT NULL
);
GO

-- ============================================
-- TABLE: Event
-- ============================================
CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    VenueID INT NOT NULL,
    EventName VARCHAR(150) NOT NULL,
    EventDate DATE NOT NULL,
    Description VARCHAR(500) NULL,
    EventType VARCHAR(20) NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserID) REFERENCES [User](UserID),
    CONSTRAINT FK_Event_Venue FOREIGN KEY (VenueID) REFERENCES Venue(VenueID)
);
GO

-- ============================================
-- TABLE: Category
-- ============================================
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(50) NOT NULL,
    DistanceKM DECIMAL(5,2) NOT NULL,
    EntryFee DECIMAL(8,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventID) REFERENCES Event(EventID)
);
GO

-- ============================================
-- TABLE: Enrolment
-- ============================================
CREATE TABLE Enrolment (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantID) REFERENCES [User](UserID),
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);
GO

-- ============================================
-- TABLE: Result
-- ============================================
CREATE TABLE Result (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME NULL,
    Position INT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Finished' CHECK (Status IN ('Finished', 'DNF', 'DQ')),
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentID) REFERENCES Enrolment(EnrolmentID)
);
GO

-- ============================================
-- TABLE: Payment
-- ============================================
CREATE TABLE Payment (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    Amount DECIMAL(8,2) NOT NULL,
    PaymentMethod VARCHAR(30) NOT NULL,
    PaymentStatus VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (PaymentStatus IN ('Paid', 'Pending', 'Failed')),
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Payment_Enrolment FOREIGN KEY (EnrolmentID) REFERENCES Enrolment(EnrolmentID)
);
GO

-- ============================================
-- SEED DATA
-- ============================================

-- Organisers (2)
INSERT INTO [User] (FullName, Email, PasswordHash, Role, PhoneNumber) VALUES
('Thabo Mokoena', 'thabo.mokoena@raceday.co.za', 'hashed_pw_1', 'Organiser', '0821234567'),
('Lindiwe Nkosi', 'lindiwe.nkosi@raceday.co.za', 'hashed_pw_2', 'Organiser', '0827654321');

-- Participants (2)
INSERT INTO [User] (FullName, Email, PasswordHash, Role, PhoneNumber) VALUES
('Pearl Ramabulana', 'pearl.r@raceday.co.za', 'hashed_pw_3', 'Participant', '0839876543'),
('Sipho Dlamini', 'sipho.d@raceday.co.za', 'hashed_pw_4', 'Participant', '0836549871');

-- Venues
INSERT INTO Venue (VenueName, Address, City, Province) VALUES
('Marks Park Sports Club', '1 Empire Rd', 'Johannesburg', 'Gauteng'),
('Kirstenbosch Gardens', 'Rhodes Dr', 'Cape Town', 'Western Cape'),
('Comrades House', '18 Connaught Rd', 'Pietermaritzburg', 'KwaZulu-Natal');

-- Events (3), OrganiserID references [User] table (1 = Thabo, 2 = Lindiwe)
INSERT INTO Event (OrganiserID, VenueID, EventName, EventDate, Description, EventType) VALUES
(1, 1, 'Joburg City Run', '2026-10-10', 'Annual road running event through Johannesburg CBD.', 'Run'),
(2, 2, 'Cape Town Cycle Challenge', '2026-11-15', 'Scenic cycling route around Cape Town.', 'Cycle'),
(1, 3, 'Midlands Charity Walk', '2026-09-20', 'Community charity walk in the KZN Midlands.', 'Walk');

-- Categories for each event
INSERT INTO Category (EventID, CategoryName, DistanceKM, EntryFee) VALUES
(1, '10km', 10.00, 150.00),
(1, '21km', 21.10, 250.00),
(2, '50km', 50.00, 350.00),
(2, '100km', 100.00, 500.00),
(3, '5km', 5.00, 80.00);

-- Sample enrolments (Participants: 3 = Pearl, 4 = Sipho)
INSERT INTO Enrolment (ParticipantID, CategoryID, Status) VALUES
(3, 1, 'Confirmed'),
(4, 2, 'Confirmed'),
(3, 5, 'Pending');
GO
