
using System.Runtime.InteropServices;
using System.Text;

namespace CSharpInterop
{
    public static class HttpService
    {
        private static readonly HttpClient client = new();

        [UnmanagedCallersOnly(EntryPoint = "send_message")]
        public static IntPtr MakePostRequest(
            IntPtr usernamePtr,
            IntPtr messagePtr)
        {
            string? username = Marshal.PtrToStringUTF8(usernamePtr);
            string? message = Marshal.PtrToStringUTF8(messagePtr);

            if ( username == null || message == null)
            {
                return IntPtr.Zero;
            }

            string result = MakePostRequestInternal( username, message);
            return Marshal.StringToHGlobalAnsi(result);
        }

        [UnmanagedCallersOnly(EntryPoint = "get_messages")]
        public static IntPtr GetMessages()
        {
            string result = MakeGetRequestInternal();
            return Marshal.StringToHGlobalAnsi(result);
        }

        [UnmanagedCallersOnly(EntryPoint = "free_string")]
        public static void FreeString(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        private static string MakePostRequestInternal(string username, string message)
        {
            String url = "https://chat.tissink.me/message";
            // add header "key": "WcRa962TFQ5MgFja3enssESn7SBMKvkaVr2JrdvwJEKEJanD5RKU36JC8ejK"
            try
            {
                var requestContent = new MultipartFormDataContent();
                requestContent.Add(new StringContent(username), "username");
                requestContent.Add(new StringContent(message), "message");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("key", "WcRa962TFQ5MgFja3enssESn7SBMKvkaVr2JrdvwJEKEJanD5RKU36JC8ejK");

                var response = client.PostAsync(url, requestContent).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        private static string MakeGetRequestInternal()
        {
            try
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("key", "WcRa962TFQ5MgFja3enssESn7SBMKvkaVr2JrdvwJEKEJanD5RKU36JC8ejK");

                var response = client.GetAsync("https://chat.tissink.me/messages").GetAwaiter().GetResult();
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
