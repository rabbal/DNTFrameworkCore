using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class ActivityType : Enumeration
    {
        public static readonly ActivityType PreparationPrint = new ActivityType(1, nameof(PreparationPrint));
        public static readonly ActivityType PrintInvoice = new ActivityType(2, nameof(PrintInvoice));
        public static readonly ActivityType SendEmail = new ActivityType(3, nameof(SendEmail));
        public static readonly ActivityType SendNote = new ActivityType(4, nameof(SendEmail));

        private ActivityType() //Required for ORM
        {
        }

        private ActivityType(int value, string name) : base(value, name)
        {
        }
    }
}