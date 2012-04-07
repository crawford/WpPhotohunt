using Photohunt.Models;
using System.Net;
using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Photohunt.Data
{
    public class ApiService
    {
        private const string BASE_URL = @"https://photohunt.csh.rit.edu/api";

        public void FetchTeamInfo(Action<bool, string, TeamInfo> callback)
        {
            MakeRequest(string.Format("{0}/info?token={1}", BASE_URL, App.SettingsService.GameKey), typeof(InfoResponse), (response) =>
            {
                if (response.Status != Response.STATUS.ERR_SUCCESS)
                {
                    callback(false, response.Message, null);
                    return;
                }

                callback(true, response.Message, ((InfoResponse)response).Info);
            });
        }

        public void FetchClues(Action<bool, string, Dictionary<string, List<Clue>>> callback)
        {
            MakeRequest(string.Format("{0}/clues", BASE_URL), typeof(ClueResponse), (response) =>
            {
                if (response.Status != Response.STATUS.ERR_SUCCESS)
                {
                    callback(false, response.Message, null);
                    return;
                }

                //Put all the clues into their categories
                Clue[] clues = ((ClueResponse)response).Clues;
                Dictionary<string, List<Clue>> categories = new Dictionary<string, List<Clue>>();
                categories["all"] = new List<Clue>();
                foreach (Clue clue in clues)
                {
                    foreach (string tag in clue.Tags)
                    {
                        if (!categories.ContainsKey(tag))
                            categories[tag] = new List<Clue>();

                        categories[tag].Add(clue);
                    }

                    categories["all"].Add(clue);
                }

                /*ClueCategory[] cats = new ClueCategory[categories.Keys.Count];
                int i = 0;
                foreach (string cat in categories.Keys)
                {
                    cats[i] = new ClueCategory(cat.ToLower(), categories[cat].ToArray());
                    i++;
                }*/


                callback(true, response.Message, categories);
            });
        }

        private void MakeRequest(string address, Type type, Action<Response> callback)
        {
            try
            {
                //Create the request for the id request
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(address);

                //Start the request
                request.BeginGetResponse((asynchronousResult) =>
                {
                    try
                    {
                        //Get the response from the request
                        using (Stream responseStream = request.EndGetResponse(asynchronousResult).GetResponseStream())
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                            Response temp = serializer.ReadObject(responseStream) as Response;
                            callback(temp);
                        }
                    }
                    catch (WebException e)
                    {
                        try
                        {
                            using (Stream responseStream = e.Response.GetResponseStream())
                            {
                                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                                Response temp = serializer.ReadObject(responseStream) as Response;
                                if (temp != null)
                                    callback(temp);
                                else
                                    callback(new Response(Response.STATUS.ERR_INTERNAL, e.Message));
                            }
                        }
                        catch (Exception ex)
                        {
                            callback(new Response(Response.STATUS.ERR_INTERNAL, ex.Message));
                        }
                    }
                    catch (Exception e)
                    {
                        callback(new Response(Response.STATUS.ERR_INTERNAL, e.Message));
                    }
                }, 0);
            }
            catch (Exception e)
            {
                callback(new Response(Response.STATUS.ERR_INTERNAL, e.Message));
            }
        } 
    }
}
