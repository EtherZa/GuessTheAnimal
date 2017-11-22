CREATE TABLE dbo.Attribute
(
	Id INT NOT NULL,
	Description VARCHAR(300) NOT NULL
	CONSTRAINT PK_Attribute_Id PRIMARY KEY (Id),
	CONSTRAINT UC_Attribute_Name UNIQUE (Description)
)