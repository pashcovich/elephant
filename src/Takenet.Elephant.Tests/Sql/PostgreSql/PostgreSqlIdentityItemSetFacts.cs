﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Takenet.Elephant.Tests.Sql.PostgreSql
{
    [Collection(nameof(PostgreSql)), Trait("Category", nameof(PostgreSql))]
    public class PostgreSqlIdentityItemSetFacts : SqlIdentityItemSetFacts
    {
        public PostgreSqlIdentityItemSetFacts(PostgreSqlFixture fixture) : base(fixture)
        {
        }
    }
}
