// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WatchList.cs" company="Nonoe">
//   KRAPP
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Nonoe.ParentalSpyware.Core.Business
{
    /// <summary>
    /// Watch list
    /// </summary>
    public class WatchList
    {
        /// <summary>
        /// Core watch list object.
        /// </summary>
        private readonly WatchList watchList = new WatchList();

        /// <summary>
        /// Adds a word to the watch list
        /// </summary>
        /// <param name="word">Word to add</param>
        public void AddWord(string word)
        {
            this.watchList.AddWord(word);
        }
    }
}
