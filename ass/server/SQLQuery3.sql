USE CardGame;

CREATE TABLE dbo.Characters
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    Age INT NOT NULL
);

INSERT INTO dbo.Users (Name, Email, Age)
VALUES
    (N'Alex', N'alex@example.com', 20),
    (N'Maria', N'maria@example.com', 21),
    (N'Ivan', N'ivan@example.com', 19);

SELECT * FROM dbo.Users;