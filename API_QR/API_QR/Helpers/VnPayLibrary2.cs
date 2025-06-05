using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace API_QR.Helpers
{
    public class VnPayLibrary2
    {
        private readonly SortedList<string, string> _requestData = new();
        private readonly SortedList<string, string> _responseData = new();

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData[key] = value;
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData[key] = value;
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out string? retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            // Bước 1: Sắp xếp dữ liệu theo thứ tự alphabet
            var sortedData = _requestData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .OrderBy(kv => kv.Key)
                .ToList();

            // Bước 2: Tạo chuỗi hash data (không encode)
            var hashData = string.Join("&", sortedData.Select(kv => $"{kv.Key}={kv.Value}"));

            Console.WriteLine($"Hash Data: {hashData}");

            // Bước 3: Tạo secure hash
            var secureHash = HmacSHA512(vnpHashSecret, hashData);

            Console.WriteLine($"Secure Hash: {secureHash}");

            // Bước 4: Tạo URL với encode
            var queryParams = sortedData.Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}").ToList();
            queryParams.Add($"vnp_SecureHash={secureHash}");

            var queryString = string.Join("&", queryParams);
            return $"{baseUrl}?{queryString}";
        }

        // Method signature chỉ nhận 2 tham số như trong error message
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            // Loại bỏ vnp_SecureHash và vnp_SecureHashType
            var responseData = _responseData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                .OrderBy(kv => kv.Key)
                .ToList();

            var hashData = string.Join("&", responseData.Select(kv => $"{kv.Key}={kv.Value}"));

            Console.WriteLine($"Response Hash Data: {hashData}");

            var myChecksum = HmacSHA512(secretKey, hashData);

            Console.WriteLine($"My Checksum: {myChecksum}");
            Console.WriteLine($"Input Hash: {inputHash}");

            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);

            using var hmac = new HMACSHA512(keyBytes);
            var hashValue = hmac.ComputeHash(inputBytes);

            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}
