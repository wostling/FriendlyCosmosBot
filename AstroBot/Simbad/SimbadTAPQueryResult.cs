using System;
using System.Linq;
using System.Collections.Generic;
using AstroBot.Objects;
using AstroBot.Objects.AstronomicalObjects;

namespace AstroBot.Simbad
{
    /// <summary>
    /// An object representing the result of a SIMBAD query
    /// </summary>
    public class SimbadTAPQueryResult
    {
        public IReadOnlyList<AstronomicalObject> AstronomicalObjects { get; private set; }

        /// <summary>
        /// Initializes the result from the result of a TAP query
        /// http://simbad.u-strasbg.fr/simbad/sim-tap
        /// </summary>
        /// <param name="tapResultText"></param>
        public SimbadTAPQueryResult(string tapResultText)
        {
            InitializeFromTapResultText(tapResultText);
        }

        /// <summary>
        /// Parse the result text and create the astronomical objects
        /// </summary>
        /// <param name="tapResultText"></param>
        private void InitializeFromTapResultText(string tapResultText)
        {
            var astronomicalObjects = new List<AstronomicalObject>();

            var rows = tapResultText.Replace("\r", "").Split("\n");

            // If less than 3 rows --> no rows in the result (since header has 2 rows)
            if (rows.Length < 3)
                return;

            var headerRow = rows.First().ToUpper();
            var columnNames = headerRow.Split("|", StringSplitOptions.None).Select(x => x.Trim());

            foreach (var row in rows.Skip(2))
            {
                if (string.IsNullOrWhiteSpace(row))
                    continue;

                var curRowColumns = row.Split("|", StringSplitOptions.None).Select(x => x.Trim());

                var properties = ParseProperties(columnNames.ToArray(), curRowColumns.ToArray());
                var shortType = properties.ContainsKey("TYPESHORT") ? Convert.ToString(properties["TYPESHORT"]) : string.Empty;

                // Depending on the type, create objects of different types
                if (shortType.Contains('*'))
                {
                    astronomicalObjects.Add(new Star(properties));
                }
                else
                {
                    astronomicalObjects.Add(new AstronomicalObject(properties));
                }
            }

            AstronomicalObjects = astronomicalObjects;
        }

        /// <summary>
        /// Parses the row into properties for the astronomical object
        /// </summary>
        /// <param name="columnNames"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private Dictionary<string, object> ParseProperties(string[] columnNames, string[] values)
        {
            var properties = new Dictionary<string, object>();

            for (var i = 0; i < columnNames.Length; i++)
            {
                var column = columnNames[i];
                var value = values[i];

                // strings are contained within quotes ""
                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    properties.Add(
                        key: column,
                        value: value.Substring(1, value.Length - 2));
                }
                // Else its a number (double)
                else if (double.TryParse(value, out double parsedDouble))
                {
                    properties.Add(
                        key: column,
                        value: parsedDouble);
                }
            }

            return properties;
        }
    }
}