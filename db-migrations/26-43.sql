USE [ProgOlymp]
GO

BEGIN TRANSACTION;
ALTER TABLE [Contests]
    DROP CONSTRAINT [FK_Contests_ContestStages_FinalStageId];

ALTER TABLE [Contests]
    DROP CONSTRAINT [FK_Contests_ContestStages_PreliminaryStageId];

DROP INDEX [IX_Contests_FinalStageId] ON [Contests];

DROP INDEX [IX_Contests_PreliminaryStageId] ON [Contests];

EXEC [sp_rename] N'[Contests].[PreliminaryStageId]', N'PreliminaryStage_YandexContestId', 'COLUMN';

ALTER TABLE [Contests]
    ADD [PreliminaryStage_Name] NVARCHAR(100) NULL;

ALTER TABLE [Contests]
    ADD [PreliminaryStage_ContestStart] DATETIMEOFFSET NULL;

ALTER TABLE [Contests]
    ADD [PreliminaryStage_ContestFinish] DATETIMEOFFSET NULL;

ALTER TABLE [Contests]
    ADD [PreliminaryStage_Duration] BIGINT NULL;

EXEC [sp_rename] N'[Contests].[FinalStageId]', N'FinalStage_YandexContestId', 'COLUMN';

ALTER TABLE [Contests]
    ADD [FinalStage_Name] NVARCHAR(100) NULL;

ALTER TABLE [Contests]
    ADD [FinalStage_ContestStart] DATETIMEOFFSET NULL;

ALTER TABLE [Contests]
    ADD [FinalStage_ContestFinish] DATETIMEOFFSET NULL;

ALTER TABLE [Contests]
    ADD [FinalStage_Duration] BIGINT NULL;

ALTER TABLE [Contests]
    ADD [Title] NVARCHAR(MAX) NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_ContestUserId] BIGINT NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_State] INT NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_Start] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_Finish] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_ContestUserId] BIGINT NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_State] INT NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_Start] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_Finish] DATETIMEOFFSET NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260221150505_upd', N'10.0.3');

COMMIT;
GO

BEGIN TRANSACTION;
UPDATE [c]
SET [c].[Title]                          = [c].[Name],
    [c].[Name]                           = CONCAT('olymp', RIGHT([c].[Name], 2)),
    [c].[PreliminaryStage_Name]          = [ps].[Name],
    [c].[PreliminaryStage_ContestStart]  = [ps].[ContestStart],
    [c].[PreliminaryStage_ContestFinish] = [ps].[ContestStart],
    [c].[PreliminaryStage_Duration]      = [ps].[Duration],
    [c].[FinalStage_Name]                = [fs].[Name],
    [c].[FinalStage_ContestStart]        = [fs].[ContestStart],
    [c].[FinalStage_ContestFinish]       = [fs].[ContestStart],
    [c].[FinalStage_Duration]            = [fs].[Duration]
FROM [Contests] [c]
         LEFT JOIN [ContestStages] [ps] ON [ps].[Id] = [c].[PreliminaryStage_YandexContestId]
         LEFT JOIN [ContestStages] [fs] ON [fs].[Id] = [c].[FinalStage_YandexContestId];

ALTER TABLE [Contests]
    ALTER COLUMN [Title] NVARCHAR(MAX) NOT NULL;

DROP TABLE [ContestStages];

COMMIT;
GO

BEGIN TRANSACTION

ALTER TABLE [YandexContestIntegration].[dbo].[ContestUsers]
    DROP CONSTRAINT [PK_ContestUsers];

ALTER TABLE [YandexContestIntegration].[dbo].[ContestUsers]
    ADD [ParticipantId] INT NULL;

COMMIT;
GO

UPDATE [ycu]
SET [ycu].[ParticipantId] = [poa].[Id]
FROM [ProgOlymp].[dbo].[Users] [pou]
         INNER JOIN [ProgOlymp].[dbo].[Applications] [poa] ON [poa].[UserId] = [pou].[Id]
         INNER JOIN [ProgOlymp].[dbo].[Contests] [poc] ON [poc].[Id] = [poa].[ContestId]
         INNER JOIN [YandexContestIntegration].[dbo].[ContestUsers] [ycu] ON [ycu].[ContestStageId] IN ([poc].[PreliminaryStage_YandexContestId],  [poc].[FinalStage_YandexContestId]) AND [ycu].[YandexIdLogin] = [pou].[Login];

DELETE
FROM [YandexContestIntegration].[dbo].[ContestUsers]
WHERE [ParticipantId] IS NULL

UPDATE [poa]
SET [poa].[PreliminaryStageParticipation_ContestUserId] = [pycu].[ContestUserId],
    [poa].[PreliminaryStageParticipation_State]         = CASE WHEN [pycu].[ContestUserId] IS NOT NULL THEN 0 ELSE NULL END,
    [poa].[FinalStageParticipation_ContestUserId]       = [fycu].[ContestUserId],
    [poa].[FinalStageParticipation_State]               = CASE WHEN [fycu].[ContestUserId] IS NOT NULL THEN 0 ELSE NULL END
FROM [ProgOlymp].[dbo].[Applications] [poa]
         INNER JOIN [ProgOlymp].[dbo].[Contests] [poc]
                    ON [poc].[Id] = [poa].[ContestId]
         INNER JOIN [YandexContestIntegration].[dbo].[ContestUsers] [pycu]
                    ON [pycu].[ContestStageId] = [poc].[PreliminaryStage_YandexContestId] AND
                       [pycu].[ParticipantId] = [poa].[Id]
         LEFT JOIN [YandexContestIntegration].[dbo].[ContestUsers] [fycu]
                   ON [fycu].[ContestStageId] = [poc].[FinalStage_YandexContestId] AND
                      [fycu].[ParticipantId] = [poa].[Id];

ALTER TABLE [YandexContestIntegration].[dbo].[ContestUsers]
    ALTER COLUMN [ParticipantId] INT NOT NULL;
GO

ALTER TABLE [YandexContestIntegration].[dbo].[ContestUsers]
    ADD CONSTRAINT [PK_ContestUsers] PRIMARY KEY ([ContestStageId], [ParticipantId]);

ALTER TABLE [YandexContestIntegration].[dbo].[ContestUsers]
    ADD CONSTRAINT [AK_ContestUsers_ContestStageId_YandexIdLogin] UNIQUE ([ContestStageId], [YandexIdLogin]);
GO
