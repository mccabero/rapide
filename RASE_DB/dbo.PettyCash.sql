USE [RASE_DB]
GO

/****** Object:  Table [dbo].[PettyCash]    Script Date: 7/16/2025 5:40:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCash](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PCNo] [int] NOT NULL,
	[TransactionDateTime] [datetime] NOT NULL,
	[PayTo] [nvarchar](max) NOT NULL,
	[CashIn] [decimal](18, 2) NOT NULL,
	[CashOut] [decimal](18, 2) NOT NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[ApprovedByUserId] [int] NOT NULL,
	[PaidByUserId] [int] NOT NULL,
	[IsPaymentReceived] [bit] NOT NULL,
	[PaymentReceivedBy] [nvarchar](max) NOT NULL,
	[JobStatusId] [int] NOT NULL,
	[CreatedById] [int] NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[UpdatedById] [int] NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_PettyCash] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PettyCash]  WITH CHECK ADD  CONSTRAINT [FK_PettyCash_ApprovedBy_User] FOREIGN KEY([ApprovedByUserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[PettyCash] CHECK CONSTRAINT [FK_PettyCash_ApprovedBy_User]
GO

ALTER TABLE [dbo].[PettyCash]  WITH CHECK ADD  CONSTRAINT [FK_PettyCash_JobStatus] FOREIGN KEY([JobStatusId])
REFERENCES [dbo].[JobStatus] ([Id])
GO

ALTER TABLE [dbo].[PettyCash] CHECK CONSTRAINT [FK_PettyCash_JobStatus]
GO

ALTER TABLE [dbo].[PettyCash]  WITH CHECK ADD  CONSTRAINT [FK_PettyCash_PaidBy_User] FOREIGN KEY([PaidByUserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[PettyCash] CHECK CONSTRAINT [FK_PettyCash_PaidBy_User]
GO


