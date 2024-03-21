USE [xva-database];

-- Create the Schedule table
CREATE TABLE Simulation (
    id INT IDENTITY(1,1) PRIMARY KEY,
    bookname VARCHAR(255) NOT NULL,
    booktype VARCHAR(50) NOT NULL,
    fileextension VARCHAR(10) NOT NULL,
    status VARCHAR(255) DEFAULT 'Pending'
);

DECLARE @Counter INT = 1;
WHILE @Counter <= 10
BEGIN
    DECLARE @RandomString VARCHAR(255) = CONCAT('Retail-', LEFT(NEWID(), 8));
    INSERT INTO Simulation (bookname, booktype, fileextension, status)
    VALUES (@RandomString, 'cpp', '.json', 'Pending');
    
    SET @Counter = @Counter + 1;
END

select *from Simulation

delete Simulation