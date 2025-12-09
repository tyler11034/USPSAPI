namespace PermitApplication.Models
{
    public class Address
    {
        public string Street { get; set; } = "";
        public string City { get; set; } = "";
        public string State { get; set; } = "FL";
        public string Zip { get; set; } = "";

        public Address() { }

        public Address(string street, string city, string zip)
        {
            Street = street;
            City = city;
            Zip = zip;
        }

        public override string ToString()
        {
            return $"{Street}, {City}, {State} {Zip}";
        }
    }
}
