using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using ImportWizard.Data;
using ImportWizard.Data.Models;
using ImportWizard.Dtos;
using ImportWizard.Sb.Settings;
using Microsoft.EntityFrameworkCore; // for AnyAsync
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ImportWizard.Sb.HostedServices
{
    public class ProfileProcessorService : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;

        public ProfileProcessorService(
            ServiceBusClient client,
            IOptions<ServiceBusSettings> opts,
            IServiceScopeFactory scopeFactory)
        {
            var s = opts.Value;

            _processor = client.CreateProcessor(
                s.TopicName,
                s.SubscriptionName,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            int? importMasterId = null;
            bool isLast = false;
            ImportUserInputDto? dto = null;

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("payload", out var payload))
                {
                    dto = payload.Deserialize<ImportUserInputDto>();
                    if (doc.RootElement.TryGetProperty("importMasterId", out var im) &&
                        im.ValueKind == JsonValueKind.Number &&
                        im.TryGetInt32(out var id))
                    {
                        importMasterId = id;
                    }
                    if (doc.RootElement.TryGetProperty("isLast", out var last) &&
                        last.ValueKind == JsonValueKind.True)
                    {
                        isLast = true;
                    }
                }
                else
                {
                    // back-compat: raw dto
                    dto = JsonSerializer.Deserialize<ImportUserInputDto>(body);
                }
            }
            catch (Exception parseEx)
            {
                Console.Error.WriteLine($"[ParseError] {parseEx.Message}");
                await args.AbandonMessageAsync(args.Message);
                return;
            }

            if (dto == null)
            {
                await args.AbandonMessageAsync(args.Message);
                return;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 1) Flip master to Processing on first handled message
                if (importMasterId is int mid)
                {
                    var master = await db.ImportMasters.FindAsync(mid);
                    if (master != null && master.Status.Equals("Queued", StringComparison.OrdinalIgnoreCase))
                    {
                        master.Status = "Processing";
                    }
                }

                // 2) Duplicate rule: EMAIL (trim + lower)
                var emailKey = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();
                var exists = await db.Users.AnyAsync(u =>
                    u.Email != null && u.Email.Trim().ToLower() == emailKey);

                if (!exists)
                {
                    var user = new User
                    {
                        Company = dto.Company,
                        Location = dto.LocationCode,
                        EmployeeId = dto.EmployeeId,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Email = dto.Email,
                        Role = dto.Role,
                        Printer = dto.Printer,
                        ActivateNow = bool.TryParse(dto.Activate, out var a) && a,
                        Comments = dto.Comments
                    };

                    db.Users.Add(user);
                }
                else
                {
                    Console.WriteLine($"[Duplicate] {dto.Email}");
                }

                // 3) If final message for this import, mark Completed
                if (importMasterId is int mid2 && isLast)
                {
                    var master2 = await db.ImportMasters.FindAsync(mid2);
                    if (master2 != null)
                    {
                        master2.Status = "Completed";
                    }
                }

                await db.SaveChangesAsync();
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ProcessError] {ex.Message}");
                await args.AbandonMessageAsync(args.Message); // allow retry; master stays Processing
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.Error.WriteLine($"[SBError] {args.Exception.Message} | Entity={args.EntityPath} | NS={args.FullyQualifiedNamespace}");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.CompletedTask; // work is in StartAsync handlers
    }
}
