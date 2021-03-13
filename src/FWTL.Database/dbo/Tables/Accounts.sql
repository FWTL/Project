CREATE TABLE [dbo].[Accounts]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [OwnerId] UNIQUEIDENTIFIER NOT NULL, 
    [ExternalAccountId] VARCHAR(20) NOT NULL,
    CONSTRAINT [IX_Unique] UNIQUE NONCLUSTERED 
    (
    	[OwnerId] ASC,
    	[ExternalAccountId] ASC
    )
)
