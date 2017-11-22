SET NOCOUNT ON;

RAISERROR('Seeding tables', 10, 1) WITH NOWAIT;
RAISERROR('  dbo.Animal', 10, 1) WITH NOWAIT;

CREATE TABLE #Animal
(
	Id INT,
	Name VARCHAR(100),
	PRIMARY KEY (Id)
)

INSERT INTO #Animal (Id, Name) VALUES (1, 'Ant');
INSERT INTO #Animal (Id, Name) VALUES (2, 'Cat');
INSERT INTO #Animal (Id, Name) VALUES (3, 'Elephant');
INSERT INTO #Animal (Id, Name) VALUES (4, 'Lion');
INSERT INTO #Animal (Id, Name) VALUES (5, 'Pigeon');

MERGE dbo.Animal T
USING 
  ( SELECT Id, Name
	FROM #Animal) AS S ON T.Id = S.Id
WHEN MATCHED THEN
	UPDATE SET T.Name = S.Name
WHEN NOT MATCHED THEN
	INSERT (Id, Name)
	VALUES(S.Id, S.Name);

DROP TABLE #Animal

RAISERROR('  dbo.Attribute', 10, 1) WITH NOWAIT;

CREATE TABLE #Attribute
(
	Id INT,
	Description VARCHAR(300),
	PRIMARY KEY (Id)
)

INSERT INTO #Attribute (Id, Description) VALUES (1, 'Does it have six legs?');
INSERT INTO #Attribute (Id, Description) VALUES (2, 'Does it have four legs?');
INSERT INTO #Attribute (Id, Description) VALUES (3, 'Does it have two legs?');
INSERT INTO #Attribute (Id, Description) VALUES (4, 'Is it grey?');
INSERT INTO #Attribute (Id, Description) VALUES (5, 'Is it yellow?');
INSERT INTO #Attribute (Id, Description) VALUES (6, 'Is it black?');
INSERT INTO #Attribute (Id, Description) VALUES (7, 'Does it have a trunk?');
INSERT INTO #Attribute (Id, Description) VALUES (8, 'Does it have a mane?');
INSERT INTO #Attribute (Id, Description) VALUES (9, 'Does it roar?');
INSERT INTO #Attribute (Id, Description) VALUES (10, 'Does it fly?');
INSERT INTO #Attribute (Id, Description) VALUES (11, 'Does it stalk its prey?');
INSERT INTO #Attribute (Id, Description) VALUES (12, 'Does it trumpet?');
INSERT INTO #Attribute (Id, Description) VALUES (13, 'Is it an insect?');

MERGE dbo.Attribute T
USING 
  ( SELECT Id, Description
	FROM #Attribute) AS S ON T.Id = S.Id
WHEN MATCHED THEN
	UPDATE SET T.Description = S.Description
WHEN NOT MATCHED THEN
	INSERT (Id, Description)
	VALUES(S.Id, S.Description);

DROP TABLE #Attribute

RAISERROR('  dbo.AnimalAttribute', 10, 1) WITH NOWAIT;

CREATE TABLE #AnimalAttribute
(
	AnimalId INT,
	AttributeId INT
	PRIMARY KEY (AnimalId, AttributeId)
)

-- ant
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (1, 1);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (1, 6);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (1, 13);

-- cat
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (2, 2);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (2, 6);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (2, 11);

-- elephant
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (3, 2);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (3, 4);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (3, 7);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (3, 12);

-- lion
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (4, 2);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (4, 5);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (4, 8);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (4, 9);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (4, 11);

-- pigeon
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (5, 3);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (5, 4);
INSERT INTO #AnimalAttribute (AnimalId, AttributeId) VALUES (5, 10);

MERGE dbo.AnimalAttribute T
USING 
  ( SELECT AnimalId, AttributeId
	FROM #AnimalAttribute) AS S ON T.AnimalId = S.AnimalId AND T.AttributeId = S.AttributeId
WHEN NOT MATCHED THEN
	INSERT (AnimalId, AttributeId)
	VALUES(S.AnimalId, S.AttributeId);

DROP TABLE #AnimalAttribute

RAISERROR('Done', 10, 1) WITH NOWAIT;