USE [RASE_DB]
GO

/****** Object:  Table [dbo].[Membership]    Script Date: 7/28/2025 3:56:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Membership](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MembershipNo] [nvarchar](50) NOT NULL,
	[MembershipDate] [datetime] NOT NULL,
	[ExpiryDate] [datetime] NOT NULL,
	[PointsEarned] [decimal](18, 0) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedById] [int] NOT NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[UpdatedById] [int] NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Membership] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Membership]  WITH CHECK ADD  CONSTRAINT [FK_Membership_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO

ALTER TABLE [dbo].[Membership] CHECK CONSTRAINT [FK_Membership_Customer]
GO


