CREATE TABLE PlateRecognition (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Image Image NOT NULL,
    Filename VARCHAR(255),
    Version INT,
    Timestamp VARCHAR(255)
);

CREATE TABLE PlateResults (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Plate VARCHAR(255),
    PlateRecognitionId INT
);

CREATE TABLE PlateBox (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Xmin INT,
    Ymin INT,
    Xmax INT,
    Ymax INT,
    PlateResultsId INT
);
