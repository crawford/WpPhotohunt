﻿using Photohunt.Models;
using System.Net;
using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Photohunt.Data
{
    public class ApiService
    {
        private const string BASE_URL = @"http://test.acrawford.com/api";

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

        public void FetchClues(Action<bool, string, Clue[]> callback)
        {
            MakeRequest(string.Format("{0}/clues", BASE_URL), typeof(ClueResponse), (response) =>
            {
                if (response.Status != Response.STATUS.ERR_SUCCESS)
                {
                    callback(false, response.Message, null);
                    return;
                }

                callback(true, response.Message, ((ClueResponse)response).Clues);
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
                            //Read the content into result (should contain the CBUserId)
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                            Response temp = serializer.ReadObject(responseStream) as Response;
                            callback(temp);
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
