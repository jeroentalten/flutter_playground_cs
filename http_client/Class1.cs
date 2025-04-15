using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpInterop
{
    public static class HttpService
    {
        private static readonly HttpClient client = new HttpClient();

        [UnmanagedCallersOnly(EntryPoint = "make_post_request")]
        public static IntPtr MakePostRequest(
            IntPtr urlPtr, 
            IntPtr usernamePtr, 
            IntPtr messagePtr, 
            IntPtr apiKeyPtr)
        {
            string url = Marshal.PtrToStringUTF8(urlPtr);
            string username = Marshal.PtrToStringUTF8(usernamePtr);
            string message = Marshal.PtrToStringUTF8(messagePtr);
            string apiKey = Marshal.PtrToStringUTF8(apiKeyPtr);

            string result = MakePostRequestInternal(url, username, message, apiKey);
            return Marshal.StringToHGlobalUni(result);
        }

        [UnmanagedCallersOnly(EntryPoint = "free_string")]
        public static void FreeString(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static string MakePostRequestInternal(string url, string username, string message, string apiKey)
        {
            try
            {
                var content = new StringContent(
                    $"{{\"username\":\"{username}\",\"message\":\"{message}\"}}",
                    Encoding.UTF8,
                    "application/json");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("key", apiKey);

                var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
