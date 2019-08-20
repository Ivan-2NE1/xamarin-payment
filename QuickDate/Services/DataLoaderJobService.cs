using System;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;


namespace QuickDate.Services
{
    public class JobRescheduble
    {
        public Context ActivtyContext;
        public int Jobid;
        public JobInfo.Builder Jobbuilder;
        public JobScheduler Jobscheduler;

        public JobRescheduble(Context activtycontext, int jobid)
        {
            ActivtyContext = activtycontext;
            Jobid = jobid;
        }

        public void StartJob()
        {
            try
            {
                //Supports After APi level 21 only 
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                ComponentName serviceComponent = new ComponentName(ActivtyContext, Java.Lang.Class.FromType(typeof(DataLoaderJobService)));

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    Jobbuilder = new JobInfo.Builder(Jobid, serviceComponent);
                    Jobbuilder.SetMinimumLatency(4000);
                    Jobbuilder.SetRequiredNetworkType(NetworkType.Any);
                    Jobbuilder.SetRequiresCharging(false);
                    Jobbuilder.SetOverrideDeadline(4000);
                    Jobbuilder.SetBackoffCriteria(3000, BackoffPolicy.Linear);
                    Jobbuilder.SetPersisted(true);
                }
                else
                {
                    Jobbuilder = new JobInfo.Builder(Jobid, serviceComponent);
                    Jobbuilder.SetPeriodic(2000);
                    Jobbuilder.SetPersisted(true);
                    Jobbuilder.SetRequiredNetworkType(NetworkType.Any);
                    Jobbuilder.SetRequiresCharging(false);
                }

                Jobscheduler = (JobScheduler)ActivtyContext.GetSystemService(Context.JobSchedulerService);
                int result = Jobscheduler.Schedule(Jobbuilder.Build());
                if (result == JobScheduler.ResultSuccess)
                {

                }
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }

        public void StopJob(int jobid = 0)
        {
            try
            {
                if (jobid == 0)
                    Jobscheduler.CancelAll();
                else
                    Jobscheduler.Cancel(jobid);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }
        }
    }

    [Service(Permission = "android.permission.BIND_JOB_SERVICE")]
    public class DataLoaderJobService : JobService
    {
        public override bool OnStartJob(JobParameters jobParams)
        {
            //Task.Run(async () =>
            //{
            //    // Work is happening asynchronously
            //   
            //});

            // Toast.MakeText(this, "JobService task running", ToastLength.Long).Show();

            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            // we don't want to reschedule the job if it is stopped or cancelled.
            return true;
        }


    }
}