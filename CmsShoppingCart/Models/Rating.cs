namespace CmsShoppingCart.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int ContentId { get; set; } // The ID of the product being rated

        public int UserId { get; set; } // The user who provided the rating
        public int RatingValue { get; set; } // rating value
    }
}
