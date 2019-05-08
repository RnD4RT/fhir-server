//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Microsoft.Health.Fhir.SqlServer.Features.Schema.Model
{
    using Microsoft.Health.Fhir.SqlServer.Features.Storage;

    internal class V1
    {
        internal readonly static SchemaVersionTable SchemaVersion = new SchemaVersionTable();
        internal readonly static SearchParamTable SearchParam = new SearchParamTable();
        internal readonly static ResourceTypeTable ResourceType = new ResourceTypeTable();
        internal readonly static SystemTable System = new SystemTable();
        internal readonly static QuantityCodeTable QuantityCode = new QuantityCodeTable();
        internal readonly static ResourceTable Resource = new ResourceTable();
        internal readonly static SelectCurrentSchemaVersionProcedure SelectCurrentSchemaVersion = new SelectCurrentSchemaVersionProcedure();
        internal readonly static UpsertSchemaVersionProcedure UpsertSchemaVersion = new UpsertSchemaVersionProcedure();
        internal readonly static UpsertResourceProcedure UpsertResource = new UpsertResourceProcedure();
        internal readonly static ReadResourceProcedure ReadResource = new ReadResourceProcedure();
        internal readonly static HardDeleteResourceProcedure HardDeleteResource = new HardDeleteResourceProcedure();
        internal class SchemaVersionTable : Table
        {
            internal SchemaVersionTable(): base("dbo.SchemaVersion")
            {
            }

            internal readonly IntColumn Version = new IntColumn("Version");
            internal readonly VarCharColumn Status = new VarCharColumn("Status", 10);
        }

        internal class SearchParamTable : Table
        {
            internal SearchParamTable(): base("dbo.SearchParam")
            {
            }

            internal readonly SmallIntColumn SearchParamId = new SmallIntColumn("SearchParamId");
            internal readonly VarCharColumn Uri = new VarCharColumn("Uri", 128);
        }

        internal class ResourceTypeTable : Table
        {
            internal ResourceTypeTable(): base("dbo.ResourceType")
            {
            }

            internal readonly SmallIntColumn ResourceTypeId = new SmallIntColumn("ResourceTypeId");
            internal readonly NVarCharColumn Name = new NVarCharColumn("Name", 50);
        }

        internal class SystemTable : Table
        {
            internal SystemTable(): base("dbo.System")
            {
            }

            internal readonly IntColumn SystemId = new IntColumn("SystemId");
            internal readonly NVarCharColumn Value = new NVarCharColumn("Value", 256);
        }

        internal class QuantityCodeTable : Table
        {
            internal QuantityCodeTable(): base("dbo.QuantityCode")
            {
            }

            internal readonly IntColumn QuantityCodeId = new IntColumn("QuantityCodeId");
            internal readonly NVarCharColumn Value = new NVarCharColumn("Value", 256);
        }

        internal class ResourceTable : Table
        {
            internal ResourceTable(): base("dbo.Resource")
            {
            }

            internal readonly SmallIntColumn ResourceTypeId = new SmallIntColumn("ResourceTypeId");
            internal readonly VarCharColumn ResourceId = new VarCharColumn("ResourceId", 64);
            internal readonly IntColumn Version = new IntColumn("Version");
            internal readonly BitColumn IsHistory = new BitColumn("IsHistory");
            internal readonly BigIntColumn ResourceSurrogateId = new BigIntColumn("ResourceSurrogateId");
            internal readonly DateTime2Column LastUpdated = new DateTime2Column("LastUpdated", 7);
            internal readonly BitColumn IsDeleted = new BitColumn("IsDeleted");
            internal readonly NullableVarCharColumn RequestMethod = new NullableVarCharColumn("RequestMethod", 10);
            internal readonly VarBinaryColumn RawResource = new VarBinaryColumn("RawResource", -1);
        }

        internal class SelectCurrentSchemaVersionProcedure : StoredProcedure
        {
            internal SelectCurrentSchemaVersionProcedure(): base("dbo.SelectCurrentSchemaVersion")
            {
            }

            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.SelectCurrentSchemaVersion";
            }
        }

        internal class UpsertSchemaVersionProcedure : StoredProcedure
        {
            internal UpsertSchemaVersionProcedure(): base("dbo.UpsertSchemaVersion")
            {
            }

            private readonly IntColumn _version = new IntColumn("@version");
            private readonly VarCharColumn _status = new VarCharColumn("@status", 10);
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.Int32 version, System.String status)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.UpsertSchemaVersion";
                command.Parameters.AddFromColumn(_version, version, "@version");
                command.Parameters.AddFromColumn(_status, status, "@status");
            }
        }

        internal class UpsertResourceProcedure : StoredProcedure
        {
            internal UpsertResourceProcedure(): base("dbo.UpsertResource")
            {
            }

            private readonly SmallIntColumn _resourceTypeId = new SmallIntColumn("@resourceTypeId");
            private readonly VarCharColumn _resourceId = new VarCharColumn("@resourceId", 64);
            private readonly NullableIntColumn _eTag = new NullableIntColumn("@eTag");
            private readonly BitColumn _allowCreate = new BitColumn("@allowCreate");
            private readonly BitColumn _isDeleted = new BitColumn("@isDeleted");
            private readonly DateTimeOffsetColumn _updatedDateTime = new DateTimeOffsetColumn("@updatedDateTime", 7);
            private readonly BitColumn _keepHistory = new BitColumn("@keepHistory");
            private readonly VarCharColumn _requestMethod = new VarCharColumn("@requestMethod", 10);
            private readonly VarBinaryColumn _rawResource = new VarBinaryColumn("@rawResource", -1);
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.Int16 resourceTypeId, System.String resourceId, System.Nullable<System.Int32> eTag, System.Boolean allowCreate, System.Boolean isDeleted, System.DateTimeOffset updatedDateTime, System.Boolean keepHistory, System.String requestMethod, System.IO.Stream rawResource)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.UpsertResource";
                command.Parameters.AddFromColumn(_resourceTypeId, resourceTypeId, "@resourceTypeId");
                command.Parameters.AddFromColumn(_resourceId, resourceId, "@resourceId");
                command.Parameters.AddFromColumn(_eTag, eTag, "@eTag");
                command.Parameters.AddFromColumn(_allowCreate, allowCreate, "@allowCreate");
                command.Parameters.AddFromColumn(_isDeleted, isDeleted, "@isDeleted");
                command.Parameters.AddFromColumn(_updatedDateTime, updatedDateTime, "@updatedDateTime");
                command.Parameters.AddFromColumn(_keepHistory, keepHistory, "@keepHistory");
                command.Parameters.AddFromColumn(_requestMethod, requestMethod, "@requestMethod");
                command.Parameters.AddFromColumn(_rawResource, rawResource, "@rawResource");
            }
        }

        internal class ReadResourceProcedure : StoredProcedure
        {
            internal ReadResourceProcedure(): base("dbo.ReadResource")
            {
            }

            private readonly SmallIntColumn _resourceTypeId = new SmallIntColumn("@resourceTypeId");
            private readonly VarCharColumn _resourceId = new VarCharColumn("@resourceId", 64);
            private readonly NullableIntColumn _version = new NullableIntColumn("@version");
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.Int16 resourceTypeId, System.String resourceId, System.Nullable<System.Int32> version)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.ReadResource";
                command.Parameters.AddFromColumn(_resourceTypeId, resourceTypeId, "@resourceTypeId");
                command.Parameters.AddFromColumn(_resourceId, resourceId, "@resourceId");
                command.Parameters.AddFromColumn(_version, version, "@version");
            }
        }

        internal class HardDeleteResourceProcedure : StoredProcedure
        {
            internal HardDeleteResourceProcedure(): base("dbo.HardDeleteResource")
            {
            }

            private readonly SmallIntColumn _resourceTypeId = new SmallIntColumn("@resourceTypeId");
            private readonly VarCharColumn _resourceId = new VarCharColumn("@resourceId", 64);
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.Int16 resourceTypeId, System.String resourceId)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.HardDeleteResource";
                command.Parameters.AddFromColumn(_resourceTypeId, resourceTypeId, "@resourceTypeId");
                command.Parameters.AddFromColumn(_resourceId, resourceId, "@resourceId");
            }
        }
    }
}