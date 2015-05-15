﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Takenet.SimplePersistence.Sql;
using Takenet.SimplePersistence.Sql.Mapping;
using Xunit;

namespace Takenet.SimplePersistence.Tests.Sql
{
    [Collection("Sql")]
    public class SqlGuidItemItemSetMapFacts : GuidItemItemSetMapFacts
    {
        private readonly SqlConnectionFixture _fixture;

        public SqlGuidItemItemSetMapFacts(SqlConnectionFixture fixture)
        {
            _fixture = fixture;
        }

        public override IItemSetMap<Guid, Item> Create()
        {
            var databaseDriver = new SqlDatabaseDriver();
            var columns = typeof(Item)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToSqlColumns();
            columns.Add("Key", new SqlType(DbType.Guid));
            var table = new Table("GuidItems", new[] { "Key", nameof(Item.GuidProperty) }, columns);
            _fixture.DropTable(table.Name);
            var keyMapper = new ValueMapper<Guid>("Key");
            var valueMapper = new TypeMapper<Item>(table);
            return new SqlSetMap<Guid, Item>(databaseDriver, _fixture.ConnectionString, table, keyMapper, valueMapper);
        }
    }
}