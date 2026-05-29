using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using VoiceCommandAPI.Hubs;
using VoiceCommandAPI.Models;
using VoiceCommandAPI.Services;

namespace VoiceCommandAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CommandsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CommandsController> _logger;
        private readonly IHubContext<CommandHub> _hubContext;
        private readonly GroqService _groqService;
        private const string CacheKey = "all_commands";

        public CommandsController(IConfiguration configuration, IMemoryCache cache, ILogger<CommandsController> logger, IHubContext<CommandHub> hubContext, GroqService groqService)
        {
            _connectionString = configuration.GetConnectionString("conStr") ?? "";
            _cache = cache;
            _logger = logger;
            _hubContext = hubContext;
            _groqService = groqService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (_cache.TryGetValue(CacheKey, out List<Command>? cached))
            {
                _logger.LogInformation("Returning commands from cache.");
                return Ok(cached);
            }

            var commands = new List<Command>();
            try
            {
                using var con = new SqlConnection(_connectionString);
                con.Open();
                using var cmd = new SqlCommand("SELECT Id, CommandText, RecognizedAt FROM Commands ORDER BY RecognizedAt DESC", con);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    commands.Add(new Command
                    {
                        Id = reader.GetInt32(0),
                        CommandText = reader.GetString(1),
                        RecognizedAt = reader.GetDateTime(2)
                    });
                }

                _cache.Set(CacheKey, commands, TimeSpan.FromSeconds(30));
                _logger.LogInformation("Returning {Count} commands from database.", commands.Count);
                return Ok(commands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching commands.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Command command)
        {
            if (string.IsNullOrWhiteSpace(command.CommandText))
                return BadRequest(new { message = "CommandText cannot be empty." });

            try
            {
                using var con = new SqlConnection(_connectionString);
                con.Open();
                using var transaction = con.BeginTransaction();
                try
                {
                    using var cmd = new SqlCommand(
                        "INSERT INTO Commands (CommandText) VALUES (@cmd)", con, transaction);
                    cmd.Parameters.AddWithValue("@cmd", command.CommandText);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                    _cache.Remove(CacheKey);
                    _logger.LogInformation("Command saved: {CommandText}", command.CommandText);

                    await _hubContext.Clients.All.SendAsync("ReceiveCommand", command.CommandText, DateTime.Now.ToString("g"));

                    return Ok(new { message = "Command saved." });
                }
                catch
                {
                    transaction.Rollback();
                    _logger.LogWarning("Transaction rolled back for command: {CommandText}", command.CommandText);
                    return StatusCode(500, new { message = "Transaction failed and was rolled back." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving command.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                con.Open();
                using var cmd = new SqlCommand("DELETE FROM Commands WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                _cache.Remove(CacheKey);
                _logger.LogInformation("Command {Id} deleted.", id);
                return Ok(new { message = "Deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting command {Id}.", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteAll()
        {
            try
            {
                using var con = new SqlConnection(_connectionString);
                con.Open();
                using var cmd = new SqlCommand("DELETE FROM Commands", con);
                cmd.ExecuteNonQuery();
                _cache.Remove(CacheKey);
                _logger.LogInformation("All commands deleted.");
                return Ok(new { message = "All history deleted." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all commands.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            var stats = new List<object>();
            try
            {
                using var con = new SqlConnection(_connectionString);
                con.Open();
                using var cmd = new SqlCommand(
                    "SELECT CommandText, COUNT(*) as Total FROM Commands GROUP BY CommandText ORDER BY Total DESC", con);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    stats.Add(new
                    {
                        command = reader.GetString(0),
                        total = reader.GetInt32(1)
                    });
                }
                _logger.LogInformation("Stats fetched successfully.");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stats.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("debug-key")]
        public IActionResult DebugKey([FromServices] IConfiguration config)
        {
            var key = config["Groq:ApiKey"] ?? "NOT FOUND";
            return Ok(new { keyLength = key.Length, keyStart = key.Substring(0, Math.Min(8, key.Length)) });
        }

        [HttpPost("recognize")]
        public async Task<IActionResult> Recognize([FromBody] Command command)
        {
            if (string.IsNullOrWhiteSpace(command.CommandText))
                return BadRequest(new { message = "Speech text cannot be empty." });

            try
            {
                string intent = await _groqService.GetIntentAsync(command.CommandText);
                _logger.LogInformation("Groq recognized intent: {Intent} from speech: {Speech}", intent, command.CommandText);
                return Ok(new { intent = intent, originalSpeech = command.CommandText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Groq error");
                return StatusCode(500, ex.Message);
            }
        }
    }
}