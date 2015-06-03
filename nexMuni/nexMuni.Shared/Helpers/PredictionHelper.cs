﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Web.Http;
using nexMuni.DataModels;

namespace nexMuni.Helpers
{
    public class PredictionHelper
    {
        private static HttpClient client = new HttpClient();
        private static XDocument xmlDoc = new XDocument();
        private static List<Route> routes = new List<Route>();

        public static async Task<List<Route>> GetPredictionTimesAsync(string url)
        {
            return GetPredictions(await GetXml(url));
        }

        public static async Task<string> GetSearchTimesAsync(string url)
        {
            var document = await GetXml(url);
            var element = document.Element("body").Element("predictions").Element("direction");

            if (element != null) return ParseTimes(element);
            else return "No times found";

        }

        private static async Task<XDocument> GetXml(string url) 
        {
            //Make sure to pull from the network and not cache everytime predictions are refreshed 
            client.DefaultRequestHeaders.IfModifiedSince = DateTime.Now;
            try
            {
                var response = await client.GetAsync(new Uri(url));
                response.EnsureSuccessStatusCode();
                var reader = await response.Content.ReadAsStringAsync();
                xmlDoc = XDocument.Parse(reader);
            }
            catch(Exception)
            {
                ErrorHandler.NetworkError("Error getting predictions. Check network connection and try again.");
            }

            return xmlDoc;
        }

        private static List<Route> GetPredictions(XDocument document)
        {
            string routeTitle, routeNum;
            IEnumerable<XElement> rootElements;

            routes.Clear();
            //If there was an error getting the xml, return an empty list
            if (document.Root == null) return routes;
            else rootElements = document.Element("body").Elements("predictions");

            foreach(XElement predictionElement in rootElements)
            {
                routeTitle = ParseTitle(predictionElement);
                routeNum = ParseRouteNum(predictionElement);

                //Check to see if the route has already been added to the collection
                if (!routes.Any(r => r.RouteNumber == routeNum))
                {
                    Route newRoute = new Route(routeTitle, routeNum);

                    var subElement = predictionElement.Element("direction");
                    if (subElement != null)
                    {
                        var dirTitle = subElement.Attribute("title").Value;
                        var times = ParseTimes(subElement);
                        newRoute.Directions.Add(new RouteDirection(dirTitle, times));
                        routes.Add(newRoute);
                    }
                }
                else
                {
                    Route tempRoute = routes.Find(r => r.RouteNumber == routeNum);

                    var subElement = predictionElement.Element("direction");
                    if (subElement != null)
                    {
                        var dirTitle = subElement.Attribute("title").Value;
                        var times = ParseTimes(subElement);
                        tempRoute.Directions.Add(new RouteDirection(dirTitle, times));
                    }

                }   
            }
            return routes;
        }

        private static string ParseTimes(XElement element)
        {
            int maxTimes;
            var builder = new StringBuilder();
            var predictionElements = element.Elements("prediction");

            if (predictionElements.Count() < 4)
                maxTimes = predictionElements.Count();
            else
                maxTimes = 4;
            
            for (int i = 0; i < maxTimes; i++)
            {
                if (i == maxTimes - 1)
                    builder.Append(predictionElements.ElementAt(i).Attribute("minutes").Value + " mins");
                else
                    builder.Append(predictionElements.ElementAt(i).Attribute("minutes").Value + ", ");
            }
            return builder.ToString();
        }

        private static string ParseTitle(XElement element)
        {
            string fullTitle = element.Attribute("routeTitle").Value;
            if (fullTitle.Contains('-'))
            {
                int index = fullTitle.IndexOf('-');
                return fullTitle.Substring(index + 1, fullTitle.Length - (index + 1));
            }
            else
            {
                int index = fullTitle.IndexOf('"');
                return fullTitle.Substring(index + 1, (fullTitle.Length - (index + 2)));
            }
        }

        private static string ParseRouteNum(XElement element)
        {
            string fullTitle = element.Attribute("routeTitle").Value;
            if (fullTitle.Contains('-'))
            {
                return fullTitle.Substring(0, fullTitle.IndexOf('-'));
            }
            else
            {
                return element.Attribute("routeTag").Value;
            }
        }

    }
}
