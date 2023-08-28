using Application.Core;
using Application.interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Commnad: IRequest<Result<Unit>>{
            public Guid Id{ get; set; }
        }

        public class Handler : IRequestHandler<Commnad, Result<Unit>>
        {
            private readonly DataContext context;
            private readonly IuserAccessor userAccessor;
            public Handler(DataContext context,IuserAccessor userAccessor)
            {
            this.userAccessor = userAccessor;
            this.context = context;
                
            }
            public async Task<Result<Unit>> Handle(Commnad request, CancellationToken cancellationToken)
            {
                   var activity = await context.Activities
                   .Include(a =>a.Attendees).ThenInclude(u => u.AppUser)
                   .SingleOrDefaultAsync(x => x.Id == request.Id);

                   if(activity == null) return null;

                   var user = await context.Users
                   .FirstOrDefaultAsync(x => x.UserName == userAccessor.GetUsername());

                     if(activity == null) return null;

                     var HostUsername = activity.Attendees
                     .FirstOrDefault(x =>x.IsHost)?.AppUser?.UserName;

                     var attendance = activity.Attendees.FirstOrDefault(x =>x.AppUser.UserName == user.UserName);
                     
                     if(attendance != null && HostUsername ==user.UserName) 
                     {
                        activity.IsCancelled = !activity.IsCancelled;
                     }

                     if(attendance != null && HostUsername != user.UserName)
                     {
                        activity.Attendees.Remove(attendance);
                     } 

                     if(attendance == null){

                        attendance = new ActivityAttendee{
                            AppUser = user,
                            Activity = activity,
                            IsHost = false

                        };
                        activity.Attendees.Add(attendance);
                     }

                     var result = await context.SaveChangesAsync()>0;

                     return result? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance");
            }
        }
    }
}