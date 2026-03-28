BEGIN TRANSACTION;
ALTER TABLE [Contests]
    ADD [PreliminaryStage_State] INT NULL;

ALTER TABLE [Contests]
    ADD [FinalStage_State] INT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260328175600_Add contest state', N'10.0.5');

COMMIT;
GO

UPDATE [Contests]
SET [PreliminaryStage_State] = 0,
    [FinalStage_State] = 0
