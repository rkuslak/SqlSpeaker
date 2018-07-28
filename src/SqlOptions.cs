// Copyright (c) Ron Kuslak. All rights reserved.

namespace SqlSpeaker.Options
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Security.Cryptography;
    using System.Xml.Linq;

    using Mono.Options;

    /// <summary>
    /// A container class for SQL Server connection options
    /// </summary>
    public class SqlOptions
    {
        /// <summary>
        /// Contains the username to connect to the server with
        /// </summary>
        private string username = null;

        private string password = null;

        /// <summary>
        /// Class to contain values for a SQL Server connection.
        /// </summary>
        public SqlOptions()
        {
            this.IntegratedSecurity = false;
        }

        /// Stores full server name and instance for connection:
        public string Server { get; set; }

        /// Stores username for connecting to server
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value;
                this.IntegratedSecurity = false;
            }
        }

        /// Stores the full plain text password of the connection.
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.password = value;
                this.IntegratedSecurity = false;
            }
        }

        /// Name of database on server to use for initial connection:
        public string Database { get; set; }

        /// Determines if we attempt log in using Windows Integrated Security.
        /// Supercedes any username or password set:
        public bool IntegratedSecurity { get; set; }

        /// <summary>
        /// Provides the current configuration as a connection string
        /// </summary>
        /// <returns>ConnectionString as String object</returns>
        public string ConnectionString
        {
            get
            {
                var con = new SqlConnectionStringBuilder();
                con.Add("server", this.Server);

                if (this.Database != null && !this.Database.Equals(string.Empty))
                {
                    con.Add("Initial Catalog", this.Database);
                }

                if (this.IntegratedSecurity)
                {
                    con.Add("Integrated Security", true);
                }
                else
                {
                    con.Add("user", this.Username);
                    con.Add("password", this.Password);
                }

                return con.ConnectionString;
            }
        }

        public void ParseOptions(string[] args)
        {
            // Parse command line options and return SqlOptions object with
            // initialization information:
            var optionCollection = new OptionSet()
            {
                { "s|Server=", "Full server and SQL instance name to connect to", n => this.Server = n },
                { "d|Databse=", "Database name to connect to on initial connection", n => this.Database = n },
                { "u|Username=", "Username of user to connect to server with", n => this.Username = n },
                { "p|Password=", "Password to use to connect to server with", n => this.Password = n },
                { "i|IntegratedSecurity=", "Use Windows integrated security (where available)", n => this.IntegratedSecurity = true },
            };

            try
            {
                optionCollection.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                throw new ArgumentException();
            }

            return;
        }

        public void ParseConfigFile()
        {
            this.ParseConfigFile(".\\config.xml");
        }

        public void ParseConfigFile(string xmlPath)
        {
            // Load the contents of the local configuration file into a
            // SqlOptions object
            xmlPath = Path.GetFullPath(xmlPath);

            if (File.Exists(xmlPath))
            {
                var xmlStream = new StreamReader(xmlPath);
                var xmlReader = new XDocument(xmlStream);
            }
            else
            {
                Console.WriteLine("No config file found.");
            }

            return;
        }

        public void WriteConfigFile()
        {
            string xmlPath = ".\\config.xml";
            xmlPath = Path.GetFullPath(xmlPath);
            var configXml = new XDocument(new XElement("appConfig", new XElement("connectionStrings", this.GetXElement())));

            Console.Write(configXml.ToString());
        }

        public XElement GetXElement()
        {
            XElement xmlElement = new XElement("connectionString");
            if (this.Server != null)
            {
                xmlElement.Add(new XAttribute("server", this.Server));
            }

            if (this.Database != null)
            {
                xmlElement.Add(new XAttribute("database", this.Database));
            }

            if (this.Username != null)
            {
                xmlElement.Add(new XAttribute("username", this.Username));
            }

            if (this.Password != null)
            {
                xmlElement.Add(new XAttribute("password", this.Password));
            }

            if (this.IntegratedSecurity)
            {
                xmlElement.Add(new XAttribute("integratedSecurity", true));
            }
            else
            {
                xmlElement.Add(new XAttribute("integratedSecurity", false));
            }

            return xmlElement;
        }
    }
}