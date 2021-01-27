"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/maphub").build();
connection.start();
var data;
connection.on("getData", function (obj) {
    data = obj;
    console.log(obj);
    var found = objlist.findIndex(x => x.name == data.zoneID);
    if (found > -1) {
        objlist.splice(found, 1);   //remove obj from obj list for re-doing
    }
    var words = "<div class='infotitle'>Total Requests: " + data.reqList.length + "</div></br>Item summary</br>"
    for (var item in data.itemList) {
        words += (
            "<span class='itemname'>" +
            data.itemList[item].name +
            "</span> : <span class='itemamnt'>" +
            data.itemList[item].requested + "</span></br>"
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

