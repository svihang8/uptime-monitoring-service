using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Collections.Generic;
using UptimeMonitor.Models;  // Your model namespace

public class HomeController : Controller
{
    // Example API endpoint that retrieves service statuses
    private readonly HttpClient _httpClient;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public IActionResult Add()
    {
        return View(); // Return the Add view to render the form
    }

    [HttpPost]
    public async Task<IActionResult> Add(string url, int intervalSeconds)
    {
        if (ModelState.IsValid)
        {
            // Create new service object matching API schema
            var newService = new
            {
                url = url,
                intervalSeconds = intervalSeconds
            };

            // Serialize the object to JSON
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(newService), Encoding.UTF8, "application/json");

            // Send POST request to your API
            var response = await _httpClient.PostAsync("http://api:80/monitor", content);

    }
        return RedirectToAction("Index");

    }


    public async Task<IActionResult> Failed() {
        var response = await _httpClient.GetStringAsync("http://api:80/monitor/failures");
        var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(response);
        return View(services);  // Pass data to the view
    }

    public async Task<IActionResult> Index()
    {
        // Call your Web API to get the uptime monitoring data
        var response = await _httpClient.GetStringAsync("http://api:80/monitor/status");
        var services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ServiceStatus>>(response);
        return View(services);  // Pass data to the view
    }
}
