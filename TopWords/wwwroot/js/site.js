"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/topWordsHub").build();

connection.on("ReceiveMessage", function (message) {
    $("#submit").text(message);
});

connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});