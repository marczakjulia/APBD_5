INSERT INTO Device (Id, Name, IsEnabled)
VALUES
    ('P-1', 'Windows Laptop', 1),
    ('SW-1', 'Apple Watch SE2', 1),
    ('ED-1', 'Smart Hub',       1);

INSERT INTO PersonalComputer (OperationSystem, DeviceId)
VALUES
    ('Windows 10', 'P-1');

INSERT INTO Smartwatch (BatteryPercentage, DeviceId)
VALUES
    (75, 'SW-1');

INSERT INTO Embedded (IpAddress, NetworkName, DeviceId)
VALUES
    ('192.168.1.1', 'Home Network', 'ED-1');