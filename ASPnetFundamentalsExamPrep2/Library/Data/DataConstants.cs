namespace Library.Data
{
    public static class DataConstants
    {
        public const int BookTitleMinLength = 10;
        public const int BookTitleMaxLength = 50;

        public const int BookAuthorMinLength = 5;
        public const int BookAuthorMaxLength = 50;

        public const int BookDescriptionMinLength = 5;
        public const int BookDescriptionMaxLength = 5000;

        public const decimal BookRatingMinValue = 0.00M;
        public const decimal BookRatingMaxValue = 10.00M;

        public const int CategoryNameMinLength = 5;
        public const int CategoryNameMaxLength = 50;
    }
}
