namespace LINE_PAY_APIv3.Dtos
{
    public class LineMessage
    {
        public class PaymentRequestDto
        {
            public int amount { get; set; }
            public string currency { get; set; }
            public string orderId { get; set; }
            public List<PackageDto> packages { get; set; }
            public RedirectUrls redirectUrls { get; set; }
            public string? Options { get; set; }
        }
        public class PackageDto
        {
            public string id { get; set; }
            public int amount { get; set; }
            public string name { get; set; }
            public List<LinePayProductDto> products { get; set; }
            public int? userFee { get; set; }

        }

        public class LinePayProductDto
        {
            public string name { get; set; }
            public int quantity { get; set; }
            public int price { get; set; }
            public string? id { get; set; }
            public string? imageUrl { get; set; }
            public int? originalPrice { get; set; }
        }

        public class RedirectUrls
        {
            public string confirmUrl { get; set; }
            public string cancelUrl { get; set; }
        }

        public class PaymentConfirmDto
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

    }
}
