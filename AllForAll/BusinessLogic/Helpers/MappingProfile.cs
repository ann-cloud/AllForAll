
using AllForAll.Models;
using AutoMapper;
using BusinessLogic.Dto.Category;
using BusinessLogic.Dto.Feedback;
using BusinessLogic.Dto.Manufacturer;
using BusinessLogic.Dto.Product;
using BusinessLogic.Dto.User;
using BusinessLogic.Dto.UserRole;

namespace AllForAll.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {
            CreateMap<ProductRequestDto, Product>();
            CreateMap<CategoryRequestDto, Category>();
            CreateMap<ManufacturerRequestDto, Manufacturer>();
            CreateMap<UserRequestDto, User>();
            CreateMap<UserLoginRequestDto, User>();
            CreateMap<UserRoleRequestDto, UserRole>();
            CreateMap<FeedbackRequestDto, Feedback>();
            
        }

        
    }
}
