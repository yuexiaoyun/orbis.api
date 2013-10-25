using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AutoMapper;
using Orbis.Api.Controllers;
using WebGrease.Css.Extensions;

namespace Orbis.Api.Extensions
{
    public static class BasicExtensions
    {
        public static string Md5Hash(this byte[] input)
        {
            var md5 = MD5.Create();
            var hash = new StringBuilder();

            md5.ComputeHash(input)
                .ForEach(x => hash.AppendFormat("{0:x2}", x));

            return hash.ToString();
        }

        public static string Md5Hash(this string input)
        {
            return Encoding.UTF8.GetBytes(input).Md5Hash();
        }

        public static string SubstringUpToFirst(this string text, string delimiter)
        {
            if (text == null)
            {
                return null;
            }

            var length = text.IndexOf(delimiter, StringComparison.InvariantCulture);

            return length >= 0 ? text.Substring(0, length) : text;
        }
    }

    public static class EntityExtensions
    {
        public static T As<T>(this Entity entity)
            where T : DataTransferObject
        {
            return (T) Mapper.Map(entity, entity.GetType(), typeof(T));
        }

        public static T As<T>(this DataTransferObject dto)
            where T : Entity
        {
            return (T) Mapper.Map(dto, dto.GetType(), typeof(T));
        }

        public static IEnumerable<T> As<T>(this IEnumerable<Entity> entities)
            where T : DataTransferObject
        {
            return entities.Select(x => (T) Mapper.Map(x, x.GetType(), typeof(T)));
        }

        public static IEnumerable<T> As<T>(this IEnumerable<DataTransferObject> dtos)
            where T : Entity
        {
            return dtos.Select(x => (T) Mapper.Map(x, x.GetType(), typeof(T)));
        }

        public static IMappingExpression<T1, T2> CreateEntityMapping<T1, T2>()
            where T1 : Entity
            where T2 : DataTransferObject
        {
            return Mapper.CreateMap<T1, T2>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.PublicId));
        }

        public static IMappingExpression<T1, T2> CreateDtoMapping<T1, T2>()
            where T1 : DataTransferObject
            where T2 : Entity
        {
            return Mapper.CreateMap<T1, T2>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.PublicId, x => x.MapFrom(y => y.Id));
        }
    }
}