using System;
using System.Threading;
using Topshelf;

namespace DeleteService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.StartAutomatically();
                x.EnableServiceRecovery(rc => rc.RestartService(1));
                x.Service<DeleteService>(s =>
                {
                    s.ConstructUsing(hostSettings => new DeleteService(hostSettings));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.SetDescription("A service that will move folders to the main trash folder.");
                x.SetDisplayName("DeletionService");
                x.SetServiceName("DeletionService");
            });


        }
    }
}