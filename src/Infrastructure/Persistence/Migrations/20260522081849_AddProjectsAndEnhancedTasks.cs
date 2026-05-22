using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectsAndEnhancedTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add new columns to TBL_Task (guarded — partial apply may have created them already)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'Description')
                    ALTER TABLE [TBL_Task] ADD [Description] nvarchar(2000) NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'DueDate')
                    ALTER TABLE [TBL_Task] ADD [DueDate] datetime2 NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'Priority')
                    ALTER TABLE [TBL_Task] ADD [Priority] int NOT NULL DEFAULT 0;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'ProjectId')
                    ALTER TABLE [TBL_Task] ADD [ProjectId] int NULL;
            ");

            // Create TBL_Project if it does not exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'TBL_Project')
                BEGIN
                    CREATE TABLE [TBL_Project] (
                        [Id]          int           NOT NULL IDENTITY,
                        [Name]        nvarchar(100) NOT NULL,
                        [Description] nvarchar(500) NULL,
                        [Color]       nvarchar(20)  NOT NULL DEFAULT N'#003087',
                        [IsDeleted]   bit           NOT NULL DEFAULT CAST(0 AS bit),
                        [CreatedDate] datetime2     NOT NULL,
                        [UpdatedDate] datetime2     NULL,
                        [UserId]      int           NOT NULL,
                        CONSTRAINT [PK_TBL_Project] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_TBL_Project_TBL_User_UserId]
                            FOREIGN KEY ([UserId]) REFERENCES [TBL_User] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            // Create indexes (guarded)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TBL_Task_ProjectId')
                    CREATE INDEX [IX_TBL_Task_ProjectId] ON [TBL_Task] ([ProjectId]);
            ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'TBL_Project') IS NOT NULL
                AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TBL_Project_UserId_IsDeleted')
                    CREATE INDEX [IX_TBL_Project_UserId_IsDeleted] ON [TBL_Project] ([UserId], [IsDeleted]);
            ");

            // Add FK — this is the only step that failed previously; NO ACTION avoids cascade-cycle error
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TBL_Task_TBL_Project_ProjectId')
                    ALTER TABLE [TBL_Task] ADD CONSTRAINT [FK_TBL_Task_TBL_Project_ProjectId]
                        FOREIGN KEY ([ProjectId]) REFERENCES [TBL_Project] ([Id]);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TBL_Task_TBL_Project_ProjectId')
                    ALTER TABLE [TBL_Task] DROP CONSTRAINT [FK_TBL_Task_TBL_Project_ProjectId];
            ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'TBL_Project') IS NOT NULL DROP TABLE [TBL_Project];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_TBL_Task_ProjectId')
                    DROP INDEX [IX_TBL_Task_ProjectId] ON [TBL_Task];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'Description')
                    ALTER TABLE [TBL_Task] DROP COLUMN [Description];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'DueDate')
                    ALTER TABLE [TBL_Task] DROP COLUMN [DueDate];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'Priority')
                    ALTER TABLE [TBL_Task] DROP COLUMN [Priority];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'TBL_Task') AND name = N'ProjectId')
                    ALTER TABLE [TBL_Task] DROP COLUMN [ProjectId];
            ");
        }
    }
}
