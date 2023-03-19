using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Repository;
using System.Text.RegularExpressions;
using Dmc.Cms.Model;
using System.Net;

namespace Dmc.Cms.App.Services
{
    public class SearchService : ServiceBase, ISearchService
    {
        #region Private Fields

        private static readonly Regex _SplitWordsRegex = new Regex(@"[^\p{L}]*\p{Z}[^\p{L}]*", RegexOptions.Compiled);

        #endregion

        #region Constructor

        public SearchService(ICmsUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
        }

        #endregion

        public bool AlsoPrepareFullTextExcerpts { get; set; } = true;

        public int MaximumNumberOfSentencesForExcerpt { get; set; } = 5;

        public async Task<Search> SearchPostsAsync(string search, int page, int perPage)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return null;
            }

            string[] searchwords = _SplitWordsRegex.Split(search);
            var uniques = new HashSet<string>(searchwords);

            Search result = new Search
            {
                SearchQuery = search,
                SearchPhrases = uniques.ToArray(),
            };

            result.NumberOfResults = await UnitOfWork.PostRepository.CountSearchAsync(searchwords);

            if (result.NumberOfResults <= 0) // don't bother the search, nothing will be found.
            {
                result.Results = new PagedList<SearchResult>(new List<SearchResult>(), page, perPage, result.NumberOfResults);
                return result;
            }

            IEnumerable<Post> searchResults = await UnitOfWork.PostRepository.SearchPostsAsync(searchwords, page, perPage);

            PrepareResult(result, searchResults, page, perPage);

            return result;
        }

        private void PrepareResult(Search result, IEnumerable<Post> searchResults, int page, int perPage)
        {
            List<SearchResult> innerList = CreateFromSearchResults(searchResults);

            // Excerpt should be calculated from both ... 

            foreach (var item in innerList)
            {
                item.DescriptionExcerpt = PrepareExcerptFromString(item.DescriptionExcerpt, result.SearchPhrases);

                if (AlsoPrepareFullTextExcerpts)
                {
                    item.ContentExcerpt = PrepareExcerptFromString(item.ContentExcerpt, result.SearchPhrases);
                }
            }

            result.Results = new PagedList<SearchResult>(innerList, page, perPage, result.NumberOfResults);
        }

        private string PrepareExcerptFromString(string source, string[] searchPhrases)
        {
            string result = WebUtility.HtmlDecode(source);
            result = StripTags(result);
            result = Regex.Replace(result, "&lt;.*?&gt;", string.Empty);
            result = PrepareExcerpt(result, searchPhrases);
            return result;
        }

        private List<SearchResult> CreateFromSearchResults(IEnumerable<Post> searchResults)
        {
            List<SearchResult> result = new List<SearchResult>();

            foreach (Post item in searchResults)
            {
                result.Add(new SearchResult
                {
                    DescriptionExcerpt = item.Description,
                    ContentExcerpt = item.Content,
                    PreviewImage = item.PreviewImage,
                    Slug = item.Slug,
                    Title = item.Title
                });
            }

            return result;
        }

        private string PrepareExcerpt(string description, string[] searchwords)
        {
            description = (description ?? "").Trim();

            if (string.IsNullOrEmpty(description))
            {
                return string.Empty;
            }

            Dictionary<string, int> numberOfMatchesPerWord = new Dictionary<string, int>();

            foreach (string word in searchwords)
            {
                Regex rx = new Regex(word, RegexOptions.IgnoreCase);
                numberOfMatchesPerWord[word] = rx.Matches(description).Count;

                // surreound all matches with match class
                description = rx.Replace(description, string.Format(@"<span class=""searchMatch"">{0}</span>", word));
            }

            // Sort by value descending
            // http://stackoverflow.com/questions/289/how-do-you-sort-a-c-sharp-dictionary-by-value
            List<KeyValuePair<string, int>> matchesList = numberOfMatchesPerWord.ToList();
            matchesList.Sort((x, y) => y.Value.CompareTo(x.Value));

            //string mostMatchedWord = matchesList[0].Key;

            List<string> relevantSentences = new List<string>();

            foreach (KeyValuePair<string, int> kvp in matchesList)
            {
                relevantSentences.AddRange(GetSentences(description, kvp.Key));
            }

            description = "";
            string sentence = null;

            for (int i = 0; i < relevantSentences.Count; i++)
            {
                sentence = relevantSentences[i];
                string separator = (relevantSentences.Count - 1) != i
                    ? "..."
                    : string.Empty;

                description = string.Concat(description, string.Format(" {0} {1} ", sentence, separator));
            }

            return description;
        }

        private List<string> GetSentences(string text, string word)
        {
            char[] delimiters = new char[] { '.', '!', '?' };
            var sentences = text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            var matches = (from sentence in sentences
                           where sentence.Contains(word)
                           select sentence).Take(MaximumNumberOfSentencesForExcerpt);

            return matches.ToList();
        }

        private static string StripTags(string toStrip)
        {
            return Regex.Replace(toStrip, @"<(.|\n)*?>", string.Empty);
        }
    }
}
