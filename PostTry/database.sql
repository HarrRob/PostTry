CREATE DATABASE useAuth;

USE useAuth;

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),   -- Auto-incremented primary key
    FirstName NVARCHAR(255),       -- First name of the user
    LastName NVARCHAR(255),        -- Last name of the user
    AUTHCODE NVARCHAR(300),                 -- Spotify authorization code
    CreatedAt DATETIME DEFAULT GETDATE()    -- Timestamp of user creation
);
