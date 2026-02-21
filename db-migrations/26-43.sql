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
    ADD [Title] NVARCHAR(MAX) NOT NULL DEFAULT N'';

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_ContestUserId] BIGINT NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_Finish] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_Start] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [FinalStageParticipation_State] INT NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_ContestUserId] BIGINT NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_Finish] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_Start] DATETIMEOFFSET NULL;

ALTER TABLE [Applications]
    ADD [PreliminaryStageParticipation_State] INT NULL;

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

DROP TABLE [ContestStages];

COMMIT;
GO
