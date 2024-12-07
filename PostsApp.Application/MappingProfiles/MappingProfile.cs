using AutoMapper;
using PostsApp.Domain.Entities;

namespace PostsApp.Application;

/// <summary>
/// Mapping profile for Automapper.
/// </summary>
public sealed class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<AddNewPostCommand, Post>()
			.ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.Id))
			.ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.Title))
			.ForMember(dest => dest.ArticleLink, opts => opts.MapFrom(src => src.ArticleLink))
			.ForMember(dest => dest.PublicationDate, opts => opts.MapFrom(src => src.PublicationDate))
			.ForMember(dest => dest.Creator, opts => opts.MapFrom(src => src.Creator))
			.ForMember(dest => dest.Content, opts => opts.MapFrom(src => src.Content))
			.ForMember(dest => dest.MediaUrl, opts => opts.MapFrom(src => src.MediaUrl))
			.ReverseMap();
	}
}
