Use [COLLEGE];
-- Table for Countries
CREATE TABLE Country (
    CountryId INT PRIMARY KEY IDENTITY(1,1),
    CountryName VARCHAR(100) NOT NULL
);
Insert into Country values ('India'),('ThaiLand');
Insert into Country values ('China'),('Russia');
Insert into Country values ('USA'),('UAE');

-- Table for Register
CREATE TABLE Register (
    UserName VARCHAR(100) NOT NULL,
    EmailId VARCHAR(100) NOT NULL UNIQUE,
    [Password] VARBINARY(64) NOT NULL,
    [Date] DATE NOT NULL,
    CountryId INT,  -- Foreign key

    FOREIGN KEY (CountryId) REFERENCES Country(CountryId)
);

INSERT INTO Register (UserName, EmailId, [Password], [Date], CountryId)
VALUES (
    'Komal Chauhan',
    'komal@gmail.com',
    HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), 'komal123')),
    '2025-05-23',
    1
),(
    'Sheetal Singh',
    'sheetal@gmail.com',
    HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), 'sheetal123')),
    '2025-06-23',
    2
),(
    'Pooja Khan',
    'pooja@gmail.com',
    HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), 'pooja123')),
    '2025-05-21',
    3
),(
    'Rinkle Solanki',
    'rinkle@gmail.com',
    HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), 'rinkle123')),
    '2025-01-23',
    4
);

-- -------------------------------------------Drop procedure SP_InsertRegisterDetails;
CREATE PROCEDURE SP_GetLoginDetails
    @EmailId VARCHAR(100),
    @Password VARCHAR(100)
AS
BEGIN
    -- Check if user with given EmailId and hashed Password exists
    IF EXISTS (
        SELECT 1 
        FROM Register 
        WHERE EmailId = @EmailId AND [Password] = HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), @Password))
    )
    BEGIN
        -- Return user details with country name
        SELECT 
            R.UserName,
            R.EmailId,
            R.[Date],
            C.CountryName
        FROM Register R
        LEFT JOIN Country C ON R.CountryId = C.CountryId
        WHERE R.EmailId = @EmailId AND [Password] = HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), @Password));
    END
    ELSE
    BEGIN
        -- Invalid credentials
        SELECT 'Invalid EmailId or Password.' AS Message;
    END
END

-- Correct information
Execute SP_GetLoginDetails 'komal@gmail.com','komal123';
-- Incorrect Information
Execute SP_GetLoginDetails 'komal@gmail.com','komal1234';

----/////////////////////////////////////////////////
CREATE PROCEDURE SP_InsertRegisterDetails
    @UserName VARCHAR(100),
    @EmailId VARCHAR(100),
    @Password VARCHAR(100),
    @Date DATE,
    @CountryId INT
AS
BEGIN
    -- Check if Email already exists
    IF EXISTS (SELECT 1 FROM Register WHERE EmailId = @EmailId)
    BEGIN
        SELECT 'Email already registered.' AS Message;
        RETURN;
    END

    -- Insert new user with hashed password
    INSERT INTO Register (UserName, EmailId, [Password], [Date], CountryId)
    VALUES (
        @UserName,
        @EmailId,
        HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), @Password)),
        @Date,
        @CountryId
    );

    SELECT 'Registration successful.' AS Message;
END


EXEC SP_InsertRegisterDetails 
    @UserName = 'Divya Agarwal',
    @EmailId = 'divya@gmail.com',
    @Password = 'divya123',
    @Date = '2024-05-23',
    @CountryId = 5;
------------------------------Drop proc SP_UpdateRegisterDetails;
CREATE PROCEDURE SP_UpdateRegisterDetails
    @EmailId VARCHAR(100),        -- Unique identifier for update
    @UserName VARCHAR(100),
    @Password VARCHAR(100),
    @Date DATE,
    @CountryId INT
AS
BEGIN
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM Register WHERE EmailId = @EmailId)
    BEGIN
        SELECT 'User not found.' AS Message;
        RETURN;
    END

    -- Update user details (hashing the new password)
    UPDATE Register
    SET 
        UserName = @UserName,
        [Password] = HASHBYTES('SHA2_256', CONVERT(VARCHAR(100), @Password)),
        [Date] = @Date,
        CountryId = @CountryId
    WHERE EmailId = @EmailId;

    SELECT 'User details updated successfully.' AS Message;
END
--////////////// Call Update SP
EXEC SP_UpdateRegisterDetails 
    @EmailId = 'komal@gmail.com',
    @UserName = 'Komal Updated',
    @Password = 'komal123',
    @Date = '2025-05-23',
    @CountryId = 2;
----------------------------
CREATE PROCEDURE SP_DeleteRegisterDetails
    @EmailId VARCHAR(100)
AS
BEGIN
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM Register WHERE EmailId = @EmailId)
    BEGIN
        SELECT 'User not found.' AS Message;
        RETURN;
    END

    -- Delete the user
    DELETE FROM Register
    WHERE EmailId = @EmailId;

    SELECT 'User deleted successfully.' AS Message;
END

EXEC SP_DeleteRegisterDetails @EmailId = 'komal@gmail.com';


ALTER PROCEDURE SP_InsertRegisterDetails
    @UserName VARCHAR(100),
    @EmailId VARCHAR(100),
    @Password VARBINARY(64),  -- ✅ Changed from VARCHAR to VARBINARY
    @Date DATE,
    @CountryId INT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Register WHERE EmailId = @EmailId)
    BEGIN
        SELECT 'Email already registered.' AS Message;
        RETURN;
    END

    INSERT INTO Register (UserName, EmailId, [Password], [Date], CountryId)
    VALUES (
        @UserName,
        @EmailId,
        @Password,       -- Already hashed in C#
        @Date,
        @CountryId
    );

    SELECT 'Registration successful.' AS Message;
END

