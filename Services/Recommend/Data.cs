using Entities.Post;
using System.Collections.Generic;
using System.Linq;

namespace Services.Recommend
{
    public class PostRatingPrediction
    {
        public float Label;
        public float Score;
    }

    public static class Posts
    {
        public static List<Post> All = new List<Post>();

        /// <summary>
        /// Get a single post.
        /// </summary>
        /// <param name="id">The identifier of the post to get.</param>
        /// <returns>The post instance corresponding to the specified identifier.</returns>        
        public static Post Get(int id)
        {
            return All.Single(m => m.Id == id);
        }

        // ... I removed all private members for brevity ...
    }
}