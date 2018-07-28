// QueryParcer.cs - A simple state engine to parse a TSQL query file and
// tokenize it into "batches" to send to the server similar to SQLCMD.EXE.
//
// Copyright (c) Ron Kuslak. All rights reserved.
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation version 2.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

namespace SqlSpeaker
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// QueryParser - A helper class to take in the fulltext string of a file,
    /// and split it into "batches" for later execution against SQL Server.
    /// </summary>
    public class QueryParser
    {
        /// <summary>
        /// Internal list of queries.
        /// </summary>
        private List<string> queries = new List<string>();

        /// <summary>
        /// Initialization for class
        /// </summary>
        /// <param name="query">String of TSQL script to parse into batches</param>
        public QueryParser(string query)
        {
            // TODO: Allow configuration to retain comments>
            // Parses passed string into a array of queries
            string currentQuery = string.Empty;

            for (var charIndex = 0; charIndex < query.Length; charIndex++)
            {
                switch (query[charIndex])
                {
                    case '/':
                    {
                        // '/* */' - box comments; seek til matching close or
                        // end of file.
                        if (charIndex < query.Length - 2 && query[charIndex + 1] == '*')
                        {
                            // Advance past start of comment, to first
                            // possible non-comment block starting character,
                            // and iterate until EOF or end of comment
                            // block:
                            charIndex += 3;
                            while (charIndex < query.Length - 1 &&
                                    !(query[charIndex - 1] == '*' &&
                                    query[charIndex] == '/'))
                            {
                                charIndex++;
                            }
                        }
                        else
                        {
                            currentQuery += query[charIndex];
                        }

                        break;
                    }

                    case '-':
                    {
                        // '--' - single line comments; scan til end of line.
                        if (charIndex < query.Length - 2 && query[charIndex + 1] == '-')
                        {
                            // Loop until new line or EOF:
                            charIndex += 2;
                            while (charIndex < query.Length - 1 &&
                                    query[charIndex + 1] != '\n')
                            {
                                charIndex++;
                            }
                        }
                        else
                        {
                            currentQuery += query[charIndex];
                        }

                        break;
                    }

                    case '"':
                    case '\'':
                    case '[':
                    {
                        // ", ', and [ - beginning of expected block. Scan til
                        // matching block end mark or end of file.
                        // String or column indicator; loop until closing
                        // character:
                        var currentChar = query[charIndex];
                        currentQuery += query[charIndex];

                        // Special case for '[]' column identifiers:
                        if (currentChar == '[')
                        {
                            currentChar = ']';
                        }

                        while (charIndex < query.Length && query[charIndex] != currentChar)
                        {
                            charIndex++;
                            currentQuery += query[charIndex];
                        }

                        break;
                    }

                    case '\n':
                    {
                        // Search next line to see if GO statements are used.
                        // Break into seperate batches if "GO" statements break
                        // up the query into multiple batches:
                        // TODO: Can have GO [0-9]* to indicate multiple runs of
                        // batch. We still need to add this. Clone previous
                        // batch and add to queries queue?
                        currentQuery += query[charIndex];

                        var tempIndex = charIndex + 1;
                        while (tempIndex < query.Length - 1 &&
                               (query[tempIndex] == ' ' | query[tempIndex] == '\t'))
                        {
                            tempIndex++;
                        }

                        if (tempIndex < query.Length - 2 &&
                               (query[tempIndex] == 'G' | query[tempIndex] == 'g') &&
                               (query[tempIndex + 1] == 'O' | query[tempIndex + 1] == 'o'))
                        {
                            // Is a GO statement unless anything other that
                            // 0 - 9 or ; appear til next line. If "illegal"
                            // characters appear, will break method and leave
                            // state engine to parse it starting with first
                            // character after newline. We could handle this
                            // differently but we are leaving this behavior so
                            // it "mirrors" SQLCMD.
                            tempIndex += 2;
                            var queryRepeaterString = string.Empty;

                            do
                            {
                                // End of GO statement line:
                                if (query[tempIndex] == '\n' | query[tempIndex] == '\r' | query[tempIndex] == ';')
                                {
                                    this.Queries.Add(currentQuery);
                                    charIndex = tempIndex;
                                    currentQuery = string.Empty;
                                    break;
                                }

                                // Number part for repeatition of query:
                                if (query[tempIndex] >= '0' && query[tempIndex] <= '9')
                                {
                                    queryRepeaterString += query[tempIndex];
                                }

                                // If it is a comment, treat as EOL for GO statement:
                                if (tempIndex < query.Length - 2 &&
                                    ((query[tempIndex] == '/' && query[tempIndex + 1] == '*') ||
                                        (query[tempIndex] == '-' && query[tempIndex + 1] == '-')))
                                {
                                    // Ensure next path will start with comment
                                    // and stamp over query
                                    this.Queries.Add(currentQuery);
                                    charIndex = tempIndex - 1;
                                    currentQuery = string.Empty;
                                    break;
                                }

                                tempIndex++;
                            }
                            while (tempIndex < query.Length - 1);
                        }

                        break;
                    }

                    default:
                    {
                        currentQuery += query[charIndex];
                        break;
                    }
                }
            }

            this.Queries.Add(currentQuery);
        }

        public List<string> Queries
        {
            get { return this.queries; }
        }
    }
}