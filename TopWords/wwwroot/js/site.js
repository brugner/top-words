"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/topWordsHub")
    .build();

connection.on("ReceiveMessage", function (message) {
    $("#submit").text(message);
});

connection.start().catch(err => console.error(err.toString())).then(function () {
    connection.invoke('getConnectionId')
        .then(function (connectionId) {
            $("#connectionId").val(connectionId);
        })
});