// Copyright (c) Ron Kuslak. All rights reserved.

namespace SqlSpeaker
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Security.Cryptography;
    using System.Xml.Linq;

    using Mono.Options;

    public class SqlSpeaker
    {
        public static void Main(string[] args)
        {
            var sqlOptions = new Options.SqlOptions();
            var sql = new SqlConnection();
            var con = new SqlConnectionStringBuilder();

            sqlOptions.ParseConfigFile();
            sqlOptions.ParseOptions(args);

            sql.ConnectionString = sqlOptions.ConnectionString;
            sql.Open();

            sqlOptions.WriteConfigFile();
        }
    }
}
