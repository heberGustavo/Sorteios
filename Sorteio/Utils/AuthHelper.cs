﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sorteio.Common;
using Sorteio.Domain.Models.EntityDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sorteio.Portal.Utils
{
    public class AuthHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public static Usuario USUARIO_LOGADO()
        {
            try
            {
                var usuario = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == PolicyKeys.USUARIO_LOGADO)?.Value;

                return JsonConvert.DeserializeObject<Usuario>(usuario);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Usuario USUARIO_LOGADO_ADM()
        {
            try
            {
                var usuario = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == PolicyKeys.USUARIO_LOGADO_ADM)?.Value;

                return JsonConvert.DeserializeObject<Usuario>(usuario);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static ClaimsPrincipal GerarClaimsUsuarioLogado(Usuario usuario)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, Guid.NewGuid().ToString())
            };

            if (usuario != null)
            {
                usuario.senha = "";
                
                if (usuario.id_tipo_usuario == DataDictionary.USUARIO_CLIENTE)
                {
                    claims.Add(new Claim(PolicyKeys.USUARIO_LOGADO, JsonConvert.SerializeObject(usuario)));
                }
                else if (usuario.id_tipo_usuario == DataDictionary.USUARIO_ADMINISTRADOR)
                {
                    claims.Add(new Claim(PolicyKeys.USUARIO_LOGADO_ADM, JsonConvert.SerializeObject(usuario)));
                }

            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(claimsIdentity);
        }
    }

    public class PolicyKeys
    {
        public static void ConfigurePolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyKeys.USUARIO_LOGADO, policy => policy.RequireClaim(PolicyKeys.USUARIO_LOGADO));
                options.AddPolicy(PolicyKeys.USUARIO_LOGADO_ADM, policy => policy.RequireClaim(PolicyKeys.USUARIO_LOGADO_ADM));
            });
        }

        public const string USUARIO_LOGADO = "usuario_logado";
        public const string USUARIO_LOGADO_ADM = "usuario_logado_adm";
    }
}
