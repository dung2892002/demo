namespace Cukcuk.Core.DTOs
{
    public class StatisticalGenderResponse
    {
        public int Total { get; set; }
        public int TotalMale { get; set; }
        public int TotalFemale { get; set; }
        public int TotalUnknown { get; set; }
        public decimal PercentMale
        {
            get
            {
                return Math.Round((decimal)TotalMale / Total, 4) * 100;
            }
            set { }
            
        }
        public decimal PercentFemale
        {
            get
            {
                return Math.Round((decimal)TotalFemale / Total, 4) * 100;
            }
            set { }
        }
        public decimal PercentUnknown
        {
            get
            {
                return 100 - PercentFemale - PercentMale;
            }
            set { }
        }
    }
}
