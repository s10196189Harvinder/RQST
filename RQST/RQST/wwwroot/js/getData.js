"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/maphub").build();
connection.start();
var data;
connection.on("getData", function (obj) {
    data = JSON.parse(String.fromCharCode.apply(null, obj));
    var found = objlist.findIndex(x => x.name == data.ZoneID);
    if (found > -1) {
        console.log("removed");
        objlist.splice(found, 1);   //remove obj from obj list for re-doing
    }
    var words = "<div class='infotitle'>Total Requests: " + data.ReqList.length + "</div></br>Item summary</br>"
    for (var item in data.ItemList) {
        words += (
            "<span class='itemname'>" +
            data.ItemList[item].name +
            "</span> : <span class='itemamnt'>" +
            data.ItemList[item].requested + "</span></br>"
        )
    }
    var newobj = {
        name: data.ZoneID,
        requests: data.ReqList.length,
        itemliststring: words
    }
    console.log(newobj.name);
    map.data.forEach(function (feature) {
        var SD_NAME = feature.getProperty('Name');
        if ((data.ZoneID) == SD_NAME) {
            console.log("same found !!");
            console.log(newobj.requests);
            console.log(newobj.itemliststring);
            feature.setProperty("requests", newobj.requests);
            feature.setProperty("itemliststring", newobj.itemliststring);
        }
    });
})

connection.on("delData", function (obj) {
    if (obj.reqList.length == 0) {
        map.data.forEach(function (feature) {
            var SD_NAME = feature.getProperty('Name');
            if (obj.zoneID == SD_NAME) {
                feature.setProperty("requests", 0);
                feature.setProperty("itemliststring", "");
            }
        });
    }
    else {
        var words = "<div class='infotitle'>Total Requests: " + obj.reqList.length + "</div></br>Item summary</br>"
        for (var item in obj.itemList) {
            words += (
                "<span class='itemname'>" +
                obj.itemList[item].name +
                "</span> : <span class='itemamnt'>" +
                obj.itemList[item].requested + "</span></br>"
            )
        }
        var newobj = {
            name: obj.zoneID,
            requests: obj.reqList.length,
            itemliststring: words
        }
        map.data.forEach(function (feature) {
            var SD_NAME = feature.getProperty('Name');
            if (newobj.name == SD_NAME) {
                feature.setProperty("requests", newobj.requests);
                feature.setProperty("itemliststring", newobj.itemliststring);
            }
        });
    }
})

