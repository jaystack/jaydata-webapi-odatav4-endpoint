﻿using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace WebApi_2_2_OData_4
{
    public class ContainmentRoutingConvention : IODataRoutingConvention
    {

        public string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            //var es = odataPath.Segments.FirstOrDefault(s => s.SegmentKind == "entityset") as EntitySetPathSegment;
            var nav = odataPath.Segments.FirstOrDefault(s => s.SegmentKind == "navigation") as NavigationPathSegment;

            if (odataPath.PathTemplate == "~/unboundaction")
            {
                UnboundActionPathSegment actionSegment = odataPath.Segments.First() as UnboundActionPathSegment;
                if (actionSegment != null)
                {
                    IEdmActionImport action = actionSegment.Action;

                    if (actionMap.Contains(action.Name))
                    {
                        return action.Name;
                    }
                }
            }
            else if (odataPath.PathTemplate == "~/unboundfunction")
            {
                UnboundFunctionPathSegment functionSegment = odataPath.Segments.First() as UnboundFunctionPathSegment;
                if (functionSegment != null)
                {
                    IEdmFunctionImport function = functionSegment.Function;

                    if (actionMap.Contains(function.Name))
                    {
                        return function.Name;
                    }
                }
            }

            if (odataPath.PathTemplate == "~/entityset/key/navigation" && nav != null)
            {
                controllerContext.RouteData.Values["key"] = (odataPath.Segments[1] as KeyValuePathSegment).Value;
                return "Get" + nav.NavigationPropertyName;
            }
            else if (odataPath.PathTemplate == "~/entityset/key/navigation/key" && nav != null)
            {
                controllerContext.RouteData.Values["key1"] = (odataPath.Segments[1] as KeyValuePathSegment).Value;
                controllerContext.RouteData.Values["key2"] = (odataPath.Segments[3] as KeyValuePathSegment).Value;
                return "Get" + nav.NavigationProperty.ToEntityType().Name;
            }

            return null;
        }

        public string SelectController(ODataPath odataPath, HttpRequestMessage request)
        {
            if (odataPath.PathTemplate == "~/unboundaction" || odataPath.PathTemplate == "~/unboundfunction")
            {
                return "NonBindableActions";
            }
            else if (odataPath.PathTemplate == "~/entityset/key/navigation" ||
                odataPath.PathTemplate == "~/entityset/key/navigation/key")
            {
                IEdmNavigationProperty navigationProperty = (odataPath.Segments[2] as NavigationPathSegment).NavigationProperty;
                var es = odataPath.Segments.First(s => s.SegmentKind == "entityset") as EntitySetPathSegment;

                return es.EntitySetName;
            }

            return null;
        }
    }
}
