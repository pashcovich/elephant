﻿using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Takenet.SimplePersistence.Sql.Mapping
{
    /// <summary>
    /// Provides scheme information for a SQL table.
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Gets the table name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the names of the table key columns.
        /// </summary>
        string[] KeyColumns { get; }

        /// <summary>
        /// Gets the columns of the table with its types.
        /// The key columns must be included.
        /// </summary>
        IDictionary<string, SqlType> Columns { get; }
    }
}