using AutoMapper;
using web.Features.ArtPieces;
using web.Features.Reviewers;
using web.Features.Reviewers.LoadLikes;

namespace web;

public class AutoMapperProfile : Profile
{
        public AutoMapperProfile()
        {
                CreateMap<Like, ReviewerLikeModel>();
                CreateMap<ArtPiece, ReviewerLikeModel>()
                        .ForMember(rlm => rlm.Id, opt => opt.Ignore());
        }
}
