using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set;}
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
        private readonly DataContext context;
        private readonly IPhotoAccessor photoAccessor;
        private readonly IuserAccessor userAccessor;
        public Handler (DataContext context,IPhotoAccessor photoAccessor,IuserAccessor userAccessor)
            {
            this.photoAccessor = photoAccessor;
            this.userAccessor = userAccessor;
            this.context = context;
                
            }

            public  async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
               var user = await context.Users.Include(p => p.Photos)
               .FirstOrDefaultAsync( x => x.UserName == userAccessor.GetUsername());
               if(user == null) return null;


               var photo = user.Photos.FirstOrDefault(x => x.id == request.Id);

               if(photo == null) return null;

               if(photo.IsMain) return Result<Unit>.Failure("You cannot delete ypur maon photo");

               var result = await photoAccessor.DeletePhoto(photo.id);

               if(result == null) return Result<Unit>.Failure("Problem deleting photo from Cloudinary");

               user.Photos.Remove(photo);

               var Success = await context.SaveChangesAsync() >0;

               if (Success) return Result<Unit>.Success(Unit.Value);

               return Result<Unit>.Failure("Problem deleting photo from API");

             }
        }
    }
}