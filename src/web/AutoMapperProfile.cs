using AutoMapper;
using web.Features.Artists;
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
                CreateMap<(Boost, ArtPiece), BoostDto>()
                        // properties from tuple's item 1 are mapped automatically, but not item 2.
                        .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.Item2.ImagePath));
                CreateMap<ArtPiece, ArtPieceDto>();
        }
}
