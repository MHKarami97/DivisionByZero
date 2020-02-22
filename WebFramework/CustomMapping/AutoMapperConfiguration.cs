﻿using AutoMapper;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Models.CustomMapping;
using Models.Models;

namespace WebFramework.CustomMapping
{
    public static class AutoMapperConfiguration
    {
        public static void InitializeAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            //With AutoMapper Instance, you need to call AddAutoMapper services and pass assemblies that contains auto-mapper Profile class
            //services.AddAutoMapper(assembly1, assembly2, assembly3);
            //See http://docs.automapper.org/en/stable/Configuration.html
            //And https://code-maze.com/automapper-net-core/

            services.AddAutoMapper(config =>
            {
                config.AddCustomMappingProfile();
                config.Advanced.BeforeSeal(configProvince =>
                {
                    configProvince.CompileMappings();
                });
            }, assemblies);

            #region Deprecated (Use AutoMapper Instance instead)
            //Mapper.Initialize(config =>
            //{
            //    config.AddCustomMappingProfile();
            //});

            ////Compile mapping after configuration to boost map speed
            //Mapper.Configuration.CompileMappings();
            #endregion
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
        {
            var repositoriesAssembly = typeof(BannerDto).Assembly;
            config.AddCustomMappingProfile(repositoriesAssembly);
        }

        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

            var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
                .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);

            config.AddProfile(profile);
        }
    }
}
