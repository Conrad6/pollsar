using System;
using System.Linq;

using AutoMapper;

using Pollsar.Shared.Models;
using Pollsar.Web.Server.Models;

namespace Pollsar.Web.Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile ()
        {
            CreateMap<User, UserViewModel>()
                .ReverseMap();
            CreateMap<NewUserViewModel, User>()
                .ForMember(u => u.LockoutEnabled, options => options.MapFrom(u => false))
                .ForMember(u => u.UserName, options => options.MapFrom(u => u.Email))
                .ForMember(u => u.SecurityStamp, options => options.MapFrom(u => Guid.NewGuid().ToString()));

            CreateMap<Poll, PollViewModel>()
                .ForMember(pvm => pvm.Categories, config =>
                {
                    config.PreCondition(poll => poll.Categories.Any());
                    config.MapFrom(poll => poll.Categories.Select(p => p.Category.CategoryName));
                })
                .ForMember(pvm => pvm.Tags, config =>
                {
                    config.PreCondition(poll => poll.Tags.Any());
                    config.MapFrom(poll => poll.Tags.Select(p => p.Tag.TagName));
                });

            CreateMap<PollChoice, PollChoiceViewModel>()
                .ForMember(vm => vm.Votes, options =>
                  {
                      options.PreCondition(pollChoice => pollChoice.Votes.Any());
                      options.MapFrom(pollChoice => pollChoice.Votes
                      .Select(p => p.VoterId ?? -1L)
                      .Where(x => x > 0L));
                  });
        }
    }
}
