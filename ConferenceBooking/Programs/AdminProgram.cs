using ConferenceBooking.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Presentation.Programs
{
    internal class AdminProgram(IServiceScopeFactory scopeFactory)
    {
        internal async Task Run(User user)
        {

        }
    }
}
