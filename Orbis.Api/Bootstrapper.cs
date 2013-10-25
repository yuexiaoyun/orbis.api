using System.Web.Mvc;
using AutoMapper;
using Microsoft.Practices.Unity;
using Orbis.Api.Extensions;
using Unity.Mvc4;

namespace Orbis.Api
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            System.Web.Mvc.DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            ConfigureObjectMappings();

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    
            RegisterTypes(container);
            ContainerContext.Create(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IDao<User, UserFilter>, UserDao>(new ContainerControlledLifetimeManager());
        }

        public static void ConfigureObjectMappings()
        {
            EntityExtensions.CreateEntityMapping<User, UserT>()
                .ForMember(x => x.Username, x => x.MapFrom(y => y.Username))
                .ForMember(x => x.Password, x => x.MapFrom(y => y.Password))
                .ForMember(x => x.EmailAddress, x => x.MapFrom(y => y.EmailAddress));
            EntityExtensions.CreateDtoMapping<UserT, User>()
                .ForMember(x => x.Username, x => x.MapFrom(y => y.Username))
                .ForMember(x => x.Password, x => x.MapFrom(y => y.Password))
                .ForMember(x => x.EmailAddress, x => x.MapFrom(y => y.EmailAddress));

            Mapper.AssertConfigurationIsValid();
        }
    }
}