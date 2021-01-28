using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RQST.Hubs;
using RQST.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQST.DAL
{
    public interface IDataStream
    {
        public FirebaseClient firebaseClient { get; set; }
        Task getDataAsync();
        Task<List<Request_NEW>> populateReqsAsync();
    }
    public class DataStream : IDataStream
    {
        private List<Request_NEW> reqList { get; set; }
        private List<items> itemList { get; set; }
        public FirebaseClient firebaseClient { set; get; }
        private IHubContext<MapHub> _context;
        public DataStream(IHubContext<MapHub> context)
        {
            _context = context;
        }
        public async Task getDataAsync()
        {
            Request_NEW req = new Request_NEW();
            var subscription = firebaseClient
                .Child("requests")
                .AsObservable<IDictionary<string, object>>()
                .Subscribe(d =>
                {
                    if (d.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        ArraySegment<Byte> segment = toReqAsync(d);
                        _context.Clients.All.SendAsync("getData", segment);
                    }
                    else
                    {
                        _context.Clients.All.SendAsync("delData", checkNDelete(d));
                    }
                });
        }
        public async Task<List<Request_NEW>> populateReqsAsync()
        {
            var reqData = await firebaseClient
                .Child("requests")
                .OnceAsync<IDictionary<string, Object>>();
            List<items> itemListN = new List<items>();
            var fbItemList = await firebaseClient
                                    .Child("items")
                                    .OnceAsync<items>();
            foreach (var item in fbItemList)
            {
                items itemActual = item.Object;
                itemActual.ID = item.Key;
                itemListN.Add(itemActual);
            }
            List<Request_NEW> reqListN = new List<Request_NEW>();
            foreach (var area in reqData)
            {
                Request_NEW req = new Request_NEW();
                req.ZoneID = area.Key;
                foreach (var requestID in area.Object)
                {
                    Request currReq = JsonConvert.DeserializeObject<Request>(requestID.Value.ToString());
                    currReq.ID = requestID.Key;
                    foreach (var item in currReq.Contents)
                    {
                        items itemF = itemListN.Find(x => x.ID == item.Key);
                        items itemN = new items(itemF.Name, item.Value, itemF.Icon, itemF.stock, itemF.BgCol);
                        items itemR = new items(itemF.Name, item.Value, itemF.Icon, itemF.stock, itemF.BgCol);
                        itemN.ID = item.Key;
                        itemR.ID = item.Key;
                        currReq.addItem(itemN);
                        req.addItem(itemR);
                    }
                    req.ReqList.Add(currReq);
                }
                reqListN.Add(req);
            }
            reqList = reqListN;
            itemList = itemListN;
            return reqListN;
        }

        public Request_NEW checkNDelete(FirebaseObject<IDictionary<string, object>> d)
        {
            var reqF = reqList.Find(x => x.ZoneID == d.Key);
            if (reqF.ReqList.Count == d.Object.Count())
            {
                reqF.ReqList.Clear();
                return reqF;
            }
            else
            {
                for (int i = 0; i < reqF.ReqList.Count; i++)
                {
                    if (d.Object.ContainsKey(reqF.ReqList[i].ID))
                    {
                        // keep request
                    }
                    else
                    {
                        foreach (var item in reqF.ReqList[i].itemList)
                        {
                            reqF.removeItem(item);
                        }
                        reqF.ReqList.RemoveAt(i);
                    }
                }
            }
            return reqF;
        }
        public ArraySegment<byte> toReqAsync(FirebaseObject<IDictionary<string, object>> d)
        {
            //check if request in list
            Request_NEW req = new Request_NEW();
            bool success = false;
            req.ZoneID = d.Key;
            req.ReqList.Clear();
            req.ItemList.Clear();
            foreach (var singularRequest in d.Object)
            {
                Request currReq;
                if (singularRequest.Value.ToString() == "System.Object")
                {
                    currReq = firebaseClient
                        .Child("requests")
                        .Child(req.ZoneID)
                        .Child(singularRequest.Key)
                        .OnceSingleAsync<Request>()
                        .Result;
                }
                else
                {
                    currReq = JsonConvert.DeserializeObject<Request>(singularRequest.Value.ToString());
                }
                currReq.ID = singularRequest.Key;
                foreach (var item in currReq.Contents)
                {
                    items itemF = itemList.Find(x => x.ID == item.Key);
                    items itemN = new items(itemF.Name, item.Value, itemF.Icon, itemF.Requested, itemF.BgCol);
                    items itemR = new items(itemF.Name, item.Value, itemF.Icon, itemF.Requested, itemF.BgCol);
                    itemN.ID = item.Key;
                    itemR.ID = item.Key;
                    req.addItem(itemR);
                }
                req.ReqList.Add(currReq);
            }
            reqList.RemoveAll(x => x.ZoneID == req.ZoneID);
            reqList.Add(req);
            ArraySegment<byte> bruh = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(req));
            return bruh;
        }
    }
}
