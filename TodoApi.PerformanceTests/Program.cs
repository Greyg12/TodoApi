using NBomber.CSharp;
using NBomber.Http;

// Creating a performance test scenario
var scenario = Scenario.Create("load test", async context =>
{
    var client = new HttpClient();

    // Sending a GET request to your local API endpoint
    var response = await client.GetAsync("https://localhost:7150/api/todo");

    // Returning OK if the response is successful, otherwise FAIL
    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
})
// Setting load simulation: 1000 users sending requests continuously for 30 seconds
.WithLoadSimulations(Simulation.KeepConstant(1000, TimeSpan.FromSeconds(30)));

// Running the performance test
NBomberRunner
    .RegisterScenarios(scenario)
    .Run();
