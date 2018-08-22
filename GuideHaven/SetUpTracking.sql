USE [GuideContextDB]
GO
CREATE FULLTEXT INDEX ON [dbo].[Guide] KEY INDEX [PK_Guide] ON ([GuideCatalog], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING AUTO)
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Guide] ADD ([Category] LANGUAGE [British English])
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Guide] ADD ([Description] LANGUAGE [British English])
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Guide] ADD ([GuideName] LANGUAGE [British English])
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Guide] ADD ([Owner] LANGUAGE [British English])
GO
ALTER FULLTEXT INDEX ON [dbo].[Guide] ENABLE
GO
USE [GuideContextDB]
GO
CREATE FULLTEXT INDEX ON [dbo].[Step] KEY INDEX [PK_Step] ON ([GuideCatalog], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING AUTO)
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Step] ADD ([Content] LANGUAGE [British English])
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Step] ADD ([Header] LANGUAGE [British English])
GO
ALTER FULLTEXT INDEX ON [dbo].[Step] ENABLE
GO
USE [GuideContextDB]
GO
CREATE FULLTEXT INDEX ON [dbo].[Tags] KEY INDEX [PK_Tags] ON ([GuideCatalog], FILEGROUP [PRIMARY]) WITH (CHANGE_TRACKING AUTO)
GO
USE [GuideContextDB]
GO
ALTER FULLTEXT INDEX ON [dbo].[Tags] ADD ([TagId] LANGUAGE [British English])
GO
ALTER FULLTEXT INDEX ON [dbo].[Tags] ENABLE
GO
