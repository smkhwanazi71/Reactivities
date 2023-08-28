using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile File{get; set;}
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
        private readonly DataContext context;
        private readonly IPhotoAccessor PhotoAccessor ;
        private readonly IuserAccessor userAccessor;
            public Handler (DataContext context,IPhotoAccessor photoAccessor,IuserAccessor userAccessor)
            {
            this.userAccessor = userAccessor;
            this.PhotoAccessor = photoAccessor;
            this.context = context;
                
            }
            public  async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                 var user = await context.Users.Include(p =>p.Photos)
                 .FirstOrDefaultAsync(x =>x.UserName == userAccessor.GetUsername());

                 if(user == null) return null;

                 var photoUploadResult = await PhotoAccessor.AddPhoto(request.File);

                 var photo = new Photo
                 {
                    Url = photoUploadResult.Url,
                    id = photoUploadResult.PublicId
                 };

                 if(!user.Photos.Any(x =>x.IsMain)) photo.IsMain = true;

                 user.Photos.Add(photo);

                 var result = await context.SaveChangesAsync() >0;
                 if (result) return Result<Photo>.Success(photo);


                 return Result<Photo>.Failure("Problem adding photo");

            }
        }
    }
}