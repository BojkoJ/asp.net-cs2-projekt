using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BOJ0043_Web.Infrastructure;

namespace BOJ0043_Web.Controllers
{
    /*
    Tento kontroler implementuje endpoint /api/documentation, který pomocí reflexe automaticky generuje dokumentaci všech 
    API koncových bodů ve formátu JSON (podobně jako Swagger/OpenAPI)
    */    [Route("api/documentation")]
    [ApiController]
    public class ApiDocumentationController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetApiDocumentation()
        {
            var apiDocumentation = new ApiDocumentation
            {
                Info = new ApiInfo
                {
                    Title = "BOJ0043 Coworking Space API",
                    Description = "API pro evidenci coworkingových center",
                    Version = "1.0.0"
                },
                Paths = GetApiPaths(),
                Tags = GetApiTags()
            };

            return Ok(apiDocumentation);
        }
        [HttpGet("ui")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ApiDocumentationUI()
        {
            // Pro tuto akci vrátíme běžný pohled (MVC) místo API odpovědi
            return Redirect("~/ApiDocumentation/Index");
        }        
        
        private List<ApiTag> GetApiTags()
        {
            var tags = new List<ApiTag>();
            
            // Reflexe - získání všech kontrolerů
            var assembly = Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes()
                .Where(type => 
                    type.IsClass && 
                    !type.IsAbstract && 
                    type.Name.EndsWith("Controller") &&
                    type.Namespace?.Contains(".Controllers") == true)
                .ToList();

            foreach (var controller in controllers)
            {
                // Přeskočte kontroler dokumentace API a ApiDocs
                if (controller == typeof(ApiDocumentationController) || controller.Name == "ApiDocsController")
                    continue;

                // Použití reflexe pro získání atributů kontroleru
                var displayNameAttr = controller.GetCustomAttribute<DisplayNameAttribute>();
                var descriptionAttr = controller.GetCustomAttribute<DescriptionAttribute>();
                
                var controllerName = controller.Name.Replace("Controller", "");
                var displayName = displayNameAttr?.DisplayName ?? controllerName;
                var description = descriptionAttr?.Description ?? $"API operace pro {displayName}";
                
                tags.Add(new ApiTag
                {
                    Name = controllerName,
                    DisplayName = displayName,
                    Description = description
                });
            }
            
            return tags;
        }        private Dictionary<string, Dictionary<string, ApiEndpoint>> GetApiPaths()
        {
            var paths = new Dictionary<string, Dictionary<string, ApiEndpoint>>();
            
            // Reflexe - získání všech kontrolerů v aplikaci
            var assembly = Assembly.GetExecutingAssembly();
            var controllers = assembly.GetTypes()
                .Where(type => 
                    type.IsClass && 
                    !type.IsAbstract && 
                    type.Name.EndsWith("Controller") &&
                    type.Namespace?.Contains(".Controllers") == true)
                .ToList();

            foreach (var controller in controllers)
            {
                // Přeskočit kontroler dokumentace API a ApiDocs
                if (controller == typeof(ApiDocumentationController) || controller.Name == "ApiDocsController")
                    continue;

                var routePrefix = GetRoutePrefix(controller);
                
                // Reflexe - získání veřejných metod kontroleru
                // Pouze metody s atributem [ApiSchema] (tj. pouze API endpoints pod regionem API metody pro WPF aplikaci)
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.GetCustomAttribute<ApiSchemaAttribute>() != null)
                    .ToList();

                foreach (var method in methods)
                {
                    // Jen pro metody s atributem HTTP verb (GET, POST, PUT, DELETE)
                    var httpMethodAttribute = GetHttpMethodAttribute(method);
                    if (httpMethodAttribute == null)
                        continue;

                    var httpMethod = GetHttpMethod(httpMethodAttribute);
                    var routeSuffix = GetRouteSuffix(method);
                    var fullPath = CombineRoutePath(routePrefix, routeSuffix);

                    if (!paths.ContainsKey(fullPath))
                    {
                        paths[fullPath] = new Dictionary<string, ApiEndpoint>();
                    }
                    var parameters = GetParameters(method);
                    var returnType = GetReturnType(method);
                    var summary = GetSummary(method);

                    // Získání názvu kontroleru jako tagu
                    var controllerName = controller.Name.Replace("Controller", "");
                    var tags = new List<string> { controllerName };

                    paths[fullPath][httpMethod.ToLower()] = new ApiEndpoint
                    {
                        Summary = summary,
                        Description = GetDescription(method),
                        OperationId = method.Name,
                        Parameters = parameters,
                        RequestBody = GetRequestBody(method),
                        Responses = GetResponses(method, returnType),
                        Tags = tags // Přidání tagu ke každému endpointu
                    };
                }
            }

            return paths;
        }        private string GetRoutePrefix(Type controller)
        {
            // Reflexe - získání atributů routování v kontroleru
            // Nejprve zkusíme nalézt explicitně definovanou RouteAttribute
            var routeAttr = controller.GetCustomAttribute<RouteAttribute>();
            if (routeAttr != null)
                return routeAttr.Template;

            // Kontrola pro standardní MVC kontrolery
            var controllerName = controller.Name.Replace("Controller", "");
            
            // Pokud je kontroler API (má atribut ApiController), použijeme api/ prefix
            if (controller.GetCustomAttributes(typeof(ApiControllerAttribute), true).Any())
                return $"api/{controllerName}";
            
            // Pro standardní MVC kontrolery použijeme pouze název kontroleru
            return controllerName;
        }

        private string GetRouteSuffix(MethodInfo method)
        {
            var routeAttr = method.GetCustomAttribute<RouteAttribute>();
            if (routeAttr != null)
                return routeAttr.Template;

            var httpGet = method.GetCustomAttribute<HttpGetAttribute>();
            if (httpGet != null && !string.IsNullOrEmpty(httpGet.Template))
                return httpGet.Template;

            var httpPost = method.GetCustomAttribute<HttpPostAttribute>();
            if (httpPost != null && !string.IsNullOrEmpty(httpPost.Template))
                return httpPost.Template;

            var httpPut = method.GetCustomAttribute<HttpPutAttribute>();
            if (httpPut != null && !string.IsNullOrEmpty(httpPut.Template))
                return httpPut.Template;

            var httpDelete = method.GetCustomAttribute<HttpDeleteAttribute>();
            if (httpDelete != null && !string.IsNullOrEmpty(httpDelete.Template))
                return httpDelete.Template;

            return method.Name;
        }       
         
         
        private string CombineRoutePath(string prefix, string suffix)
        {
            // Pokud prefix obsahuje "api/", přidáme lomítko na začátek (API style)
            bool isApiRoute = prefix.StartsWith("api/");
            
            if (string.IsNullOrEmpty(suffix))
                return isApiRoute ? $"/{prefix}" : prefix;

            // Pokud suffix již začíná lomítkem, přidáme ho k prefixu
            if (suffix.StartsWith("/"))
                return isApiRoute ? $"/{prefix}{suffix}" : $"{prefix}{suffix}";

            // Jinak přidáme lomítko mezi prefix a suffix
            return isApiRoute ? $"/{prefix}/{suffix}" : $"{prefix}/{suffix}";
        }        
        
        private Attribute GetHttpMethodAttribute(MethodInfo method)
        {
            var httpGet = method.GetCustomAttribute<HttpGetAttribute>();
            if (httpGet != null)
                return httpGet;

            var httpPost = method.GetCustomAttribute<HttpPostAttribute>();
            if (httpPost != null)
                return httpPost;

            var httpPut = method.GetCustomAttribute<HttpPutAttribute>();
            if (httpPut != null)
                return httpPut;

            var httpDelete = method.GetCustomAttribute<HttpDeleteAttribute>();
            if (httpDelete != null)
                return httpDelete;

            var httpPatch = method.GetCustomAttribute<HttpPatchAttribute>();
            if (httpPatch != null)
                return httpPatch;

            // Metody bez explicitního HTTP atributu považujeme za GET
            // Ale pouze pokud nemají tyto atributy pro formuláře - ty by neměly být v API dokumentaci
            if (!method.GetCustomAttributes<ValidateAntiForgeryTokenAttribute>().Any() && 
                !method.GetCustomAttributes<NonActionAttribute>().Any())
            {
                // Vytvoříme dummy HttpGet atribut pro metody bez explicitního HTTP atributu
                return new HttpGetAttribute();
            }

            return null;
        }

        private string GetHttpMethod(Attribute httpMethodAttribute)
        {
            if (httpMethodAttribute is HttpGetAttribute) return "GET";
            if (httpMethodAttribute is HttpPostAttribute) return "POST";
            if (httpMethodAttribute is HttpPutAttribute) return "PUT";
            if (httpMethodAttribute is HttpDeleteAttribute) return "DELETE";
            if (httpMethodAttribute is HttpPatchAttribute) return "PATCH";
            return "GET"; // Default
        }

        private List<ApiParameter> GetParameters(MethodInfo method)
        {
            var parameters = new List<ApiParameter>();
            
            // Getneme parametry metody a zkontrolujeme, zda mají atributy [FromQuery] nebo [FromRoute]
            foreach (var param in method.GetParameters())
            {
                // Přeskočíme parametry, které jsou z těla požadavku
                if (param.GetCustomAttribute<FromBodyAttribute>() != null)
                    continue;

                // Přeskočíme parametry, které jsou z ASP.NET Core frameworku
                if (param.ParameterType.Namespace?.StartsWith("Microsoft.") == true ||
                    param.ParameterType.Namespace?.StartsWith("System.") == true)
                    continue;

                var fromQuery = param.GetCustomAttribute<FromQueryAttribute>();
                var fromRoute = param.GetCustomAttribute<FromRouteAttribute>();
                
                var parameterType = "query";
                if (fromRoute != null)
                    parameterType = "path";
                
                parameters.Add(new ApiParameter
                {
                    Name = param.Name,
                    In = parameterType,
                    Description = GetParameterDescription(param),
                    Required = !param.IsOptional,
                    Schema = new ApiSchema
                    {
                        Type = GetParameterType(param.ParameterType)
                    }
                });
            }
            
            return parameters;
        }

        private string GetParameterDescription(ParameterInfo param)
        {
            var descAttr = param.GetCustomAttribute<DescriptionAttribute>();
            return descAttr?.Description ?? $"Parameter {param.Name}";
        }

        private string GetParameterType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return "integer";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
            if (type == typeof(bool)) return "boolean";
            if (type == typeof(DateTime)) return "string";
            if (type.IsEnum) return "string";
            
            // For complex types
            return "object";
        }

        private ApiRequestBody GetRequestBody(MethodInfo method)
        {
            // Check for custom ApiSchemaAttribute
            var schemaAttr = method.GetCustomAttribute<ApiSchemaAttribute>();
            Type requestType = null;
            if (schemaAttr != null && schemaAttr.RequestType != null)
            {
                requestType = schemaAttr.RequestType;
            }
            else
            {
                // Fallback: find parameter with [FromBody]
                var bodyParam = method.GetParameters()
                    .FirstOrDefault(p => p.GetCustomAttribute<FromBodyAttribute>() != null);
                if (bodyParam != null)
                    requestType = bodyParam.ParameterType;
            }
            if (requestType == null)
                return null;
            return new ApiRequestBody
            {
                Description = $"Request body for {method.Name}",
                Required = true,
                Content = new Dictionary<string, ApiContent>
                {
                    ["application/json"] = new ApiContent
                    {
                        Schema = new ApiSchema
                        {
                            Type = GetParameterType(requestType),
                            Reference = $"#/components/schemas/{requestType.Name}"
                        }
                    }
                }
            };
        }

        private Dictionary<string, ApiResponse> GetResponses(MethodInfo method, Type returnType)
        {
            var responses = new Dictionary<string, ApiResponse>();
            // Check for custom ApiSchemaAttribute
            var schemaAttr = method.GetCustomAttribute<ApiSchemaAttribute>();
            Type responseType = null;
            if (schemaAttr != null && schemaAttr.ResponseType != null)
            {
                responseType = schemaAttr.ResponseType;
            }
            else
            {
                responseType = returnType;
            }
            // Default success response
            responses["200"] = new ApiResponse
            {
                Description = "Successful operation",
                Content = responseType != typeof(void) ?
                    new Dictionary<string, ApiContent>
                    {
                        ["application/json"] = new ApiContent
                        {
                            Schema = new ApiSchema
                            {
                                Type = GetParameterType(responseType),
                                Items = responseType.IsGenericType &&
                                       (responseType.GetGenericTypeDefinition() == typeof(List<>) ||
                                        responseType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ?
                                    new ApiSchema
                                    {
                                        Type = GetParameterType(responseType.GetGenericArguments()[0]),
                                        Reference = $"#/components/schemas/{responseType.GetGenericArguments()[0].Name}"
                                    } : null,
                                Reference = !responseType.IsGenericType && responseType != typeof(void) &&
                                           responseType != typeof(string) && responseType != typeof(int) &&
                                           responseType != typeof(bool) && responseType != typeof(object) ?
                                    $"#/components/schemas/{responseType.Name}" : null
                            }
                        }
                    } : null
            };
            // Default error responses
            responses["400"] = new ApiResponse { Description = "Bad request" };
            responses["401"] = new ApiResponse { Description = "Unauthorized" };
            responses["404"] = new ApiResponse { Description = "Not found" };
            responses["500"] = new ApiResponse { Description = "Internal server error" };
            return responses;
        }

        private Type GetReturnType(MethodInfo method)
        {
            // Get the real return type from IActionResult, ActionResult<T> etc.
            var returnType = method.ReturnType;
            
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ActionResult<>))
            {
                returnType = returnType.GetGenericArguments()[0];
            }
            
            if (returnType == typeof(IActionResult) || returnType == typeof(ActionResult))
            {
                returnType = typeof(object); // Generic object
            }
            
            return returnType;
        }

        private string GetSummary(MethodInfo method)
        {
            var summaryAttr = method.GetCustomAttribute<DisplayNameAttribute>();
            return summaryAttr?.DisplayName ?? method.Name;
        }        private string GetDescription(MethodInfo method)
        {
            var descAttr = method.GetCustomAttribute<DescriptionAttribute>();
            return descAttr?.Description ?? $"API Endpoint {method.Name}";
        }
    }    // Třídy pro sturkuturu API dokumentace 
    public class ApiDocumentation
    {
        public string OpenApi { get; set; } = "3.0.0";
        public ApiInfo Info { get; set; }
        public Dictionary<string, Dictionary<string, ApiEndpoint>> Paths { get; set; }
        public List<ApiTag> Tags { get; set; }
    }

    public class ApiTag
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class ApiInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }    public class ApiEndpoint
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public string OperationId { get; set; }
        public List<ApiParameter> Parameters { get; set; }
        public ApiRequestBody RequestBody { get; set; }
        public Dictionary<string, ApiResponse> Responses { get; set; }
        public List<string> Tags { get; set; }
    }

    public class ApiParameter
    {
        public string Name { get; set; }
        public string In { get; set; } // query, path, header, cookie
        public string Description { get; set; }
        public bool Required { get; set; }
        public ApiSchema Schema { get; set; }
    }

    public class ApiRequestBody
    {
        public string Description { get; set; }
        public bool Required { get; set; }
        public Dictionary<string, ApiContent> Content { get; set; }
    }

    public class ApiContent
    {
        public ApiSchema Schema { get; set; }
    }

    public class ApiResponse
    {
        public string Description { get; set; }
        public Dictionary<string, ApiContent> Content { get; set; }
    }

    public class ApiSchema
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public ApiSchema Items { get; set; }
        public string Reference { get; set; }
    }
}
