using FluentValidation;
using FWTL.Core.Commands;
using FWTL.Core.Events;
using FWTL.Core.Services;
using FWTL.Core.Validation;
using FWTL.TelegramClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FWTL.Aggregate;
using FWTL.Core.Database;
using NodaTime;

namespace FWTL.Domain.Jobs
{
    public class CreateJob
    {
        public class Request : IRequest
        {
            public Guid AccountId { get; set; }

            public string DialogId { get; set; }
        }

        public class Command : Request, ICommand
        {
            public Command()
            {
            }

            public Command(ICurrentUserService currentUserService, IGuidService guidService)
            {
                UserId = currentUserService.CurrentUserId;
                Id = guidService.New;
            }

            public Guid UserId { get; set; }

            public Guid Id { get; set; }
        }

        public class Handler : ICommandHandler<Command>
        {
            private readonly ITelegramClient _telegramClient;
            private readonly DatabaseContext _databaseContext;
            private readonly IClock _clock;

            public Handler(ITelegramClient telegramClient, DatabaseContext databaseContext, IClock clock)
            {
                _telegramClient = telegramClient;
                _databaseContext = databaseContext;
                _clock = clock;
            }

            public IList<IEvent> Events { get; } = new List<IEvent>();

            public async Task ExecuteAsync(Command command)
            {
                bool areThereJobsInProgress = _databaseContext.Jobs
                    .Where(j => j.DialogId == command.DialogId)
                    .Where(j => j.JobStatus == JobStatuses.InProgress).Any();

                if (areThereJobsInProgress)
                {
                    throw new AppValidationException(nameof(Command.DialogId),
                        "There is already pending job for that dialog");
                }

                var previousCompletedJob = _databaseContext.Jobs
                                                .Where(j => j.DialogId == command.DialogId)
                                                .Where(j => j.JobStatus == JobStatuses.Completed).FirstOrDefault() ??
                                            new Job();

                var newJob = new Job()
                {
                    CreatedAt = _clock.GetCurrentInstant(),
                    DialogId = command.DialogId,
                    Id = command.Id,
                    JobStatus = JobStatuses.InProgress,
                    MaxHistoryId = previousCompletedJob.MaxHistoryId,
                    MessagesToProcess = 0,
                    ProcessedMessages = 0
                };

                newJob.AccountJobs.Add(new AccountJob()
                {
                    AccountId = command.AccountId,
                    JobId = command.Id
                });
            }
        }

        public class Validator : AppAbstractValidation<Command>
        {
            public Validator()
            {
                RuleFor(x => x.DialogId).NotEmpty();
            }
        }
    }
}