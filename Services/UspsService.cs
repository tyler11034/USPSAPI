using System.Xml.Linq;
using PermitApplication.Models;

namespace PermitApplication.Services
{
    public class UspsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _userId;

        public UspsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _userId = configuration["Usps:UserId"] ?? throw new ArgumentNullException("USPS UserId not configured");
        }

        public async Task<Address?> ValidateAddress(Address address)
        {
            string xml = BuildCorrectXmlRequest(address);
            string response = await SendRequestAsync(xml);
            return ParseResponse(response);
        }

        private string BuildCorrectXmlRequest(Address address)
        {
            // THIS IS THE CORRECT FORMAT FOR API=Verify
            return $@"<AddressValidateRequest USERID=""{_userId}"">
                    <Revision>1</Revision>
                    <Address ID=""0"">
                        <Address1></Address1>
                        <Address2>{EscapeXml(address.Street)}</Address2>
                        <City>{EscapeXml(address.City)}</City>
                        <State>{address.State}</State>
                        <Zip5>{address.Zip}</Zip5>
                        <Zip4></Zip4>
                    </Address>
                </AddressValidateRequest>";
        }

        private async Task<string> SendRequestAsync(string xmlRequest)
        {
            string url = $"https://secure.shippingapis.com/ShippingAPI.dll?API=Verify&XML={Uri.EscapeDataString(xmlRequest)}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private Address? ParseResponse(string xmlResponse)
        {
            try
            {
                var doc = XDocument.Parse(xmlResponse);
                var addressNode = doc.Root?.Element("Address");

                // Successful response
                string street = addressNode?.Element("Address2")?.Value?.Trim() ?? "Street_Example";
                string city = addressNode?.Element("City")?.Value?.Trim() ?? "City_Example";
                string state = addressNode?.Element("State")?.Value?.Trim() ?? "State_Example";
                string zip = addressNode?.Element("Zip5")?.Value?.Trim() ?? "Zip_Example";

                return new Address(street, city, zip) { State = state };
            }
            catch
            {
                return null;
            }
        }

        private static string EscapeXml(string s) =>
            System.Security.SecurityElement.Escape(s) ?? "";
    }
}