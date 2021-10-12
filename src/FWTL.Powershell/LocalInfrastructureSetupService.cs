using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using FWTL.Core.Services;

namespace FWTL.Powershell
{
    public class LocalInfrastructureService : IInfrastructureService
    {
        public async Task<Result> CreateTelegramApi(Guid accountId)
        {
            var script = await File.ReadAllTextAsync(@".\..\..\k8\telegram\helm-install.ps1");

            using PowerShell ps = PowerShell.Create();
            ps.AddScript(script);
            ps.AddParameter("AccountId", accountId);
            ps.AddParameter("PathToChart", @".\..\..\k8\telegram\");

            await ps.InvokeAsync();
            return ps.HadErrors ? new Result(ps.Streams.Error.Select(e => e.Exception.Message).ToList()) : new Result();
        }

        public async Task<Result> DeleteTelegramApi(Guid accountId)
        {
            var script = await File.ReadAllTextAsync(@".\..\..\k8\telegram\helm-uninstall.ps1");

            using PowerShell ps = PowerShell.Create();
            ps.AddScript(script);
            ps.AddParameter("AccountId", accountId);
            ps.AddParameter("PathToChart", @".\..\..\k8\telegram\");

            await ps.InvokeAsync();
            return ps.HadErrors ? new Result(ps.Streams.Error.Select(e => e.Exception.Message).ToList()) : new Result();
        }
    }
}