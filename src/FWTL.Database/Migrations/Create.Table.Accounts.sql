CREATE TABLE [dbo].[Accounts](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[OwnerId] [uniqueidentifier] NOT NULL,
	[ExternalAccountId] [varchar](20) NOT NULL,
 CONSTRAINT [IX_Unique] UNIQUE NONCLUSTERED 
 (
 	[OwnerId] ASC,
 	[ExternalAccountId] ASC
 )
)