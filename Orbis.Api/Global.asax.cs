using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using Orbis.Api.Controllers;

namespace Orbis.Api
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ContainerContext.Configure();

            ConfigureObjectMappings();
        }

        protected void Application_End()
        {
            ContainerContext.Dispose();
        }

        private void ConfigureObjectMappings()
        {
            Mapper.CreateMap<User, UserT>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.PublicId))
                .ForMember(x => x.Username, x => x.MapFrom(y => y.Username))
                .ForMember(x => x.Password, x => x.MapFrom(y => y.Password))
                .ForMember(x => x.EmailAddress, x => x.MapFrom(y => y.EmailAddress));
            Mapper.CreateMap<UserT, User>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.PublicId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.Username, x => x.MapFrom(y => y.Username))
                .ForMember(x => x.Password, x => x.MapFrom(y => y.Password))
                .ForMember(x => x.EmailAddress, x => x.MapFrom(y => y.EmailAddress));

            Mapper.AssertConfigurationIsValid();
        }
    }
}