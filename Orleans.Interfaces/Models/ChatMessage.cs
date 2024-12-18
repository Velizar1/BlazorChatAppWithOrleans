namespace Orleans.Interfaces.Models
{
    [GenerateSerializer]
    public record ChatMessage
    {
        [Id(0)]
        public string Message { get; set; }

        [Id(1)]
        public DateTime CreatedAt { get; set; }

        [Id(2)]
        public string Sender { get; set; }

    }
}
