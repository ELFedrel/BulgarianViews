using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Common
{
    public static class EntityConstants
    {
        public static class User
        {
            public const int BioMinLength = 5;
            public const int BioMaxLength = 500;
            public const int UserNameMinLength = 3;
            public const int UserNameMaxLength = 20;
            //public const string DefaultProfilePictureURL = "/images/default-profile.png";
        }

        // LocationPost Entity Constants
        public static class LocationPost
        {
            public const int TitleMinLength = 5;
            public const int TitleMaxLength = 150;
            public const int DescriptionMinLength = 20;
            public const int DescriptionMaxLength = 2000;

        }

        // Location Entity Constants
        public static class Location
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 100;
        }

        // Comment Entity Constants
        public static class Comment
        {
            public const int ContentMinLength = 5;
            public const int ContentMaxLength = 300;
        }

       
        public static class Tag
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

        }

        public static class Rating
        {
            public const int MinRating = 1;
            public const int MaxRating = 5;
        }
    }
}
