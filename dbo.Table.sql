CREATE TABLE [dbo].Pisarze (
    [Id_Pisarz] INT            IDENTITY (1, 1) NOT NULL,
    [Imie]     NVARCHAR (MAX) NOT NULL,
    [Nazwisko]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.Pisarze] PRIMARY KEY CLUSTERED ([Id_Pisarz] ASC)
);