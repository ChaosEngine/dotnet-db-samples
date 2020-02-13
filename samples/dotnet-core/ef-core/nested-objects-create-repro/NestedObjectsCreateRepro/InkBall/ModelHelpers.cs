using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace InkBall.Module.Model
{
	/// <summary>
	///     Converts <see cref="DateTime" /> using <see cref="DateTime.ToBinary" />. This
	///     will preserve the <see cref="DateTimeKind" />.
	/// </summary>
	internal class DateTimeToBytesConverter : ValueConverter<DateTime, byte[]>
	{
		/// <summary>
		///     A <see cref="ValueConverterInfo" /> for the default use of this converter.
		/// </summary>
		public static ValueConverterInfo DefaultInfo { get; }
			= new ValueConverterInfo(typeof(DateTime), typeof(byte[]), i => new DateTimeToBytesConverter(i.MappingHints));

		static DateTime UnixTime => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		static double DateTimeToUnixTimestamp(DateTime dateTime)
		{
			return (TimeZoneInfo.ConvertTimeToUtc(dateTime) - UnixTime).TotalSeconds;
		}

		/// <summary>
		///     Creates a new instance of this converter.
		/// </summary>
		/// <param name="mappingHints">
		///     Hints that can be used by the <see cref="ITypeMappingSource" /> to create data types with appropriate
		///     facets for the converted data.
		/// </param>
		public DateTimeToBytesConverter(ConverterMappingHints mappingHints = null)
			: base(
				dt => FromType2Db(dt),
				bytes => FromDb2Type(bytes),
				mappingHints)
		{
		}

		static DateTime FromDb2Type(byte[] bytes)
		{
			try
			{
				//return (bytes == null || bytes.Length <= 0) ?
				//	DateTime.MinValue : DateTime.FromBinary(BitConverter.ToInt64(bytes, 0));

				if (bytes == null || bytes.Length <= 0)
					return DateTime.MinValue;

				bytes.AsSpan().Reverse();
				var num = BitConverter.ToInt64(bytes, 0);
				var res = /*UnixTime*/DateTime.MinValue + TimeSpan.FromTicks(num);

				return res;
			}
			catch (Exception)
			{
				return DateTime.MinValue;
			}
		}

		static byte[] FromType2Db(DateTime dt)
		{
			try
			{
				//return (dt == null || dt == DateTime.MinValue) ?
				//		new byte[] { } : BitConverter.GetBytes(dt.ToBinary());

				if (dt == null || dt <= DateTime.MinValue)
					return new byte[] { };

				TimeSpan ts = dt - DateTime.MinValue;
				var res = BitConverter.GetBytes(ts.Ticks);
				res.Reverse();

				return res;
			}
			catch (Exception)
			{
				return new byte[] { };
			}
		}
	}

	internal static class MigrationExtensions
	{
		internal static MigrationBuilder CreateTimestampTrigger(this MigrationBuilder migrationBuilder, IEntityType entityType,
			string timeStampColumnName, string primaryKey)
		{
			switch (migrationBuilder.ActiveProvider)
			{
				case "Microsoft.EntityFrameworkCore.Sqlite":
					var tableName = entityType.Relational().TableName;
					//var primaryKey = entityType.FindPrimaryKey();

					string command =
$@"CREATE TRIGGER {tableName}_update_{timeStampColumnName}_Trigger
AFTER UPDATE ON {tableName}
BEGIN
	UPDATE {tableName} SET {timeStampColumnName} = datetime(CURRENT_TIMESTAMP, 'localtime') WHERE {primaryKey} = NEW.{primaryKey};
END;";
					//Console.Error.WriteLine($"executing '{command}'");
					migrationBuilder.Sql(command);
					break;

				case "Microsoft.EntityFrameworkCore.SqlServer":
					tableName = entityType.Relational().TableName;
					//var primaryKey = entityType.FindPrimaryKey();

					command =
$@"CREATE TRIGGER [dbo].[{tableName}_update_{timeStampColumnName}_Trigger] ON [dbo].[{tableName}]
	AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

	UPDATE {tableName}
	SET {timeStampColumnName} = GETDATE()
	FROM {tableName} t
	INNER JOIN INSERTED i ON i.{primaryKey} = t.{primaryKey}
	WHERE t.{primaryKey} = i.{primaryKey}
END";
					//Console.Error.WriteLine($"executing '{command}'");
					migrationBuilder.Sql(command);
					break;

				case "Oracle.EntityFrameworkCore":
					tableName = entityType.Relational().TableName;
					//var primaryKey = entityType.FindPrimaryKey();

					command =
$@"CREATE OR REPLACE TRIGGER ""{tableName}_update_{timeStampColumnName}_Trigger""
	BEFORE UPDATE ON ""{tableName}""
	FOR EACH ROW
BEGIN
	:NEW.""{timeStampColumnName}"" := SYSTIMESTAMP;
END;";
					//Console.Error.WriteLine($"executing '{command}'");
					migrationBuilder.Sql(command);
					break;

				case "Pomelo.EntityFrameworkCore.MySql":
				case "Npgsql.EntityFrameworkCore.PostgreSQL":
				default:
					break;
			}

			return migrationBuilder;
		}

		internal static MigrationBuilder DropTimestampTrigger(this MigrationBuilder migrationBuilder, IEntityType entityType, string timeStampColumnName)
		{
			switch (migrationBuilder.ActiveProvider)
			{
				case "Microsoft.EntityFrameworkCore.Sqlite":
					var tableName = entityType.Relational().TableName;

					string command = $@"DROP TRIGGER IF EXISTS {tableName}_update_{timeStampColumnName}_Trigger;";

					//Console.Error.WriteLine($"executing '{command}'");
					migrationBuilder.Sql(command);
					break;

				case "Microsoft.EntityFrameworkCore.SqlServer":
					tableName = entityType.Relational().TableName;

					command = $@"DROP TRIGGER [dbo].[{tableName}_update_{timeStampColumnName}_Trigger];";

					//Console.Error.WriteLine($"executing '{command}'");
					migrationBuilder.Sql(command);
					break;

				case "Oracle.EntityFrameworkCore":
					tableName = entityType.Relational().TableName;

					command = $@"
DECLARE
  l_count integer;
BEGIN
  SELECT COUNT(*) INTO l_count FROM user_triggers
  WHERE trigger_name = '{tableName}_update_{timeStampColumnName}_Trigger';

  IF l_count > 0 THEN
     EXECUTE IMMEDIATE 'DROP TRIGGER ""{tableName}_update_{timeStampColumnName}_Trigger""';
  END IF;
END;";
					//Console.Error.WriteLine($"executing '{command}'");
					migrationBuilder.Sql(command);
					break;

				case "Pomelo.EntityFrameworkCore.MySql":
				case "Npgsql.EntityFrameworkCore.PostgreSQL":
				default:
					break;
			}

			return migrationBuilder;
		}
	}
}
