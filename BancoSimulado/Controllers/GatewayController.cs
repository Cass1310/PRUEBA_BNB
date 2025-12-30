using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace BancoSimulado.Controllers
{
    [ApiController]
    [Route("gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GatewayController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [HttpPost("clientes")]
        public async Task<IActionResult> Clientes()
        {
            return await ForwardRequest("/clientes");
        }

        [HttpPost("transferencias")]
        public async Task<IActionResult> Transferencias()
        {
            return await ForwardRequest("/transferencias");
        }

        [HttpPost("pagos/qr")]
        public async Task<IActionResult> PagoQr()
        {
            return await ForwardRequest("/pagos/qr");
        }

        [HttpGet("{**path}")]
        public async Task<IActionResult> Get(string path)
        {
            return await ForwardRequest("/" + path);
        }

        [HttpPost("{**path}")]
        public async Task<IActionResult> Post(string path)
        {
            return await ForwardRequest("/" + path);
        }

        [HttpPut("{**path}")]
        public async Task<IActionResult> Put(string path)
        {
            return await ForwardRequest("/" + path);
        }

        [HttpDelete("{**path}")]
        public async Task<IActionResult> Delete(string path)
        {
            return await ForwardRequest("/" + path);
        }

        [HttpPatch("{**path}")]
        public async Task<IActionResult> Patch(string path)
        {
            return await ForwardRequest("/" + path);
        }

        private async Task<IActionResult> ForwardRequest(string targetPath)
        {
            var method = new HttpMethod(Request.Method);
            var url = $"{Request.Scheme}://{Request.Host}{targetPath}{Request.QueryString.Value}";

            var request = new HttpRequestMessage(method, url);

            // Copiar body si aplica
            if (Request.ContentLength.GetValueOrDefault() > 0 &&
                (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
            {
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                var contentType = string.IsNullOrWhiteSpace(Request.ContentType)
                    ? "application/json"
                    : Request.ContentType;

                request.Content = new StringContent(body, Encoding.UTF8, contentType);
            }

            // Copiar Authorization header
            if (Request.Headers.ContainsKey("Authorization"))
            {
                request.Headers.Authorization =
                    AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            }

            // Marcar como llamado interno del gateway
            request.Headers.Add("X-Gateway-Forward", "1");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, responseBody);
        }
    }
}
