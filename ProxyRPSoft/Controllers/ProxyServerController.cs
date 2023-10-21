using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ProxyRPSoft.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProxyServerController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Request(string jsonString, string? sessionCookie = "")
        {
            try
            {
                // Kiểm tra xem chuỗi JSON có giá trị hay không
                if (string.IsNullOrEmpty(jsonString))
                {
                    return BadRequest("Invalid JSON input");
                }

                // Gửi POST request đến API
                using (HttpClient client = new HttpClient())
                { 
                    // Đặt URL của API
                    string apiUrl = "http://test.stockprice.vn/Hub/Feeder.svc/S";

                    // Tạo nội dung của request từ chuỗi JSON
                    StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    if (!string.IsNullOrEmpty(sessionCookie))
                    {
                        // Thêm header Cookie vào request
                        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);
                    }

                    // Gửi request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                    // Kiểm tra xem request có thành công hay không
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung của response
                    //CookieContainer cookies = new CookieContainer();
                    //foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie"))
                    //    cookies.SetCookies(new Uri(apiUrl), cookieHeader);
                    //string cookieValue = cookies.GetCookies(new Uri(apiUrl)).FirstOrDefault(c => c.Name == "ASP.NET_SessionId")?.Value;
                    //HttpContext.Session.SetString("sessionCookie", cookieValue);

                    //Response.Cookies.Append("sessionCookie", cookieValue, new CookieOptions()
                    //{
                    //    Expires = DateTimeOffset.Now.AddHours(4),
                    //    Path = "/",
                    //    HttpOnly = true,
                    //    Secure = true,
                    //});
                    //var cookie = JsonSerializer.Serialize(cookies);
                    //Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                    //await Response.WriteAsync(jsonString, Encoding.UTF8);
                    
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    // Trả về kết quả thành công
                    return Ok(apiResponse);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("requestlogin")]
        public async Task<IActionResult> RequestLogin(string jsonString, string? sessionCookie = "")
        {
            try
            {
                // Kiểm tra xem chuỗi JSON có giá trị hay không
                if (string.IsNullOrEmpty(jsonString))
                {
                    return BadRequest("Invalid JSON input");
                }

                // Gửi POST request đến API
                using (HttpClient client = new HttpClient())
                {
                    // Đặt URL của API
                    string apiUrl = "http://test.stockprice.vn/Hub/Feeder.svc/S";

                    // Tạo nội dung của request từ chuỗi JSON
                    StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                    if (!string.IsNullOrEmpty(sessionCookie))
                    {
                        // Thêm header Cookie vào request
                        client.DefaultRequestHeaders.Add("Cookie", sessionCookie);
                    }

                    // Gửi request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    //var cookies = response.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                    // Kiểm tra xem request có thành công hay không
                    response.EnsureSuccessStatusCode();

                    // Đọc nội dung của response
                    CookieContainer cookies = new CookieContainer();
                    foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie"))
                        cookies.SetCookies(new Uri(apiUrl), cookieHeader);
                    string cookieValue = cookies.GetCookies(new Uri(apiUrl)).FirstOrDefault(c => c.Name == "ASP.NET_SessionId")?.Value;
                    //HttpContext.Session.SetString("sessionCookie", cookieValue);

                    //Response.Cookies.Append("sessionCookie", cookieValue, new CookieOptions()
                    //{
                    //    Expires = DateTimeOffset.Now.AddHours(4),
                    //    Path = "/",
                    //    HttpOnly = true,
                    //    Secure = true,
                    //});
                    //var cookie = JsonSerializer.Serialize(cookies);
                    //Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                    //await Response.WriteAsync(jsonString, Encoding.UTF8);

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    cookieValue = "ASP.NET_SessionId=" + cookieValue;
                    var cookieJson = new { cookie = cookieValue };

                    // Parse the apiResponse into a JObject
                    JObject apiResponseJson = JObject.Parse(apiResponse);

                    // Convert the cookieJson to a JObject
                    JObject cookieJsonObj = JObject.FromObject(cookieJson);


                    // Merge the cookieJsonObj into the apiResponseJson
                    apiResponseJson.Merge(cookieJsonObj);

                    // Convert the merged JSON object back to a string
                    string mergedJsonString = apiResponseJson.ToString();

                    return Ok(mergedJsonString);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
