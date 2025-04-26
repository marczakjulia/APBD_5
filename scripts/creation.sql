CREATE TABLE Device
(
    Id        VARCHAR(50)    NOT NULL PRIMARY KEY,  
    Name      NVARCHAR(100)  NOT NULL,
    IsEnabled BIT            NOT NULL
);

CREATE TABLE PersonalComputer
(
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    OperationSystem VARCHAR(50)  NOT NULL,
    DeviceId        VARCHAR(50)  NOT NULL UNIQUE,
    CONSTRAINT FK_PersonalComputer_Device
        FOREIGN KEY (DeviceId) REFERENCES Device (Id)
            ON DELETE CASCADE
);

CREATE TABLE Smartwatch
(
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    BatteryPercentage INT          NOT NULL,
    DeviceId          VARCHAR(50)  NOT NULL UNIQUE,
    CONSTRAINT FK_Smartwatch_Device
        FOREIGN KEY (DeviceId) REFERENCES Device (Id)
            ON DELETE CASCADE
);

CREATE TABLE Embedded
(
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    IpAddress   VARCHAR(50)   NOT NULL,
    NetworkName VARCHAR(100)  NOT NULL,
    DeviceId    VARCHAR(50)   NOT NULL UNIQUE,
    CONSTRAINT FK_Embedded_Device
        FOREIGN KEY (DeviceId) REFERENCES Device (Id)
            ON DELETE CASCADE
);


