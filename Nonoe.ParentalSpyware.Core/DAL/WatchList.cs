// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Watchlist.cs" company="Nonoe">
//   Created by JOK 2013
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Nonoe.ParentalSpyware.Core.DAL
{
    using System;
    using System.Configuration;
    using System.Data.SQLite;
    using System.Globalization;

    /// <summary>
    /// Word watch list
    /// </summary>
    public class WatchList
    {
        /// <summary>
        /// SQL lite connection
        /// </summary>
        private SQLiteConnection sqlCon;

        /// <summary>
        /// SQL lite command
        /// </summary>
        private SQLiteCommand sqlCmd;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchList"/> class. 
        /// </summary>
        public WatchList()
        {
            const string Sql = "CREATE TABLE IF NOT EXISTS wordWatchlist(" +
                               "id INTEGER PRIMARY KEY AUTOINCREMENT," +
                               "word VARCHAR(255) NOT NULL," +
                               "active bit NOT NULL" +
                               ");";
            this.ExecuteQuery(Sql);
        }

        /// <summary>
        /// Gets the connection string for the SQL Lite database.
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                return "Data Source=" + ConfigurationManager.AppSettings["Nonoe.ParentalSpyware.SQLLite"].ToString(CultureInfo.InvariantCulture) + ";Version=3;New=False;Compress=True;";
            }
        }

        /// <summary>
        /// Adds a word to the watch list.
        /// </summary>
        /// <param name="word">The word to add.</param>
        /// <returns>Success / False</returns>
        public bool AddWordToWatchList(string word)
        {
            var sql = "INSERT wordWatchlist(word, active) VALUES " + word + ", 1";

            try
            {
                this.ExecuteQuery(sql);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Execute the query on the SQL Lite DB.
        /// </summary>
        /// <param name="txtQuery">The txt query.</param> 
        private void ExecuteQuery(string txtQuery)
        {
            using (this.sqlCon = new SQLiteConnection(ConnectionString))
            {
                this.sqlCon.Open();
                using (this.sqlCmd = this.sqlCon.CreateCommand())
                {
                    this.sqlCmd.CommandText = txtQuery;
                    this.sqlCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
