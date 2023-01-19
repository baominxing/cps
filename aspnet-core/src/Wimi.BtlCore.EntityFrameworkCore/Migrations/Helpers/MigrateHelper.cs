using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace Wimi.BtlCore.Migrations.Helpers
{
    public static class MigrateHelper
    {
        public static void SqlFile(this MigrationBuilder migrationBuilder, string sqlFile)
        {
            NotEmpty(sqlFile, nameof(sqlFile));
            
            if (!Path.IsPathRooted(sqlFile))
            {
                sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sqlFile);
            }

            migrationBuilder.Sql(File.ReadAllText(sqlFile));
        }

        private static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{parameterName}-ArgumentIsNullOrWhitespace");

            }

            return value;
        }

        public static string GetSqlScriptBasePath()
        {
            var devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations");
            var releasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Migrations");
            
            if (Directory.Exists(devPath))
            {
                return devPath;
            }

            if (Directory.Exists(releasePath))
            {
                return releasePath;
            }

            return null;
        }
    }
}
