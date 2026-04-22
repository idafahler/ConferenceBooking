using ConferenceBooking.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application.ServiceInterfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsResult> GetStatisticsAsync();
    }
}
