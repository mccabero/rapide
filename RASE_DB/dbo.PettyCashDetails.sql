USE [RASE_DB]
GO

/****** Object:  Table [dbo].[PettyCashDetails]    Script Date: 7/16/2025 5:40:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PettyCashDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PettyCashId] [int] NOT NULL,
	[Particulars] [nvarchar](max) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_PettyCashDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PettyCashDetails]  WITH CHECK ADD  CONSTRAINT [FK_PettyCashDetails_PettyCash] FOREIGN KEY([PettyCashId])
REFERENCES [dbo].[PettyCash] ([Id])
GO

ALTER TABLE [dbo].[PettyCashDetails] CHECK CONSTRAINT [FK_PettyCashDetails_PettyCash]
GO


