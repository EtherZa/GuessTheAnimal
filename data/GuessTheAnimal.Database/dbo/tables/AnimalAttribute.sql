CREATE TABLE dbo.AnimalAttribute
(
	AnimalId INT NOT NULL,
	AttributeId INT NOT NULL,
	CONSTRAINT PK_AnimalAttribute_AnimalId_AttributeId PRIMARY KEY (AnimalId, AttributeId),
	CONSTRAINT FK_AnimalAttribute_AnimalId FOREIGN KEY (AnimalId) REFERENCES dbo.Animal(Id),
	CONSTRAINT FK_AnimalAttribute_AttributeId FOREIGN KEY (AttributeId) REFERENCES dbo.Attribute(Id)
)