namespace HomeBankingMinHub.Models.DTO_s
{
    public class LoanApplicationDTO
    {
        public long LoanId { get; set; }
        public double Amount { get; set; }
        public string Payments { get; set; }
        public string Type { get; set; }
        public string ToAccountNumber { get; set; }
    }
}
