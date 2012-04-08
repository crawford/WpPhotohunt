﻿using Photohunt.Models;
using System.Net;
using System;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Photohunt.Data
{
    public class ApiService
    {
        private const string BASE_URL = @"http://test.acrawford.com/api"; //@"https://photohunt.csh.rit.edu/api";

        public void FetchTeamInfo(Action<bool, string, TeamInfo> callback)
        {
            MakeRequest(string.Format(@"{0}/info?token={1}", BASE_URL, App.SettingsService.GameKey), typeof(InfoResponse), (response) =>
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
            MakeRequest(string.Format(@"{0}/clues", BASE_URL), typeof(ClueResponse), (response) =>
            {
                if (response.Status != Response.STATUS.ERR_SUCCESS)
                {
                    callback(false, response.Message, null);
                    return;
                }

                //Put all the clues into their categories
                Clue[] clues = ((ClueResponse)response).Clues;
                Dictionary<string, List<Clue>> categories = new Dictionary<string, List<Clue>>();
                categories[@"all"] = new List<Clue>();
                foreach (Clue clue in clues)
                {
                    foreach (string tag in clue.Tags)
                    {
                        if (!categories.ContainsKey(tag))
                            categories[tag] = new List<Clue>();

                        categories[tag].Add(clue);
                    }

                    categories[@"all"].Add(clue);
                }

                callback(true, response.Message, categories);
            });
        }

        public void UploadPhoto(Photo photo, List<Photo> updating, Action<bool, Photo, List<Photo>> callback)
        {
            System.Diagnostics.Debug.WriteLine("Uploading photo " + photo.GetHashCode());

            string boundary = @"-----------";
            byte[] boudaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(@"{0}/photos/new?token={1}", BASE_URL, App.SettingsService.GameKey));
            request.ContentType = @"multipart/form-data; boundary=" + boundary;
            request.Method = @"POST";

            request.BeginGetRequestStream((asynchronousResult) =>
            {
                string json = "{\"clues\":[],\"judge\":true,\"notes\":\"\"}";
                byte[] metadataBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"json\"; filename=\"poop\"\r\nContent-Type: application/json\r\n\r\n" + json);
                byte[] fileheaderBytes = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"photo\"; filename=\"poop\"\r\nContent-Type: image/jpeg\r\n\r\n");
                byte[] trailerBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                try
                {
                    using (Stream stream = request.EndGetRequestStream(asynchronousResult))
                    {
                        stream.Write(boudaryBytes, 0, boudaryBytes.Length);
                        stream.Write(metadataBytes, 0, metadataBytes.Length);
                        stream.Write(boudaryBytes, 0, boudaryBytes.Length);
                        stream.Write(fileheaderBytes, 0, fileheaderBytes.Length);

                        lock (App.IsolatedStorageFileLock)
                        {
                            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                            {
                                using (IsolatedStorageFileStream fileStream = store.OpenFile(photo.Path.AbsolutePath, FileMode.Open, FileAccess.Read))
                                {
                                    byte[] buffer = new byte[4096];
                                    int bytesRead;
                                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                        stream.Write(buffer, 0, bytesRead);
                                }
                            }
                        }

                        stream.Write(trailerBytes, 0, trailerBytes.Length);
                    }  
                }
                catch (Exception ex)
                {
                    callback(false, photo, updating);
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                request.BeginGetResponse((asynchResult) =>
                {
                    try
                    {
                        //Get the response from the request
                        using (Stream responseStream = request.EndGetResponse(asynchResult).GetResponseStream())
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PhotoResponse));
                            PhotoResponse response = serializer.ReadObject(responseStream) as PhotoResponse;

                            if (response.Status == Response.STATUS.ERR_SUCCESS)
                            {
                                photo.ServerId = response.Id;
                                callback(true, photo, updating);
                            }
                            else
                            {
                                callback(false, photo, updating);
                                System.Diagnostics.Debug.WriteLine("Error:" + response.Message);
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        callback(false, photo, updating);
                        try
                        {
                            using (Stream responseStream = e.Response.GetResponseStream())
                            {
                                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PhotoResponse));
                                PhotoResponse response = serializer.ReadObject(responseStream) as PhotoResponse;
                                if (response == null)
                                {
                                    System.Diagnostics.Debug.WriteLine("Error uploading photo");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(response.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        callback(false, photo, updating);
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }, null);
            }, null);
        }

        public void SendPhotoMetadata(Photo photo, List<Photo> updating, Action<bool, Photo, List<Photo>> callback)
        {
            System.Diagnostics.Debug.WriteLine("Updating photo metadata");
            callback(true, photo, updating);
        }

        private void MakeRequest(string address, Type type, Action<Response> callback)
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
            }, null);
        }
    }
}
